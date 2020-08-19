using System.Collections.Generic;

namespace CSharpRefactor
{
    public class InvoiceSum
    {
        private readonly IEnumerable<InvoiceParseResult> _invoices;

        public InvoiceSum(IEnumerable<InvoiceParseResult> invoices)
        {
            _invoices = invoices;
        }

        public InvoicesSum Sum()
        {
            var sum = 0m;
            var discountedSum = 0m;

            // what should happen if errorText is non-empty, but there is an amount nonetheless?
            foreach (var invoice in _invoices)
            {
                sum += invoice.Amount ?? 0;
                discountedSum += invoice.DiscountedAmount ?? 0;
            }
            
            return new InvoicesSum(sum, discountedSum);
        }
    }
}