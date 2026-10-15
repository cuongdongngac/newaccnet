using System;
using System.ComponentModel;

namespace DataAccess.EntityClasses
{
    public partial class InventoryVoucherLineEntity
    {
        private MaterialTaxLineEntity _subscribedTaxLine;

        protected override void OnInitClassMembersComplete()
        {
            base.OnInitClassMembersComplete();
            SyncTaxLineSubscription();
        }

        private void SyncTaxLineSubscription()
        {
            var currentTaxLine = this.MaterialTaxLine;
            if (ReferenceEquals(_subscribedTaxLine, currentTaxLine))
                return;

            if (_subscribedTaxLine != null)
            {
                _subscribedTaxLine.PropertyChanged -= MaterialTaxLine_PropertyChanged;
            }

            _subscribedTaxLine = currentTaxLine;

            if (_subscribedTaxLine != null)
            {
                _subscribedTaxLine.PropertyChanged += MaterialTaxLine_PropertyChanged;
            }
        }

        private void MaterialTaxLine_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MaterialTaxLineEntity.SellPrice))
            {
                OnPropertyChanged(nameof(SellPrice));
                OnPropertyChanged(nameof(SellAmount));
                OnPropertyChanged(nameof(TaxAmount));
                this.InventoryVoucher?.NotifyLinesChanged();
            }
            else if (e.PropertyName == nameof(MaterialTaxLineEntity.Rate))
            {
                OnPropertyChanged(nameof(Rate));
                OnPropertyChanged(nameof(TaxAmount));
                this.InventoryVoucher?.NotifyLinesChanged();
            }
            else if (e.PropertyName == nameof(MaterialTaxLineEntity.Taxrateid))
            {
                OnPropertyChanged(nameof(Taxrateid));
                OnPropertyChanged(nameof(TaxAmount));
                this.InventoryVoucher?.NotifyLinesChanged();
            }
        }

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(MaterialTaxLine))
            {
                SyncTaxLineSubscription();
                OnPropertyChanged(nameof(SellPrice));
                OnPropertyChanged(nameof(Rate));
                OnPropertyChanged(nameof(Taxrateid));
                OnPropertyChanged(nameof(SellAmount));
                OnPropertyChanged(nameof(TaxAmount));
                this.InventoryVoucher?.NotifyLinesChanged();
            }
            else if (propertyName == nameof(Quantity) || propertyName == nameof(Price))
            {
                OnPropertyChanged(nameof(Amount));
                OnPropertyChanged(nameof(SellAmount));
                OnPropertyChanged(nameof(TaxAmount));
                this.InventoryVoucher?.NotifyLinesChanged();
            }
        }

        private MaterialTaxLineEntity EnsureTaxLine(bool createIfMissing)
        {
            if (this.MaterialTaxLine == null && createIfMissing)
            {
                this.MaterialTaxLine = new MaterialTaxLineEntity();
            }
            SyncTaxLineSubscription();
            return this.MaterialTaxLine;
        }

        public double Amount => Math.Round((this.Quantity ?? 0) * (this.Price ?? 0), 0);

        public double SellAmount => Math.Round((this.Quantity ?? 0) * (this.SellPrice ?? 0), 0);

        public double TaxAmount
        {
            get
            {
                bool isSales = (this.InventoryVoucher?.VoucherType == "XB")
                    || (string.IsNullOrEmpty(this.InventoryVoucher?.VoucherType)
                        && this.InventoryVoucher?.JournalEntry?.Dbcr == -1
                        && (this.InventoryVoucher?.Taxflag ?? false));
                double baseAmount = isSales ? this.SellAmount : this.Amount;
                return Math.Round(baseAmount * (this.Rate ?? 0), 0);
            }
        }

        public double? SellPrice
        {
            get => this.MaterialTaxLine?.SellPrice;
            set
            {
                var taxLine = EnsureTaxLine(value != null && value != 0);
                if (taxLine != null)
                {
                    taxLine.SellPrice = value;
                }
            }
        }

        public double? Rate
        {
            get => this.MaterialTaxLine?.Rate;
            set
            {
                var taxLine = EnsureTaxLine(value != null && value != 0);
                if (taxLine != null)
                {
                    taxLine.Rate = value;
                }
            }
        }

        public int? Taxrateid
        {
            get => this.MaterialTaxLine?.Taxrateid;
            set
            {
                var taxLine = EnsureTaxLine(value != null);
                if (taxLine != null)
                {
                    taxLine.Taxrateid = (short?)value;
                }
            }
        }

        public void CleanUpTaxLine()
        {
            if (this.MaterialTaxLine != null && this.MaterialTaxLine.IsNew)
            {
                bool hasSellPrice = this.MaterialTaxLine.SellPrice.HasValue && this.MaterialTaxLine.SellPrice.Value != 0;
                bool hasRate = this.MaterialTaxLine.Rate.HasValue && this.MaterialTaxLine.Rate.Value != 0;
                bool hasTaxRateId = this.MaterialTaxLine.Taxrateid.HasValue;

                if (!hasSellPrice && !hasRate && !hasTaxRateId)
                {
                    this.MaterialTaxLine = null;
                }
            }
        }
    }
}
