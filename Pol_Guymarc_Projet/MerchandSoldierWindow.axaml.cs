// importation des bibliothèques
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;

//importation de nos classes
namespace Pol_Guymarc_Projet
{
    // classe de la fenêtre du marchand de soldats
    public partial class MerchantSoldierWindow : Window
    {
        private static MerchantSoldierWindow? _instance;
        // soldat actuellement sélectionné
        private Soldier? _selectedSoldier;
        // liste des soldats disponibles à l'achat
        private List<Soldier>? _soldierlist;
        // variable globale contenant la guilde
        private readonly Guilde _guilde;
        
        // Singleton : récupération de l'instance avec Guilde
        public static MerchantSoldierWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new MerchantSoldierWindow(guilde);
            }
            return _instance;
        }

        // permet de créer une nouvelle fenêtre sans singleton
        public static MerchantSoldierWindow CreateNew(Guilde guilde)
        {
            return new MerchantSoldierWindow(guilde);
        }

        // Constructeur privé de la fenêtre
        private MerchantSoldierWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);

            // création de la liste des soldats achetables
            _soldierlist = new List<Soldier>();
            _soldierlist.Add((new Bandit("Voleur")));
            _soldierlist.Add((new Archer("Archer")));
            _soldierlist.Add((new Giant("Géant")));
            _soldierlist.Add((new Swordsman("Epeiste")));
            _soldierlist.Add((new Paladin("Paladin")));

            // sélection du premier soldat de la liste
            _selectedSoldier = _soldierlist[0];

            // affichage des informations du soldat sélectionné
            ShowSoldierInfos(_selectedSoldier);

            // affichage de l'argent disponible
            MoneyYouHave.Text = "Vous avez " + _guilde.GetMoney() + " pièces";
        }

        protected override void OnClosed(EventArgs e)
        // méthode appelée à la fermeture de la fenêtre
        {
            base.OnClosed(e);
            _instance = null;
        }
        
        private void ShowSoldierInfos(Soldier soldier)
        // affiche les informations d'un soldat
        {
            SoldierName.Text = soldier.GetName();
            SoldierPicture.Source =
                new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                    "images", soldier.GetImageName()));
            SoldierDescription.Text = "Description : " + soldier.GetDescription();
            SoldierDamages.Text = "Dégats: " + soldier.GetDamages();
            SoldierDiscretionPoints.Text = "Points de discretion: " + soldier.GetDiscretionPoints();
            SoldierPv.Text = "PV: " + soldier.GetMaxPv();
            SoldierNumberOfBread.Text = "Nombre de pains par jour: " + soldier.GetBreadsADay();
            SoldierNumberOfMeats.Text = "Nombre de viandes par jour: " + soldier.GetMeatsADay();
            SoldierSalaryADay.Text = "Salaire par jour: " + soldier.GetSalaryADay() + " pièces.";
            SoldierBuyingPrice.Text = "Prix: " + soldier.GetBuyingPrice();

            // vérifie si le joueur a assez d'argent pour acheter le soldat
            if (_guilde.GetMoney() >= soldier.GetBuyingPrice())
            {
                BuySoldierButton.IsVisible = true;
            }
        }

        private void GoLeftToSoldier(object? sender, RoutedEventArgs e)
        // permet de naviguer vers le soldat précédent
        {
            NewSoldierName.Text = String.Empty;
            Validation.IsVisible = false;
            NewName.IsVisible = false;

            if (_selectedSoldier == _soldierlist[0])
            {
                _selectedSoldier = _soldierlist[_soldierlist.Count - 1];
            }
            else
            {
                for (int i = 0; i < _soldierlist.Count; i++)
                {
                    if (_soldierlist[i] == _selectedSoldier)
                    {
                        _selectedSoldier = _soldierlist[i - 1];
                    }
                }
            }

            ShowSoldierInfos(_selectedSoldier);
        }

        private void GoRightToSoldier(object? sender, RoutedEventArgs e)
        // permet de naviguer vers le soldat suivant
        {
            NewSoldierName.Text = String.Empty;
            Validation.IsVisible = false;
            NewName.IsVisible = false;

            if (_selectedSoldier == _soldierlist[_soldierlist.Count - 1])
            {
                _selectedSoldier = _soldierlist[0];
            }
            else
            {
                for (int i = 0; i < _soldierlist.Count; i++)
                {
                    if (_soldierlist[i] == _selectedSoldier)
                    {
                        _selectedSoldier = _soldierlist[i + 1];
                        break;
                    }
                }
            }

            ShowSoldierInfos(_selectedSoldier);
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

        private void ThenChooseName(object? sender, RoutedEventArgs e)
        // affiche les champs pour choisir le nom du soldat
        {
            NewName.IsVisible = true;
            Validation.IsVisible = true;
        }

        private async void CreateAndAddingNewSoldier(object? sender, RoutedEventArgs e)
        // permet de créer et d'ajouter un nouveau soldat à la guilde
        {
            bool exitAfter;

            // vérifie que le nom du soldat n'est pas vide
            if (!(string.IsNullOrWhiteSpace(NewSoldierName.Text)))
            {
                exitAfter = true;
                _selectedSoldier.SetName(NewSoldierName.Text);
                _guilde.BuySoldier(_selectedSoldier);
                BuyingANewSoldier.Text = "Vous venez d'acheter un nouveau soldat";
            }
            else
            {
                BuyingANewSoldier.Text = "Veuillez donner un nom à ce soldat";
                exitAfter = false;
            }

            // affichage d'une notification temporaire
            var flyout = FlyoutBase.GetAttachedFlyout(Validation);
            flyout?.ShowAt(Validation);
            
            await Task.Delay(2000);
            flyout?.Hide();

            // retour au menu principal après l'achat
            if (exitAfter)
            {
                BackToMainMenu();
            }
        }
    }
}
