using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;


namespace Pol_Guymarc_Projet
{
    public partial class GoingEveningWindow : Window
    {
        private static GoingEveningWindow? _instance;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static GoingEveningWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new GoingEveningWindow(guilde);
            }

            return _instance;
        }

        public static GoingEveningWindow CreateNew(Guilde guilde)
        {
            return new GoingEveningWindow(guilde);
        }

        // Constructeur privé
        private GoingEveningWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            
            
        }

        private void ShowUndesirableEvents(object? sender, EventArgs e)
        {
            AllMissionsState.IsVisible = false;
            InProgressMissions.IsVisible = false;
            foreach (Mission mission in _guilde.GetMissionsList())
            {
                if (mission.GetState() == "En cours")
                {
                    mission.ActualizeNumberOfDays();
                    TextBlock txt = new TextBlock();
                    if (mission.getNumberOfDaysLeft() == 0)
                    {
                        mission.EndureMission();
                        if (mission.GetState() == "Réussie")
                        {
                            _guilde.AtualiseAmelioration(mission.GetSoldierOnIt());
                            txt.Text = "- "+mission.GetSoldierOnIt().GetName()+" , a réussi la mission n° "+mission.GetId();
                            if (mission.GetSoldierOnIt().GetState() == "Blessé")
                            {
                                txt.Text += "\nMalheureusement, il est blessé et ne pourra pas effectuer de mission de si tôt.";
                            }
                            txt.Text += "\nVous gagnez "+mission.GetObjectToReceive().GetName()+" ,ainsi que "+mission.GetEarnings()+" pièces";
                            txt.Text += "\n" +mission.GetSoldierOnIt().GetName()+ " reçoit "+30 * (mission.GetDifficulty() + mission.getNumberOfDaysTotal())+ " points d'exp";
                            mission.GetSoldierOnIt().SuccedMission(mission);
                            if (mission.GetSoldierOnIt().GetExp() >=
                                (int)(3 * Math.Pow(10, mission.GetSoldierOnIt().GetLevel())))
                            {
                                int oldDamages = mission.GetSoldierOnIt().GetDamages();
                                int oldDiscretionPoints= mission.GetSoldierOnIt().GetDiscretionPoints();
                                int oldMaxPv = mission.GetSoldierOnIt().GetMaxPv();
                                int oldBreadsADay = mission.GetSoldierOnIt().GetBreadsADay();
                                int oldMeatsADay = mission.GetSoldierOnIt().GetMeatsADay();
                                int oldSalaryADay = mission.GetSoldierOnIt().GetSalaryADay();
                                    
                                txt.Text += "\n" +mission.GetSoldierOnIt().GetName()+ " passe au niveau "+(mission.GetSoldierOnIt().GetLevel()+1);
                                _guilde.AtualiseAmelioration(mission.GetSoldierOnIt());
                                txt.Text += "\nDégats: " + oldDamages + " --> " +
                                            mission.GetSoldierOnIt().GetDamages();
                                txt.Text += "\nPoints de discretion: " + oldDiscretionPoints + " --> " +
                                            mission.GetSoldierOnIt().GetDiscretionPoints();
                                txt.Text += "\nPv max: " + oldMaxPv + " --> " +
                                            mission.GetSoldierOnIt().GetMaxPv();
                                txt.Text += "\nNombre de pains par jour: " + oldBreadsADay + " --> " +
                                            mission.GetSoldierOnIt().GetBreadsADay();
                                txt.Text += "\nNombre de viandes par jour: " + oldMeatsADay + " --> " +
                                            mission.GetSoldierOnIt().GetMeatsADay();
                                txt.Text += "\nSalaire par jour: " + oldSalaryADay + " pièces --> " +
                                            mission.GetSoldierOnIt().GetSalaryADay()+" pièces";
                            }

                            if (mission.GetSoldierOnIt().GetState() == "En mission")
                            {
                                mission.GetSoldierOnIt().BeingFree();
                            }
                            _guilde.SetMoney(_guilde.GetMoney()+mission.GetEarnings());
                            _guilde.AddObjects(mission.GetObjectToReceive(),1);
                            SuccedMissions.Children.Add(txt);
                            SuccedMissions.IsVisible = true;
                            AllMissionsState.IsVisible = true;
                        }
                        else if (mission.GetState() == "Ratée")
                        {
                            txt.Text = "- "+mission.GetSoldierOnIt().GetName()+" est mort en tentant la mission n° "+mission.GetId();
                            FailedMissions.Children.Add(txt);
                            FailedMissions.IsVisible = true;
                        }
                    }
                    else 
                    {
                        txt.Text = "- "+mission.GetSoldierOnIt().GetName()+" est en encore sur la mission n° "+mission.GetId();
                        txt.Text += " \nIl reste "+(-mission.getNumberOfDaysLeft())+" jours avant qu'elle soit terminée";
                        InProgressMissions.Children.Add(txt);
                        InProgressMissions.IsVisible = true;
                    }
                }
            }
            EveningAnounce.Text += _guilde.GetDayCounter();
            _guilde.MarquedSoldiersWhenDead();
            _guilde.SkipDayMoment();
        }
        private void BackToMainMenu(object? sender, RoutedEventArgs e)
        {
            var gameWindow = GameWindow.GetInstance(_guilde);

            // Affiche la fenêtre principale
            gameWindow.Show();
            Close();
        }
    }
}      