using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;


namespace Pol_Guymarc_Projet
{
    public partial class YouLooseWindow : Window
    {
        private static YouLooseWindow? _instance;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static YouLooseWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new YouLooseWindow(guilde);
            }

            return _instance;
        }

        public static YouLooseWindow CreateNew(Guilde guilde)
        {
            return new YouLooseWindow(guilde);
        }

        // Constructeur privé
        private YouLooseWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }
        private void ShowEndGame(object? sender, EventArgs e)
        {
            DayScore.Text += _guilde.GetDayCounter();
            if (_guilde.GetNumberOfMeats() + _guilde.GetnumberOfMeatsComingTomorrow() -
                _guilde.CalculateTodaysNumberOfMeatsDistributedToSoldiers() < 0)
            {
                NotEnoughMeats.IsVisible = true;
                MeatsWeHave.Text +=  _guilde.GetNumberOfMeats();
                MeatsComing.Text += _guilde.GetnumberOfMeatsComingTomorrow();
                MeatsWeOwedSoldiers.Text += _guilde.CalculateTodaysNumberOfMeatsDistributedToSoldiers();
            }
            if (_guilde.GetNumberOfBreads() + _guilde.GetnumberOfBreadsComingTomorrow() -
                _guilde.CalculateTodaysNumberOfBreadsDistributedToSoldiers() < 0)
            {
                NotEnoughBreads.IsVisible = true;
                BreadsWeHave.Text +=  _guilde.GetNumberOfBreads();
                BreadsComing.Text += _guilde.GetnumberOfBreadsComingTomorrow();
                BreadsWeOwedSoldiers.Text += _guilde.CalculateTodaysNumberOfBreadsDistributedToSoldiers();
            }
            if (_guilde.GetMoney() - _guilde.CalculateTodaysMoneyDistributedToSoldiers() < 0)
            {
                NotEnoughMoney.IsVisible = true;
                MoneyWeHave.Text+=_guilde.GetMoney()+" pièces";
                MoneyWeOwedSoldiers.Text+=_guilde.CalculateTodaysMoneyDistributedToSoldiers()+" pièces";
            }
            int counterOfSoldiersAbleToFight = 0;
            foreach (Soldier soldier in _guilde.GetSoldiersList())
            {
                if (soldier.GetState() == "Libre" || soldier.GetState() == "En mission"||soldier.GetState() =="Au repos")
                {
                    counterOfSoldiersAbleToFight++;
                }
            }
            if (counterOfSoldiersAbleToFight == 0)
            {
                NotEnoughSoldiers.IsVisible=true;
                
            }
        }
        private void ExitTheGame(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}      