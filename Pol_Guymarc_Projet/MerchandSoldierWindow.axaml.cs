using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;

namespace Pol_Guymarc_Projet
{
    public partial class MerchantSoldierWindow : Window
    {
        private static MerchantSoldierWindow? _instance;
        private Soldier? _selectedSoldier;
        private List<Soldier>? soldierlist;
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
        public static MerchantSoldierWindow CreateNew(Guilde guilde)
        {
            return new MerchantSoldierWindow(guilde);
        }
        // Constructeur privé
        private MerchantSoldierWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            soldierlist = new List<Soldier>();
            soldierlist.Add((new Bandit("Voleur")));
            soldierlist.Add((new Archer("Archer")));
            soldierlist.Add((new Giant("Géant")));
            soldierlist.Add((new Swordsman("Epeiste")));
            soldierlist.Add((new Paladin("Paladin")));
            _selectedSoldier = soldierlist[0];
            ShowSoldierInfos(_selectedSoldier);
            
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null; // Permet de recréer plus tard
        }

        // Méthodes pour gérer les soldats
        private void ShowSoldierInfos(Soldier soldier)
        {
            SoldierName.Text = soldier.GetName();
            SoldierPicture.Source =
                new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                    "images", soldier.GetImageName()));
            SoldierDescription.Text="Description : " + soldier.GetDescription();
            SoldierDamages.Text ="Dégats: "+ soldier.GetDamages();
            SoldierDiscretionPoints.Text="Points de discretion: "+soldier.GetDiscretionPoints();
            SoldierPv.Text ="PV: "+soldier.GetMaxPv() ;
            SoldierNumberOfBread.Text = "Nombre de pains par jour: " + soldier.GetBreadsADay();
            SoldierNumberOfMeats.Text="Nombre de viandes par jour: " + soldier.GetMeatsADay();
            SoldierSalaryADay.Text = "Salaire par jour: " + soldier.GetSalaryADay() + " pièces.";
            SoldierBuyingPrice.Text="Prix: "+soldier.GetBuyingPrice();

            if (_guilde.GetMoney() >= soldier.GetBuyingPrice())
            {
                BuySoldierButton.IsVisible = true;
            }
        }

        private void GoLeftToSoldier(object? sender, RoutedEventArgs e)
        {
            NewSoldierName.Text=String.Empty;
            NewName.IsVisible = false;
            if (_selectedSoldier == soldierlist[0])
            {
                _selectedSoldier = soldierlist[soldierlist.Count - 1];
            }
            else
            {
                for (int i=0; i<soldierlist.Count; i++)
                {
                    if (soldierlist[i] == _selectedSoldier)
                    {
                        _selectedSoldier=soldierlist[i-1];
                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }

        private void GoRightToSoldier(object? sender, RoutedEventArgs e)
        {
            NewSoldierName.Text=String.Empty;
            NewName.IsVisible = false;
            if (_selectedSoldier == soldierlist[soldierlist.Count -1])
            {
                _selectedSoldier = soldierlist[0];
            }
            else
            {
                for (int i=0; i<soldierlist.Count; i++)
                {
                    if (soldierlist[i] == _selectedSoldier)
                        
                    {
                        _selectedSoldier=soldierlist[i+1];
                        break;

                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }
        private void BackToMainMenue()
        {
            var gameWindow = GameWindow.GetInstance(_guilde);

            // Affiche la fenêtre principale
            gameWindow.Show();
            Close();
        }
        private void BackToMainMerchant(object? sender, RoutedEventArgs e)
        {
            var merchantWindow = MerchantWindow.GetInstance(_guilde);
            merchantWindow.Show();
            Close();
        }

        private void ThenChooseName(object? sender, RoutedEventArgs e)
        {
            NewName.IsVisible = true;
            Validation.IsVisible = true;
        }

        private async void CreateAndAddingNewSoldier(object? sender, RoutedEventArgs e)
        {
            bool exitAfter;
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
            var flyout = FlyoutBase.GetAttachedFlyout(Validation);
            flyout?.ShowAt(Validation);

            // Attendre 2 secondes puis cacher le flyout
            await Task.Delay(2000);
            flyout?.Hide();
            if (exitAfter)
            {
                // Réinitialiser le TextBox et cacher le panneau de saisie
                //NewSoldierName.Text = string.Empty;
                //NewName.IsVisible = false;
                //Validation.IsVisible = false;
                BackToMainMenue();
            }
        }
    }
}