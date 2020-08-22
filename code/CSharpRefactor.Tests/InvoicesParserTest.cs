using System;
using System.Collections.Generic;
using System.Linq;
using LaYumba.Functional;
using static LaYumba.Functional.F;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class InvoicesParserTest
    {
        private static void AssertValid<T>(Validation<T> actual, Action<T> assertion)
        {
            actual.Match(
                Invalid: (_) => Assert.True(false, $"Expected Valid, but got Invalid: {actual}"),
                Valid: assertion);
        }
        
        private static void AssertInvalid<T>(Validation<T> actual, Action<IEnumerable<string>> assertionWithErrorMessages)
        {
            actual.Match(
                Invalid: (errs) => assertionWithErrorMessages(errs.Select(err => err.Message)),
                Valid: (_) => Assert.True(false, $"Expected Invalid, but got Valid: {actual}")
                );
        }
        
       [Fact]
       public void ReadInvoices__No_Input_Files__Sum_Is_0()
       {
           var actual = InvoicesParser.ParseInvoices(
               new Validation<IEnumerable<string>>[0],  
               None, 
               None);
           
           var expected = new InvoicesSum(0, 0);
           
           AssertValid(actual, 
               (val) => Assert.Equal(expected, val));
       }
       
       [Fact]
       public void ReadInvoices__Valid_Input_File_No_Discount__Amount_Is_Read()
       {
           var actual = InvoicesParser.ParseInvoices(
               new[]
               {
                  CreateRawInvoice("42") 
               },
               None,
               None);
       
           var expected = new InvoicesSum(42, 0);
           
           AssertValid(actual,
               (val) => Assert.Equal(expected, val));
       }
       
       [Fact]
       public void ReadInvoices__Input_File_Empty__Error_Message()
       {
           var actual = InvoicesParser.ParseInvoices(
               new[]
               {
                   CreateRawInvoice("")
               },
                None, None);
           
           var expected = new string[] {"Invoice amount  could not be parsed" };
           
           AssertInvalid(actual, 
               (errs) => Assert.Equal(expected, errs));
       }
       
       [Fact]
       public void ReadInvoices__Input_File_Too_Many_Lines__Empty_Result()
       {
           var actual = InvoicesParser.ParseInvoices(
               new[]
               {
                   CreateRawInvoice("42", "7"),  
               }, 
               None,
               None);
           
           var expected = new string[] {"Line count not exactly 1" };
           
           AssertInvalid(actual, 
               (errs) => Assert.Equal(expected, errs));
       }
       
       [Fact]
        public void ReadInvoices__Invalid_File_Content__Error_Result()
        {
            const string path = "invoice";
            var actual = InvoicesParser.ParseInvoices(new[]
            {
                CreateRawInvoice("abc") 
            }
                , None, None);
        
            var expected = new string[] {"Invoice amount abc could not be parsed" };
           
            AssertInvalid(actual, 
                (errs) => Assert.Equal(expected, errs));
        }
        
        [Fact]
        public void ReadInvoices__Invalid_Contents__Errors_Are_Collected()
        {
            var actual = InvoicesParser.ParseInvoices(new[]
                {
                    CreateRawInvoice("abc"),
                    CreateRawInvoice("42", "7"),
                }
                , None, None);
        
            var expected = new string[] {"Invoice amount abc could not be parsed", "Line count not exactly 1" };
           
            AssertInvalid(actual, 
                (errs) => Assert.Equal(expected, errs));
        }
        
       [Fact]
       public void ReadInvoices__With_Discount__Discount_Is_Applied()
       {
           var actual = InvoicesParser.ParseInvoices(
               new[]
               {
                   CreateRawInvoice("10"),
               },
               10, 
               true
               );
       
           var expected = new InvoicesSum(10, 9);
           
           AssertValid(actual,
               (val) => Assert.Equal(expected, val));
       }
       
       [Fact]
       public void ReadInvoices__Two_Invoices_With_Discount__Discount_Is_Applied_And_Sum_Is_Correct()
       {
           var actual = InvoicesParser.ParseInvoices(
               new[]
               {
                   CreateRawInvoice("10"),
                   CreateRawInvoice("10"),
               },
               10, 
               true
           );
       
           var expected = new InvoicesSum(20, 18);
           
           AssertValid(actual,
               (val) => Assert.Equal(expected, val));
       }
       
       [Fact]
       public void ReadInvoices__With_Discount_But_Not_Allowed__Discount_Is_Not_Applied()
       {
           var actual = InvoicesParser.ParseInvoices(
               new[]
               {
                   CreateRawInvoice("10"),
               },
               10, 
               false
           );
       
           // this is up for debate: if there is a discountPercentage, but the discount is not allowed
           // we calculate a discountedSum anyway (which is just the normal sum) - but if any of these is None, the 
           // discountedSum stays at 0. Should it be the same as the normal sum then, too?
           var expected = new InvoicesSum(10, 10);
           
           AssertValid(actual,
               (val) => Assert.Equal(expected, val));
       }
       
       private static Validation<IEnumerable<string>> CreateRawInvoice(string content, string secondContent = null)
       {
           return 
               secondContent != null
                   ? Valid((IEnumerable<string>)new[] {content, secondContent})
                   : Valid((IEnumerable<string>)new[] {content});
           
       }
    }
}
