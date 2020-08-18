using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class InvoicesTest
    {
        [Fact]
        public void ReadInvoices__No_Input_Files__Empty_Result()
        {
            var actual = Invoices.ReadInvoices(new string[] {}, null, () => false);
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Valid_Input_File_No_Discount__Amount_Is_Read()
        {
            const string path = "invoice1.txt";
            var actual = Invoices.ReadInvoices(new string[]
            {
                path
            }, null, () => false);

            var expected = new Dictionary<string, ParseResult>()
            {
                {path, new ParseResult(42m, null)}
            };
            
            Assert.Equal(expected, actual);
        }
    }
}
