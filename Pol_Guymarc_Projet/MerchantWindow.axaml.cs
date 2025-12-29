// importation des bibliothèques
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;

//importation de nos classes
namespace Pol_Guymarc_Projet
{
    // classe représentant la fenêtre principale du marchand
    public partial class MerchantWindow : Window
    {
        // instance unique de la fenêtre (Singleton)
        private static MerchantWindow? _instance;
        // référence vers la guilde actuelle
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static MerchantWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new MerchantWindow(guilde);
            }

            return _instance;
        }

        // création forcée d'une nouvelle fenêtre (sans singleton)
        public static MerchantWindow CreateNew(Guilde guilde)
        {
            return new MerchantWindow(guilde);
        }

        // Constructeur privé de la fenêtre
        private MerchantWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);
            _instance = null; 
            // permet de recréer la fenêtre ultérieurement
        }

        private void BackToMainMenu(object? sender, RoutedEventArgs e)
        // retour au menu principal du jeu
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        private void BuyNewSoldier(object? sender, RoutedEventArgs e)
        // ouverture de la fenêtre du marchand de soldats
        {
            var soldierMerchantWindow = MerchantSoldierWindow.GetInstance(_guilde);
            soldierMerchantWindow.Show();
            Close();
        }

        private void BuyNewObject(object? sender, RoutedEventArgs e)
        // ouverture de la fenêtre du marchand d'objets
        {
            var objectMerchantWindow = MerchantObjectWindow.GetInstance(_guilde);
            objectMerchantWindow.Show();
            Close();
        }

        private void BuyMoreFood(object? sender, RoutedEventArgs e)
        // ouverture de la fenêtre du marchand de nourriture
        {
            var foodMerchantWindow = MerchantFoodWindow.GetInstance(_guilde);
            foodMerchantWindow.Show();
            Close();
        }
    }
}
