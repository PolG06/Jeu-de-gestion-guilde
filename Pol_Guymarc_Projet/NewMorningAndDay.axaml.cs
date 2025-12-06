using Avalonia.Controls;
using Avalonia.Interactivity;

using Pol_Guymarc_Projet.Classes;
using System;

namespace Pol_Guymarc_Projet
{
    public partial class NewMorningAndDayWindow : Window
    {
        private static NewMorningAndDayWindow? _instance;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static NewMorningAndDayWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new NewMorningAndDayWindow(guilde);
            }

            return _instance;
        }

        public static NewMorningAndDayWindow CreateNew(Guilde guilde)
        {
            return new NewMorningAndDayWindow(guilde);
        }

        // Constructeur privé
        private NewMorningAndDayWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        private void ShowNewMorningAndDay(object? sender, EventArgs e)
        {
            NewDayAnounce.Text += _guilde.GetDayCounter();
            
            MoneyWeHave.Text += _guilde.GetMoney() + " pièces";
            MoneyWeOwedSoldiers.Text += _guilde.CalculateTodaysMoneyDistributedToSoldiers() + " pièces";
            MoneyWeHaveForToday.Text += _guilde.GetMoney() - _guilde.CalculateTodaysMoneyDistributedToSoldiers() + " pièces";

            BreadsWeHave.Text += _guilde.GetNumberOfBreads();
            BreadsComing.Text += _guilde.GetnumberOfBreadsComingTomorrow();
            BreadsWeOwedSoldiers.Text += _guilde.CalculateTodaysNumberOfBreadsDistributedToSoldiers();
            BreadsWeHaveForToday.Text += _guilde.GetNumberOfBreads()+_guilde.GetnumberOfBreadsComingTomorrow()- _guilde.CalculateTodaysNumberOfBreadsDistributedToSoldiers();
            
            MeatsWeHave.Text += _guilde.GetNumberOfMeats();
            MeatsComing.Text += _guilde.GetnumberOfMeatsComingTomorrow();
            MeatsWeOwedSoldiers.Text += _guilde.CalculateTodaysNumberOfMeatsDistributedToSoldiers();
            MeatsWeHaveForToday.Text += _guilde.GetNumberOfMeats()+_guilde.GetnumberOfMeatsComingTomorrow()- _guilde.CalculateTodaysNumberOfMeatsDistributedToSoldiers();

            SoldiersWeHave.Text += _guilde.GetSoldiersList().Count;
            _guilde.SkipDayMoment();
        }
        
        private void BackToMainMenu(object? sender, RoutedEventArgs e)
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            
            
            gameWindow.Show();
            Close();
        }
    }
}      