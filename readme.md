# 1. Introduction

## 1.1 Objectifs produit
Le but de ce projet est de concevoir "Plot Those Lines", un logiciel permettant l'affichage, le stockage et l'analyse de séries temporelles.

## 1.2 Objectifs pédagogiques
Ce projet permet de pratiquer et de valider les notions vues en module.

## 1.2 Description du domaine 

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
