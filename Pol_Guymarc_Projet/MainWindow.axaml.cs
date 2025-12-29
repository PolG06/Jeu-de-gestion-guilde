// importation des bibliothèques
using System;
using Avalonia.Controls;
using Avalonia.Input;
using Pol_Guymarc_Projet.Classes;

//importation de nos classes
namespace Pol_Guymarc_Projet;

public partial class MainWindow : Window
// classe de la fenêtre principale
{
    private Guilde guilde;
    // constructeur de la fenêtre principale
    public MainWindow()
    {
        InitializeComponent();
    }

    private void CreateGuilde(object? sender, EventArgs e)
    // permet de créer et d'initialiser la guilde au lancement du jeu
    {
        // création de la guilde
        guilde = Guilde.Instance;

        // création du premier soldat et ajout à l'armée
        guilde.AddSoldier(new Swordsman("Maximus"));

        // création des évènements indésirables et ajout à la liste
        guilde.AddUndesirableEvent(new Fire());
        guilde.AddUndesirableEvent(new Storm());
        guilde.AddUndesirableEvent(new Thief());

        // instanciation des objets utilisables dans le jeu
        AmethystPerl amethystPerl = new AmethystPerl();
        RedRubis redRubis = new RedRubis();
        GoldBar goldBar = new GoldBar();
        LittlePotion littlePotion = new LittlePotion();
        MediumPotion mediumPotion = new MediumPotion();
        BigPotion bigPotion = new BigPotion();
        LightCape lightCape = new LightCape();
        MetalFists metalFists = new MetalFists();
        SoulSword soulSword = new SoulSword();
        OverPowerdBow overPowerdBow = new OverPowerdBow();
        LightArmor lightArmor = new LightArmor();
        Recovery recovery = new Recovery();

        // ajout des objets dans l'inventaire avec une quantité initiale de 0
        guilde.AddObjects(amethystPerl, 0);
        guilde.AddObjects(redRubis, 0);
        guilde.AddObjects(goldBar, 0);
        guilde.AddObjects(littlePotion, 0);
        guilde.AddObjects(mediumPotion, 0);
        guilde.AddObjects(bigPotion, 0);
        guilde.AddObjects(lightCape, 0);
        guilde.AddObjects(metalFists, 0);
        guilde.AddObjects(soulSword, 0);
        guilde.AddObjects(overPowerdBow, 0);
        guilde.AddObjects(lightArmor, 0);
        guilde.AddObjects(recovery, 0);

        // création de la première mission et ajout à la liste
        guilde.AddMission(new Mission(1, 1, mediumPotion));
    }

    private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    // permet de passer de la fenêtre principale à la fenêtre de jeu
    {
        var gameWindow = GameWindow.GetInstance(guilde);
        gameWindow.Show();
        Close();
    }
}
