using Avalonia.Controls;
using Avalonia.Interactivity;
using Pol_Guymarc_Projet.Classes;
using System;
using System.Collections.Generic;

namespace Pol_Guymarc_Projet
{
    public partial class GoingAfternoonWindow : Window
    {
        private static GoingAfternoonWindow? _instance;
        private readonly Guilde _guilde;

        // Singleton : récupération de l'instance avec Guilde
        public static GoingAfternoonWindow GetInstance(Guilde guilde)
        {
            if (_instance == null)
            {
                _instance = new GoingAfternoonWindow(guilde);
            }

            return _instance;
        }

        public static GoingAfternoonWindow CreateNew(Guilde guilde)
        {
            return new GoingAfternoonWindow(guilde);
        }

        // Constructeur privé
        private GoingAfternoonWindow(Guilde guilde)
        {
            _guilde = guilde;
            InitializeComponent();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            _instance = null;

        }

        private void ShowUndesirableEvents(object? sender, EventArgs e)
        {
            AStormHappened.IsVisible = false;
            AFireHappened.IsVisible = false;
            AThiefHappened.IsVisible = false;
            int counter = 0;
            Dictionary<string, Object> undesirableEventsHappened = _guilde.AreTheUndesirableEventsComing();
            if ((int)undesirableEventsHappened["Reparation Costs Because of Storm"] > 0)
            { 
                counter++;
                AStormHappened.IsVisible = true;
                StormReparationCostsText.Text = "Coûts des réparations: "+undesirableEventsHappened["Reparation Costs Because of Storm"] + " pièces";
                _guilde.SetMoney(_guilde.GetMoney()-(int)undesirableEventsHappened["Reparation Costs Because of Storm"]);
            }
            if ((int)undesirableEventsHappened["Reparation Costs Because of Fire"] > 0)
            {
                counter++;
                AFireHappened.IsVisible = true;
                FireReparationCostsText.Text ="Coûts des réparations: "+ undesirableEventsHappened["Reparation Costs Because of Fire"] + " pièces";
                _guilde.SetMoney(_guilde.GetMoney()-(int)undesirableEventsHappened["Reparation Costs Because of Fire"]);
                
                if (((List<Soldier>)undesirableEventsHappened["Dead Soldiers In Fire"]).Count == 0)
                {
                    FireDeadSoldiersText.Text = "Soldats morts: Aucun, vous avez eu de la chance.";
                }
                else
                {
                    FireDeadSoldiersText.Text = "Soldats morts: ";
                    foreach (Soldier soldier in (List<Soldier>)undesirableEventsHappened["Dead Soldiers In Fire"])
                    {
                        TextBlock txt = new TextBlock();
                        txt.Text = "- " + soldier.GetName() + " ( ";
                        if (soldier is Bandit)
                        {
                            txt.Text += "Voleur";
                        }
                        else if (soldier is Giant)
                        {
                            txt.Text += "Géant";
                        }
                        else if (soldier is Swordsman)
                        {
                            txt.Text += "Epeiste";
                        }
                        else if (soldier is Paladin)
                        {
                            txt.Text += "Paladin";
                        }
                        else
                        {
                            txt.Text += "Archer";
                        }
                        txt.Text += " Niveau "+soldier.GetLevel()+" )";
                        AFireHappened.Children.Add(txt);
                        soldier.ToDie();
                    }
                    
                }
            }
            if (((Dictionary<Objects, int>)undesirableEventsHappened["Stolen Objects"]).Count >0)
            {
                counter++;
                AThiefHappened.IsVisible = true;
                ThiefStolenObjectsText.Text = "Un intru s'est introduit au royaume d'Avalonia et a volé des objets: ";
                
                foreach (KeyValuePair<Objects, int> element in (Dictionary<Objects, int>)undesirableEventsHappened["Stolen Objects"])
                {
                    TextBlock txt = new TextBlock();
                    txt.Text = "- "+element.Key.GetName()+" , Quantité: "+element.Value;
                    AThiefHappened.Children.Add(txt);
                    _guilde.RemoveObject(element.Key,element.Value);
                    
                }
            }

            AfternoonAnounce.Text += _guilde.GetDayCounter();
            if (counter == 0)
            {
                Informations.Text = "Aucun évènement indésirable s'est produit cet après midi, vous avez de la chance";
            }
            else if (counter == 1)
            {
                Informations.Text = "Un évènement particulier s'est produit cet après midi:";
            }
            else 
            {
                Informations.Text = "Plusieurs évènements particuliers se sont produits cet après midi: ";
            }
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