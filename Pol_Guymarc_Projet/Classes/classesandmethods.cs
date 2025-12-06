using System.Linq;

namespace Pol_Guymarc_Projet.Classes;

using System;
using System.Collections.Generic;

public class Guilde
{
    private static Guilde _instance;
    private static readonly object _lock = new Objects();

    private List<Soldier> SoldiersList;
    private string dayMoment;
    private int numberOfBreads;
    private int numberOfMeats;
    private int Money;
    private int dayCounter;
    private Dictionary<Objects, int> Objectsdict;
    private List<UndesirableEvents> undesirableEventsList;
    private List<Mission> missionsList;
    private int numberOfBreadsComingTomorrow;
    private int numberOfMeatsComingTomorrow;
    private Dictionary<Objects, int> objectsComingTomorrow;

    private Guilde()
    {
        SoldiersList = new List<Soldier>();
        dayMoment = "Matin";
        numberOfBreads = 10;
        numberOfMeats = 10;
        Money = 100;
        dayCounter = 1;
        Objectsdict = new Dictionary<Objects, int>();
        undesirableEventsList = new List<UndesirableEvents>();
        missionsList = new List<Mission>();
        numberOfBreadsComingTomorrow = 0;
        numberOfMeatsComingTomorrow = 0;
        objectsComingTomorrow=new Dictionary<Objects,int>();
    }

    // 🧭 Propriété d’accès à l’instance unique
    public static Guilde Instance
    {
        get
        {
            // Double-check locking pour sécurité multi-thread
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                        _instance = new Guilde();
                }
            }

            return _instance;
        }
    }

    public void SkipDayMoment()
    {
        if (dayMoment == "Matin")
        {
            dayMoment = "Après-midi";
        }
        else if (dayMoment == "Après-midi")
        {
            dayMoment = "Soir";
            foreach (Soldier soldier in SoldiersList)
            {
                AtualiseAmelioration(soldier);
            }
        }
        else
        {
            ActualizeRessourcesToday();
            SoldierSleeping();
            Actualize();
            dayCounter++;
            dayMoment = "Matin";
        }
    }

    public void AddObjects(Objects _object, int Quantity)
    {
        if (Objectsdict.ContainsKey(_object))
        {
            Objectsdict[_object] += Quantity;
        }
        else
        {
            Objectsdict[_object] = Quantity;
        }
    }

    public void SoldierSleeping()
    {
        foreach (Soldier soldier in SoldiersList)
        {
            soldier.SetFatigue((int)(soldier.GetFatigue() * 0.6));
        }
    }

    public void ActualizeRessourcesToday()
    {
        Money-=CalculateTodaysMoneyDistributedToSoldiers();
        numberOfBreads += numberOfBreadsComingTomorrow-CalculateTodaysNumberOfBreadsDistributedToSoldiers();
        numberOfMeats += numberOfMeatsComingTomorrow-CalculateTodaysNumberOfMeatsDistributedToSoldiers();
        foreach (KeyValuePair<Objects, int> entry in objectsComingTomorrow)
        {
            Objectsdict[entry.Key] += entry.Value;
        }
        numberOfBreadsComingTomorrow = 0;
        numberOfMeatsComingTomorrow = 0;
        objectsComingTomorrow=new Dictionary<Objects,int>();
    }
    
    
    public void AddSoldier(Soldier soldier)
    {
        SoldiersList.Add(soldier);
    }

    public void SendSoldierToMission(Soldier soldier, Mission mission)
    {
        mission.SetSoldierOn(soldier);
    }
    
    public void UsePotion(Soldier soldier, Potion potion)
    {
        if (soldier.GetActualPV() + potion.getPvHealed() > soldier.GetMaxPv())
        {
            soldier.SetActualsPv(soldier.GetMaxPv());
        }
        else
        {
            soldier.SetActualsPv(soldier.GetActualPV() + potion.getPvHealed());
        }

    }

    public void Actualize()
    {
        foreach (UndesirableEvents _event in undesirableEventsList)
        {
            _event.IntensifyProba();
        }
        MarquedSoldiersWhenDead();
        RemoveDeadSoldiers();
        foreach (Soldier soldier in SoldiersList)
        {
            if (soldier.GetState() == "Au repos")
            {
                soldier.WakingUp();
            }
        }
        

        Random rnd = new Random();
        int randomNumber = rnd.Next(1, GetDayCounter() + 1);
        for (int i = 0; i < randomNumber; i++)
        {
            Random rnd2 = new Random();
            int randomNumber2 = rnd2.Next(1, GetDayCounter() + 1);
            Random rnd3 = new Random();
            int randomNumber3 = rnd3.Next(1, GetDayCounter() + 1);

            AddMission(new Mission(randomNumber2, randomNumber3,
                Objectsdict.Keys.ElementAt(rnd.Next(Objectsdict.Count))));
        }
    }

    public Dictionary<string, Object> AreTheUndesirableEventsComing()
    {
        Dictionary<string, Object> BilanDict = new Dictionary<string, Object>();
        BilanDict["Reparation Costs Because of Storm"] = 0;
        BilanDict["Reparation Costs Because of Fire"] = 0;
        BilanDict["Dead Soldiers In Fire"] = new List<Soldier>();
        BilanDict["Stolen Objects"] = new Dictionary<Objects, int>();
        foreach (UndesirableEvents _event in undesirableEventsList)
        {
            Random R = new Random();
            double RND = R.NextDouble();

            if (RND < _event.GetHappeningProba())
                //The event is happening
            {
                if (_event is Storm storm)
                {
                    BilanDict["Reparation Costs Because of Storm"] = (int)BilanDict["Reparation Costs Because of Storm"] + storm.GetReparationCosts();
                }
                else if (_event is Fire fire)
                {
                    BilanDict["Reparation Costs Because of Fire"]= (int)BilanDict["Reparation Costs Because of Fire"] + fire.GetReparationCosts();
                    foreach (Soldier soldier in SoldiersList)
                    {
                        Random R2 = new Random();
                        double RND2 = R2.NextDouble();
                        if (RND2<fire.GetMortality() && soldier.GetState()!="En mission")
                        {
                            //The soldier is dead in the fire
                            soldier.ToDie();
                            ((List<Soldier>)BilanDict["Dead Soldiers In Fire"]).Add(soldier);
                        }
                    }
                }
                else if (_event is Thief thief)
                {
                    var stolenObjects = (Dictionary<Objects, int>)BilanDict["Stolen Objects"];
                    foreach (KeyValuePair<Objects, int> ObjectQuantity in Objectsdict)
                    {
                        if (ObjectQuantity.Value > 0)
                        {
                            stolenObjects[ObjectQuantity.Key] = 0;
                            for (int i = 1; i < ObjectQuantity.Value + 1; i++)
                            {
                                Random R2 = new Random();
                                double RND2 = R2.NextDouble();
                                if (RND2 < thief.getProbaParObjet())
                                {
                                    stolenObjects[ObjectQuantity.Key]++;
                                }
                            }

                            if (stolenObjects[ObjectQuantity.Key] == 0)
                            {
                                ((Dictionary<Objects, int>)BilanDict["Stolen Objects"]).Remove(ObjectQuantity.Key);
                            }
                        }

                    }
                }
            }
        }

        return BilanDict;
    }

    public void MarquedSoldiersWhenDead()
    {
        foreach (Soldier soldier in SoldiersList)
        {
            if (soldier.GetActualPV() == 0)
            {
                soldier.ToDie();
            }
        }
    }

    public int CalculateTodaysNumberOfBreadsDistributedToSoldiers()
    {
        int counter = 0;
        foreach (Soldier soldier in GetSoldiersList())
        {
            counter += soldier.GetBreadsADay();
        }

        return counter;
    }

    public int CalculateTodaysNumberOfMeatsDistributedToSoldiers()
    {
        int counter = 0;
        foreach (Soldier soldier in GetSoldiersList())
        {
            counter += soldier.GetMeatsADay();
        }

        return counter;
    }

    public void RemoveObject(Objects objects, int quantity)
    {
        if (Objectsdict[objects] <= quantity)
        {
            Objectsdict[objects] -= quantity;
        }
    }

    public int CalculateTodaysMoneyDistributedToSoldiers()
    {
        int counter = 0;
        foreach (Soldier soldier in GetSoldiersList())
        {
            counter += soldier.GetSalaryADay();
        }

        return counter;
    }

    public void RemoveDeadSoldiers()
    {
        SoldiersList.RemoveAll(soldier => soldier.GetState() == "Mort");
    }

    public void AddMission(Mission mission)
    {
        missionsList.Add(mission);
    }

    public List<Soldier> GetSoldiersList()
    {
        return SoldiersList;
    }

    public String GetDayMoment()
    {
        return dayMoment;
    }

    public int GetNumberOfBreads()
    {
        return numberOfBreads;
    }

    public int GetNumberOfMeats()
    {
        return numberOfMeats;
    }

    public int GetnumberOfBreadsComingTomorrow()
    {
        return numberOfBreadsComingTomorrow;
    }

    public int GetnumberOfMeatsComingTomorrow()
    {
        return numberOfMeatsComingTomorrow;
    }

    public int GetMoney()
    {
        return Money;
    }

    public int GetDayCounter()
    {
        return dayCounter;
    }

    public List<Mission> GetMissionsList()
    {
        return missionsList;
    }

    public Dictionary<Objects, int> GetObjectsDict()
    {
        return Objectsdict;
    }
    public void SetMoney(int _money)
    {
        Money = _money;
    }
    
    public void BuyObject(Objects _object, int Quantity)
    {
        if (Money >= Quantity * _object.GetBuyingPrice())
        {
            Money -= Quantity * _object.GetBuyingPrice();
            AddObjects(_object, Quantity);
        }
    }

    public void SellObject(Objects _object, int Quantity)
    {
        if (Objectsdict.ContainsKey(_object))
        {
            if (Objectsdict[_object] >= Quantity)
            {
                Objectsdict[_object] -= Quantity;
                Money += Quantity * _object.GetSellingPrice();
            }
        }
    }

    public void GiveObjectToSoldier(Objects _object, Soldier soldier)
    {
        if (_object is MilitaryEquipement _military_Object)
        {
            soldier.SetObject(_military_Object);
            if (soldier is IMakingSuper superSoldier)
            {
                superSoldier.MakeSuper();
            }
        }
        else if (_object is Potion _potion_Object)
        {
            soldier.SetActualsPv(soldier.GetActualPV() + _potion_Object.getPvHealed());
            if (soldier.GetActualPV() > soldier.GetMaxPv())
            {
                soldier.SetActualsPv(soldier.GetMaxPv());
            }
        }
        else if (_object is Recovery)
        {
            soldier.RemoveInjury();
        }

        Objectsdict[_object]--;

    }
    
    public void BuySoldier(Soldier soldier)
    {
        if (Money >= soldier.GetBuyingPrice())
        {
            SoldiersList.Add(soldier);
            Money -= soldier.GetBuyingPrice();
        }
    }
    public List<Soldier> WhoCanReceiveThisMilitaryEquipement(MilitaryEquipement _militaryEquipement)
    {
        List<Soldier> authorisedSolidersList = new List<Soldier>();

        foreach (Soldier soldier in SoldiersList)
        {
            if ((_militaryEquipement.getTypeCompatibleHero() == "Voleur") && (soldier is Bandit) && (soldier.GetObject()!=_militaryEquipement))
            {
                authorisedSolidersList.Add(soldier);
            }
            else if ((_militaryEquipement.getTypeCompatibleHero() == "Géant") && (soldier is Giant)&& (soldier.GetObject()!=_militaryEquipement))
            {
                authorisedSolidersList.Add(soldier);
            }
            else if ((_militaryEquipement.getTypeCompatibleHero() == "Epéiste") && (soldier is Swordsman)&& (soldier.GetObject()!=_militaryEquipement))
            {
                authorisedSolidersList.Add(soldier);
            }
            else if ((_militaryEquipement.getTypeCompatibleHero() == "Archer") && (soldier is Archer)&& (soldier.GetObject()!=_militaryEquipement))
            {
                authorisedSolidersList.Add(soldier);
            }
            else if ((_militaryEquipement.getTypeCompatibleHero() == "Paladin") && (soldier is Paladin)&& (soldier.GetObject()!=_militaryEquipement))
            {
                authorisedSolidersList.Add(soldier);
            }
        }
        return authorisedSolidersList;
    }
    public List<Soldier> WhoCanReceivePotion()
    {
        List<Soldier> authorisedSolidersList = new List<Soldier>();

        foreach (Soldier soldier in SoldiersList)

        {
            if (soldier.GetMaxPv() > soldier.GetActualPV())
            {
                authorisedSolidersList.Add(soldier);
            }
        }
        return authorisedSolidersList;
    }
    public List<Soldier> WhoCanReceiveRecovery()
    {
        List<Soldier> authorisedSolidersList = new List<Soldier>();

        foreach (Soldier soldier in SoldiersList)

        {
            if (soldier.GetState() == "Blessé")
            {
                authorisedSolidersList.Add(soldier);
            }
        }
        return authorisedSolidersList;
    }
    public List<Soldier> WhoCanReceiveObject(Objects _object)
    {
        if (_object is MilitaryEquipement equipement)
        {
            return WhoCanReceiveThisMilitaryEquipement(equipement);
        }
        else if (_object is Potion)
        {
            return WhoCanReceivePotion();
        }
        else if (_object is Recovery)
        {
            return WhoCanReceiveRecovery();
        }
        return new List<Soldier>();
    }
    public void AtualiseAmelioration(Soldier soldier)
    {
        if (soldier is Bandit)
        {
            soldier.UpgradeSoldier(1.07, 1.1, 1.2, 1.05, 1.05, 1.15);
        }
        else if (soldier is Giant)
        {
            soldier.UpgradeSoldier(1.15, 1.2, 1.05, 1.15, 1.2, 1.15);
        }
        else if (soldier is Swordsman)
        {
            soldier.UpgradeSoldier(1.1, 1.15, 1.1, 1.1, 1.15, 1.15);
        }
        else if (soldier is Archer)
        {
            soldier.UpgradeSoldier(1.05, 1.1, 1.15, 1.05, 1.1, 1.1);
        }
        else if (soldier is Paladin)
        {
            soldier.UpgradeSoldier(1.15, 1.5, 1.08, 1.2, 1.25, 1.2);
        }
    }
    public void BuyBreads(int _numberofbreads)
    {
        numberOfBreadsComingTomorrow += _numberofbreads;
        Money -= 3 * _numberofbreads;
    }
    public void BuyMeats(int _numberofmeats)
    {
        numberOfMeatsComingTomorrow += _numberofmeats;
        Money -= 3 * _numberofmeats;
    }
    public void AddUndesirableEvent(UndesirableEvents _event)
    {
    undesirableEventsList.Add(_event);
    }

}

public interface IMakingSuper
{
    void MakeSuper();
}
    public class Soldier
    {
        protected String name;
        protected int exp;
        protected int level;
        protected int fatigue;
        protected Objects objectPossessed;
        protected String state;
        protected String description;
        protected int damages;
        protected int discretionPoints;
        protected int actualPv;
        protected int maxPv;
        protected int numberOfBreads;
        protected int numberOfMeats;
        protected int salaryADay;
        protected int buyingPrice;
        protected String imageName;
        public Soldier(String _name, String _description, int _damage, int _discretionPoints, int Pv, int _numberOfBreads, int _numberOfMeats, int _salaryADay, int _buyingPrince, String _imageName)
        {
            name = _name;
            exp = 0;
            level = 1;
            fatigue = 0;
            objectPossessed = new Objects();
            state = "Libre";
            description = _description;
            damages = _damage;
            discretionPoints = _discretionPoints;
            actualPv = Pv;
            maxPv = Pv;
            numberOfBreads = _numberOfBreads;
            numberOfMeats = _numberOfMeats;
            salaryADay = _salaryADay;
            buyingPrice = _buyingPrince;
            imageName = _imageName;
        }
        public Soldier()
        {
            name = "Inconnu";
            exp = 0;
            level = 1;
            fatigue = 0;
            objectPossessed = new Objects();
            state = "Libre";
            description = "Inconnue";
            damages = 0;
            discretionPoints = 0;
            maxPv = 0;
            actualPv = 0;
            numberOfBreads = 0;
            numberOfMeats = 0;
            salaryADay = 0;
            buyingPrice = 0;
            imageName = "Inconnue";

        }
        public void UpgradeSoldier(double damagesMultiplier, double pvMultiplier, double discretionMultiplier, double breadsMultiplier, double meatsMultiplier, double salaryMultiplier)
        {
            int seuil = (int)(3 * Math.Pow(10, level));

            if (exp >= seuil)
            {
                exp -= seuil;
                level++;

                damages = (int)Math.Floor(damages * damagesMultiplier);
                maxPv = (int)Math.Floor(maxPv * pvMultiplier);
                actualPv = (int)Math.Floor(actualPv * pvMultiplier);
                discretionPoints = (int)Math.Floor(discretionPoints * discretionMultiplier);
                numberOfBreads = (int)Math.Floor(numberOfBreads * breadsMultiplier);
                numberOfMeats = (int)Math.Floor(numberOfMeats * meatsMultiplier);
                salaryADay = (int)Math.Floor(salaryADay * salaryMultiplier);
            }

        }

        public void SuccedMission(Mission mission)
        {
            exp += 30 * (mission.GetDifficulty() + mission.getNumberOfDaysTotal());
        }

        
        public void SeBlesser()
        {
            state = "Blessé";
        }

        public void BeingFree()
        {
            state = "Libre";
        }

        public void RemoveInjury()
        {
            state = "Libre";
        }
        public void ToDie()
        {
            state = "Mort";
        }
        public void Sleep()
        {
            state = "Au repos";
        }

        public void WakingUp()
        {
            fatigue = (int)(fatigue* 0.6);
            state = "Libre";
        }
        public void GoToMission()
        {
            state = "En mission";
        }
        public String GetName()
        {
            return name;
        }
        public int GetExp()
        {
            return exp;
        }
        public int GetLevel()
        {
            return level;
        }
        public double GetFatigue()
        {
            return fatigue;
        }
        public Objects GetObject()
        {
            return objectPossessed;
        }
        public String GetState()
        {
            return state;
        }
        public String GetDescription()
        {
            return description;
        }
        public int GetDamages()
        {
            return damages;
        }
        public int GetDiscretionPoints()
        {
            return discretionPoints;
        }
        public int GetMaxPv()
        {
            return maxPv;
        }
        public int GetActualPV()
        {
            return actualPv;
        }
        public int GetBreadsADay()
        {
            return numberOfBreads;
        }
        public int GetMeatsADay()
        {
            return numberOfMeats;
        }
        public int GetSalaryADay()
        {
            return salaryADay;
        }

        public String GetImageName()
        {
            return imageName;
        }
        public int GetBuyingPrice()
        {
            return buyingPrice;
        }
        public void SetFatigue(int _fatigue)
        {
            fatigue = _fatigue;
        }

        public void SetName(String _name)
        {
            name = _name;
        }
        public void SetObject(Objects objects)
        {
            objectPossessed = objects;
        }
        public void SetActualsPv(int _actualPv)
        {
            actualPv = _actualPv;
        }
    }
    public class Bandit : Soldier, IMakingSuper
    {
        public Bandit(String name) : base(name, "Discret, Le voleur effectuera vos mission scrètes sans se faire repérer", 20, 150, 50, 2, 2, 10, 80,"bandit.png") { }
        public void MakeSuper()
        {
            discretionPoints *= 2;
        }
    }
    public class Giant : Soldier, IMakingSuper
    {
        public Giant(String name) : base(name, "Lourd et puissant, il résistera aux pires coups", 100, 10, 250, 4, 5, 20,220, "giant.png") { }
        public void MakeSuper()
        {
            damages *= 2;
        }
    }
    public class Swordsman : Soldier, IMakingSuper
    {
        public Swordsman(String name) : base(name, "Un soldat loyal qui n'a que son épée et son royaume dans son coeur ", 80, 60, 150, 3, 5, 40,170, "swordsman.png") { }
        public void MakeSuper()
        {
            discretionPoints = (int)(discretionPoints * 1.3);
            damages = (int)(damages * 1.7);
        }
    }
    public class Archer : Soldier, IMakingSuper
    {
        public Archer(String name) : base(name, "Avec son arc et ses flèches, il tuera tout ce qu'il verra au loin", 40, 120, 50, 2, 1, 15,100, "archer.png") { }
        public void MakeSuper()
        {
            damages *= 2;
        }
    }
    public class Paladin : Soldier, IMakingSuper
    {

        public Paladin(String name) : base(name, "Il fera toujours de son mieux pour blesser l'ennemi", 180, 50, 200, 4, 6, 80, 250, "paladin.png") { }
        public void MakeSuper()
        {
            discretionPoints = (int)(discretionPoints * 1.5);
            damages = (int)(damages * 1.5);
        }
    }

    public class Objects
    {
        protected String name;
        protected String description;
        protected int buyingPrice;
        protected int sellingPrice;
        protected String pictureName;

        public Objects(String _name, String _description, int _buyingPrice, int _sellingPrice, String _pictureName )
        {
            name = _name;
            description = _description;
            buyingPrice = _buyingPrice;
            sellingPrice = _sellingPrice;
            pictureName = _pictureName;
        }
        public Objects()
        {
            name = "Aucun";
            description = "Aucune";
            buyingPrice = 0;
            sellingPrice = 0;
            pictureName = "Aucune";
        }
        public String GetName()
        {
            return name;
        }
        public String GetDescription()
        {
            return description;
        }
        public int GetBuyingPrice()
        {
            return buyingPrice;
        }

        public String GetImageName()
        {
            return pictureName;
        }
        public int GetSellingPrice()
        {
            return sellingPrice;
        }
    }
    public class Potion : Objects
    {
        private int pvHealed;

        public Potion(String name, String _description, int buyingPrice, int sellingPrice, int _pvHealed, String _pictureName) : base(name, _description, buyingPrice, sellingPrice, _pictureName)
        {
            pvHealed = _pvHealed;
        }
        public int getPvHealed()
        {
            return pvHealed;
        }
    }

    public class MilitaryEquipement : Objects
    {
        private String typeHeroCompatible;

        public MilitaryEquipement(String name, String _description, int buyingPrice, int sellingPrice, String _typeHeroCompatible, String _pictureName) : base(name, _description, buyingPrice, sellingPrice, _pictureName)
        {
            typeHeroCompatible = _typeHeroCompatible;
        }
        public String getTypeCompatibleHero()
        {
            return typeHeroCompatible;
        }
    }
    public class Recovery : Objects
    {
        public Recovery() : base("Sort de Guerison", "A le pouvoir de soigner toute blessure", 20, 15,"Recovery.png") { }
    }
    public class AmethystPerl : Objects
    {
        public AmethystPerl() : base("Perle d'amethyste", "Un objet rare venant de contrées lointaines", 100, 80,"AmethystPearl.png") { }
    }
    public class RedRubis : Objects
    {
        public RedRubis() : base("Rubis rouge", "Un joyau provenant d'une ancienne famille noble", 150, 140,"redRubis.png") { }
    }
    public class RoyalNeckLace : Objects
    {
        public RoyalNeckLace() : base("Collier royal", "Un objectPossessed très précieux tout droit venu de l'ancien Royaume", 500, 340,"RoyalNeckLace.png") { }
    }
    public class GoldBar : Objects
    {
        public GoldBar() : base("Lingot d'or", "Ce lingo provient surement d'une mine de l'autre coté de la rivière", 15, 10,"goldBar.jpg") { }
    }
    public class LittlePotion : Potion
    {
        public LittlePotion() : base("Petite Potion", "Permet de soigner 25 PV", 10, 8, 50,"littlepotion.png") { }
    }
    public class MediumPotion : Potion
    {
        public MediumPotion() : base("Moyenne Potion", "Permet de soigner 50 PV", 15, 13, 100,"mediumpotion.png") { }
    }
    public class BigPotion : Potion
    {
        public BigPotion() : base("Grande Potion", "Permet de soigner 100 PV", 20, 18, 175,"bigPotion.png") { }
    }
    public class LightCape : MilitaryEquipement
    {
        public LightCape() : base("Cape légère", "Peremttra à votre voleuse d'être encore plus furtive", 60, 45, "Voleur","lightcape.png") { }
    }
    public class MetalFists : MilitaryEquipement
    {
        public MetalFists() : base("Poings en Métal", "Peremttra à votre Géant de mettre des coups de points toujours plus redoutables", 100, 95, "Géant","metalFists.jpg") { }
    }
    public class SoulSword : MilitaryEquipement
    {
        public SoulSword() : base("Epee de l'âme", "Avec cela, votre épéiste pourra même trancher l'air", 100, 95, "Epéiste", "soulSword.png") { }
    }
    public class OverPowerdBow : MilitaryEquipement
    {
        public OverPowerdBow() : base("Arc Surpuissant", "Avec cela, votre archer pourra même transpercer un mur de pierre", 100, 95, "Archer","overpoweredBow.png") { }
    }
    public class LightArmor : MilitaryEquipement
    {
        public LightArmor() : base("Armure légère", "Votre paladin sera immédiatement plus agile et rapide, offrant un avantage en combat", 120, 105, "Paladin","LightArmor.png") { }
    }


    public class Mission
    {
        private static int missionCounter = 0;
        private int Id;
        private int difficulty;
        private int numberOfDaysLeft;
        private int numberOfDaysTotal;
        private String state;
        private Soldier soldierOnIt;
        private Objects objectToReceive;

        public Mission(int _difficulty, int _numberOfDays, Objects _objectToReceive)
        {
            missionCounter++;
            Id = missionCounter;
            difficulty = _difficulty;
            numberOfDaysLeft = _numberOfDays;
            numberOfDaysTotal = _numberOfDays;
            state = "Libre";
            soldierOnIt = new Soldier();
            objectToReceive = _objectToReceive;
        }

        public int GetId()
        {
            return Id;
        }
        
        public int SurvivingChances(Soldier soldier)
        {
            return ((int)(100*(soldier.GetActualPV() + soldier.GetDamages() + soldier.GetDiscretionPoints()-soldier.GetFatigue())) /
                    (difficulty * 340));
        }
        public void EndureMission()
        {
            Soldier soldier = soldierOnIt;
            int survivingChances=SurvivingChances(soldier);
            Random rnd = new Random();
            double randomValue = rnd.NextDouble();
            Random rnd2 = new Random();
            int randomValue2 = rnd2.Next(0, 101);
            soldier.SetFatigue((int)(soldier.GetFatigue()+(100-soldier.GetFatigue()) * randomValue2/100));
            //soldier.SetFatigue((int)(soldier.GetFatigue()+(10000*(100-soldier.GetFatigue()) / Math.Pow(survivingChances * randomValue2, 1.5))));
            if (soldier.GetFatigue() > 100)
            {
                soldier.SetFatigue(100);
            }
            if (soldier.GetFatigue()/100>=randomValue2)
            {
                soldier.SeBlesser();
            }
            soldier.SetActualsPv((int)(soldier.GetActualPV()*(1- (randomValue / (survivingChances/100.0)))));
            if (soldier.GetActualPV()<=0)
            {
                soldier.SetActualsPv(0);
                soldier.ToDie();
                state=("Ratée");
            }
            else
            {
                state=("Réussie");
            } 
        }

        public int getNumberOfDaysLeft()
        {
            return numberOfDaysLeft;
        }

        public int getNumberOfDaysTotal()
        {
            return numberOfDaysTotal;
        }
        public void ActualizeNumberOfDays()
        {
            numberOfDaysLeft--;
        }
        
        public int GetDifficulty()
        {
            return difficulty;
        }
        public String GetState()
        {
            return state;
        }
        public Soldier GetSoldierOnIt()
        {
            return soldierOnIt;
        }

        public int GetEarnings()
        {
            return (int)(50*(Math.Sqrt(difficulty * numberOfDaysTotal)));
        }

        public Objects GetObjectToReceive()
        {
            return objectToReceive;
        }
        public void SetSoldierOn(Soldier _soldierOnIt)
        {
            state = "En cours";
            soldierOnIt = _soldierOnIt;
            soldierOnIt.GoToMission();
        }

    }
    public interface IIntesify
    {
        void intensify();
    }
    public class UndesirableEvents
    {
        protected String name;
        protected String description;
        protected double HappeningProba;

        public UndesirableEvents(String _name, String _description, double _happeningProba)
        {
            name = _name;
            description = _description;
            HappeningProba = _happeningProba;
        }

        public void IntensifyProba()
        {
            HappeningProba *= 1.1;
        }
        public double GetHappeningProba()
        {
            return HappeningProba;
        }
    }
    public class Storm : UndesirableEvents, IIntesify
    {
        private int reparationCosts;


        public Storm() : base("Tempête", "Un tempête s'est abatue sur votre royaume", 0.2)
        {
            reparationCosts = 15;
        }
        public void intensify()
        {
            reparationCosts = (int)Math.Floor(reparationCosts * 1.3);
        }
        public int GetReparationCosts()
        {
            return reparationCosts;
        }
    }

    public class Fire : UndesirableEvents, IIntesify
    {
        private int reparationCosts;
        private double mortality;


        public Fire() : base("Feu", "Un incendie a dévasté votre royaume", 0.1)
        {
            reparationCosts = 20;
            mortality = 0.1;
        }
        public void intensify()
        {
            reparationCosts = (int)Math.Floor(reparationCosts * 1.3);
            mortality *= reparationCosts * 1.1;
        }
        public int GetReparationCosts()
        {
            return reparationCosts;
        }
        public double GetMortality()
        {
            return mortality;
        }

    }
    public class Thief : UndesirableEvents, IIntesify
    {
        private double probaPerObject;


        public Thief() : base("Vol", "Un intrus a infiltré votre royaume pour voler des objets", 0.15)
        {
            probaPerObject = 0.1;
        }
        public void intensify()
        {
            probaPerObject *= probaPerObject * 1.1;
        }
        public double getProbaParObjet()
        {
            return probaPerObject;
        }

    }

