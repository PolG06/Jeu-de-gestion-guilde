## Gestion d'une guilde en C# avec l'utilisation des Templates Avalonia, incluant plusieurs fonctionnalités

## À propos

Il s'agit de mon projet de la fin de mon module C# de ma 2ème année de Bachelor en Informatique, ce projet est l'aboutissement d'une semaine d'apprentissage. Le but de ce projet est de gérer une guilde de soldats, d’effectuer des missions et de gérer les ressources de la guilde.

## Fonctionnalités principales:
- Utilisation d’interfaces C# et de bibliothèques standards .NET
- Utilisation de POO et du principe d'héritage
- Interface graphique interactive avec un menu et plusieurs fenêtres
- Utilisation du pattern Singleton, de constructeurs privés et de l’encapsulation des variables
- Utilisation de getters/setters

## Prérequis pour l'utilisation:
- JetBrains Rider 2025.2.4
- SDK .NET 7.0 minimum
- Templates Avalonia


## Comment utiliser le projet:

1-Dans un terminal, entrer la commande: git clone suivi du lien dans la barre d'adresse

2-Installer JetBrains Rider 2025.2.4: https://www.jetbrains.com/idea/download/?section=windows

3-Installer SDK.NET: https://dotnet.microsoft.com/en-us/download

4-Installer le framework Avalonia, dans un terminal powershell, taper la commande: dotnet new install Avalonia.Templates

5-Ouvrir le fichier dans JetBrains

6-Pour le lancer, cliquer sur la flèche ou faire Ctrl + F5

## Fonctionnement du jeu

Le joueur doit gérer sa guilde, faire des missions afin de gagner de l'argent et de pourvoir acheter des objets, de la nourriture. Il est possible d’améliorer les soldats et d’en recruter de nouveaux.

L'objectif est que la partie dure le plus longtemps possible avec des mission qui deviennent de plus en plus longues et compliquées. 

Un soldat peut mourir lors d'une mission, suite à quoi il ne pourra pas revivre. Il a également un état de fatigue qui fait qu'il ne pourra pas enchainer les missions. Au cours de celles-ci, il peut également se blesser, l'empêchant de pouvoir repartir en mission par la suite. En accomplissant une mission, la guilde reçoit de l'argent et un objet en récompense. 

Des évènements indésirables (vol, incendie, tempêtes) peuvent arriver chaque jour, sabotant une partie de la guilde. 

Il existe plusieurs types de soldats, d'objets, chacun ayant ses caractéristiques propres. Lorsqu'ils ont atteint un certain nombre d'exp, ils sont améliorés, leurs statistiques s'améliorent mais leurs salaires, et leur demande en nourriture aussi. Il est possible pour chaque type de soldat, de leur donner un équipement militaire uniquement, ce qui leur donne un bonus permanent

Les objets de l'inventaire peuvent être vendus chez le marchand.

Pour passer au jour suivant, une quantité minimale de ressources est requise pour pouvoir nourrir et payer les soldats de la guilde.

La partie se termine lorsqu'aucun des soldats de la guilde n'est en capacité d'être envoyé en mission ou que la guilde n'a pas assez d'argent ou de nourriture pour payer et nourrir tous ses soldats le jour suivant. 

## Technologies utilisées:
- C#
- .NET 7.0
- Avalonia UI
- JetBrains Rider