#nullable enable
using System;
using System.Linq;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace DataAccess.EntityClasses
{
    public partial class InventoryVoucherEntity
    {
        private string? _voucherType;

        public string? VoucherType
        {
            get => _voucherType;
            set
            {
                if (_voucherType != value)
                {
                    _voucherType = value;
                    OnPropertyChanged(nameof(VoucherType));
                    NotifyLinesChanged(true);
                }
            }
        }

        public double TotalCostAmount => Math.Round(this.InventoryVoucherLines?.Sum(l => l.Amount) ?? 0, 0);

        public double TotalSellAmount => Math.Round(this.InventoryVoucherLines?.Sum(l => l.SellAmount) ?? 0, 0);

        public double TotalTaxAmount => Math.Round(this.InventoryVoucherLines?.Sum(l => l.TaxAmount) ?? 0, 0);

        protected override void OnInitClassMembersComplete()
        {
            base.OnInitClassMembersComplete();

            var lines = this.InventoryVoucherLines;
            if (lines != null)
            {
                lines.EntityAdded -= InventoryVoucherLines_EntityAdded;
                lines.EntityAdded += InventoryVoucherLines_EntityAdded;
                lines.EntityRemoved -= InventoryVoucherLines_EntityRemoved;
                lines.EntityRemoved += InventoryVoucherLines_EntityRemoved;
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(Taxflag))
            {
                NotifyLinesChanged(true);
            }
        }

        private void InventoryVoucherLines_EntityAdded(object? sender, CollectionChangedEventArgs e)
        {
            bool shouldPropagate = true;
            if (e.InvolvedEntity is InventoryVoucherLineEntity line)
            {
                shouldPropagate = line.IsNew || line.IsDirty;
            }
            NotifyLinesChanged(shouldPropagate);
        }

        private void InventoryVoucherLines_EntityRemoved(object? sender, CollectionChangedEventArgs e)
        {
            NotifyLinesChanged(true);
        }

        public void NotifyLinesChanged(bool propagateToParent = true)
        {
            OnPropertyChanged(nameof(TotalCostAmount));
            OnPropertyChanged(nameof(TotalSellAmount));
            OnPropertyChanged(nameof(TotalTaxAmount));

            if (propagateToParent)
            {
                this.JournalEntry?.NotifyChildCollectionChanged(this);
            }
        }
    }
}
