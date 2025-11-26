using System;
using Avalonia.Controls;
using Avalonia.Input;
using Pol_Guymarc_Projet.Classes;

namespace Pol_Guymarc_Projet;

public partial class MainWindow : Window
{
    private Guilde guilde;
    public MainWindow()
    {
        InitializeComponent();
    }
    private void CreateGuilde(object? sender, EventArgs e)
    {
        //Creation of your Guilde
        guilde = Guilde.Instance;
        //Creation of your first Hero and adding it to your Army
        guilde.AddSoldier(new Swordsman("Maximus"));
        //Creation of the natural or not undesirable events and adding to the list 
        guilde.AddUndesirableEvent(new Fire());
        guilde.AddUndesirableEvent(new Storm());
        guilde.AddUndesirableEvent(new Thief());
        //instanciation of the objects your would be able to get and use
        AmethystPerl amethystPerl= new AmethystPerl();
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
        //Adding them to the inventary dictionnary
        guilde.AddObjects(amethystPerl,0);
        guilde.AddObjects(redRubis,0);
        guilde.AddObjects(goldBar,0);
        guilde.AddObjects(littlePotion,0);
        guilde.AddObjects(mediumPotion,0);
        guilde.AddObjects(bigPotion,0);
        guilde.AddObjects(lightCape,0);
        guilde.AddObjects(metalFists,0);
        guilde.AddObjects(soulSword,0);
        guilde.AddObjects(overPowerdBow,0);
        guilde.AddObjects(lightArmor,0);
        guilde.AddObjects(recovery,0);
        //Creation of the first mission and adding it to the list
        guilde.AddMission(new Mission(1,1,mediumPotion));
    }

    private void MainWindow_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var gameWindow = GameWindow.GetInstance(guilde); // on passe l'instance existante
        gameWindow.Show();
        Close();
    }
}