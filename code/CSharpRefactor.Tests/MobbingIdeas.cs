using System;
using System.Collections.Generic;
using Xunit;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace CSharpRefactor.Tests
{
    public class Kontakt
    {
        public int Id { get; set; }
        public string Vorname { get; set; }
        public string Spitzname { get; set; }
        public DateTime Geburtstag { get; set; }

        public Option<Adresse> AdresseOpt { get; set; }

        public Option<bool> BonusGeschenk
        {
            get
            {
                return AdresseOpt.Map(x => x.Contains("Magdeburg"));
            }
        }

        public string Gruesse(Func<string, string, string> erzeugeTextFn)
        {
            return erzeugeTextFn(Vorname, Spitzname);
        }
    }

    public class Adresse
    {
        public string Value { get; set; }
    }

    public class SomeService
    {
        private readonly Dictionary<int, Kontakt> _sammlung = new Dictionary<int, Kontakt>();
        public void DoSomething()
        {
            var benutzer = new Kontakt
            {
                Vorname = "foo",
                Spitzname = "foo",
                Geburtstag = DateTime.Now
            };
            //
            // var dateOfBirth = person.DateOfBirth;
            // var tenDaysLater = dateOfBirth.AddDays(10);
            //
            
            _sammlung.Add(benutzer.Id, benutzer);

            benutzer.Vorname = "huhu";
        }
    }

    public class BenutzerTests
    {
        [Fact]
        public void MutateBenutzer()
        {
            var benutzer = new Kontakt
            {
                Vorname = "foo",
                Spitzname = "foo",
                Geburtstag = DateTime.Now
            };

            var b2 = benutzer;
            b2.Vorname = "xxx";
            
            Assert.Equal("foo", benutzer.Vorname);
        }

        [Fact]
        public void Begruessungen()
        {
            Func<string, string, string> standard = (a, b) => a + b;
            Func<string, string, string> deluxe = (a, b) => "delux";
            Func<string, string, string> debug = (a, b) => "debug";
            
            var benutzer = new Kontakt
            {
                Vorname = "foo",
                Spitzname = "foo",
                Geburtstag = DateTime.Now
            };
            
            Assert.Equal("foofoo", benutzer.Gruesse(standard));
            Assert.Equal("delux", benutzer.Gruesse(deluxe));
            Assert.Equal("debug", benutzer.Gruesse(debug));
        }
    }

}