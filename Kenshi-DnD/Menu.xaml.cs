using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
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


        TextBox adventureName;
        TextBox diceSides;
        TextBox diceMinWin;
        TextBox startingCats;
        TextBox characterName;
        TextBox characterTitle;
        TextBox characterBackgroundStory;

        ComboBox characterRace;
        ComboBox characterSubrace;
        TextBlock remainingPoints;


        Slider bruteForceSlider;
        Slider dexteritySlider;
        Slider resistanceSlider;
        Slider agilitySlider;


        const int DEFAULT_POINTS_ON_HERO_MAKER = 10;
        const string DEFAULT_HERO_NAME = "Beep";
        const int DEFAULT_CATS = 200;
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

            if (MenuBorder.Visibility == Visibility.Visible)
            {
                MenuBorder.Visibility = Visibility.Collapsed;
                MenuWindow.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                MenuBorder.Visibility = System.Windows.Visibility.Visible;
                MenuWindow.Visibility = System.Windows.Visibility.Visible;
            }
            if (MenuWindow.Children.Count == 0)
            {
                MenuWindow.Orientation = Orientation.Vertical;
                adventureName = new TextBox();

                adventureName.BorderBrush = Brushes.Black;
                adventureName.BorderThickness = new System.Windows.Thickness(2);
                adventureName.Name = "AdventureName";
                adventureName.TextChanged += OnlyNumsAndLetters;
                MenuWindow.Children.Add(PutTextOnElement("Nombre de aventura: ", adventureName));

                diceSides = new TextBox();
                diceSides.BorderBrush = Brushes.Black;
                diceSides.BorderThickness = new System.Windows.Thickness(2);
                diceSides.Name = "DiceSides";
                diceSides.TextChanged += OnlyNums;

                MenuWindow.Children.Add(PutTextOnElement("Número de caras en el dado: ", diceSides));

                diceMinWin = new TextBox();
                diceMinWin.BorderBrush = Brushes.Black;
                diceMinWin.BorderThickness = new System.Windows.Thickness(2);
                diceMinWin.Name = "DiceMinWin";
                diceMinWin.TextChanged += OnlyNums;

                MenuWindow.Children.Add(PutTextOnElement("Número mínimo considerado victoria(Mínimo incluido): ", diceMinWin));

                startingCats = new TextBox();
                startingCats.BorderBrush = Brushes.Black;
                startingCats.BorderThickness = new System.Windows.Thickness(2);
                startingCats.Name = "StartingCats";
                startingCats.TextChanged += OnlyNums;
                startingCats.Text = DEFAULT_CATS.ToString();

                MenuWindow.Children.Add(PutTextOnElement("Cantidad de cats al empezar: ", startingCats));

                characterName = new TextBox();
                characterName.BorderBrush = Brushes.Black;
                characterName.BorderThickness = new System.Windows.Thickness(2);
                characterName.Name = "CharacterName";
                characterName.TextChanged += OnlyText;
                characterName.Text = DEFAULT_HERO_NAME;

                MenuWindow.Children.Add(PutTextOnElement("Nombre de héroe inicial: ", characterName));

                characterTitle = new TextBox();
                characterTitle.BorderBrush = Brushes.Black;
                characterTitle.BorderThickness = new System.Windows.Thickness(2);
                characterTitle.Name = "CharacterName";
                characterTitle.TextChanged += OnlyText;
                characterTitle.Text = DEFAULT_HERO_NAME;

                MenuWindow.Children.Add(PutTextOnElement("Título de héroe inicial: ", characterTitle));

                characterRace = new ComboBox();

                characterRace.ItemsSource = RacesDictionary.RacesAndSubraces.Keys;
                characterRace.SelectionChanged += ChangeSubraces;

                MenuWindow.Children.Add(PutTextOnElement("Raza del héroe: ", characterRace));

                characterSubrace = new ComboBox();



                MenuWindow.Children.Add(PutTextOnElement("Raza del héroe: ", characterSubrace));

                TextBlock remainingPoints = new TextBlock();

                remainingPoints.Name = "RemainingPoints";
                remainingPoints.Tag = DEFAULT_POINTS_ON_HERO_MAKER;

                remainingPoints.Text = remainingPoints.Tag.ToString();

                MenuWindow.Children.Add(PutTextOnElement("Puntos restantes: ", remainingPoints));

                bruteForceSlider = new Slider();
                bruteForceSlider.Minimum = 1;
                bruteForceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER;
                bruteForceSlider.Value = 1;


                MenuWindow.Children.Add(PutTextOnElement("Fuerza bruta: ", bruteForceSlider));

                dexteritySlider = new Slider();
                dexteritySlider.Minimum = 1;
                dexteritySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER;
                dexteritySlider.Value = 1;
                

                MenuWindow.Children.Add(PutTextOnElement("Destreza: ", dexteritySlider));


                resistanceSlider = new Slider();
                resistanceSlider.Minimum = 1;
                resistanceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER;
                resistanceSlider.Value = 1;

                MenuWindow.Children.Add(PutTextOnElement("Resistencia: ", resistanceSlider));

                agilitySlider = new Slider();
                agilitySlider.Minimum = 1;
                agilitySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER;
                agilitySlider.Value = 1;
                

                MenuWindow.Children.Add(PutTextOnElement("Agilidad: ", agilitySlider));

                Grid statsGrid = new Grid();
            }
                        




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
            if (caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
        }
        private void OnlyNums(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int caretIndex = -1;
            string text = textBox.Text;
            string result = "";
            for (int i = 0; i < text.Length; i += 1)
            {
                if (char.IsAsciiDigit(text[i]))
                {
                    result += text[i];
                }
                else
                {
                    caretIndex = i;
                }
            }
            textBox.Text = result;
            if (caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
        }
        private void OnlyText(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            int caretIndex = -1;
            string text = textBox.Text;
            string result = "";
            for (int i = 0; i < text.Length; i += 1)
            {
                if (char.IsAsciiLetter(text[i]))
                {
                    result += text[i];
                }
                else
                {
                    caretIndex = i;
                }
            }
            textBox.Text = result;
            if (caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
        }
        
        private void UpdateHeroStatGrid(Grid statsGrid, Hero hero)
        {

            if (hero == null)
            {
                MessageBox.Show("Tienes que hacer un héroe válido");
                return;
            }
            double width = statsGrid.ActualWidth;

            Debug.WriteLine(width);

            statsGrid.Children.Clear();
            statsGrid.ColumnDefinitions.Clear();
            statsGrid.RowDefinitions.Clear();


            double biggestNum;
            double bruteForce = hero.GetStat(Stats.Stat.BruteForce);
            biggestNum = bruteForce;
            double dexterity = hero.GetStat(Stats.Stat.Dexterity);
            if (biggestNum < dexterity)
            {
                biggestNum = dexterity;
            }
            double resistance = hero.GetStat(Stats.Stat.Resistance);
            if (biggestNum < resistance)
            {
                biggestNum = resistance;
            }
            double agility = hero.GetStat(Stats.Stat.Agility);
            if (biggestNum < agility)
            {
                biggestNum = agility;
            }

            double proportion = width / biggestNum;


            for (int i = 0; i < 4; i += 1)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                statsGrid.RowDefinitions.Add(rowDefinition);
            }
            for (int i = 0; i < 4; i += 1)
            {
                System.Windows.Shapes.Rectangle rectangle = new System.Windows.Shapes.Rectangle();
                TextBlock textBlock = new TextBlock();
                rectangle.HorizontalAlignment = HorizontalAlignment.Left;
                textBlock.HorizontalAlignment = HorizontalAlignment.Left;
                rectangle.VerticalAlignment = VerticalAlignment.Stretch;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                textBlock.Margin = new Thickness(0, 2, 0, 2);
                rectangle.Margin = new Thickness(0, 2, 0, 2);
                rectangle.Stroke = Brushes.Black;
                switch (i)
                {
                    case 0:
                        {
                            rectangle.Width = proportion * bruteForce < 0 ? 0 : proportion * bruteForce;
                            rectangle.Fill = Brushes.SaddleBrown;
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@FBT@: " + bruteForce));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Fuerza Bruta: " + bruteForce,
                                "La fuerza bruta es usada para ataques físicos, y para definir el nivel de artes marciales.");
                            ToolTipService.SetInitialShowDelay(textBlock, 100);
                            break;
                        }
                    case 1:
                        {
                            rectangle.Width = proportion * dexterity < 0 ? 0 : proportion * dexterity;
                            rectangle.Fill = Brushes.SteelBlue;

                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@DST@: " + dexterity));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Destreza: " + dexterity,
                                "La destreza es usada para apuntar en ataques a distancia, y para definir el nivel de artes marciales.");

                            break;
                        }
                    case 2:
                        {
                            rectangle.Width = proportion * resistance < 0 ? 0 : proportion * resistance;
                            rectangle.Fill = Brushes.Olive;
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@RES@: " + resistance));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Resistencia " + resistance,
                                "La resistencia es usada para defenderse de los ataques enemigos, se necesita tener dos puntos por \nencima de la fuerza del atacante " +
                                "para cada posibilidad de defenderse un punto de ataque.");
                            break;
                        }
                    case 3:
                        {
                            rectangle.Width = proportion * agility < 0 ? 0 : proportion * agility;
                            rectangle.Fill = Brushes.Gold;
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@AG@: " + agility));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Agilidad: " + agility,
                                "La agilidad es usada para atacar más seguido que tus enemigos, determinar el número de ataques en \nartes marciales." +
                                "\nTambién para saquear a tus compañeros...");
                            break;
                        }
                }
                Grid.SetRow(rectangle, i);
                Grid.SetRow(textBlock, i);
                //Textblock must be added last, otherwise it won't show on top of rectangle
                statsGrid.Children.Add(rectangle);
                statsGrid.Children.Add(textBlock);
            }
        }
    
        
    
        private void ChangeSubraces(object sender, EventArgs e)
        {
            string race = (string)characterRace.SelectedItem;
            characterSubrace.ItemsSource = RacesDictionary.RacesAndSubraces[race];
        }
        private Grid PutTextOnElement(string text, UIElement element)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            
            TextBlock textBlock = new TextBlock();

            textBlock.Inlines.AddRange(mainWindow.DecorateText(text));
            Grid.SetColumn(textBlock, 0);
            Grid.SetColumn(element, 1);
            grid.Children.Add(textBlock);
            grid.Children.Add(element);

            return grid;
        }
    }
}
