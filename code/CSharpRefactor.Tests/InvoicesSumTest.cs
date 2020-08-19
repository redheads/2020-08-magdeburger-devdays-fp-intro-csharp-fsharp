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
    }
}
