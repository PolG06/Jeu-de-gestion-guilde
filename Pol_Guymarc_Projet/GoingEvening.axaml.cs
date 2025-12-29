// importation des bibliothèques nécessaires
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;

// namespace du projet
namespace Pol_Guymarc_Projet
{
    // classe de la fenêtre du passage au soir
    public partial class GoingEveningWindow : Window
    {
        private static GoingEveningWindow? _instance;
        // variable globale contenant la guilde
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

        // permet de créer une nouvelle fenêtre sans singleton
        public static GoingEveningWindow CreateNew(Guilde guilde)
        {
            return new GoingEveningWindow(guilde);
        }

        // Constructeur privé de la fenêtre
        private GoingEveningWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);
            _instance = null; // Permet de recréer plus tard une nouvelle fenêtre
        }

        private void ShowUndesirableEvents(object? sender, EventArgs e)
        // affiche les évènements du soir liés aux missions
        {
            AllMissionsState.IsVisible = false;
            InProgressMissions.IsVisible = false;

            // parcours de toutes les missions de la guilde
            foreach (Mission mission in _guilde.GetMissionsList())
            {
                if (mission.GetState() == "En cours")
                {
                    mission.ActualizeNumberOfDays();
                    TextBlock txt = new TextBlock();

                    // si la mission est terminée
                    if (mission.getNumberOfDaysLeft() == 0)
                    {
                        mission.EndureMission();

                        // si la mission est réussie
                        if (mission.GetState() == "Réussie")
                        {
                            _guilde.AtualiseAmelioration(mission.GetSoldierOnIt());
                            txt.Text = "- " + mission.GetSoldierOnIt().GetName() +
                                       " , a réussi la mission n° " + mission.GetId();

                            // cas où le soldat est blessé
                            if (mission.GetSoldierOnIt().GetState() == "Blessé")
                            {
                                txt.Text += "\nMalheureusement, il est blessé et ne pourra pas effectuer de mission de si tôt.";
                            }

                            // récompenses de la mission
                            txt.Text += "\nVous gagnez " + mission.GetObjectToReceive().GetName() +
                                        " ,ainsi que " + mission.GetEarnings() + " pièces";
                            txt.Text += "\n" + mission.GetSoldierOnIt().GetName() +
                                        " reçoit " + 30 * (mission.GetDifficulty() + mission.getNumberOfDaysTotal()) +
                                        " points d'exp";

                            mission.GetSoldierOnIt().SuccedMission(mission);

                            // gestion de la montée de niveau
                            if (mission.GetSoldierOnIt().GetExp() >=
                                (int)(3 * Math.Pow(10, mission.GetSoldierOnIt().GetLevel())))
                            {
                                int oldDamages = mission.GetSoldierOnIt().GetDamages();
                                int oldDiscretionPoints = mission.GetSoldierOnIt().GetDiscretionPoints();
                                int oldMaxPv = mission.GetSoldierOnIt().GetMaxPv();
                                int oldBreadsADay = mission.GetSoldierOnIt().GetBreadsADay();
                                int oldMeatsADay = mission.GetSoldierOnIt().GetMeatsADay();
                                int oldSalaryADay = mission.GetSoldierOnIt().GetSalaryADay();

                                txt.Text += "\n" + mission.GetSoldierOnIt().GetName() +
                                            " passe au niveau " + (mission.GetSoldierOnIt().GetLevel() + 1);

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
                                txt.Text += "\nSalaire par jour: " + oldSalaryADay +
                                            " pièces --> " + mission.GetSoldierOnIt().GetSalaryADay() + " pièces";
                            }

                            // remise du soldat à l'état libre
                            if (mission.GetSoldierOnIt().GetState() == "En mission")
                            {
                                mission.GetSoldierOnIt().BeingFree();
                            }

                            // ajout des gains à la guilde
                            _guilde.SetMoney(_guilde.GetMoney() + mission.GetEarnings());
                            _guilde.AddObjects(mission.GetObjectToReceive(), 1);

                            SuccedMissions.Children.Add(txt);
                            SuccedMissions.IsVisible = true;
                            AllMissionsState.IsVisible = true;
                        }
                        // si la mission est ratée
                        else if (mission.GetState() == "Ratée")
                        {
                            txt.Text = "- " + mission.GetSoldierOnIt().GetName() +
                                       " est mort en tentant la mission n° " + mission.GetId();
                            FailedMissions.Children.Add(txt);
                            FailedMissions.IsVisible = true;
                        }
                    }
                    // si la mission n'est pas encore terminée
                    else
                    {
                        txt.Text = "- " + mission.GetSoldierOnIt().GetName() +
                                   " est en encore sur la mission n° " + mission.GetId();
                        txt.Text += " \nIl reste " + (-mission.getNumberOfDaysLeft()) +
                                    " jours avant qu'elle soit terminée";
                        InProgressMissions.Children.Add(txt);
                        InProgressMissions.IsVisible = true;
                    }
                }
            }

            // affichage du jour actuel
            EveningAnounce.Text += _guilde.GetDayCounter();

            // mise à jour des soldats morts
            _guilde.MarquedSoldiersWhenDead();

            // passage au moment suivant de la journée
            _guilde.SkipDayMoment();
        }

        private void BackToMainMenu(object? sender, RoutedEventArgs e)
        // permet de revenir à la fenêtre principale
        {
            var gameWindow = GameWindow.GetInstance(_guilde);

            // Affiche la fenêtre principale
            gameWindow.Show();
            Close();
        }
    }
}
