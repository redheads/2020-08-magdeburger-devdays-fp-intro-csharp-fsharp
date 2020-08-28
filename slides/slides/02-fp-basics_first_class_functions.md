## FP 101 - functions

### 1st class functions in C# #

Funktionen können als Parameter verwendet werden

---

...Ähnlichkeit mit Interfaces in der OO-Welt...

----

Strategy-Pattern

```csharp
interface ICalculateSalary
{
  int ByInput(int i);            // <- Methodensignatur
}

class Manager: ICalculateSalary
{
  int ByInput(int i) => i*2;     // <- Implementierung
}
```

<pre><code data-noescape data-trim class="lang-csharp hljs">
class SomeService
{
  int DoSomething(<span class="highlightcode">ICalculateSalary salary</span>, int i) 
    => <span class="highlightcode">salary</span>.ByInput(i);        // <- "deligiert"
}
</code></pre>

(Verhalten als Parameter uebergeben)

----

Factory-Pattern

<pre><code data-noescape data-trim class="lang-csharp hljs">
interface IWorkerEfficiency { }

class BestWorker: IWorkerEfficiency { }
class NormalWorker: IWorkerEfficiency { }

class WorkerEfficiencyFactory
{
  static <span class="highlightcode">IWorkerEfficiency</span> Get(string name)
  {
    return name.Contains("magdeburg")
      ? new BestWorker()
      : new NormalWorker();
  }
}
</code></pre>

(Verhalten wird zurueckgegeben)

----

...Interfaces, aber auf Methoden-Ebene...

Verhalten (=Signatur) einer Funktion kann in C# auch direkt beschrieben werden: 

`Func<input1, inputN, result>`

---

### 1st class functions in C# #

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
