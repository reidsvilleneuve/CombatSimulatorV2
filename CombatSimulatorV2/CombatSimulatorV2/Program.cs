using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CombatSimulatorV2
{
    class Program
    {
        static void Main(string[] args)
        {
            Game game = new Game();
            game.PlayGame();
        }
    }

    public enum ElementalType { Standard, Water, Fire, Plant } //Rock-paper-scissors effect based on elemental type.
    public enum AttackRange { melee, ranged } //Ranged cant hit melee, and visa versa.
    public enum WeaponType { fist, claw, sword, axe, club, spear, dagger, bow, crossbow, sling } //Used to set damage verbs.

    /// <summary>
    /// Values and functions requried for a Player in CombatSimulatorV2.
    /// </summary>
    public class Player
    {
        private int _health;
        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        private int _mana;
        public int Mana
        {
            get { return _mana; }
            set { _mana = value; }
        }
        
        private ElementalType _element;
        public ElementalType Element
        {
            get { return _element; }
            set { _element = value; }
        }

        private Weapon _weapon;
        public Weapon Weapon
        {
            get { return _weapon; }
            set { _weapon = value; }
        }

        public bool IsAlive
        {
            get
            {
                if (this.Health > 0) return true;
                else return false;
            }
        }

        private int _kills;
        public int Kills
        {
            get { return _kills; }
            set { _kills = value; }
        }

        /// <summary>
        /// Generates player. Some random properties.
        /// </summary>
        public Player()
        {
            Random rng = new Random();

            //Generate health
            this.Health = rng.Next(500, 601);

            //Generate mana
            this.Mana = rng.Next(100, 201);

            //Genearte elemental type
            this.Element = (ElementalType)rng.Next(0, Enum.GetNames(typeof(ElementalType)).Length);

            //Generate weapon
            this.Weapon = new Weapon();

            this.Kills = 0; 
        }

        private enum Actions { attack = 1, heal, equip, summon, animate }

        private Actions ChooseAction()
        {
            int input = 0;
            do
            {
                Console.Clear();
                Console.Write("What would you like to do?\n1. Attack\n2. Heal (20 mana)\n3. Equip a summoned weapon" +
                    "\n4. Summon a new weapon (30 mana)\n5. Animate a weapon (40 mana)\n\n(1, 2, 3, 4, 5?) ");

                int.TryParse(Console.ReadKey().KeyChar.ToString(), out input); //Convert user input to int if possible.
            } while (!(input > 0 && input < 6)); //Loop until input is 1-5.

            Console.WriteLine("\n\n");
            return (Actions)input;
        }

        public void Attack(List<Enemy>enemies, List<Weapon>unequippedWeapons, List<Weapon>animatedWeapons)
        {
            int input = 0;
            Random rng = new Random();

            switch (ChooseAction()) //Present player with a choice of things to do.
            {
                case Actions.attack:
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("Attack which enemy?\n" + string.Join("\n", enemies
                         .Select((x, i) => (i + 1) + ". " + x.Name)) //Each enemy in enemies list, numbered.
                         + "\n\n(" + string.Join(", ", enemies.Select((y, i) => (i + 1))) + "?)");//Number choice prompt

                        int.TryParse(Console.ReadKey().KeyChar.ToString(), out input);
                    } while (!(input > 0 && input <= enemies.Count)); //Loop until valid input.

                    enemies[input - 1].Health -= this.Weapon.Attack(enemies[input - 1]);
                    if(!enemies[input - 1].IsAlive)
                    {
                        enemies.RemoveAt(input - 1);
                        this.Kills = this.Kills + 1;
                    }
                    break;

                case Actions.heal:
                    Console.Clear();

                    int healAmount = rng.Next(30, 51);
                    this.Health += healAmount;
                    this.Mana -= 20;

                    Console.Write("You healed yourself for " + healAmount + "! \n\n(Press any key to continue.)");
                    break;

                case Actions.equip:
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("Equip which weapon?\n" + string.Join("\n", unequippedWeapons
                         .Select((x, i) => (i + 1) + ". " + x.Name)) //Each enemy in enemies list, numbered.
                         + "\n\n(" + string.Join(", ", unequippedWeapons.Select((y, i) => (i + 1))) + "?)");//Number choice prompt

                        int.TryParse(Console.ReadKey().KeyChar.ToString(), out input);
                    } while (!(input > 0 && input <= unequippedWeapons.Count)); //Loop until valid input.

                    Console.Clear();

                    unequippedWeapons.Add(this.Weapon);
                    unequippedWeapons.RemoveAt(input - 1);

                    Console.Write("You unequipped your " + unequippedWeapons.Last().Name + " and grabbed the "
                        + this.Weapon.Name + "!\n\n(Press any key to continue.) ");
                    break;

                case Actions.summon:
                    unequippedWeapons.Add(this.Weapon);
                    this.Weapon = new Weapon();
                    this.Mana -= 30;

                    Console.Clear();

                    if(unequippedWeapons.Count > 10)
                    {
                        Console.WriteLine("Your " + unequippedWeapons.First() + " fizzles out of existence! You can't seem to " +
                           "sustain 11 weapons at once!\n");

                        unequippedWeapons.RemoveAt(0);
                    }

                    Console.Write("You dropped your " + unequippedWeapons.Last() + " and summoned a "
                        + this.Weapon.Name + "!\n\n(Press any key to continue.) ");

                    break;

                case Actions.animate:
                    int randomWeapon = rng.Next(0, unequippedWeapons.Count - 1);
                    animatedWeapons.Add(unequippedWeapons[randomWeapon]);
                    unequippedWeapons.RemoveAt(randomWeapon);
                    
                    Console.Clear();

                    Console.Write("Your " + animatedWeapons.Last() + " jumps to life and attacks the enemy!"
                        +"\n\n(Press any key to continue.)");
                    break;
                default:
                    break;
            }
        }

    }

    /// <summary>
    /// Values and functions required for an enemy object in CombatSimulatorV2.
    /// </summary>
    public class Enemy
    {
        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private ElementalType _element;
        public ElementalType Element
        {
            get { return _element; }
            set { _element = value; }
        }

        private int _health;
        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        private Weapon _enemyWeapon;
        public Weapon EnemyWeapon
        {
            get { return _enemyWeapon; }
            set { _enemyWeapon = value; }
        }

        public bool IsAlive
        {
            get
            {
                if (this.Health > 0) return true;
                else return false;
            }
        }

        /// <summary>
        /// Creates an enemy with the specified stats.
        /// </summary>
        /// <param name="name">The name of the enemy. I.e. "Restless Skeleton."</param>
        /// <param name="health">The health of the enemy.</param>
        /// <param name="element">The elemental type of the enemy.</param>
        /// <param name="weapon">The Weapon that the enemy carries.</param>
        public Enemy(string name, int health, ElementalType element, Weapon weapon)
        {
            this.Name = name;
            this.Health = health;
            this.Element = element;
            this.EnemyWeapon = weapon;
        }
          
        /// <summary>
        /// Copies the specified Enemy object.
        /// </summary>
        /// <param name="enemyToCopy">The Enemy object to copy.</param>
        public Enemy(Enemy enemyToCopy)
        {
            this.Name = enemyToCopy.Name;
            this.Health = enemyToCopy.Health;
            this.Element = enemyToCopy.Element;
            this.EnemyWeapon = enemyToCopy.EnemyWeapon;
        }

        enum enemyType { beast, humanoid }

        /// <summary>
        /// Generates a randomized enemy.
        /// </summary>
        public Enemy()
        {
            Random rng = new Random();

            //Generate health
            this.Health = rng.Next(15, 50);

            //Generate elemental type. Random number between 0 and the length of the enum.
            this.Element = (ElementalType)rng.Next(0, Enum.GetNames(typeof(ElementalType)).Length);

            //Beast or humanoid?
            enemyType randomType = (enemyType)rng.Next(0,2);
            string[] randomNouns;
            string noun = string.Empty;

            switch (randomType)
            {
                case enemyType.beast:
                    //Generate noun.
                    randomNouns = ("cheetah wolf bird cougar hedgehog aardvark skunk muskrat rat bear " +
                        "ermine ocelot jaguar beaver leopard").Split(' ');
                    noun = randomNouns[rng.Next(0, randomNouns.Length)];

                    //Generate weapon.
                    this.EnemyWeapon = new Weapon(WeaponType.claw);
                    break;

                case enemyType.humanoid:
                    //Generate noun.
                    randomNouns = ("knight demon gorilla fighter ninja horror skeleton zombie vampire shadowknight" +
                    "shaman goblin orc monster").Split(' ');
                    noun = randomNouns[rng.Next(0, randomNouns.Length)];

                    //Generate weapon.
                    this.EnemyWeapon = new Weapon();
                    break;
            }

            string[] randomAdjs = ("Dark Evil Menacing Angry Hellish Furious Glowing Ghostly Ghastly Accursed Fallen " +
                "Giant Huge Corrupted Enslaved Terrifying Barbed Spiked Infested Strange Odd Gross").Split(' ');
            string adj = randomAdjs[rng.Next(0, randomAdjs.Length)];

            //Finish name.
            this.Name = adj + " " + this.Element.ToString().ToLower() + " " + noun;
        }

        /// <summary>
        /// Attack specified player.
        /// </summary>
        /// <param name="player">Player object to attack.</param>
        public void Attack(Player player)
        {
            player.Health -= this.EnemyWeapon.Attack(this, player);
        }

        /// <summary>
        /// Attack specified animated weapon.
        /// </summary>
        /// <param name="weapon">Weapon object to attack.</param>
        public void Attack(List<Weapon> weapons)
        {
            Random rng = new Random();
            int randomTarget = rng.Next(0, weapons.Count);
            weapons[randomTarget].Health -= this.EnemyWeapon.Attack(this, weapons[randomTarget]);
        }
    }

    /// <summary>
    /// Values requried for a weapon in CombatSimulatorV2.
    /// </summary>
    public class Weapon
    {
        //Properties

        private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        private int[] _damageRange = new int[2];
        public int[] DamageRange 
        {
            get { return _damageRange; }
            set { _damageRange = value; }
        }

        private ElementalType _element;
        public ElementalType Element
        {
            get { return _element; }
            set { _element = value; }
        }

        private WeaponType _type;
        public WeaponType Type
        {
            get { return _type; }
            set 
            {
                _type = value;
                switch (Type)
                {
                    case WeaponType.fist:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "socks", "smacks", "slaps", "biffs", "jabs" };
                        break;
                    case WeaponType.claw:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "scratches", "rips", "scrapes", "scores", "gashes" };
                        break;
                    case WeaponType.sword:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "slices", "cuts", "slashes", "shears", "severs" };
                        break;
                    case WeaponType.axe:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "cleaves", "hacks", "minces", "lops", "truncates" };
                        break;
                    case WeaponType.club:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "bats", "belts", "clouts", "smashes", "wallops" };
                        break;
                    case WeaponType.spear:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "pierces", "perforates", "spikes", "stabs", "punctures" };
                        break;
                    case WeaponType.dagger:
                        _range = AttackRange.melee;
                        _damageVerbs = new List<string>() { "pierces", "perforates", "spikes", "stabs", "punctures" };
                        break;
                    case WeaponType.bow:
                        _range = AttackRange.ranged;
                        _damageVerbs = new List<string>() { "bullseyes", "targets", "shoots", "points", "punctures" };
                        break;
                    case WeaponType.crossbow:
                        _range = AttackRange.ranged;
                        _damageVerbs = new List<string>() { "bullseyes", "targets", "shoots", "points", "punctures" };
                        break;
                    case WeaponType.sling:
                        _range = AttackRange.ranged;
                        _damageVerbs = new List<string>() { "bullseyes", "belts", "clouts", "smacks", "wallops" };
                        break;
                    default:

                        break;
                }
            }
        }

        private int _health;
        public int Health
        {
            get { return _health; }
            set { _health = value; }
        }

        private int _missChance;
        public int MissChance 
        {
            get { return _missChance; }
            set { _missChance = value; }
        }

        private List<string> _damageVerbs = new List<string>();
        public List<string> DamageVerbs 
        {
            get { return _damageVerbs; }
        }

        private AttackRange _range;
        public AttackRange Range
        {
            get { return _range; }
        }

        //Constructors. Can pass in all values manually, another weapon to copy,
        //a WeaponType enmum for randomized values with specified type, or nothing for all randomized vales.

        /// <summary>
        /// A weapon with specified stats.
        /// </summary>
        /// <param name="name">The name of the weapon. "Iron Sword"</param>
        /// <param name="description">A quick description. "This sword is somewhat beat up."</param>
        /// <param name="damageRange">A 2-element array representing the damage range. [min, max]</param>
        /// <param name="elementalType">The elemental type of the weapon.</param>
        /// <param name="weaponType">The type of weapon. Range is automatically set based off of this value.</param>
        /// <param name="health">How much damage the weapon can take.</param>
        /// <param name="missChance">Chance to miss with this weapon.</param>
        public Weapon(string name, string description, int[] damageRange, ElementalType elementalType, 
            WeaponType weaponType, int health, int missChance)
        {
            this.Name = name;
            this.Description = description;
            this.DamageRange = damageRange;
            this.Element = elementalType;
            this.Type = weaponType;
            this.Health = health;
            this.MissChance = missChance;
        }

        public Weapon(Weapon weaponToCopy)
        {
            this.Name = weaponToCopy.Name;
            this.Description = weaponToCopy.Description;
            this.DamageRange = weaponToCopy.DamageRange;
            this.Element = weaponToCopy.Element;
            this.Type = weaponToCopy.Type;
            this.Health = weaponToCopy.Health;
            this.MissChance = weaponToCopy.MissChance;
        }

        public Weapon(WeaponType weaponType)
        {
            Random rng = new Random();
            
            //Generate weapon health
            this.Health = rng.Next(10, 101);

            //Generate damage range
            this.DamageRange = new int[2] { rng.Next(5, 20), rng.Next(20, 36) };

            //Set weapon type
            this.Type = weaponType;

            //Generate element type. Random number between 0 and the length of the enum.
            this.Element = (ElementalType)rng.Next(0, Enum.GetNames(typeof(ElementalType)).Length);

            //Generate miss chance.
            this.MissChance = rng.Next(5, 20);

            //Generate weapon name.
            string[] adjs = ("Enthusiastic Therapeutic Pointless Jealous Aromatic Wacky Unknown Penitent Adjoining " +
                "Concerned Imperfect Cloistered Likeable Inconclusive Savory Splendid Broad Nasty Yellow Wise Sad Mindless " +
                "Torpid Ragged Combative Breakable Dark Auspicious Outstanding Outrageous Numerous Efficient Complete " +
                "Soggy Homeless White Windy Terrible Spurious Dusty Hurt Mere Domineering Unequal Obedient Ultra Gamy").Split(' ');

            string[] nouns = ("Order Pages Building Smiles Parting Pain Competition Weather Protest Education Sense " +
                "Person Apparatus Levels Mind Cooks Twist Impulse Night Trouble Doubt Experts Profit Opinions Hours Carvings " +
                "Destruction Stretches Smoke Soup Existence Discussions Guides Limits Tricks Wine Jelly Quality Polish Drinks " +
                "Worthiness").Split(' ');

            string randomAdj = adjs[rng.Next(0, adjs.Length)];
            string randomNoun = nouns[rng.Next(0, nouns.Length)];

            this.Name = Element + " " + Type + " of " + randomAdj + " " + randomNoun;

            //Generate weapon description.

            string[] glowSynons = ("glows blooms glitters glimmers gleams glares").Split(' ');
            string randomSynon = glowSynons[rng.Next(0, glowSynons.Length)];

            this.Description = Name + ": This ";
            if (this.Element == ElementalType.Standard)
                this.Description += "is a standard " + Type + ".";
            else
                this.Description += Type + " " + randomSynon + " of " + Element + ".";

        }

        public Weapon()
        {
            Random rng = new Random();
    
            //Generate weapon health
            this.Health = rng.Next(10, 101);

            //Generate damage range
            this.DamageRange = new int[2] { rng.Next(5, 20), rng.Next(20, 36) };

            //Generate weapon type. Random number between 0 and the length of the enum.
            this.Type = (WeaponType)rng.Next(0, Enum.GetNames(typeof(WeaponType)).Length);

            //Generate element type. Random number between 0 and the length of the enum.
            this.Element = (ElementalType)rng.Next(0, Enum.GetNames(typeof(ElementalType)).Length);

            //Generate miss chance.
            this.MissChance = rng.Next(5, 20);

            //Generate weapon name.
            string[] adjs = ("Enthusiastic Therapeutic Pointless Jealous Aromatic Wacky Unknown Penitent Adjoining " +
                "Concerned Imperfect Cloistered Likeable Inconclusive Savory Splendid Broad Nasty Yellow Wise Sad Mindless " +
                "Torpid Ragged Combative Breakable Dark Auspicious Outstanding Outrageous Numerous Efficient Complete " +
                "Soggy Homeless White Windy Terrible Spurious Dusty Hurt Mere Domineering Unequal Obedient Ultra Gamy").Split(' ');

            string[] nouns = ("Order Pages Building Smiles Parting Pain Competition Weather Protest Education Sense " + 
                "Person Apparatus Levels Mind Cooks Twist Impulse Night Trouble Doubt Experts Profit Opinions Hours Carvings " +
                "Destruction Stretches Smoke Soup Existence Discussions Guides Limits Tricks Wine Jelly Quality Polish Drinks " +
                "Worthiness").Split(' ');
 
            string randomAdj = adjs[rng.Next(0, adjs.Length)];
            string randomNoun = nouns[rng.Next(0, nouns.Length)];

            this.Name = Element + " " + Type + " of " + randomAdj + " " + randomNoun;

            //Generate weapon description.

            string[] glowSynons = ("glows blooms glitters glimmers gleams glares").Split(' ');
            string randomSynon = glowSynons[rng.Next(0, glowSynons.Length)];

            this.Description = Name + ": This ";
            if (this.Element == ElementalType.Standard)
                this.Description += "is a standard " + Type + ".";  
            else
                this.Description += Type + " " + randomSynon + " of " + Element + ".";


        }

        public int Attack(Enemy target)
        {
            Random rng = new Random();

            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);
            
            string verb = this._damageVerbs[rng.Next(0, this._damageVerbs.Count)]; //Randomly select a damage verb
            string damageMessage = string.Empty;

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > this.MissChance)
                damage = rng.Next(this.DamageRange[0], this.DamageRange[1] + 1); //.Next's upper bound is exclusive, so +1 to correct that.

            // ---- Elemental calculations
            
            switch (this.Element)
            {
                case ElementalType.Water:
                    if (target.Element == ElementalType.Fire)
                        damage += damage;
                    break;

                case ElementalType.Fire:
                    if (target.Element == ElementalType.Plant)
                            damage += damage;
                    break;

                case ElementalType.Plant:
                    if (target.Element == ElementalType.Water)
                        damage += damage;
                    break;

                default:
                    break;
            }

            // ---- Combat text.

            Console.Clear();

            damageMessage = "Your attack "; // Always displayed.

            if (damage == 0)
                damageMessage += "tries to "; //Displayed on a miss


            damageMessage += verb + " the " + target.Name + " with your " + this.Name;

            if (damage == 0) // Miss and hit messages.
                damageMessage += ", but misses!";
            else
                damageMessage += "dealing " + damage + " damage!";

            Console.Write(damageMessage + "\n\n(Press any key to continue) ");
            Console.ReadKey();

            return damage;
        }

        public int Attack(Enemy attacker, Player target)
        {
            Random rng = new Random();

            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);

            string verb = this._damageVerbs[rng.Next(0, this._damageVerbs.Count)]; //Randomly select a damage verb
            string damageMessage = string.Empty;

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > this.MissChance)
                damage = rng.Next(this.DamageRange[0], this.DamageRange[1] + 1); //.Next's upper bound is exclusive, so +1 to correct that.

            // ---- Elemental calculations

            switch (this.Element)
            {
                case ElementalType.Water:
                    if (target.Element == ElementalType.Fire)
                        damage += damage;
                    break;

                case ElementalType.Fire:
                    if (target.Element == ElementalType.Plant)
                        damage += damage;
                    break;

                case ElementalType.Plant:
                    if (target.Element == ElementalType.Water)
                        damage += damage;
                    break;

                default:
                    break;
            }


            // ---- Combat text.

            Console.Clear();
            damageMessage = "The " + attacker.Name; // Always displayed.

            if (damage == 0)
                damageMessage += " tries to "; //Displayed on a miss


            damageMessage += verb + " you with its " + this.Name;

            if (damage == 0) // Miss and hit messages.
                damageMessage += ", but misses!";
            else
                damageMessage += "dealing " + damage + " damage!";

            Console.Write(damageMessage + "\n\n(Press any key to continue) ");
            Console.ReadKey();

            return damage;

        }

        public int Attack(Enemy attacker, Weapon target)
        {
            Random rng = new Random();

            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);

            string verb = this._damageVerbs[rng.Next(0, this._damageVerbs.Count)]; //Randomly select a damage verb
            string damageMessage = string.Empty;

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > this.MissChance)
                damage = rng.Next(this.DamageRange[0], this.DamageRange[1] + 1); //.Next's upper bound is exclusive, so +1 to correct that.

            // ---- Elemental calculations

            switch (this.Element)
            {
                case ElementalType.Water:
                    if (target.Element == ElementalType.Fire)
                        damage += damage;
                    break;

                case ElementalType.Fire:
                    if (target.Element == ElementalType.Plant)
                        damage += damage;
                    break;

                case ElementalType.Plant:
                    if (target.Element == ElementalType.Water)
                        damage += damage;
                    break;

                default:
                    break;
            }


            // ---- Combat text.

            Console.Clear();

            damageMessage = "The " + attacker.Name; // Always displayed.

            if (damage == 0)
                damageMessage += " tries to "; //Displayed on a miss


            damageMessage += verb + " your animated " + target.Name + " with its " + this.Name;

            if (damage == 0) // Miss and hit messages.
                damageMessage += ", but misses!";
            else
                damageMessage += "dealing " + damage + " damage!";

            Console.Write(damageMessage + "\n\n(Press any key to continue) ");
            Console.ReadKey();

            return damage;

        }

        public int calcDamageOnly(ElementalType target)
        {
            Random rng = new Random();

            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > this.MissChance)
                damage = rng.Next(this.DamageRange[0], this.DamageRange[1] + 1); //.Next's upper bound is exclusive, so +1 to correct that.

            // ---- Elemental calculations

            switch (this.Element)
            {
                case ElementalType.Water:
                    if (target == ElementalType.Fire)
                        damage += damage;
                    break;

                case ElementalType.Fire:
                    if (target == ElementalType.Plant)
                        damage += damage;
                    break;

                case ElementalType.Plant:
                    if (target  == ElementalType.Water)
                        damage += damage;
                    break;

                default:
                    break;
            }
            return damage;
        }

        public int AttackAsAnimated(Enemy target)
        {
            Random rng = new Random();

            int damage = 0; // Will remain at 0 if player misses.
            int missValue = rng.Next(1, 101);

            string verb = this._damageVerbs[rng.Next(0, this._damageVerbs.Count)]; //Randomly select a damage verb
            string damageMessage = string.Empty;

            // Miss chance. The higher the missChance, the less likely missValue will be higher than missChance.
            if (missValue > this.MissChance)
                damage = rng.Next(this.DamageRange[0], this.DamageRange[1] + 1); //.Next's upper bound is exclusive, so +1 to correct that.

            // ---- Elemental calculations

            switch (this.Element)
            {
                case ElementalType.Water:
                    if (target.Element == ElementalType.Fire)
                        damage += damage;
                    break;

                case ElementalType.Fire:
                    if (target.Element == ElementalType.Plant)
                        damage += damage;
                    break;

                case ElementalType.Plant:
                    if (target.Element == ElementalType.Water)
                        damage += damage;
                    break;

                default:
                    break;
            }

            // ---- Combat text.

            Console.Clear();

            damageMessage = "Your animated " + this.Name; // Always displayed.

            if (damage == 0)
                damageMessage += " tries to "; //Displayed on a miss


            damageMessage += verb + " the " + target.Name;

            if (damage == 0) // Miss and hit messages.
                damageMessage += ", but misses!";
            else
                damageMessage += " dealing " + damage + " damage!";

            Console.Write(damageMessage + "\n\n(Press any key to continue) ");
            Console.ReadKey();

            return damage;
        }

        public void AnimatedAttack(List<Enemy> enemies, Player player)
        {
            Random rng = new Random();
            int randomTarget = rng.Next(0, enemies.Count);

            enemies[randomTarget].Health -= this.AttackAsAnimated(enemies[randomTarget]);

            if (!enemies[randomTarget].IsAlive)
            {
                enemies.RemoveAt(randomTarget);
                player.Kills = player.Kills + 1;
            }

        }




    }

    /// <summary>
    /// The main engine behind the CombatSimulatorV2 game.
    /// </summary>
    public class Game
    {
        private Player _thePlayer = new Player();
        public Player ThePlayer
        {
            get { return _thePlayer; }
            set { _thePlayer = value; }
        }

        private List<Enemy> _theEnemies = new List<Enemy>();
        public List<Enemy> TheEnemies
        {
            get { return _theEnemies; }
            set { _theEnemies = value; }
        }

        private List<Weapon> _unequippedWeapons = new List<Weapon>();
        public List<Weapon> UnequippedWeapons
        {
            get { return _unequippedWeapons; }
            set { _unequippedWeapons = value; }
        }

        private List<Weapon> _animatedWeapons = new List<Weapon>();
        public List<Weapon> AnimatedWeapons
        {
            get { return _animatedWeapons; }
            set { _animatedWeapons = value; }
        }

        /// <summary>
        /// Creates an instance of Game.
        /// </summary>
        public Game()
        {
            this.ThePlayer = new Player();
            this.TheEnemies = new List<Enemy> { new Enemy() };
            this.UnequippedWeapons = new List<Weapon>();
            this.AnimatedWeapons = new List<Weapon>();
        }

        public void DisplayCombatInfo()
        {
            //Player info
            Console.WriteLine("You are at " + ThePlayer.Health + " health.\n");

            //Animated weapons info
            if (this.AnimatedWeapons.Count == 0)
                Console.WriteLine("You don't have any summoned weapons yet!\n");
            else 
                Console.WriteLine("Your animated weapons: " + string.Join("\n", AnimatedWeapons.Select(x =>
                    x.Name + ": " + x.Health))+ "\n");

            //Enemy info
            if (this.TheEnemies.Count == 0)
                Console.WriteLine("There are no enemies yet!\n");
            else
                Console.WriteLine("Enemies: \n" + string.Join("\n", TheEnemies.Select(x =>
                    x.Name + ": " + x.Health)) + "\n");

            Console.Write("(Press any key to continue.) ");
            Console.ReadKey();
        }

        public void PlayGame()
        {
            Random rng = new Random();
            do
            {
                while(this.ThePlayer.IsAlive && this.ThePlayer.Kills < 101)
                {
                    this.TheEnemies.Add(new Enemy());
                    
                    DisplayCombatInfo();

                    this.ThePlayer.Attack(this.TheEnemies, this.UnequippedWeapons, this.AnimatedWeapons);
                    
                    if(AnimatedWeapons.Count > 0)
                        foreach(Weapon i in AnimatedWeapons)
                            i.AnimatedAttack(this.TheEnemies, this.ThePlayer);

                    if(TheEnemies.Count > 0)
                        foreach(Enemy i in TheEnemies)
                        {
                            if(rng.Next(1, 101) > 20 && AnimatedWeapons.Count > 0) //Attack player 20% of the time if there are weapons animated.
                                i.Attack(this.AnimatedWeapons);
                            else
                                i.Attack(ThePlayer);
                        }
                }

                Console.Clear();

                if(this.ThePlayer.IsAlive)
                    Console.WriteLine("You got 100 kills! You win!\n");
                else
                    Console.WriteLine("You died! You loose!");
                Console.Write("Play again? (Y to play again, or any other key to exit.");
            } while (char.ToLower(Console.ReadKey().KeyChar) == 'y');
        }
    }
}
