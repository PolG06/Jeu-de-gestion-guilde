using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using Avalonia.Media.Imaging;
namespace Pol_Guymarc_Projet
{
    public partial class MissionWindow : Window
    {
        private static MissionWindow? _instance;
        private Mission? _selectedMission;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static MissionWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new MissionWindow(guilde);
            }

            return _instance;
        }

        public static MissionWindow CreateNew(Guilde guilde)
        {
            return new MissionWindow(guilde);
        }

        // Constructeur privé
        private MissionWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
                // Vérifier que la liste contient au moins une mission
                
            if (_guilde.GetMissionsList().Count > 0)
            {
                _selectedMission = _guilde.GetMissionsList()[0];
                ShowMissionInfos(_selectedMission); // affichage sûr, après que les contrôles sont initialisés
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null; // Permet de recréer plus tard
        }
        private void ShowMissionInfos(Mission mission)
        {
            if (mission.GetState() == "Libre")
            {
                EnvoyerSoldat.Content = "Assigner un soldat";
                EnvoyerSoldat.IsVisible = true;
                MissionNumberOfDays.Text = "Durée: " + mission.getNumberOfDaysTotal() + " jours";
                
            }
            else if(mission.GetState() == "En cours")
            {
                EnvoyerSoldat.IsVisible = false;
                MissionSoldierOnIt.Text="Soldat dessus: "+mission.GetSoldierOnIt().GetName();
                MissionNumberOfDays.Text = "Nombre de jours restants: " + mission.getNumberOfDaysLeft();
            }
            else if(mission.GetState() == "Réussie")
            {
                EnvoyerSoldat.IsVisible = true;
                EnvoyerSoldat.Content = "Recommencer";
                MissionNumberOfDays.Text = "Durée: " + mission.getNumberOfDaysTotal() + " jours";
            }
            else if(mission.GetState() == "Ratée")
            {
                EnvoyerSoldat.IsVisible = true;
                EnvoyerSoldat.Content = "Réessayer";
                MissionNumberOfDays.Text = "Durée: " + mission.getNumberOfDaysTotal() + " jours";
            }
            MissionName.Text = "Mission n° " + mission.GetId();
            MissionPicture.Source=new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                "images", "mission.png"));
            MissionDifficulty.Text = "Difficulté :" + mission.GetDifficulty();
            
            MissionState.Text = "Etat: " + mission.GetState();
            MissionEarnings.Text = "Argent à gagner: " + mission.GetEarnings()+" pièces";
            MissionObjectsToReceive.Text = "Objet à gagner: " + mission.GetObjectToReceive().GetName();
            NumberOfMissions.Text ="Nombre total de missions: "+ _guilde.GetMissionsList().Count;
            NumberOfFreeMissions.Text ="Nombre total de missions libres : "+ NumberOfMissionsByState("Libre");
            NumberOfActiveMissions.Text = "Nombre total de missions en cours: "+NumberOfMissionsByState("En cours");
            NumberOfCompletedMissions.Text = "Nombre total de missions réussies: "+NumberOfMissionsByState("Réussie");
            NumberOfFailedMissions.Text = "Nombre total de missions ratées: "+NumberOfMissionsByState("Ratée");
        }

        private int NumberOfMissionsByState(string state)
        {
            int counter = 0;
            foreach (Mission mission in _guilde.GetMissionsList())
            {
                if (mission.GetState() == state)
                {
                    counter++;
                }
            }

            return counter;
        }
        private void GoLeftToMission(object? sender, RoutedEventArgs e)
        {
            EnvoyerSoldat.IsVisible = true;
            if (_selectedMission== _guilde.GetMissionsList()[0])
            {
                _selectedMission = _guilde.GetMissionsList()[_guilde.GetMissionsList().Count - 1];
            }
            else
            {
                for (int i=0; i<_guilde.GetMissionsList().Count; i++)
                {
                    if (_guilde.GetMissionsList()[i] == _selectedMission)
                    {
                        _selectedMission=_guilde.GetMissionsList()[i-1];
                    }
                }
            }
            ShowMissionInfos(_selectedMission);
        }

        private void GoRightToMission(object? sender, RoutedEventArgs e)
        {
            EnvoyerSoldat.IsVisible = true;
            if (_selectedMission == _guilde.GetMissionsList()[_guilde.GetMissionsList().Count -1])
            {
                _selectedMission = _guilde.GetMissionsList()[0];
            }
            else
            {
                for (int i=0; i<_guilde.GetMissionsList().Count; i++)
                {
                    if (_guilde.GetMissionsList()[i] == _selectedMission)
                        
                    {
                        _selectedMission=_guilde.GetMissionsList()[i+1];
                        break;

                    }
                }
            }
            ShowMissionInfos(_selectedMission);
        }
        private void BackToMainMenue(object? sender, RoutedEventArgs e)
        {
            var gameWindow = GameWindow.GetInstance(_guilde);

            // Affiche la fenêtre principale
            gameWindow.Show();
            Close();
        }

        private void SendSoldierOnIt(object? sender, RoutedEventArgs e)
        {
            var selectingSoldierToStartMissionWindow = SelectingSoldierToStartMissionWindow.GetInstance(_guilde, _selectedMission);
            // Affiche la fenêtre principale
            selectingSoldierToStartMissionWindow.Show();
            Close();
        }
    }
}