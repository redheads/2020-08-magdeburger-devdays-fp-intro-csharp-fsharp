using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class InvoicesParserTest
    {
        [Fact]
        public void ReadInvoices__No_Input_Files__Empty_Result()
        {
            var actual = InvoicesParser.ParseInvoices(
                new KeyValuePair<string, InterimResult<IEnumerable<string>>>[] { },  
                null, 
                null);
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Valid_Input_File_No_Discount__Amount_Is_Read()
        {
            const string path = "invoice";
            
            var actual = InvoicesParser.ParseInvoices(
                new[]
                {
                   new KeyValuePair<string, InterimResult<IEnumerable<string>>>(path, new InterimResult<IEnumerable<string>>(new[] { "42" })) 
                },
                null,
                null);
        
            var expected = new Dictionary<string, InvoiceParseResult>()
            {
                {path, new InvoiceParseResult(0, 42m, null)}
            };
            
            Assert.Equal(expected, actual);
        }
        
        [Fact]
        public void ReadInvoices__Input_File_Empty__Empty_Result()
        {
            const string path = "invoice";
            var actual = InvoicesParser.ParseInvoices(
                new[]
                {
                    new KeyValuePair<string, InterimResult<IEnumerable<string>>>(path, new InterimResult<IEnumerable<string>>(new string[] { })) 
                },
                 null, null);
            
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoices__Input_File_Too_Many_Lines__Empty_Result()
        {
            const string path = "invoice";
            var actual = InvoicesParser.ParseInvoices(
                new[]
                {
                    new KeyValuePair<string, InterimResult<IEnumerable<string>>>(path, new InterimResult<IEnumerable<string>>(new[] { "42", "7"})) 
                }, 
                null,
                null);
            
            Assert.Empty(actual);
        }
        
       [Fact]
        public void ReadInvoices__Invalid_File_Content__Error_Result()
        {
            const string path = "invoice";
            var actual = InvoicesParser.ParseInvoices(new[]
            {
                new KeyValuePair<string, InterimResult<IEnumerable<string>>>(path, new InterimResult<IEnumerable<string>>(new[] { "abc"})) 
            }
                , null, null).ToArray();
        
            Assert.Equal("Invoice amount abc could not be parsed", actual[0].Value.ErrorText);
        }
        
        [Fact]
        public void ReadInvoices__With_Discount__Discount_Is_Applied()
        {
            var invoice = CreateInvoice("10");
            var actual = InvoicesParser.ParseInvoices(
                invoice, 10, true
                ).ToArray();
        
            Assert.Equal(9, actual[0].Value.DiscountedAmount);
        }

        [Fact]
        public void ReadInvoices__With_Discount_But_Not_Allowed__Discount_Is_Not_Applied()
        {
            const string path = "invoice";
            var actual = InvoicesParser.ParseInvoices(
                new[]
                {
                    new KeyValuePair<string, InterimResult<IEnumerable<string>>>(path, new InterimResult<IEnumerable<string>>(new[] { "10"})) 
                }, 10, false).ToArray();
        
            Assert.Null(actual[0].Value.DiscountedAmount);
        }
        
        private static IEnumerable<KeyValuePair<string, InterimResult<IEnumerable<string>>>> CreateInvoice(string content, string secondContent = null)
        {
            return new[]
            {
                new KeyValuePair<string, InterimResult<IEnumerable<string>>>("invoice", 
                    new InterimResult<IEnumerable<string>>(
                        secondContent != null
                            ? new[] { content, secondContent }
                            : new[] { content }
                    )) 
            };
        }
    }
}
