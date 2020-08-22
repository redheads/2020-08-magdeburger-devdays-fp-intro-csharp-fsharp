using System.Collections.Generic;
using LaYumba.Functional;

namespace CSharpRefactor
{
    public static class InvoiceSum
    {
        public static InvoicesSum AggregateSum(InvoicesSum acc, InvoiceParseResult invoice)
        {
            return new InvoicesSum(acc.Sum + invoice.Amount ?? 0, 
                acc.DiscountedSum + invoice.DiscountedAmount.GetOrElse(0));
        }
    }
}