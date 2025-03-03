# Plateforme de Distribution de Jeux Vidéo

## Description
Ce projet est une plateforme permettant d’acheter, télécharger et jouer à des jeux vidéo. Il comprend un serveur web, une application cliente sous Windows, un serveur de jeu et un jeu multijoueur.

## Fonctionnalités
- **Gestion des jeux** : Liste des jeux disponibles, téléchargement et lancement.
- **Gestion des utilisateurs** : Inscription, connexion et suivi des jeux possédés.
- **Administration** : Ajout, modification et suppression de jeux et de catégories.
- **Jeu multijoueur** : Un serveur de jeu dédié pour gérer les sessions en ligne.
- **API REST** : Accès aux données pour la gestion des jeux et utilisateurs.

## Technologies utilisées
- **Backend** : ASP.Net Core
- **Client Windows** : WPF
- **Serveur de jeu** : C#
- **Jeu** : C# (Godot, Unity, WPF, MAUI...)

## Structure du Projet
- **Gauniv.WebServer** : Serveur Web et API REST
- **Gauniv.Client** : Application Windows pour naviguer et jouer
- **Gauniv.Network** : Communication entre client et serveur
- **Gauniv.GameServer** : Serveur de jeu multijoueur
- **Gauniv.Game** : Jeu multijoueur jouable

## Application (WPF, MAUI, WINUI)
L'application permet de :

-Lister les jeux avec pagination et filtres (par catégorie, prix, jeux possédés, etc.).
-Lister les jeux possédés avec pagination et filtres.
-Afficher les détails d’un jeu (nom, description, statut, catégories).
-Gérer les jeux : télécharger, supprimer et lancer.
-Les boutons "jouer" et "supprimer" sont invisibles si le jeu n'est pas téléchargé.
-Le bouton "télécharger" est invisible si le jeu est déjà disponible.


## Installation
1. Cloner le projet :
   ```sh
   git clone https://github.com/belahmed1/ProjetDotNet_BELKZIZ_BADDOU_DRHIMER.git
   ```
2. Restaurer les dépendances :
   ```sh
   dotnet restore
   ```
3. Compiler la solution :
   ```sh
   dotnet build
   ```
4. Lancer le serveur web :
   ```sh
   dotnet run --project Gauniv.WebServer
   ```
5. Lancer le client Windows :
   ```sh
   dotnet run --project Gauniv.Client
   ```
6. Lancer le serveur de jeu :
   ```sh
   dotnet run --project Gauniv.GameServer
   ```
7. Lancer le jeu :
   ```sh
   dot

**Identifiants Administrateur

	Username : Admin

	Password : AdminPassword123!
