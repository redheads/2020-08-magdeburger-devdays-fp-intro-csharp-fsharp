using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class InvoicesSumTest
    {
        [Fact]
        public void Sum__Invoice_Amounts__Correct_Sum()
        {
            var acc = new InvoicesSum(0, 0);
            var actual = InvoiceSum.AggregateSum(acc, new InvoiceParseResult(0, 1, 3));
            var expected = new InvoicesSum(1, 3);
            Assert.Equal(expected, actual);
        }
    }
}
