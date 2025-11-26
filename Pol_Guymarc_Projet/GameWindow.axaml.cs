using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;

namespace Pol_Guymarc_Projet;

public partial class GameWindow : Window
{
    private static GameWindow? _instance;
    private readonly Guilde _guilde;

    // Propriété pour accéder à l’instance
    public static GameWindow GetInstance(Guilde guilde)
    {
        if (_instance == null)
        {
            _instance = new GameWindow(guilde);
        }
        return _instance;
    }

    // Constructeur privé
    private GameWindow(Guilde guilde)
    {
        _guilde = guilde;
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _instance = null; // Permet de recréer plus tard
    }
    //Creation of the Guilde
    private void SeeSoldiers(object? sender, RoutedEventArgs e)
    {
        var soldierWindow = SoldierWindow.CreateNew(_guilde); 
        soldierWindow.Show();
        Close();
    }

    private void SeeInventary(object? sender, RoutedEventArgs e)
    {
        var inventaryWindow = InventaryWindow.CreateNew(_guilde);; 
        inventaryWindow.Show();
        Close();
    }

    private void SeeMissions(object? sender, RoutedEventArgs e)
    {
        var missionWindow = MissionWindow.CreateNew(_guilde);
        missionWindow.Show();
        Close();
    }

    private void GoToMerchant(object? sender, RoutedEventArgs e)
    {
        var merchantWindow = MerchantWindow.CreateNew(_guilde);
        merchantWindow.Show();
        Close();
    }

    private void SkipTheDayMoment(object? sender, RoutedEventArgs e)
    {
        if (_guilde.GetDayMoment() == "Matin")
        {
            var nextDayMoment = GoingAfternoonWindow.CreateNew(_guilde);
            nextDayMoment.Show();
            Close();
            
        }
        else if (_guilde.GetDayMoment() == "Après-midi")
        {
            var nextDayMoment = GoingEveningWindow.CreateNew(_guilde);
            nextDayMoment.Show();
            Close();
            
        }
        else
        {
            bool continuer= true;
            if (_guilde.GetNumberOfMeats() + _guilde.GetnumberOfMeatsComingTomorrow() - _guilde.CalculateTodaysNumberOfMeatsDistributedToSoldiers() < 0)
            {
                continuer = false;
            }
            if (_guilde.GetNumberOfBreads() + _guilde.GetnumberOfBreadsComingTomorrow() -
                   _guilde.CalculateTodaysNumberOfBreadsDistributedToSoldiers() < 0)
            {
                continuer = false;
            }
            if (_guilde.GetMoney() - _guilde.CalculateTodaysMoneyDistributedToSoldiers() < 0)
            {
                continuer = false;
            }

            int counterOfSoldiersAbleToFight = 0;
            foreach (Soldier soldier in _guilde.GetSoldiersList())
            {
                if (soldier.GetState() == "Libre" || soldier.GetState() == "En mission"|| soldier.GetState()=="Au repos")
                {
                    counterOfSoldiersAbleToFight++;
                }
            }

            if (counterOfSoldiersAbleToFight == 0)
            {
                continuer = false;
            }

            if (continuer)
            {
                var nextDayMoment = NewMorningAndDayWindow.CreateNew(_guilde);
                nextDayMoment.Show();
                Close();
            }
            else
            {
                var youLoosewindow = YouLooseWindow.CreateNew(_guilde);
                youLoosewindow.Show();
                Close();
            }
        }

    }
    
    private void AtualizeDisplayGameWindow(object? sender, EventArgs e)
    {
        DayCounter.Text = "Jour n°" + _guilde.GetDayCounter();
        MoneyCounter.Text += _guilde.GetMoney() + " Pièces";
        BreadsCounter.Text += _guilde.GetNumberOfBreads();
        if (_guilde.GetnumberOfBreadsComingTomorrow() > 0)
        {
            BreadsCounter.Text += " ("+_guilde.GetnumberOfBreadsComingTomorrow()+" pains ont été commandés et arrivent demain)";
        }
        MeatsCounter.Text += _guilde.GetNumberOfMeats();
        if (_guilde.GetnumberOfMeatsComingTomorrow() > 0)
        {
            MeatsCounter.Text += " ("+_guilde.GetnumberOfMeatsComingTomorrow()+" pains ont été commandés et arrivent demain)";
        }
        DayMoment.Text = "Moment de la journée: " + _guilde.GetDayMoment();
        NextDayMoneyCounter.Text += _guilde.CalculateTodaysMoneyDistributedToSoldiers()+" pièces";
        NextDayBreadsCounter.Text += _guilde.CalculateTodaysNumberOfBreadsDistributedToSoldiers()+ " pains";
        NextDayMeatsCounter.Text +=  _guilde.CalculateTodaysNumberOfMeatsDistributedToSoldiers()+ " viandes";
    }
}