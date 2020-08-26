## Mögliches Vorhandensein eines Werts

#### oder: null muss weg.

----

```csharp
// Enthält die Signatur die ganze Wahrheit?
public string Stringify&lt;T&gt;(T data)
{
    return null;
}
```

----

```csharp
// Sind Magic Values eine gute Idee?
public int Intify(string s)
{
    int result = -1;
    int.TryParse(s, out result);
    return result;
}
```

----

```csharp
public class Data
{
    public string Name;
}

public class Do
{
    public Data CreateData() => null;

    public string CreateAndUseData()
    {
        var data = CreateData();
        // kein null-Check -> ist dem Compiler egal
        return data.Name;
    }
}
```

----

## Option

```fsharp
type Option&lt;'T&gt; = Some&lt;'T&gt; | None
```

- entweder ein Wert ist da - dann ist er in "Some" eingepackt
- oder es ist kein Wert da, dann gibt es ein leeres "None"
- alternative Bezeichnungen: Optional, Maybe

----

## Mit Option

```csharp
public Option&lt;int&gt; IntifyOption(string s)
{
    int result = -1;
    bool success = int.TryParse(s, out result);
    return success ? Some(result) : None;
}
```

----

### Wie komme ich an einen eingepackten Wert ran?

> Pattern matching allows you to match a value against some patterns to select a branch of the code.

```csharp
public string Stringify&lt;T&gt;(Option&lt;T&gt; data)
{
    return data.Match(
        None: () => "",
        Some: (existingData) => existingData.ToString()
    );
}
```

----

### Vorteile

- Explizite Semantik: Wert ist da - oder eben nicht
- Auch für Nicht-Programmierer verständlich(er): "optional" vs. "nullable"
- Die Signatur von Match erzwingt eine Behandlung beider Fälle - nie wieder vergessene Null-Checks!
- Achtung: In C# bleibt das Problem, dass "Option" auch ein Objekt ist - und daher selbst null sein kann

---

In FP unterscheidet man die Art der Wrapper-Klassen (z.B. IEnumerable) anhand der Funktionen, die sie bereitstellen
