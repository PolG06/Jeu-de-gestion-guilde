//La fenêtre principale

//importation des bibliothèques
using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;

//importation de nos classes
namespace Pol_Guymarc_Projet;

public partial class GameWindow : Window
//la classe de la fenêtre
{
    private static GameWindow? _instance;
    //on va réutiliser la guilde que l'on vient de créer dans notre fenêtre principale
    private readonly Guilde _guilde;
    
    //Singleton : Méthode que l'on va appeler pour éviter de créer 2 fois la même fenêtre
    public static GameWindow GetInstance(Guilde guilde)
    {
        if (_instance == null)
        {
            _instance = new GameWindow(guilde);
        }
        return _instance;
    }
    
    //constructeur privé de la classe où l'on récupère la guilde en tant que variable globale
    private GameWindow(Guilde guilde)
    {
        _guilde = guilde;
        InitializeComponent();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _instance = null; // Permet de recréer plus tard une nouvelle fenêtre
    }
    private void SeeSoldiers(object? sender, RoutedEventArgs e)
    // permet d'ouvrir une fenêtre où l'on va afficher tous nos soldats
    {
        var soldierWindow = SoldierWindow.CreateNew(_guilde); 
        soldierWindow.Show();
        Close();
    }

    private void SeeInventary(object? sender, RoutedEventArgs e) 
    // permet d'ouvrir une fenêtre où l'on va afficher tous les objets contenus dans notre inventaire
    {
        var inventaryWindow = InventaryWindow.CreateNew(_guilde);; 
        inventaryWindow.Show();
        Close();
    }

    private void SeeMissions(object? sender, RoutedEventArgs e)
    // permet d'ouvrir une fenêtre où l'on va afficher toutes nos missions 
    {
        var missionWindow = MissionWindow.CreateNew(_guilde);
        missionWindow.Show();
        Close();
    }

    private void GoToMerchant(object? sender, RoutedEventArgs e)
    // permet d'ouvrir la fenêtre principale du marchand 
    {
        var merchantWindow = MerchantWindow.CreateNew(_guilde);
        merchantWindow.Show();
        Close();
    }

    private void SkipTheDayMoment(object? sender, RoutedEventArgs e)
    //permet de passer le moment de la journée
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
            //si c'est le soir, on verifie si la partie n'est pas perdue, sinon on passe au jour suivant 
            //on va donc passer une série de conditions pour savoir si la partie continue toujours
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
    //méthode qui actualise le contenu des textes dans la fenêtre
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