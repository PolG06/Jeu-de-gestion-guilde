// importation des bibliothèques
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;

// importation de nos classes
namespace Pol_Guymarc_Projet
{
    // Fenêtre permettant de sélectionner un soldat pour l'envoyer en mission
    public partial class SelectingSoldierToStartMissionWindow : Window
    {
        // instance unique (Singleton)
        private static SelectingSoldierToStartMissionWindow? _instance;
        // mission à laquelle le soldat sera envoyé
        private readonly Mission _selectedMission;
        // soldat actuellement sélectionné
        private Soldier? _selectedSoldier;
        // liste des soldats disponibles
        private List<Soldier> soldierList;
        // référence vers la guilde
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde et mission
        public static SelectingSoldierToStartMissionWindow GetInstance(Guilde guilde, Mission selectedMission)
        {
            if (_instance == null)
            {
                _instance = new SelectingSoldierToStartMissionWindow(guilde, selectedMission);
            }
            return _instance;
        }

        // Constructeur privé
        private SelectingSoldierToStartMissionWindow(Guilde guilde, Mission selectedMission)
        {
            _selectedMission = selectedMission;
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            // récupérer les soldats libres
            soldierList = _guilde.GetSoldiersList().ToList();
            soldierList.RemoveAll(soldier => soldier.GetState() != "Libre");

            if (soldierList.Count == 0)
            {
                _selectedSoldier = new Soldier(); // soldat "vide" si aucun n'est libre
            }
            else
            {
                _selectedSoldier = soldierList[0];
            }

            ShowSoldierInfos(_selectedSoldier);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null; // permet de recréer la fenêtre plus tard
        }

        // Affiche les informations du soldat sélectionné
        private void ShowSoldierInfos(Soldier soldier)
        {
            if (soldier.GetDescription() == "Inconnue")
            {
                SoldierSendToMission.IsVisible = false;
                GoRight.IsVisible = false;
                GoLeft.IsVisible = false;
                ErrorMessage.Text = "Vous n'avez pas de soldat disponible à envoyer en mission";
                SeeSoldier.IsVisible = false;
                return;
            }

            SeeSoldier.IsVisible = true;
            ErrorMessage.Text = string.Empty;
            SoldierSendToMission.IsVisible = true;
            GoRight.IsVisible = true;
            GoLeft.IsVisible = true;

            SoldierName.Text = soldier.GetName();
            SoldierLevel.Text = "Niveau : " + soldier.GetLevel();
            SoldierPicture.Source = new Bitmap(Path.Combine(
                Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                "images", soldier.GetImageName()));
            SoldierExp.Text = "Exp: " + soldier.GetExp() + " pts / " + (int)(3 * Math.Pow(10, soldier.GetLevel())) + " pts";
            SoldierObjectPossessed.Text = "Objet possédé: " + soldier.GetObject().GetName();
            SoldierState.Text = "Etat: " + soldier.GetState();
            SoldierFatigue.Text = "Fatigue : " + soldier.GetFatigue() + " /100";
            SoldierDamages.Text = "Dégâts: " + soldier.GetDamages();
            SoldierSurvivingChances.Text = "Chances de survie pour cette mission: " +
                                           _selectedMission.SurvivingChances(soldier) + "%";
            SoldierDiscretionPoints.Text = "Points de discrétion: " + soldier.GetDiscretionPoints();
            SoldierPv.Text = soldier.GetActualPV() + " / " + soldier.GetMaxPv() + " PV";
            SoldierNumberOfBread.Text = "Nombre de pains par jour: " + soldier.GetBreadsADay();
            SoldierNumberOfMeats.Text = "Nombre de viandes par jour: " + soldier.GetMeatsADay();
            SoldierSalaryADay.Text = "Salaire par jour: " + soldier.GetSalaryADay() + " pièces.";
        }

        // naviguer vers le soldat précédent
        private void GoLeftToSoldier(object? sender, RoutedEventArgs e)
        {
            if (_selectedSoldier == soldierList[0])
            {
                _selectedSoldier = soldierList[soldierList.Count - 1];
            }
            else
            {
                for (int i = 0; i < soldierList.Count; i++)
                    if (soldierList[i] == _selectedSoldier)
                        _selectedSoldier = soldierList[i - 1];
            }
            ShowSoldierInfos(_selectedSoldier);
        }

        // naviguer vers le soldat suivant
        private void GoRightToSoldier(object? sender, RoutedEventArgs e)
        {
            if (_selectedSoldier == soldierList[soldierList.Count - 1])
            {
                _selectedSoldier = soldierList[0];
            }
            else
            {
                for (int i = 0; i < soldierList.Count; i++)
                    if (soldierList[i] == _selectedSoldier)
                    {
                        _selectedSoldier = soldierList[i + 1];
                        break;
                    }
            }
            ShowSoldierInfos(_selectedSoldier);
        }

        // retour à la fenêtre principale du jeu
        private void BackToMainMenu()
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        // retour à la liste des missions
        private void BackToMainMission(object? sender, RoutedEventArgs e)
        {
            var missionWindow = MissionWindow.GetInstance(_guilde);
            missionWindow.Show();
            Close();
        }

        // validation de l'envoi du soldat à la mission
        private async void SendToMission(object? sender, RoutedEventArgs e)
        {
            SoldierSendToMissionMessage.Text = "Vous venez d'assigner " + _selectedSoldier.GetName() +
                                               " à la mission n° " +
                                               (_guilde.GetMissionsList().IndexOf(_selectedMission) + 1);

            var flyout = FlyoutBase.GetAttachedFlyout(SoldierSendToMission);
            flyout?.ShowAt(SoldierSendToMission);

            // Attendre 2 secondes puis cacher le flyout
            await Task.Delay(2000);
            flyout?.Hide();

            _guilde.SendSoldierToMission(_selectedSoldier, _selectedMission);

            BackToMainMenu();
        }
    }
}
