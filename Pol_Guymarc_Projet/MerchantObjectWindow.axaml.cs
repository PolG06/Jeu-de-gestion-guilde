// importation des bibliothèques
using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.IO;
using Avalonia.Media.Imaging;
using System.Linq;
using System.Collections.Generic;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;

//importation de nos classes
namespace Pol_Guymarc_Projet
{
    // classe de la fenêtre du marchand d'objets
    public partial class MerchantObjectWindow : Window
    {
        private static MerchantObjectWindow? _instance;
        // objet actuellement sélectionné
        private Objects? _selectedObject;
        // liste des objets disponibles chez le marchand
        private List<Objects>? _objectList;
        // variable globale contenant la guilde
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static MerchantObjectWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new MerchantObjectWindow(guilde);
            }

            return _instance;
        }

        // Constructeur privé de la fenêtre
        private MerchantObjectWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        // méthode appelée à l'ouverture de la fenêtre
        {
            base.OnOpened(e);

            // récupération de la liste des objets disponibles
            _objectList = _guilde.GetObjectsDict().Keys.ToList();
            _selectedObject = _objectList[0];

            // affichage des informations du premier objet
            ShowObjectInfos(_selectedObject);
        }

        protected override void OnClosed(EventArgs e)
        // méthode appelée à la fermeture de la fenêtre
        {
            base.OnClosed(e);
            _instance = null; // Permet de recréer la fenêtre plus tard
        }

        private void ShowObjectInfos(Objects _object)
        // affiche les informations d'un objet
        {
            MoneyYouHave.Text = "Vous avez " + _guilde.GetMoney() + " pièces";
            ObjectName.Text = "Nom: " + _object.GetName();
            ObjectDescription.Text = "Description: " + _object.GetDescription();
            ObjectBuyingPrice.Text = "Prix d'achat: " + _object.GetBuyingPrice() + " pièces.";
            ObjectSellingPrice.Text = "Prix de vente: " + _object.GetSellingPrice() + " pièces.";
            ObjectQuantity.Text = "Vous en avez: " + _guilde.GetObjectsDict()[_object];

            // chargement de l'image de l'objet
            ObjectPicture.Source =
                new Bitmap(Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.Parent.Parent.FullName,
                    "images", _object.GetImageName()));

            // affichage des informations selon le type d'objet
            if (_object is MilitaryEquipement militaryequipement)
            {
                ObjectType.Text = "Type: Equipement Militaire";
                ObjectOther.Text = "Peut être donné à " + militaryequipement.getTypeCompatibleSoldier();
            }
            else if (_object is Potion potion)
            {
                ObjectType.Text = "Type: Potion";
                ObjectOther.Text = "Soigne " + potion.getPvHealed() + " Pv";
            }
            else if (_object is Recovery)
            {
                ObjectType.Text = "Type: Guerison";
            }
            else if ((_object is AmethystPerl) || (_object is RedRubis) || (_object is RoyalNeckLace) ||
                     (_object is GoldBar))
            {
                ObjectType.Text = "Type: Objet Rare";
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

                    // réinitialisation de l'interface d'achat
                    HowManyToBuy.IsVisible = false;
                    BuyingObject.IsVisible = false;
                    QuantityBuying.Value = 0;
                    break;
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

                    // réinitialisation de l'interface d'achat
                    HowManyToBuy.IsVisible = false;
                    BuyingObject.IsVisible = false;
                    QuantityBuying.Value = 0;
                    break;
                }
            }
        }

        private void BackToMainMenu()
        // permet de revenir à la fenêtre principale
        {
            var gameWindow = GameWindow.GetInstance(_guilde);
            gameWindow.Show();
            Close();
        }

        private void BackToMainMerchant(object? sender, RoutedEventArgs e)
        // permet de revenir au menu principal du marchand
        {
            var merchantWindow = MerchantWindow.GetInstance(_guilde);
            merchantWindow.Show();
            Close();
        }

        private void SelectingthisObject(object? sender, RoutedEventArgs e)
        // affiche l'interface pour choisir la quantité à acheter
        {
            HowManyToBuy.IsVisible = true;
            BuyingObject.IsVisible = true;
        }

        private async void BuythisObject(object? sender, RoutedEventArgs e)
        // permet d'acheter une quantité donnée de l'objet sélectionné
        {
            bool exit = false;
            int quantity = (int)(QuantityBuying.Value ?? 0);

            // vérification des conditions d'achat
            if (quantity > 0 && _selectedObject.GetBuyingPrice() * quantity <= _guilde.GetMoney())
            {
                _guilde.BuyObject(_selectedObject, quantity);
                ValidateBuyingObject.Text = "Vous venez d'acheter " + quantity + " " + _selectedObject.GetName();
                exit = true;
            }
            else if (quantity > 0 && _selectedObject.GetBuyingPrice() * quantity > _guilde.GetMoney())
            {
                ValidateBuyingObject.Text = "Vous n'avez pas assez d'argent pour acheter " +
                                            quantity + " " + _selectedObject.GetName();
            }
            else
            {
                ValidateBuyingObject.Text = "Erreur, la valeur entrée n'est pas valide";
            }

            // affichage d'une notification temporaire
            var flyout = FlyoutBase.GetAttachedFlyout(BuyingObject);
            flyout?.ShowAt(BuyingObject);

            await Task.Delay(2000);
            flyout?.Hide();

            // retour au menu principal après l'achat
            if (exit)
            {
                BackToMainMenu();
            }
        }
    }
}
