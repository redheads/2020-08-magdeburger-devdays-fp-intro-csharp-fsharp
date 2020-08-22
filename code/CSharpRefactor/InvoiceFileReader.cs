using System;
using System.Collections.Generic;
using System.Linq;

namespace CSharpRefactor
{
    public class InvoiceFileReader
    {
        private static InterimResult<IEnumerable<string>> ReadInvoiceFromFile(string filePath)
        {
            try
            {
                var contents = System.IO.File.ReadLines(filePath).ToArray();
                return new InterimResult<IEnumerable<string>>(contents);
            }
            catch (Exception e)
            {
                return new InterimResult<IEnumerable<string>>(e.Message);
            }
        }

        public static IEnumerable<KeyValuePair<string, InterimResult<IEnumerable<string>>>> ReadInvoicesFromFiles(IEnumerable<string> invoiceFilePaths)
        {
            var results = new List<KeyValuePair<string, InterimResult<IEnumerable<string>>>>();
            foreach (var invoiceFilePath in invoiceFilePaths)
            {
                results.Add( 
                    new KeyValuePair<string, InterimResult<IEnumerable<string>>>(invoiceFilePath, 
                        ReadInvoiceFromFile(invoiceFilePath)
                    )
                );
            }

            return results;
        }
    }
}