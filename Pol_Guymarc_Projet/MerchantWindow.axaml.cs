using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;


namespace Pol_Guymarc_Projet
{
    public partial class MerchantWindow : Window
    {
        private static MerchantWindow? _instance;
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

        public static MerchantWindow CreateNew(Guilde guilde)
        {
            return new MerchantWindow(guilde);
        }

        // Constructeur privé
        private MerchantWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            _instance = null; 
            
        }
        private void BackToMainMenu(object? sender, RoutedEventArgs e)
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        private void BuyNewSoldier(object? sender, RoutedEventArgs e)
        {
            var soldierMerchantWindow = MerchantSoldierWindow.GetInstance(_guilde);
            soldierMerchantWindow.Show();
            Close();
        }

        private void BuyNewObject(object? sender, RoutedEventArgs e)
        {
            var objectMerchantWindow = MerchantObjectWindow.GetInstance(_guilde);
            objectMerchantWindow.Show();
            Close();
        }

        private void BuyMoreFood(object? sender, RoutedEventArgs e)
        {
            var foodMerchantWindow = MerchantFoodWindow.GetInstance(_guilde);
            foodMerchantWindow.Show();
            Close();
        }
    }
}      