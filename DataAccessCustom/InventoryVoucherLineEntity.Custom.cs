using System;

namespace DataAccess.EntityClasses
{
    public partial class InventoryVoucherLineEntity
    {
        private MaterialTaxLineEntity EnsureTaxLine(bool createIfMissing)
        {
            if (this.MaterialTaxLine == null && createIfMissing)
            {
                this.MaterialTaxLine = new MaterialTaxLineEntity();
            }
            return this.MaterialTaxLine;
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
