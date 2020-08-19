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
        
        [Fact]
        public void ReadInvoices__Input_File_Empty__Empty_Result()
        {
            const string path = "invoice2.txt";
            var sut = new InvoicesParser(new string[] {path}, null, null);
            var actual = sut.ReadAndParseInvoices();
            
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Input_File_Too_Many_Lines__Empty_Result()
        {
            const string path = "invoice3.txt";
            var sut = new InvoicesParser(new string[] {path}, null, null);
            var actual = sut.ReadAndParseInvoices();
            
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__NonExisting_Input__Error_Result()
        {
            const string path = "invoice_abc.txt";
            var sut = new InvoicesParser(new string[] {path}, null, null);
            var actual = sut.ReadAndParseInvoices().ToArray();

            Assert.Contains("Could not find file", actual[0].Value.ErrorText);
        }
        
        [Fact]
        public void ReadInvoices__Invalid_File_Content__Error_Result()
        {
            const string path = "invoice4.txt";
            var sut = new InvoicesParser(new string[] {path}, null, null);
            var actual = sut.ReadAndParseInvoices().ToArray();

            Assert.Equal("Invoice amount abc could not be parsed", actual[0].Value.ErrorText);
        }
        
        [Fact]
        public void ReadInvoices__With_Discount__Discount_Is_Applied()
        {
            const string path = "invoice5.txt";
            var sut = new InvoicesParser(new string[] {path}, 10, true);
            var actual = sut.ReadAndParseInvoices().ToArray();

            Assert.Equal(9, actual[0].Value.DiscountedAmount);
        }
        
        [Fact]
        public void ReadInvoices__With_Discount_But_Not_Allowed__Discount_Is_Not_Applied()
        {
            const string path = "invoice5.txt";
            var sut = new InvoicesParser(new string[] {path}, 10, false);
            var actual = sut.ReadAndParseInvoices().ToArray();

            Assert.Null(actual[0].Value.DiscountedAmount);
        }
    }
}
