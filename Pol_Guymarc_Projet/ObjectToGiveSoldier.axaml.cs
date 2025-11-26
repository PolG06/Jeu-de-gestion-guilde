using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls.Primitives;

namespace Pol_Guymarc_Projet
{
    public partial class ObjectToGiveSoldierWindow : Window
    {
        private readonly Soldier _soldier;
        private static ObjectToGiveSoldierWindow? _instance;
        private Objects? _selectedObject;
        private List<Objects>? _objectList;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static ObjectToGiveSoldierWindow GetInstance(Guilde guilde, Soldier soldier)
        {
            if (_instance == null)
            {
                _instance = new ObjectToGiveSoldierWindow(guilde, soldier);
            }

            return _instance;
        }

        // Constructeur privé
        private ObjectToGiveSoldierWindow(Guilde guilde, Soldier soldier)
        {
            _guilde = guilde;
            _soldier = soldier;
            InitializeComponent();
        }
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _instance = null; 
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            _objectList = _guilde.GetObjectsDict().Keys.ToList();
            _objectList.RemoveAll(_object =>
                (_guilde.GetObjectsDict()[_object] == 0) ||
                (!(_guilde.WhoCanReceiveObject(_object).Contains(_soldier))) ||
                (_soldier.GetObject() == _object) ||
                (_object is AmethystPerl) ||
                (_object is RoyalNeckLace) ||
                (_object is GoldBar) ||
                (_object is RedRubis)
            );
            
            // Vérifier que la liste contient au moins un soldat
            if (_objectList.Count > 0)
            {
                _selectedObject = _objectList[0];
            }
            else
            {
                _selectedObject = new Objects();
            }

            ShowObjectInfos(_selectedObject); // affichage sûr, après que les contrôles sont initialisés
        }

        private void ShowObjectInfos(Objects _object)
        {
            if (_object.GetName() == "Aucun")
            {
                ObjectDescription.Text = "Vous ne pouvez donner aucun objet à ce soldat";
                GoLeft.IsVisible = false;
                GoRight.IsVisible = false;
                GivingObject.IsVisible = false;
                TitleObjectToGive.IsVisible = false;

            }
            else
            {
                TitleObjectToGive.IsVisible = true;
                GoLeft.IsVisible = true;
                GoRight.IsVisible = true;
                GivingObject.IsVisible = true;
                ObjectName.Text = "Nom: " + _object.GetName();
                ObjectDescription.Text = "Description: " + _object.GetDescription();
                ObjectQuantity.Text = "Vous en avez: " + _guilde.GetObjectsDict()[_object];
                ObjectPicture.Source =
                    new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                        "images", _object.GetImageName()));
                if (_object is MilitaryEquipement militaryequipement)
                {
                    ObjectType.Text = "Type: Equipement Militaire";
                    ObjectOther.Text = "Peut être donné à " + militaryequipement.getTypeCompatibleHero();
                }
                else if (_object is Potion potion)
                {
                    ObjectType.Text = "Type: Potion";
                    ObjectOther.Text = "Soigne " + potion.getPvHealed() + " Pv";
                }
                else if (_object is Recovery)
                {
                    ObjectType.Text = "Type: Guerison";
                    ObjectOther.Text = "Peut guérir la blessure de votre soldat";
                }
                
            }


        }

        private void GoLeftToObject(object? sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _objectList.Count; i++)
            {
                if (_objectList[i] == _selectedObject)
                {
                    if (i == 0)
                    {
                        _selectedObject = _objectList[_objectList.Count - 1];
                    }
                    else
                    {
                        _selectedObject = _objectList[i - 1];
                    }

                    ShowObjectInfos(_selectedObject);
                }


            }
        }

        private void GoRightToObject(object? sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _objectList.Count; i++)
            {
                if (_objectList[i] == _selectedObject)
                {
                    if (i == _objectList.Count - 1)
                    {
                        _selectedObject = _objectList[0];
                    }
                    else
                    {
                        _selectedObject = _objectList[i + 1];
                    }

                    ShowObjectInfos(_selectedObject);
                }


            }
        }
        private void BackToMainMenu()
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }
        private void BackToMainSoldier(object? sender, RoutedEventArgs e)
        {
            var soldierWindow = SoldierWindow.GetInstance(_guilde);
            soldierWindow.Show();
            Close();
        }

        private async void ValidateGivingObject(object? sender, RoutedEventArgs e)
        {
            if (_selectedObject is Potion potion)
            {
                _guilde.UsePotion(_soldier, potion);
                _guilde.GetObjectsDict()[_selectedObject] -=1;
            }
            else if (_selectedObject is MilitaryEquipement militaryequipement)
            {
                _guilde.GiveObjectToSoldier(militaryequipement,_soldier);
            }
            else if (_selectedObject is Recovery)
            {
                _soldier.RemoveInjury();
                _guilde.GetObjectsDict()[_selectedObject] -= 1;
            }
            ValidateGivingObjectToSoldier.Text="Vous venez de donner "+_selectedObject.GetName()+" a "+_soldier.GetName();
            var flyout = FlyoutBase.GetAttachedFlyout(GivingObject);
            flyout?.ShowAt(GivingObject);

            // Attendre 2 secondes puis cacher le flyout
            await Task.Delay(2000);
            flyout?.Hide();
            BackToMainMenu();
            
        }
    }

}