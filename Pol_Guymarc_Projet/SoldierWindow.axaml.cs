using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;
using Avalonia.Media.Imaging;


namespace Pol_Guymarc_Projet
{
    public partial class SoldierWindow : Window
    {
        private static SoldierWindow? _instance;
        private Soldier? _selectedSoldier;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static SoldierWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new SoldierWindow(guilde);
            }
            return _instance;
        }
        public static SoldierWindow CreateNew(Guilde guilde)
        {
            return new SoldierWindow(guilde);
        }
        // Constructeur privé
        private SoldierWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);

            // Vérifier que la liste contient au moins un soldat
            if (_guilde.GetSoldiersList().Count > 0)
            {
                _selectedSoldier = _guilde.GetSoldiersList()[0];
                ShowSoldierInfos(_selectedSoldier); // affichage sûr, après que les contrôles sont initialisés
            }
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null; // Permet de recréer plus tard
        }

        // Méthodes pour gérer les soldats
        private void ShowSoldierInfos(Soldier soldier)
        {
            SoldierGiveObject.IsVisible = true;
            SoldierGoSleep.IsVisible = true;
            SoldierSendToMission.IsVisible = true;
            if (soldier.GetState() == "En mission")
            {
                SoldierSendToMission.IsVisible = false;
                SoldierGoSleep.IsVisible = false;
                SoldierGiveObject.IsVisible = false;
            }
            else if (soldier.GetState() == "Au repos")
            {
                SoldierGiveObject.IsVisible = false;
            }
            else if(soldier.GetState()=="Blessé")
            {
                SoldierGiveObject.IsVisible = false;
                SoldierGoSleep.IsVisible = false;
            }
            else if (soldier.GetState() == "Mort")
            {
                SoldierSendToMission.IsVisible = false;
                SoldierGiveObject.IsVisible = false;
                SoldierGoSleep.IsVisible = false;
            }
            SoldierName.Text = soldier.GetName();
            SoldierLevel.Text = "Niveau : " + soldier.GetLevel();
            SoldierPicture.Source =
                new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                    "images", soldier.GetImageName()));
            SoldierExp.Text="Exp: "+soldier.GetExp()+" pts/ "+(int)(3* Math.Pow(10, soldier.GetLevel()))+" pts";
            SoldierObjectPossessed.Text = "Objet possédé: "+soldier.GetObject().GetName();
            if (soldier.GetObject().GetName() != "Aucun")
            {
                SoldierObjectPossessedPicture.Source=new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                    "images", soldier.GetObject().GetImageName()));
            }
            SoldierDescription.Text="Description : " + soldier.GetDescription();
            SoldierState.Text="Etat: "+soldier.GetState();
            SoldierFatigue.Text="Fatigue : " + soldier.GetFatigue()+" /100";
            SoldierDamages.Text ="Dégats: "+ soldier.GetDamages();
            SoldierDiscretionPoints.Text="Points de discretion: "+soldier.GetDiscretionPoints();
            SoldierPv.Text = soldier.GetActualPV() + "/ " + soldier.GetMaxPv() + " PV";
            SoldierNumberOfBread.Text = "Nombre de pains par jour: " + soldier.GetBreadsADay();
            SoldierNumberOfMeats.Text="Nombre de viandes par jour: " + soldier.GetMeatsADay();
            SoldierSalaryADay.Text = "Salaire par jour: " + soldier.GetSalaryADay() + " pièces.";
        }

        private void GoLeftToSoldier(object? sender, RoutedEventArgs e)
        {
            if (_selectedSoldier == _guilde.GetSoldiersList()[0])
            {
                _selectedSoldier = _guilde.GetSoldiersList()[_guilde.GetSoldiersList().Count - 1];
            }
            else
            {
                for (int i=0; i<_guilde.GetSoldiersList().Count; i++)
                {
                    if (_guilde.GetSoldiersList()[i] == _selectedSoldier)
                    {
                        _selectedSoldier=_guilde.GetSoldiersList()[i-1];
                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }

        private void GoRightToSoldier(object? sender, RoutedEventArgs e)
        {
            if (_selectedSoldier == _guilde.GetSoldiersList()[_guilde.GetSoldiersList().Count -1])
            {
                _selectedSoldier = _guilde.GetSoldiersList()[0];
            }
            else
            {
                for (int i=0; i<_guilde.GetSoldiersList().Count; i++)
                {
                    if (_guilde.GetSoldiersList()[i] == _selectedSoldier)
                        
                    {
                        _selectedSoldier=_guilde.GetSoldiersList()[i+1];
                        break;

                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }
        private void BackToMainMenue(object? sender, RoutedEventArgs e)
        {
            var gameWindow = GameWindow.GetInstance(_guilde);

            // Affiche la fenêtre principale
            gameWindow.Show();
            Close();
        }

        private void GiveObject(object? sender, RoutedEventArgs e)
        {
            
            var objectToGiveSoldier = ObjectToGiveSoldierWindow.GetInstance(_guilde, _selectedSoldier);
            objectToGiveSoldier.Show();
            Close();
        }
        private async void GoSleep(object? sender, RoutedEventArgs e)
        {
            _selectedSoldier.Sleep();
            SoldierGoSleepText.Text = "Vous venez d'envoyer " + _selectedSoldier.GetName() + " au repos";
            var flyout = FlyoutBase.GetAttachedFlyout(SoldierGoSleep);
            flyout?.ShowAt(SoldierGoSleep);

            // Attendre 2 secondes puis cacher le flyout
            await Task.Delay(2000);
            flyout?.Hide();
        }

        private void SendToMission(object? sender, RoutedEventArgs e)
        {
            var selectingMissionToSendSoldierWindow = SelectingMissionToSendSoldierWindow.GetInstance(_guilde,_selectedSoldier);
            // Affiche la fenêtre principale
            selectingMissionToSendSoldierWindow.Show();
            Close();
        }
    }
}