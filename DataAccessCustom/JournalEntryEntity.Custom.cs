using System;
using System.ComponentModel;
using System.Linq;
using DataAccess.HelperClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace DataAccess.EntityClasses
{
    public partial class JournalEntryEntity
    {
        private bool _isMaterialBound = false;
        private bool _isRecalculatingFromChild = false;

        public bool IsMaterialBound
        {
            get
            {
                if (_isMaterialBound) return true;
                if (InventoryVouchers != null && InventoryVouchers.Count > 0) return true;
                if (ParentEntry != null && ParentEntry.InventoryVouchers != null && ParentEntry.InventoryVouchers.Count > 0) return true;
                if (SubEntries != null && SubEntries.Any(s => s.InventoryVouchers != null && s.InventoryVouchers.Count > 0)) return true;
                return false;
            }
            set => _isMaterialBound = value;
        }

        public bool UpdateFilter { get; set; } = false;

        /// <summary>
        /// Cờ tạm ngắt hiệu ứng Domino (dùng khi đang nạp dữ liệu hàng loạt hoặc khi Rollback/Cancel trên form chi tiết).
        /// Mặc định = false (Domino luôn tự động chạy khi thêm/sửa/xóa dòng chi tiết con).
        /// </summary>
        public bool IsDominoSuspended { get; set; } = false;

        protected override void OnInitClassMembersComplete()
        {
            base.OnInitClassMembersComplete();
            AttachChildCollectionHandlers();
        }

        private void AttachChildCollectionHandlers()
        {
            HookCollection(this.InventoryVouchers);
            HookCollection(this.DebtDetails);
            HookCollection(this.CostDetails);
            HookCollection(this.AssetDetails);
            HookCollection(this.InvestmentDetails);
            HookCollection(this.CurrencyDetails);
        }

        private void HookCollection<T>(EntityCollection<T> collection) where T : EntityBase2, IEntity2
        {
            if (collection == null) return;
            collection.EntityAdded -= ChildCollection_EntityAdded;
            collection.EntityAdded += ChildCollection_EntityAdded;
            collection.EntityRemoved -= ChildCollection_EntityRemoved;
            collection.EntityRemoved += ChildCollection_EntityRemoved;

            foreach (var item in collection)
            {
                item.PropertyChanged -= ChildEntity_PropertyChanged;
                item.PropertyChanged += ChildEntity_PropertyChanged;
            }
        }

        private void ChildCollection_EntityAdded(object sender, CollectionChangedEventArgs e)
        {
            if (e.InvolvedEntity is EntityBase2 entity)
            {
                entity.PropertyChanged -= ChildEntity_PropertyChanged;
                entity.PropertyChanged += ChildEntity_PropertyChanged;

                if (IsDominoSuspended) return;

                bool shouldRecalculate = entity.IsNew || entity.IsDirty;
                if (!shouldRecalculate && entity is InventoryVoucherEntity iv)
                {
                    shouldRecalculate = iv.InventoryVoucherLines != null && iv.InventoryVoucherLines.Any(l => l.IsNew || l.IsDirty);
                }

                if (shouldRecalculate)
                {
                    NotifyChildCollectionChanged(entity);
                }
            }
        }

        private void ChildCollection_EntityRemoved(object sender, CollectionChangedEventArgs e)
        {
            if (e.InvolvedEntity is EntityBase2 entity)
            {
                entity.PropertyChanged -= ChildEntity_PropertyChanged;
                if (IsDominoSuspended) return;
                NotifyChildCollectionChanged(entity);
            }
        }

        private void ChildEntity_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDominoSuspended || sender == null || string.IsNullOrEmpty(e.PropertyName)) return;

            if (sender is InvestmentDetailEntity inv &&
                (e.PropertyName == nameof(InvestmentDetailEntity.Quantity) || e.PropertyName == nameof(InvestmentDetailEntity.Inprice)))
            {
                double calc = (inv.Quantity ?? 0) * (inv.Inprice ?? 0);
                if (Math.Abs((inv.Originamount ?? 0) - calc) > 0.001)
                {
                    inv.Originamount = calc;
                }
            }

            if (e.PropertyName == nameof(DebtDetailEntity.Amount) ||
                e.PropertyName == nameof(CostDetailEntity.Amount) ||
                e.PropertyName == nameof(AssetDetailEntity.Quantity) ||
                e.PropertyName == nameof(AssetDetailEntity.Price) ||
                e.PropertyName == nameof(InvestmentDetailEntity.Quantity) ||
                e.PropertyName == nameof(InvestmentDetailEntity.Inprice) ||
                e.PropertyName == nameof(InvestmentDetailEntity.Originamount) ||
                e.PropertyName == nameof(CurrencyDetailEntity.Quantity) ||
                e.PropertyName == nameof(CurrencyDetailEntity.ExchangeRate) ||
                e.PropertyName == nameof(InventoryVoucherEntity.TotalCostAmount) ||
                e.PropertyName == nameof(InventoryVoucherEntity.TotalSellAmount) ||
                e.PropertyName == nameof(InventoryVoucherEntity.TotalTaxAmount))
            {
                NotifyChildCollectionChanged(sender);
            }
        }

        public void NotifyChildCollectionChanged(object childSource = null)
        {
            if (IsDominoSuspended || _isRecalculatingFromChild) return;

            try
            {
                _isRecalculatingFromChild = true;

                if (CanUpdateFromChild(childSource))
                {
                    double? calculatedAmount = CalculateTotalFromChildType(childSource);
                    if (calculatedAmount.HasValue)
                    {
                        double rounded = Math.Round(calculatedAmount.Value, 0);
                        if (!this.Amount.HasValue || Math.Abs(this.Amount.Value - rounded) > 0.001)
                        {
                            this.Amount = rounded;
                        }
                    }
                }

                OnPropertyChanged("ChildDetailsChanged");
            }
            finally
            {
                _isRecalculatingFromChild = false;
            }
        }

        private double? CalculateTotalFromChildType(object childSource)
        {
            if (childSource is InventoryVoucherEntity || childSource is InventoryVoucherLineEntity)
            {
                return this.InventoryVouchers?.Sum(iv => iv.TotalCostAmount) ?? 0;
            }
            if (childSource is DebtDetailEntity)
            {
                return this.DebtDetails?.Sum(d => d.Amount ?? 0) ?? 0;
            }
            if (childSource is CostDetailEntity)
            {
                return this.CostDetails?.Sum(c => c.Amount ?? 0) ?? 0;
            }
            if (childSource is AssetDetailEntity)
            {
                return this.AssetDetails?.Sum(a => (a.Quantity ?? 0) * (a.Price ?? 0)) ?? 0;
            }
            if (childSource is InvestmentDetailEntity)
            {
                return this.InvestmentDetails?.Sum(s => (s.Quantity ?? 0) * (s.Inprice ?? 0)) ?? 0;
            }
            if (childSource is CurrencyDetailEntity)
            {
                return this.CurrencyDetails?.Sum(c => (c.Quantity ?? 0) * (c.ExchangeRate ?? 0)) ?? 0;
            }

            if (this.InventoryVouchers != null && this.InventoryVouchers.Count > 0)
                return this.InventoryVouchers.Sum(iv => iv.TotalCostAmount);
            if (this.DebtDetails != null && this.DebtDetails.Count > 0)
                return this.DebtDetails.Sum(d => d.Amount ?? 0);
            if (this.CostDetails != null && this.CostDetails.Count > 0)
                return this.CostDetails.Sum(c => c.Amount ?? 0);
            if (this.AssetDetails != null && this.AssetDetails.Count > 0)
                return this.AssetDetails.Sum(a => (a.Quantity ?? 0) * (a.Price ?? 0));
            if (this.InvestmentDetails != null && this.InvestmentDetails.Count > 0)
                return this.InvestmentDetails.Sum(s => (s.Quantity ?? 0) * (s.Inprice ?? 0));
            if (this.CurrencyDetails != null && this.CurrencyDetails.Count > 0)
                return this.CurrencyDetails.Sum(c => (c.Quantity ?? 0) * (c.ExchangeRate ?? 0));

            return null;
        }

        public bool CanUpdateFromChild(object child)
        {
            if (!this.UpdateFilter) return true;
            if (child == null) return true;
            string typeName = child.GetType().Name;
            return typeName.Contains("Inventory") || typeName.Contains("Material");
        }

        public bool TryUpdateAmountFromChild(object child, double newAmount)
        {
            if (!CanUpdateFromChild(child)) return false;
            this.Amount = Math.Round(newAmount, 0);
            return true;
        }

        /// <summary>
        /// Kiểm tra dòng bút toán có sổ chi tiết phụ (Công nợ, Vật tư, Chi phí, Tài sản, Cổ phiếu, Ngoại tệ) hay không.
        /// </summary>
        public bool HasSubLedgerDetails()
        {
            if (this.DebtDetails != null && this.DebtDetails.Count > 0) return true;
            if (this.InventoryVouchers != null && this.InventoryVouchers.Count > 0) return true;
            if (this.CostDetails != null && this.CostDetails.Count > 0) return true;
            if (this.AssetDetails != null && this.AssetDetails.Count > 0) return true;
            if (this.InvestmentDetails != null && this.InvestmentDetails.Count > 0) return true;
            if (this.CurrencyDetails != null && this.CurrencyDetails.Count > 0) return true;
            return false;
        }

        public bool HasChildDetails()
        {
            if (HasSubLedgerDetails()) return true;
            if (this.SubEntries != null && this.SubEntries.Count > 0) return true;
            return false;
        }

        public void ClearChildDetails()
        {
            this.DebtDetails?.Clear();
            this.InventoryVouchers?.Clear();
            this.CostDetails?.Clear();
            this.AssetDetails?.Clear();
            this.InvestmentDetails?.Clear();
            this.CurrencyDetails?.Clear();
            this.SubEntries?.Clear();
            this.IsMaterialBound = false;
            this.UpdateFilter = false;
        }
    }
}
