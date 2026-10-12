using System;
using System.Linq;

namespace DataAccess.EntityClasses
{
    public partial class JournalEntryEntity
    {
        private bool _isMaterialBound = false;

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

        public bool CanUpdateFromChild(object child)
        {
            if (!this.UpdateFilter) return true;
            if (child == null) return false;
            string typeName = child.GetType().Name;
            return typeName.Contains("Inventory") || typeName.Contains("Material");
        }

        public bool TryUpdateAmountFromChild(object child, double newAmount)
        {
            if (!CanUpdateFromChild(child)) return false;
            this.Amount = Math.Round(newAmount, 0);
            return true;
        }

        public bool HasChildDetails()
        {
            if (this.DebtDetails != null && this.DebtDetails.Count > 0) return true;
            if (this.InventoryVouchers != null && this.InventoryVouchers.Count > 0) return true;
            if (this.CostDetails != null && this.CostDetails.Count > 0) return true;
            if (this.AssetDetails != null && this.AssetDetails.Count > 0) return true;
            if (this.InvestmentDetails != null && this.InvestmentDetails.Count > 0) return true;
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
            this.SubEntries?.Clear();
            this.IsMaterialBound = false;
            this.UpdateFilter = false;
        }
    }
}
