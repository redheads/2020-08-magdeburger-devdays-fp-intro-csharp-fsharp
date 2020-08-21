## FP 101

- **Functions as First Class Citizens**
- Immutability
- Pure Functions (see Immutability)

That's it!

---

#### Immutability in C# #

```csharp
public class Customer
{
  public string Name { get; set; } // set -> mutable :-(
}
```

vs

```csharp
public class Customer
{
  public Customer(string name)
  {
    Name = name;
  }
  
  public string Name { get; } // <- immutable
}
```

---

Syntax matters!

Classic C#

```csharp
int Add(int a, int b)
{
  return a + b;
}
```

Expression body

```csharp
int Add(int a, int b) => a + b;
```

---

Syntax matters!

Classic C#

```csharp
int Add(int a, int b)
{
  Console.WriteLine("bla"); // <- side effect!
  return a + b;
}
```

Expression body: side effects are less likely

```csharp
int Add(int a, int b) => a + b;
```

---

#### 1st class functions in C# #

Funktionen können als Parameter verwendet werden.

Ä2hnlichkeit mit Interfaces beim OO Strategy-Pattern.

<pre><code data-noescape data-trim class="lang-csharp hljs">
// Func as input parameter
public string Greet(<span class="highlightcode">Func&lt;string, string&gt; greeterFunction</span>, string name)
{
  return <span class="highlightcode">greeterFunction</span>(name);
}
</code></pre>

<pre><code data-noescape data-trim class="lang-csharp hljs">
// Func as return value
<span class="highlightcode">Func&lt;string, string&gt; formatGreeting</span> = name => $"Hello, {name}";
var greetingMessage = Greet(formatGreeting, "Magdeburg");
// -> greetingMessage: "Hello, Magdeburg"
</code></pre>

---

#### Pure Functions in C# #

- haben niemals Seiteneffekte!
- sollten immer nach `static` umwandelbar sein

---

### Imperativ...

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
    {
        incomes.Add(person.Income);
    }
}

var avg = incomes.Sum() / incomes.Count;
```

versus...

----

### Deklarativ

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

---

Schränken uns diese FP Paradigmen ein?

---

Wie kann man mit diesem "Purismus" Software schreiben, die etwas tut?

---

## Kleine Funktionen zu größeren verbinden

- Gängige Vorgehensweise: Kleine Funktionen werden zu immer größeren Funktionalitäten zusammengesteckt
- Problem: Nicht alle Funktionen passen gut zusammen
