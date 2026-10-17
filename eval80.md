# Évaluation intermédiaire (80 %) – Albert Braimi

> Mise à jour du 16.10.2026 : les méthodes d'extension ne sont **pas exigées** (notion non vue en cours), même si le CdC les mentionne

---

## En bref

Projet propre et bien structuré (séparation Core / App, MVVM, commentaires utiles, dette technique identifiée par des `// TODO`). Les fonctionnalités principales sont là : import fichier + API NOAA, fusion sans doublons, stockage local, affichage multi-séries avec cases à cocher, coupure des courbes en cas de trou dans les données. **Il manque des exigences techniques du CdC** : aucun test unitaire, quelques `foreach` restants, et le rapport ne contient encore aucune des sections de la livraison finale.

## Axes de travail prioritaires pour la finalisation

1. **Créer un projet de tests** (xUnit ou MSTest) avec au moins 3 tests significatifs sur `SpaceWeather.Core` : `Merge` (doublons, tri, conservation de la dernière valeur), `ImportFromJson` (regroupement par énergie), `ToPlotArrays` (insertion de `NaN` sur un trou).
2. **Remplacer les `foreach` restants** (`LoadSeries`, `RefreshPlot`) par du LINQ quand c'est pertinent.
3. **Compléter le rapport** : rapport de tests, bilan du déroulement, bilan produit (prévu / réalisé), conclusion. Intégrer les US, les tests d'acceptation et les maquettes directement dans le rapport, pas seulement des liens.
4. **Enrichir la section IA** avec 2 ou 3 exemples concrets (prompt, réponse, ce qui a été gardé ou modifié).
5. JdT : ajouter les liens vers les commits et compléter les descriptions vides.
