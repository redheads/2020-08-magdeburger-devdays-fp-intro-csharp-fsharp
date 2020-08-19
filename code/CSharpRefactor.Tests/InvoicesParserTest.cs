using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class InvoicesParserTest
    {
        [Fact]
        public void ReadInvoices__No_Input_Files__Empty_Result()
        {
            var sut = new InvoicesParser(new string[] {}, null, null);
            var actual = sut.ReadAndParseInvoices();
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Valid_Input_File_No_Discount__Amount_Is_Read()
        {
            const string path = "invoice1.txt";
            var sut = new InvoicesParser(new string[] {path}, null, null);
            var actual = sut.ReadAndParseInvoices();

            var expected = new Dictionary<string, InvoiceParseResult>()
            {
                {path, new InvoiceParseResult(0, 42m, null)}
            };
            
            Assert.Equal(expected, actual);
        }
    }
}
