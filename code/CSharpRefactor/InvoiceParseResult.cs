using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace CSharpRefactor
{
    public class InvoiceParseResult
    {
        public readonly int Id;
        public readonly decimal? Amount;
        public readonly Option<decimal> DiscountedAmount;

        public InvoiceParseResult(int id, decimal? amount, Option<decimal> discountedAmount)
        {
            Id = id;
            Amount = amount;
            DiscountedAmount = discountedAmount;
        }
        
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Id.GetHashCode();
                
                // Suitable nullity checks etc, of course :)
                if (Amount.HasValue)
                {
                    hash = hash * 23 + Amount.Value.GetHashCode();
                }
                    
                hash = hash * 23 + DiscountedAmount.GetHashCode();
                
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as InvoiceParseResult;

            if (item == null)
            {
                return false;
            }

            return 
                Id.Equals(item.Id)
                && (Amount.HasValue && item.Amount.HasValue && Amount.Equals(item.Amount))
                || (!Amount.HasValue && !item.Amount.HasValue)
                && DiscountedAmount.Equals(item.DiscountedAmount);
        }
    }
}