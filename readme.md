# 1. Introduction

## 1.1 Objectifs produit
Le but de ce projet est de concevoir "Plot Those Lines", un logiciel permettant l'affichage, le stockage et l'analyse de séries temporelles.

## 1.2 Objectifs pédagogiques
Ce projet permet de pratiquer et de valider les notions vues en module : la programmation fonctionnelle avec LINQ, les méthodes d'extension, les tests unitaires et la séparation du code en couches.

## 1.3 Description du domaine 

### ___Domaine d'application___
Ce projet s'inscrit dans le domaine de la météo spatiale. Cette discipline étudie l'activité du Soleil et ses interactions avec l'environnement terrestre. Lors d'éruptions solaires majeures, le Soleil expulse des tempêtes de radiations dangereuses pour les astronautes, les satellites et les passagers des vols commerciaux polaires. L'objectif de notre logiciel est de permettre l'analyse visuelle de ces tempêtes de radiations.

### ___Sources de données___
Pour alimenter l'application, les données sont extraites des serveurs publics de la _NOAA_. Les informations sont récupérées sous forme de fichiers JSON, actualisés régulièrement.

### ___Séries temporelles exploitées___
Afin d'offrir une analyse lisible et pertinente, j'ai sélectionné 5 séries temporelles issues du même fichier de relevés, qui partagent toutes la même unité de mesure mais mesurent des niveaux d'énergie différents :
* **Protons >= 1 MeV** : Flux de protons à faible énergie.
* **Protons >= 5 MeV** : Flux de protons à énergie modérée.
* **Protons >= 10 MeV** : Flux critique utilisé par la NOAA pour déclencher les alertes de tempêtes de radiations mondiales.
* **Protons >= 50 MeV** : Flux de protons à haute énergie.
* **Protons >= 100 MeV** : Flux de protons à très haute énergie qui peuvent pénétrer la coque des engins spaciaux.

### ___Cohérence des séries___
Les séries proviennent du même satellite, ont la même unité et la même cadence. Les afficher sur le même graphique permet de voir comment une tempête se propage d'un niveau d'énergie à l'autre : les protons de faible énergie augmentent presque toujours, mais seules les tempêtes les plus fortes font aussi monter les séries à haute énergie.

---

# 2. Analyse fonctionnelle

- **[US1](https://github.com/alberrboyyy/space-weather/issues/1)***
- **[US2](https://github.com/alberrboyyy/space-weather/issues/2)**
- **[US3](https://github.com/alberrboyyy/space-weather/issues/3)**
- **[US4](https://github.com/alberrboyyy/space-weather/issues/4)**

---

# 3. Planification initiale
- **Semaine 1 :** Recherche du sujet et recherche des données
- **Semaine 2 :** Création des US, du rapport et de la maquette
- **Semaine 3 :** US1
- **Semaine 4 :** US2
- **Semaine 5 :** US3
- **Semaine 6 :** US4
- **Semaine 7 :** US4 (Eventuellement future US5)
- **Semaine 8 :** Retouches et livraison

---

# 4. Conception technique

## 4.1 Technologies
- **.NET 8 / C#** : langage imposé par le module.
- **Avalonia 11** : interface graphique multiplateforme.
- **ScottPlot 5** : affichage des courbes, avec zoom et déplacement intégrés.
- **CommunityToolkit.Mvvm** : simplifie l'écriture des ViewModels.

## 4.2 Structure du projet
La solution est séparée en deux projets :

- **SpaceWeather.Core** : la logique, sans aucune dépendance à l'interface.
  - `Models` : les séries temporelles (`TimeSeries`, `TimeSeriesPoint`) et le format brut du JSON NOAA.
  - `Import` : lecture d'un fichier JSON NOAA et regroupement des mesures en séries par niveau d'énergie.
  - `Storage` : sauvegarde, chargement et fusion des séries stockées localement.
- **SpaceWeather.App** : l'application Avalonia, organisée en MVVM.
  - `Views` : la fenêtre principale et le graphique ScottPlot.
  - `ViewModels` : l'état de l'interface (liste des séries, cases cochées, messages d'erreur).
  - `Extensions` : conversion des points en tableaux pour ScottPlot.

## 4.3 Fonctionnement
- **Import** : depuis un fichier JSON ou directement depuis l'API de la NOAA (données des 7 derniers jours).
- **Fusion** : un nouvel import complète les séries existantes au lieu de les remplacer, les doublons (même date) sont éliminés.
- **Stockage local** : les séries sont enregistrées dans un fichier `series.json` dans le dossier de données de l'utilisateur, et rechargées au démarrage.
- **Affichage** : chaque série peut être cochée ou décochée. Quand il manque des mesures, la courbe est coupée au lieu de relier deux points éloignés.

---

# 5. Usage de l'IA

J'ai utilisé _Claude Code_ comme une **documentation interactive**, centrée sur le *pourquoi* des choix techniques plutôt que comme un générateur de code autonome.

Ma méthode était toujours la même : je décrivais le besoin, l'IA proposait une approche et m'expliquait son raisonnement, je posais des questions sur tout ce que je ne comprenais pas, et le code n'était intégré qu'une fois l'approche validée. Je relisais ensuite chaque modification pour pouvoir l'expliquer.

L'IA m'a aidé à mettre en place la stack technique (Avalonia + ScottPlot sur .NET 8), à concevoir le pipeline de données (import des fichiers JSON de la NOAA avec LINQ, stockage local) et à relier l'interface MVVM au graphique ScottPlot.

Les choix d'architecture sont les miens. L'IA a aussi ses limites : certaines de ses propositions étaient plus complexes que nécessaire pour ce projet, et j'ai dû les simplifier. Il fallait donc garder un regard critique plutôt qu'accepter tout ce qu'elle proposait.

Avec le recul, cette façon de travailler m'a permis d'apprendre plus vite, tout en restant capable d'expliquer chaque partie du code.
