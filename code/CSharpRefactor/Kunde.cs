using System;
using System.Collections.Generic;

namespace CSharpRefactor
{
    public class Kunde
    {
        public int Id { get; }
        public string Vorname { get; }
        public string Spitzname { get; }
        public DateTime Geburtsdatum { get; }

        public Kunde(int id, string vorname, string spitzname, DateTime geburtsdatum)
        {
            if (string.IsNullOrWhiteSpace(vorname))
            {
                throw new Exception();
            }

            Id = id;
            Vorname = vorname;
            Spitzname = spitzname;
            Geburtsdatum = geburtsdatum;
        }

        public string Greetings(Func<string, string, string> greetingFunc) =>
            greetingFunc(Vorname, Spitzname);
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