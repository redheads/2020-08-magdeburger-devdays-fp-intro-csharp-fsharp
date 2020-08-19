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
        public void Sum__Empty_Input__Zero()
        {
            var sut = new InvoiceSum(new InvoiceParseResult[] {});
            var actual = sut.Sum();
            var expected = new InvoicesSum(0m, 0m);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Sum__Invoice_Amounts__Correct_Sum()
        {
            var sut = new InvoiceSum(new InvoiceParseResult[]
            {
                new InvoiceParseResult(0, 1, 3), 
                new InvoiceParseResult(1, 2, 4), 
            });
            var actual = sut.Sum();
            var expected = new InvoicesSum(3, 7);
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void Sum__Invoice_With_Error__Correct_Sum()
        {
            var sut = new InvoiceSum(new InvoiceParseResult[]
            {
                new InvoiceParseResult(0, 1, 3), 
                new InvoiceParseResult(1, "error"), 
            });
            var actual = sut.Sum();
            var expected = new InvoicesSum(1, 3);
            Assert.Equal(expected, actual);
        }
    }
}
