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


namespace Pol_Guymarc_Projet
{
    public partial class SelectingSoldierToGiveObjectWindow : Window
    {
        private static SelectingSoldierToGiveObjectWindow? _instance;
        private readonly Objects _selectedobject;
        private Soldier? _selectedSoldier;
        private List<Soldier> soldierList;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static SelectingSoldierToGiveObjectWindow GetInstance(Guilde guilde, Objects selectedObject)
        {
            if (_instance == null)
            {
                _instance = new SelectingSoldierToGiveObjectWindow(guilde, selectedObject);
            }
            return _instance;
        }
        private SelectingSoldierToGiveObjectWindow(Guilde guilde, Objects selectedObject)
        {
            _selectedobject = selectedObject;
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            
            soldierList = _guilde.WhoCanReceiveObject(_selectedobject).ToList();
            if (soldierList.Count == 0)
            {
                _selectedSoldier =new Soldier();
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
            _instance = null;
        }
        
        private void ShowSoldierInfos(Soldier soldier)
        {
            if (soldier.GetDescription() == "Inconnue")
            {
                GivingObjectToSoldier.IsVisible = false;
                GoRight.IsVisible = false;
                GoLeft.IsVisible = false;
                ErrorMessage.Text = "Vous n'avez pas de soldat à qui donner cet equipement";
                SeeSoldier.IsVisible = false;
            }
            else
            {
                if (_selectedobject is AmethystPerl || _selectedobject is RedRubis ||
                    _selectedobject is RoyalNeckLace || _selectedobject is GoldBar)
                {
                    GivingObjectToSoldier.IsVisible = false;
                }
                else
                {
                    GivingObjectToSoldier.IsVisible = true;
                }
                SeeSoldier.IsVisible = true;
                ErrorMessage.Text = string.Empty;
                GoRight.IsVisible = true;
                GoLeft.IsVisible = true;
                SoldierName.Text = soldier.GetName();
                SoldierLevel.Text = "Niveau : " + soldier.GetLevel();
                SoldierPicture.Source =
                    new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                        "images", soldier.GetImageName()));
                SoldierExp.Text="Exp: "+soldier.GetExp()+" pts/ "+(int)(3* Math.Pow(10, soldier.GetLevel()))+" pts";
                SoldierObjectPossessed.Text = "Objet possédé: "+soldier.GetObject().GetName();
                SoldierState.Text="Etat: "+soldier.GetState();
                SoldierFatigue.Text="Fatigue : " + soldier.GetFatigue()+" /100";
                SoldierDamages.Text ="Dégats: "+ soldier.GetDamages();
                SoldierDiscretionPoints.Text="Points de discretion: "+soldier.GetDiscretionPoints();
                SoldierPv.Text = soldier.GetActualPV() + "/ " + soldier.GetMaxPv() + " PV";
                SoldierNumberOfBread.Text = "Nombre de pains par jour: " + soldier.GetBreadsADay();
                SoldierNumberOfMeats.Text="Nombre de viandes par jour: " + soldier.GetMeatsADay();
                SoldierSalaryADay.Text = "Salaire par jour: " + soldier.GetSalaryADay() + " pièces.";
                if (_selectedobject is Potion potion)
                {
                    if (_selectedSoldier.GetActualPV() + potion.getPvHealed() > _selectedSoldier.GetMaxPv())
                    {
                        SoldierPv.Text += " --> " + _selectedSoldier.GetMaxPv() + " / " + _selectedSoldier.GetMaxPv() +
                                          " Pv";
                    }
                    else
                    {
                        SoldierPv.Text += " --> " + _selectedSoldier.GetActualPV()+potion.getPvHealed() + " / " + _selectedSoldier.GetMaxPv() +
                                          " Pv";
                    }
                }
                else if (_selectedobject is Recovery)
                {
                    SoldierState.Text += " --> Libre";
                }
                else if (_selectedobject is LightCape)
                {
                    SoldierDiscretionPoints.Text += " --> " + 2 * _selectedSoldier.GetDiscretionPoints();
                }
                else if (_selectedobject is MetalFists)
                {
                    SoldierDamages.Text += " --> " + 2 * _selectedSoldier.GetDamages();
                }
                else if (_selectedobject is SoulSword)
                {
                    SoldierDamages.Text += " --> " + (int)(1.7 * _selectedSoldier.GetDamages());
                    SoldierDiscretionPoints.Text += " --> " + (int)(1.3 * _selectedSoldier.GetDiscretionPoints());
                }
                else if (_selectedobject is OverPowerdBow)
                {
                    SoldierDamages.Text += " --> " + 2 * _selectedSoldier.GetDamages();
                }
                else if (_selectedobject is LightArmor)
                {
                    SoldierDamages.Text += " --> " + (int)(1.5 * _selectedSoldier.GetDamages());
                    SoldierDiscretionPoints.Text += " --> " + (int)(1.5 * _selectedSoldier.GetDiscretionPoints());
                }
            }
            
        }

        private void GoLeftToSoldier(object? sender, RoutedEventArgs e)
        {
            if (_selectedSoldier == soldierList[0])
            {
                _selectedSoldier = soldierList[soldierList.Count - 1];
            }
            else
            {
                for (int i=0; i<soldierList.Count; i++)
                {
                    if (soldierList[i] == _selectedSoldier)
                    {
                        _selectedSoldier=soldierList[i-1];
                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }

        private void GoRightToSoldier(object? sender, RoutedEventArgs e)
        {
            if (_selectedSoldier == soldierList[soldierList.Count -1])
            {
                _selectedSoldier = soldierList[0];
            }
            else
            {
                for (int i=0; i<soldierList.Count; i++)
                {
                    if (soldierList[i] == _selectedSoldier)
                        
                    {
                        _selectedSoldier=soldierList[i+1];
                        break;

                    }
                }
            }
            ShowSoldierInfos(_selectedSoldier);
        }
        private void BackToMainMenu()
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }
        private void BackToMainObjects(object? sender, RoutedEventArgs e)
        {
            var inventaryWindow = InventaryWindow.GetInstance(_guilde);
            inventaryWindow.Show();
            Close();
        }
        
        private async void GiveObject(object? sender, RoutedEventArgs e)
        {
            ObjectGivenToSoldier.Text = "Vous venez de donner " + _selectedobject.GetName() +
                                        " à " + _selectedSoldier.GetName();
            if (_selectedobject is MilitaryEquipement)
            {
                if (_selectedSoldier is Bandit)
                {
                    ObjectGivenToSoldier.Text += "\nSes points de discretion passent de " +
                                                _selectedSoldier.GetDiscretionPoints() + " à " +
                                                _selectedSoldier.GetDiscretionPoints() * 2;
                }
                else if (_selectedSoldier is Giant)
                {
                    ObjectGivenToSoldier.Text +="\nSes dégâts passent de " +
                                                _selectedSoldier.GetDamages() + " à " +
                                                _selectedSoldier.GetDamages() * 2;
                }
                else if (_selectedSoldier is Swordsman)
                {
                    ObjectGivenToSoldier.Text +="\nSes dégâts passent de " +
                                                _selectedSoldier.GetDamages() + " à " +(int)
                                                (_selectedSoldier.GetDamages() * 1.7);
                    
                    ObjectGivenToSoldier.Text += "\nSes points de discretion passent de " +
                                                 _selectedSoldier.GetDiscretionPoints() + " à " +
                                                 (int)(_selectedSoldier.GetDiscretionPoints() * 1.3);
                }
                else if (_selectedSoldier is Archer)
                {
                    ObjectGivenToSoldier.Text +="\nSes dégâts passent de " +
                                                _selectedSoldier.GetDamages() + " à " +
                                                _selectedSoldier.GetDamages() * 2;
                }
                else if (_selectedSoldier is Paladin)
                {
                    ObjectGivenToSoldier.Text +="\nSes dégâts passent de " +
                                                _selectedSoldier.GetDamages() + " à " +(int)
                                                (_selectedSoldier.GetDamages() * 1.5);
                    
                    ObjectGivenToSoldier.Text += "\nSes points de discretion passent de " +
                                                 _selectedSoldier.GetDiscretionPoints() + " à " +
                                                 (int)(_selectedSoldier.GetDiscretionPoints() * 1.5);
                }
            }
                                               
            var flyout = FlyoutBase.GetAttachedFlyout(GivingObjectToSoldier);
            flyout?.ShowAt(GivingObjectToSoldier);
            await Task.Delay(4000);
            flyout?.Hide();
            _guilde.GiveObjectToSoldier(_selectedobject, _selectedSoldier);
            BackToMainMenu();
        }
    }
}