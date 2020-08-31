using System;
using FluentAssertions;
using Xunit;

namespace CSharpRefactor.Tests
{
    public class KundenRepositoryTests
    {
        [Fact]
        public void KundeZumRepositoryHinzufegenFunktioniert()
        {
            var repository = new Kundenrepository();
            var kunde = new Kunde(1,"TestName","Test",new DateTime(1990,1,1));
            kunde.Id.Should().Be(1);
            kunde.Vorname.Should().Be("TestName");
            kunde.Spitzname.Should().Be("Test");
            kunde.Geburtsdatum.Should().Be(new DateTime(1990, 1, 1));
            repository.Add(kunde);

            repository.Count.Should().Be(1);
        }
        [Fact]
        public void KundeohneVornameUngültig()
        {
            Action action = () => new Kunde(1,"","Test",new DateTime(1990,1,1));
            action.Should().Throw<Exception>();
        }

        [Fact]
        public void Kundengruesse()
        {
            Func<string, string, string> GreetingFunc() => (vorname, spitzname) => $"{vorname} {spitzname}";

            var kunde = new Kunde(1, "TestVorname", "Ajax", new DateTime(1990, 1, 1));
            var result = kunde.Greetings(GreetingFunc());
            result.Should().ContainAll("TestVorname", "Ajax");
        }
    }
}