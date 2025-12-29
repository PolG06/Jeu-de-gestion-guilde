// importation des bibliothèques nécessaires
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

//importation de nos classes
namespace Pol_Guymarc_Projet
{
    // classe de la fenêtre de l'inventaire
    public partial class InventaryWindow : Window
    {
        private static InventaryWindow? _instance;
        // objet actuellement sélectionné dans l'inventaire
        private Objects? _selectedObject;
        // liste des objets présents dans l'inventaire
        private List<Objects>? _objectList;
        // variable globale contenant la guilde
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static InventaryWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new InventaryWindow(guilde);
            }

            return _instance;
        }

        // permet de créer une nouvelle fenêtre sans singleton
        public static InventaryWindow CreateNew(Guilde guilde)
        {
            return new InventaryWindow(guilde);
        }

        // Constructeur privé de la fenêtre
        private InventaryWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);

            // récupération de la liste des objets de la guilde
            _objectList = _guilde.GetObjectsDict().Keys.ToList();

            // suppression des objets dont la quantité est nulle
            for (int i = 0; i < _objectList.Count(); i++)
            {
                if (_guilde.GetObjectsDict()[_objectList[i]] == 0)
                {
                    _objectList.RemoveAt(i);
                    i--;
                }
            }

            // vérifie que l'inventaire contient au moins un objet
            if (_objectList.Count > 0)
            {
                _selectedObject = _objectList[0];
            }
            else
            {
                _selectedObject = new Objects();
            }

            // affichage des informations de l'objet sélectionné
            ShowObjectInfos(_selectedObject);
        }

        private void ShowObjectInfos(Objects object_)
        // affiche les informations d'un objet
        {
            ValidateSelling.IsVisible = false;
            ValidateAndSell.IsVisible = false;
            GivingObject.IsVisible = true;

            // cas où l'inventaire est vide
            if (object_.GetName() == "Aucun")
            {
                ObjectDescription.Text = "Votre inventaire est vide";
                GivingObject.IsVisible = false;
                SellingObject.IsVisible = false;
                GoLeft.IsVisible = false;
                GoRight.IsVisible = false;
            }
            else
            {
                // affichage des boutons et informations de l'objet
                GivingObject.IsVisible = true;
                SellingObject.IsVisible = true;
                GoLeft.IsVisible = true;
                GoRight.IsVisible = true;

                ObjectName.Text = "Nom: " + object_.GetName();
                ObjectDescription.Text = "Description: " + object_.GetDescription();
                ObjectBuyingPrice.Text = "Prix d'achat: " + object_.GetBuyingPrice() + " pièces.";
                ObjectSellingPrice.Text = "Prix de vente: " + object_.GetSellingPrice() + " pièces.";
                ObjectQuantity.Text = "Quantité: " + _guilde.GetObjectsDict()[object_];

                // chargement de l'image de l'objet
                ObjectPicture.Source =
                    new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                        "images", object_.GetImageName()));

                // affichage des informations spécifiques selon le type d'objet
                if (object_ is MilitaryEquipement militaryequipement)
                {
                    ObjectType.Text = "Type: Equipement Militaire";
                    ObjectOther.Text = "Peut être donné à " + militaryequipement.getTypeCompatibleSoldier();
                }
                else if (object_ is Potion potion)
                {
                    ObjectType.Text = "Type: Potion";
                    ObjectOther.Text = "Soigne " + potion.getPvHealed() + " Pv";
                }
                else if (object_ is Recovery)
                {
                    ObjectType.Text = "Type: Guerison";
                }
                else if ((object_ is AmethystPerl) || (object_ is RedRubis) || (object_ is RoyalNeckLace) ||
                         (object_ is GoldBar))
                {
                    ObjectType.Text = "Type: Objet Rare";
                }
            }
        }

        private void GoLeftToObject(object? sender, RoutedEventArgs e)
        // permet de naviguer vers l'objet précédent
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
        // permet de naviguer vers l'objet suivant
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

        private void BackToMainMenu(object? sender, RoutedEventArgs e)
        // permet de revenir à la fenêtre principale
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        private void SellThisObject(object? sender, RoutedEventArgs e)
        // affiche l'interface de vente d'objet
        {
            ValidateSelling.IsVisible = true;
            ValidateAndSell.IsVisible = true;
            GivingObject.IsVisible = false;
        }

        private async void SellQuantityOfObject(object? sender, RoutedEventArgs e)
        // permet de vendre une quantité donnée de l'objet sélectionné
        {
            bool achat_valide = false;
            int quantity = (int)(QuantityToSell.Value ?? 0);

            // vérification de la quantité à vendre
            if (quantity > 0 && _guilde.GetObjectsDict()[_selectedObject] >= quantity)
            {
                achat_valide = true;
                _guilde.SellObject(_selectedObject, quantity);
                NotificationText.Text = "Vous venez de vendre " + quantity + " " +
                                        _selectedObject.GetName() + "\npour " +
                                        _selectedObject.GetSellingPrice() * quantity + " pièces";
            }
            else if (quantity > 0 && _guilde.GetObjectsDict()[_selectedObject] < quantity)
            {
                NotificationText.Text = "Vous ne pouvez vendre que " +
                                        _guilde.GetObjectsDict()[_selectedObject] + " " +
                                        _selectedObject.GetName();
            }
            else
            {
                NotificationText.Text = "Erreur, la valeur entrée n'est pas valide";
            }

            // affichage d'une notification temporaire
            var flyout = FlyoutBase.GetAttachedFlyout(ValidateAndSell);
            flyout?.ShowAt(ValidateAndSell);

            await Task.Delay(2000);
            flyout?.Hide();

            // retour au menu principal si la vente est valide
            if (achat_valide)
            {
                BackToMainMenu(sender, e);
            }
        }

        private void GivingObjectToSoldier(object? sender, RoutedEventArgs e)
        // ouvre la fenêtre de sélection du soldat à qui donner l'objet
        {
            var selectingSoldierToGiveObjectWindow =
                SelectingSoldierToGiveObjectWindow.GetInstance(_guilde, _selectedObject);
            selectingSoldierToGiveObjectWindow.Show();
            Close();
        }
    }
}
