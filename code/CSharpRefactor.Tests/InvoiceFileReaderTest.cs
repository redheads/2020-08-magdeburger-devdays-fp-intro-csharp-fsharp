using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class InvoiceFileReaderTest
    {
        [Fact]
        public static void ReadInvoicesFromFiles__No_Input__Empty_Result()
        {
            var actual = InvoiceFileReader.ReadInvoicesFromFiles(new string[] {});
            Assert.Empty(actual);
        }
        
        [Fact]
        public void ReadInvoicesFromFiles__Valid_Input_File__Content_Is_Read()
        {
            const string path = "invoice1.txt";
            var actual = InvoiceFileReader.ReadInvoicesFromFiles(new string[] {path});

            var expected = new List<KeyValuePair<string, InterimResult<IEnumerable<string>>>>() {
                new KeyValuePair<string, InterimResult<IEnumerable<string>>>(
                    path,
                    new InterimResult<IEnumerable<string>>(new string[] {"42"}))
            };
            
            
            Assert.Equal(expected.Select(x => x.Value.Contents), actual.Select(x => x.Value.Contents));
        }
        
        [Fact]
        public void ReadInvoicesFromFiles__NonExisting_Input__Error_Result()
        {
            const string path = "invoice_abc.txt";
            var actual = InvoiceFileReader.ReadInvoicesFromFiles(new string[] {path}).ToArray();

            Assert.Contains("Could not find file", actual[0].Value.ErrorText);
        }
    }
}