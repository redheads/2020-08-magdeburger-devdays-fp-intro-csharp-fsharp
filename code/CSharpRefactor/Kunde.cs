using System;
using System.Collections.Generic;
using LaYumba.Functional;
using static LaYumba.Functional.F;

namespace CSharpRefactor
{
    public class Kunde
    {
        public int Id { get; }
        public string Vorname { get; }
        public Option<string> Spitzname { get; }
        public DateTime Geburtsdatum { get; }

        public Kunde(int id, string vorname, string spitzname, DateTime geburtsdatum)
        {
            if (string.IsNullOrWhiteSpace(vorname))
            {
                throw new Exception();
            }

            Id = id;
            Vorname = vorname;
            Spitzname = string.IsNullOrWhiteSpace(spitzname) ?None : Some(spitzname);
            Geburtsdatum = geburtsdatum;
        }

        public string Greetings(Func<string, Option<string>, string> greetingFunc) =>
            greetingFunc(Vorname, Spitzname);

        public static string GreetingsStatic(Func<string, Option<string>, string> greetingFunc, Kunde kunde) =>
            greetingFunc(kunde.Vorname, kunde.Spitzname);
    }

    public class Kundenrepository
    {
        private List<Kunde> Kunden { get; set; } = new List<Kunde>();
        public int Count => Kunden.Count;

        public void Add(Kunde kunde)
        {
            Kunden.Add(kunde);
        }
    }
}