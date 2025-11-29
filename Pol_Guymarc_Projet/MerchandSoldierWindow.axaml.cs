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
        private List<Soldier>? _soldierlist;
        private readonly Guilde _guilde;
        
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
        private MerchantSoldierWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            _soldierlist = new List<Soldier>();
            _soldierlist.Add((new Bandit("Voleur")));
            _soldierlist.Add((new Archer("Archer")));
            _soldierlist.Add((new Giant("Géant")));
            _soldierlist.Add((new Swordsman("Epeiste")));
            _soldierlist.Add((new Paladin("Paladin")));
            _selectedSoldier = _soldierlist[0];
            ShowSoldierInfos(_selectedSoldier);
            MoneyYouHave.Text = "Vous avez " + _guilde.GetMoney() + " pièces";

        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null;
        }
        
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
            Validation.IsVisible = false;
            NewName.IsVisible = false;
            if (_selectedSoldier == _soldierlist[0])
            {
                _selectedSoldier = _soldierlist[_soldierlist.Count - 1];
            }
            else
            {
                for (int i=0; i<_soldierlist.Count; i++)
                {
                    if (_soldierlist[i] == _selectedSoldier)
                    {
                        _selectedSoldier=_soldierlist[i-1];
                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }

        private void GoRightToSoldier(object? sender, RoutedEventArgs e)
        {
            NewSoldierName.Text=String.Empty;
            Validation.IsVisible = false;
            NewName.IsVisible = false;
            if (_selectedSoldier == _soldierlist[_soldierlist.Count -1])
            {
                _selectedSoldier = _soldierlist[0];
            }
            else
            {
                for (int i=0; i<_soldierlist.Count; i++)
                {
                    if (_soldierlist[i] == _selectedSoldier)
                        
                    {
                        _selectedSoldier=_soldierlist[i+1];
                        break;

                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
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
            
            await Task.Delay(2000);
            flyout?.Hide();
            if (exitAfter)
            {
                BackToMainMenu();
            }
        }
    }
}