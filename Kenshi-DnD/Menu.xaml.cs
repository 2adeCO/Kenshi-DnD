using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        MainWindow mainWindow;
        Adventure myAdventure;
        Random rnd;
        ContentControl controller;
        Cursor[] cursors;
        public Menu(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd)
        {
            InitializeComponent();
            TitleText.Inlines.Clear();
            TitleText.Inlines.AddRange(mainWindow.DecorateText("@134@KENSHI_DND@\n@8@Por@ @7@Santiago Cabrero@"));

            this.cursors = cursors;
            this.controller = controller;
            this.mainWindow = mainWindow;
            this.rnd = rnd;
            Dice myDice = new Dice(6, 5, rnd);

            Limb[] limbs = new Limb[4];
            Limb[] limbs2 = new Limb[4];
            for (int i = 0; i < 3; i += 1)
            {
                limbs[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            for (int i = 0; i < 3; i += 1)
            {
                limbs2[i] = new Limb("Limb2", 3, 3, 0, 0, 0, 0);
            }
            Faction faction1 = new Faction(1, "DAW", "Dawer", 2);

            Race human = new Race("Humano", -1, 1, 10, -1, 1);


            Hero hero2 = new Hero("Héroe habilidoso", "El Arquero", 8, 2, 2, 3, 8, human, human, limbs2);

            myAdventure = new Adventure("Prueba", hero2, new Random(), myDice);

        }
        public void GoToCombat(object sender, EventArgs e)
        {
            Limb[] limbs = new Limb[4];
            Limb[] limbs2 = new Limb[4];
            for (int i = 0; i < 3; i += 1)
            {
                limbs[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            for (int i = 0; i < 3; i += 1)
            {
                limbs2[i] = new Limb("Limb2", 3, 3, 0, 0, 0, 0);
            }
            Faction faction1 = new Faction(1, "DAW", "Dawer", 2);
            Dice myDice = new Dice(6, 5, rnd);
            Race human = new Race("Humano", -1, 1, 10, -1, 1);
            RangedItem specialItem = new RangedItem("El arma única", 5, 6, 200, 100, 2, new StatModifier(20, 6, 0, -3, -3), Rarity.Rarities.Meitou);
            Hero hero1 = new Hero("Héroe bruto", "El PartePiedras", 10, 4, 1, 4, 5, human, human, limbs);
            Hero hero2 = new Hero("Héroe habilidoso", "El Arquero", 8, 2, 2, 3, 8, human, human, limbs2);

            Hero hero3 = new Hero("Héroe tanque", "El Arquero", 24, 2, 2, 3, 8, human, human, limbs2);
            Hero hero4 = new Hero("Héroe rapidisimo", "El Arquero", 8, 2, 8, 3, 8, human, human, limbs2);
            Monster monster1 = new Monster("Monstruo medio veloz", 3, faction1, 10, 3, 5, Immunities.Immunity.ResistantToRanged, 100, 20, null);
            Monster monster2 = new Monster("Monstruo lento", 2, faction1, 20, 4, 2, Immunities.Immunity.ResistantToBoth, 100, 20, specialItem);
            StatModifier genericStats = new StatModifier(5, 0, 0, 1, -1);
            StatModifier genericRangedStats = new StatModifier(2, 2, 0, 0, -2);
            PlayerInventory myInventory = myAdventure.GetInventory();
            myInventory.AddItem(new MeleeItem("Espada", 10, 2, 2, false, genericStats, false, Rarity.Rarities.Junk));
            myInventory.AddItem(new MeleeItem("Poción de curación", 10, 2, 1, true, new StatModifier(0, 0, 5, 0, 0), true, Rarity.Rarities.Mk));
            myInventory.AddItem(new MeleeItem("Poción de fuerza", 6, 1, 1, false, genericStats, true, Rarity.Rarities.Catun));
            myInventory.AddItem(new RangedItem("Ballesta de principiante", 2, 10, 15, 4, 3, genericRangedStats, Rarity.Rarities.RustCovered));
            Monster[] monsters = new Monster[] { monster1, monster2 };
            controller.Content = new CombatWindow(mainWindow, cursors, rnd, myAdventure, monsters);
        }
        public void GoToShop(object sender, EventArgs e)
        {
            controller.Content = new Shop();
        }
        public void OpenAdventureMaker(object sender, EventArgs e)
        {
            MenuBorder.Visibility = System.Windows.Visibility.Visible;
            MenuWindow.Visibility = System.Windows.Visibility.Visible;
            MenuWindow.Orientation = Orientation.Vertical;

            TextBox textBox = new TextBox();

            textBox.BorderBrush = Brushes.Black;
            textBox.BorderThickness = new System.Windows.Thickness(2);

            textBox.TextChanged += OnlyNumsAndLetters;
            MenuWindow.Children.Add(textBox);

        }
        public void OpenSavedAdventures(object sender, EventArgs e)
        {

        }
        private void OnlyNumsAndLetters(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int caretIndex = -1;
            string text = textBox.Text;
            string result = "";
            for (int i = 0; i < text.Length; i += 1)
            {
                if (char.IsAsciiLetterOrDigit(text[i]))
                {
                    result += text[i];
                }
                else
                {
                    caretIndex = i;
                }
            }
            textBox.Text = result;
            if(caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
        }
    }
}
