using System.Collections.Generic;
using LaYumba.Functional;

namespace CSharpRefactor
{
    public class InvoiceSum
    {
        public static InvoicesSum Sum(IEnumerable<InvoiceParseResult> invoices)
        {
            var sum = 0m;
            var discountedSum = 0m;

            // what should happen if errorText is non-empty, but there is an amount nonetheless?
            foreach (var invoice in invoices)
            {
                sum += invoice.Amount ?? 0;
                discountedSum += invoice.DiscountedAmount.GetOrElse(0);
            }
            
            return new InvoicesSum(sum, discountedSum);
        }
        
        public static InvoicesSum AggregateSum(InvoicesSum acc, InvoiceParseResult invoice)
        {
            return new InvoicesSum(acc.Sum + invoice.Amount ?? 0, 
                acc.DiscountedSum + invoice.DiscountedAmount.GetOrElse(0));
        }
    }
}