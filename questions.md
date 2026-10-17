# Questions de compréhension

## Question 1 - TODO

**Passage** : MainWindow.axaml.cs:67

```csharp
// TODO : lecture synchrone sur le thread de l'UI, un gros fichier bloquerait la fenêtre
```

Expliquer le problème; que faut-il faire pour traiter ce TODO ?

---

## Question 2 – LINQ

**Passage** : `src/SpaceWeather.App/Extensions/TimeSeriesPointExtensions.cs`, lignes 12-37

```csharp
var entries = points
    .Zip(points.Skip(1), (prev, curr) => (prev, curr))
    .SelectMany(pair => ToPlotEntries(pair.prev, pair.curr, breakThreshold))
    .Prepend((X: points[0].Timestamp.ToOADate(), Y: points[0].Value))
    .ToList();
...
if (curr.Timestamp - prev.Timestamp > breakThreshold)
    yield return (curr.Timestamp.ToOADate(), double.NaN);
yield return (curr.Timestamp.ToOADate(), curr.Value);
```

- Expliquez ce que fait .Zip ? Quel paramètre doit-on lui passer ?
- Que fait `yield return` ? Quand le code de `ToPlotEntries` est-il réellement exécuté ?
- Que se passe-t-il si la liste ne contient qu'un seul point ?
- Pourquoi `SelectMany` et pas `Select` ?

---

## Question 3 – Barre de défilement

**Passage** : `src/SpaceWeather.App/Views/MainWindow.axaml.cs`, ligne 52, et lignes 169-187

```csharp
PlotControl.Plot.RenderManager.AxisLimitsChanged += (_, _) => Dispatcher.UIThread.Post(UpdateScrollBar);
```

**Questions** :

- Expliquez ce que fait ce code, en particulier la syntaxe le `+=`, `(_, _)` et `Post`

---

## Question 4

**Passage** : `TimeSeriesPointExtensions:12`

```csharp
    public static (double[] Xs, double[] Ys) ToPlotArrays(this List<TimeSeriesPoint> points, TimeSpan breakThreshold)
```

- Que fait ce "this" dans la liste des paramètres ?
