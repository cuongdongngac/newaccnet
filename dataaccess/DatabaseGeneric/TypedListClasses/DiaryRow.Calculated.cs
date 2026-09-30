namespace DataAccess.TypedListClasses
{
    public partial class DiaryRow
    {
        private decimal SignedAmount => (decimal)((Dbcr ?? 0) * (Amount ?? 0d));

        public decimal Debit => SignedAmount > 0m ? SignedAmount : 0m;

        public decimal Credit => SignedAmount <= 0m ? -SignedAmount : 0m;

        public string DebitAccount => Debit != 0m ? AccountId : null;

        public string CreditAccount => Credit != 0m ? AccountId : null;
    }
}
