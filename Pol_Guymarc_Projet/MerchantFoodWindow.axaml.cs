// importation des bibliothèques
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;

//importation de nos classes
namespace Pol_Guymarc_Projet
{
    // classe de la fenêtre du marchand de nourriture
    public partial class MerchantFoodWindow : Window
    {
        private static MerchantFoodWindow? _instance;
        // variable globale contenant la guilde
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

        // Constructeur privé de la fenêtre
        private MerchantFoodWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);

            // affichage de l'argent et des ressources disponibles
            AmountOfMoney.Text = "Vous avez: " + _guilde.GetMoney() + " pièces";
            AmountOfMeats.Text = "Vous avez: " + _guilde.GetNumberOfBreads() + " pains";
            AmountOfBreads.Text = "Vous avez: " + _guilde.GetNumberOfMeats() + " viandes";
        }

        protected override void OnClosed(EventArgs e)
        // méthode appelée à la fermeture de la fenêtre
        {
            base.OnClosed(e);
            _instance = null; // Permet de recréer la fenêtre plus tard
        }
        
        private void BackToMainMenu()
        // permet de revenir à la fenêtre principale
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        private void BackToMainMerchant(object? sender, RoutedEventArgs e)
        // permet de revenir au menu principal du marchand
        {
            var merchantWindow = MerchantWindow.GetInstance(_guilde);
            merchantWindow.Show();
            Close();
        }

        private async void ValidateBuyingFood(object? sender, RoutedEventArgs e)
        // permet de valider l'achat de nourriture
        {
            bool exit = false;
            int breads = (int)(NumberOfBreadsToBuy.Value ?? 0);
            int meats = (int)(NumberOfMeatsToBuy.Value ?? 0);

            // vérification des valeurs saisies
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
                // achat des pains et des viandes
                _guilde.BuyBreads(breads);
                _guilde.BuyMeats(meats);
                NotificationText.Text = "Vous venez de commander " + breads +
                                        " pains et " + meats + " viandes";
                exit = true;
            }

            // affichage d'une notification temporaire
            var flyout = FlyoutBase.GetAttachedFlyout(ValidateFoodBuyingButton);
            flyout?.ShowAt(ValidateFoodBuyingButton);

            // attente de 2 secondes avant de cacher la notification
            await Task.Delay(2000);
            flyout?.Hide();

            // retour au menu principal après l'achat
            if (exit)
            {
                BackToMainMenu();
            }
        }
    }
}
