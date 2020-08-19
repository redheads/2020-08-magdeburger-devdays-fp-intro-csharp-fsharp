namespace CSharpRefactor
{
    public class InvoicesSum
    {
        public decimal Sum;
        public decimal DiscountedSum;

        public InvoicesSum(decimal sum, decimal discountedSum)
        {
            Sum = sum;
            DiscountedSum = discountedSum;
        }
        
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Sum.GetHashCode();
                hash = hash * 23 + DiscountedSum.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as InvoicesSum;

            if (item == null)
            {
                return false;
            }

            return
                Sum.Equals(item.Sum)
                && DiscountedSum.Equals(item.DiscountedSum);
        }
    }
}