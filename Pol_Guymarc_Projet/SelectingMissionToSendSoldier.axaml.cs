// importation des bibliothèques
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;

// importation de nos classes
namespace Pol_Guymarc_Projet
{
    // classe représentant la fenêtre pour sélectionner une mission pour un soldat
    public partial class SelectingMissionToSendSoldierWindow : Window
    {
        // référence vers le soldat à envoyer en mission
        private readonly Soldier _soldier;
        // liste des missions disponibles
        private List<Mission> missionList;
        // instance unique de la fenêtre (Singleton)
        private static SelectingMissionToSendSoldierWindow? _instance;
        // mission actuellement sélectionnée
        private static Mission? _selectedMission;
        // référence vers la guilde
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde et Soldier
        public static SelectingMissionToSendSoldierWindow GetInstance(Guilde guilde, Soldier soldier)
        {
            if (_instance == null)
            {
                _instance = new SelectingMissionToSendSoldierWindow(guilde, soldier);
            }

            return _instance;
        }

        // Constructeur privé
        private SelectingMissionToSendSoldierWindow(Guilde guilde, Soldier soldier)
        {
            _guilde = guilde;
            _soldier = soldier;
            // récupérer toutes les missions, en supprimant celles déjà en cours
            missionList = _guilde.GetMissionsList();
            missionList.RemoveAll(m => m.GetState() == "En cours");
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);
            if (_guilde.GetMissionsList().Count > 0)
            {
                _selectedMission = _guilde.GetMissionsList()[0];
            }
            else
            {
                _selectedMission = new Mission(0, 0, new Objects());
            }

            ShowMissionInfos(_selectedMission); // afficher les infos de la mission sélectionnée
        }

        protected override void OnClosed(EventArgs e)
        // méthode appelée à la fermeture de la fenêtre
        {
            base.OnClosed(e);
            _instance = null; // permet de recréer la fenêtre plus tard
        }

        private void ShowMissionInfos(Mission mission)
        // affiche les informations de la mission sélectionnée
        {
            if (_selectedMission.GetDifficulty() == 0)
            {
                MissionDifficulty.Text = "Aucune Mission n'est disponible pour envoyer ce soldat";
                StartMission.IsVisible = false;
                GoLeft.IsVisible = false;
                GoRight.IsVisible = false;
            }
            else
            {
                MissionName.Text = "Mission n° " + _selectedMission.GetId();
                MissionPicture.Source = new Bitmap(Path.Combine(
                    Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                    "images", "mission.png"));
                MissionDifficulty.Text = "Difficulté :" + mission.GetDifficulty();
                MissionNumberOfDays.Text = "Durée: " + mission.getNumberOfDaysTotal() + " jours";
                MissionSurvivingChance.Text =
                    "Chances de survie pour ce soldat: " + _selectedMission.SurvivingChances(_soldier) + " %";
                MissionState.Text = "Etat: " + mission.GetState();
                MissionEarnings.Text = "Argent à gagner: " + mission.GetEarnings();
                MissionEarnings.Text += "\nObjet à gagner: " + mission.GetObjectToReceive().GetName();
            }
        }

        private void GoLeftToMission(object? sender, RoutedEventArgs e)
        // naviguer vers la mission précédente
        {
            if (_selectedMission == missionList[0])
            {
                _selectedMission = missionList[missionList.Count - 1];
            }
            else
            {
                for (int i = 0; i < missionList.Count; i++)
                {
                    if (missionList[i] == _selectedMission)
                    {
                        _selectedMission = missionList[i - 1];
                    }
                }
            }
            ShowMissionInfos(_selectedMission);
        }

        private void GoRightToMission(object? sender, RoutedEventArgs e)
        // naviguer vers la mission suivante
        {
            if (_selectedMission == missionList[missionList.Count - 1])
            {
                _selectedMission = missionList[0];
            }
            else
            {
                for (int i = 0; i < missionList.Count; i++)
                {
                    if (missionList[i] == _selectedMission)
                    {
                        _selectedMission = missionList[i + 1];
                        break;
                    }
                }
            }
            ShowMissionInfos(_selectedMission);
        }

        private void BackToMainMenu()
        // retour à la fenêtre principale du jeu
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        private void BackToMainSoldier(object? sender, RoutedEventArgs e)
        // retour à la fenêtre de gestion des soldats
        {
            var soldierWindow = SoldierWindow.GetInstance(_guilde);
            soldierWindow.Show();
            Close();
        }

        private async void ValidateSendingToMission(object? sender, RoutedEventArgs e)
        // validation de l'envoi du soldat en mission
        {
            ValidatingSendingSoldierToMission.Text =
                _soldier.GetName() + " a été assigné à la mission: n° " + (_guilde.GetMissionsList().IndexOf(_selectedMission) + 1);
            _guilde.SendSoldierToMission(_soldier, _selectedMission);

            var flyout = FlyoutBase.GetAttachedFlyout(StartMission);
            flyout?.ShowAt(StartMission);

            // attendre 2 secondes puis cacher le flyout
            await Task.Delay(2000);
            flyout?.Hide();

            BackToMainMenu();
        }
    }
}
