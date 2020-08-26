## FP 101 - Pure Functions

### Pure Functions in C# #

- haben niemals Seiteneffekte!
- sollten immer nach `static` umwandelbar sein

---

## Imperativ...

**Wie** mache ich etwas 

```csharp
var people = new List&lt;Person&gt;
{
    new Person { Age = 20, Income = 1000 },
    new Person { Age = 26, Income = 1100 },
    new Person { Age = 35, Income = 1300 }
};

var incomes = new List&lt;int&gt;();
foreach (var person in people)
{
    if (person.Age &gt; 25)
        incomes.Add(person.Income);
}

var avg = incomes.Sum() / incomes.Count;
```

versus...

----

## Deklarativ

**Was** will ich erreichen?

Bsp: Filter / Map / Reduce

<pre><code data-noescape data-trim class="lang-csharp hljs">
var people = new List&lt;Person&gt; {
  new Person { Age = 20, Income = 1000 },
  new Person { Age = 26, Income = 1100 },
  new Person { Age = 35, Income = 1300 }
}

var averageIncomeAbove25 = people
  .<span class="highlightcode">Where</span>(p => p.Age > 25) // <span class="highlightcode">"Filter"</span>
  .<span class="highlightcode">Select</span>(p => p.Income)  // <span class="highlightcode">"Map"</span>
  .<span class="highlightcode">Average</span>();             // <span class="highlightcode">"Reduce"</span>
</code></pre>

- aussagekräftiger
- weniger fehleranfällig

---

<!-- .slide: data-background="images/fp-languages-overview.png" data-background-size="contain" -->

Note:
csharp und java haben beide mehr FP features. Man kann aber immer RotzCode in den Sprachen schreiben.
Sprachen, die einem es schwermachen, RotzCode zu schreiben, sind besser (Stichwort: Goto considered harmful).

---

Schränken uns diese FP Paradigmen ein?

---

Wie kann man mit diesem "Purismus" Software schreiben, die etwas tut?

---

## Kleine Funktionen zu größeren verbinden

- Gängige Vorgehensweise: Kleine Funktionen werden zu immer größeren Funktionalitäten zusammengesteckt
- Problem: Nicht alle Funktionen passen gut zusammen
