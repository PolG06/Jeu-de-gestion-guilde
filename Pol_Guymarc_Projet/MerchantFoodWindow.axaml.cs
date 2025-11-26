using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;


namespace Pol_Guymarc_Projet
{
    public partial class MerchantFoodWindow : Window
    {
        private static MerchantFoodWindow? _instance;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static MerchantFoodWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new MerchantFoodWindow(guilde);
            }

            return _instance;
        }

        // Constructeur privé
        private MerchantFoodWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            AmountOfMoney.Text ="Vous avez: "+ _guilde.GetMoney()+ " pièces";
            AmountOfMeats.Text ="Vous avez: "+ _guilde.GetNumberOfBreads()+" pains";
            AmountOfBreads.Text ="Vous avez: "+ _guilde.GetNumberOfMeats()+ " viandes";
            
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null; // Permet de recréer la fenêtre plus tard
        }
        
        private void BackToMainMenu()
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }
        private void BackToMainMerchant(object? sender, RoutedEventArgs e)
        {
            var merchantWindow = MerchantWindow.GetInstance(_guilde);
            merchantWindow.Show();
            Close();
        }

        private async void ValidateBuyingFood(object? sender, RoutedEventArgs e)
        {
            bool exit=false;
            int breads = (int)(NumberOfBreadsToBuy.Value ?? 0);
            int meats = (int)(NumberOfMeatsToBuy.Value ?? 0);
            
            if (breads < 0 || meats < 0)
            {
                NotificationText.Text = "Les valeurs ne peuvent pas être négatives.";
            }
            else if ((3 * (breads + meats)) > _guilde.GetMoney())
            {
                NotificationText.Text = "Vous n'avez pas assez d'argent pour acheter tout cela.";
                NumberOfBreadsToBuy.Value = 0;
                NumberOfMeatsToBuy.Value = 0;
            }
            else if (breads == 0 && meats == 0)
            {
                NotificationText.Text = "Veuillez entrer une quantité à acheter.";
            }
            else
            {
                _guilde.BuyBreads(breads);
                _guilde.BuyMeats(meats);
                NotificationText.Text ="Vous venez de commander "+breads+" pains et "+meats+" viandes";
                exit = true;
            }

            // Afficher le flyout
            var flyout = FlyoutBase.GetAttachedFlyout(ValidateFoodBuyingButton);
            flyout?.ShowAt(ValidateFoodBuyingButton);

            // Attendre 2 secondes puis cacher le flyout
            await Task.Delay(2000);
            flyout?.Hide();
            if (exit)
            {
                BackToMainMenu();
            }
        }
    }
}      