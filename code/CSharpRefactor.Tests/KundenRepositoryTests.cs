using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace CSharpRefactor.Tests
{
    public class KundenRepositoryTests
    {
        [Fact]
        public void KundeohneVornameUngültig()
        {
            Action action = () => new Kunde(1,"","Test",new DateTime(1990,1,1));
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Kundengruesse()
        {
            Func<string, Option<string>, string> GreetingFunc() 
                => (vorname, spitzname) =>
                {
                    return spitzname.Match(
                    None: () => vorname,
                    Some: (s) => $"{vorname} {s}"
                    );
                };

            var kunde = new Kunde(1, "TestVorname", "Ajax", new DateTime(1990, 1, 1));
            var result = kunde.Greetings(GreetingFunc());
            result.Should().ContainAll("TestVorname", "Ajax");
        }

        [Fact]
        public void KundenGeburtstageListe()
        {
            bool FilterFunc(Kunde k) => k.Geburtsdatum == new DateTime(1990, 1, 1);
            
            var liste = new List<Kunde>()
            {
                new Kunde(1, "A", null, new DateTime(1990, 1, 1)),
                new Kunde(2, "B", null, new DateTime(1990, 1, 1)),
                new Kunde(3, "C", null, new DateTime(1991, 2, 3))
            };
            
            
            var result = liste.Where(FilterFunc)
                .Select(k => 1)
                .Count();
            result.Should().Be(2);

            var result2 = liste.Count(FilterFunc);
            result2.Should().Be(2);
        }
    }
}