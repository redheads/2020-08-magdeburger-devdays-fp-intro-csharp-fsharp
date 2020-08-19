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
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {}, null, null);
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Valid_Input_File_No_Discount__Amount_Is_Read()
        {
            const string path = "invoice1.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, null, null);

            var expected = new Dictionary<string, InvoiceParseResult>()
            {
                {path, new InvoiceParseResult(0, 42m, null)}
            };
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ReadInvoices__Input_File_Empty__Empty_Result()
        {
            const string path = "invoice2.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, null, null);
            
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Input_File_Too_Many_Lines__Empty_Result()
        {
            const string path = "invoice3.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, null, null);
            
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__NonExisting_Input__Error_Result()
        {
            const string path = "invoice_abc.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, null, null).ToArray();

            Assert.Contains("Could not find file", actual[0].Value.ErrorText);
        }
        
        [Fact]
        public void ReadInvoices__Invalid_File_Content__Error_Result()
        {
            const string path = "invoice4.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, null, null).ToArray();

            Assert.Equal("Invoice amount abc could not be parsed", actual[0].Value.ErrorText);
        }
        
        [Fact]
        public void ReadInvoices__With_Discount__Discount_Is_Applied()
        {
            const string path = "invoice5.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, 10, true).ToArray();

            Assert.Equal(9, actual[0].Value.DiscountedAmount);
        }
        
        [Fact]
        public void ReadInvoices__With_Discount_But_Not_Allowed__Discount_Is_Not_Applied()
        {
            const string path = "invoice5.txt";
            var actual = InvoicesParser.ReadAndParseInvoices(new string[] {path}, 10, false).ToArray();

            Assert.Null(actual[0].Value.DiscountedAmount);
        }
    }
}
