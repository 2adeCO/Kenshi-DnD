using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Text.Json;

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
        bool changingSliders;

        bool currentAdventureIsValid = false;

        Race[] allRaces;

        int remainingPoints;
        const int DEFAULT_DICE_SIDES = 6;
        const int DEFAULT_DICE_MIN_WIN = 4;
        const int DEFAULT_POINTS_ON_HERO_MAKER = 14;
        const string DEFAULT_FACTION_NAME = "El club famelico";
        const string DEFAULT_HERO_NAME = "Beep";
        const string DEFAULT_HERO_TITLE = "El Elegido";
        const string DEFAULT_HERO_BACKGROUND = "Una cualquiera... Abusado por las terribles condiciones de estas tierras, se escabulle ganándose la vida como puede," +
            " realizando ilícitas actividades, tratando de ganarse un nombre sin morir en el intento";
        const int DEFAULT_CATS = 200;
        public Menu(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd, Race[] allRaces)
        {
            changingSliders = true;
            InitializeComponent();
            this.allRaces = allRaces;
            this.cursors = cursors;
            this.controller = controller;
            this.mainWindow = mainWindow;
            this.rnd = rnd;
            TitleText.Inlines.Clear();
            TitleText.Inlines.AddRange(mainWindow.DecorateText("@134@KENSHI_DND@\n@8@Por@ @7@Santiago Cabrero@"));
            remainingPoints = DEFAULT_POINTS_ON_HERO_MAKER - 4 ;
            remainingPointsText.Text = remainingPoints.ToString();
            
            adventureName.Text = "AventuraDe" + DEFAULT_HERO_NAME;
            factionName.Text = DEFAULT_FACTION_NAME;
            factionColor.ItemsSource = new string[] { "Rojo", "Verde", "Azul", "Morado", "Dorado", "Naranja", "Gris azulado", "Gris", "Negro" };
            factionColor.SelectedItem = "Rojo";
            diceMinWin.Text = DEFAULT_DICE_MIN_WIN.ToString();
            diceSides.Text = DEFAULT_DICE_SIDES.ToString();
            characterName.Text = DEFAULT_HERO_NAME;
            characterTitle.Text = DEFAULT_HERO_TITLE;
            startingCats.Text = DEFAULT_CATS.ToString();
            characterBackgroundStory.Text = "";

            changingSliders = false;
            characterRace.ItemsSource = RacesDictionary.RacesAndSubraces.Keys;
            characterRace.SelectedItem = "Humano";
            characterSubrace.ItemsSource = RacesDictionary.RacesAndSubraces["Humano"];

            bruteForceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            dexteritySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            resistanceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            agilitySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            
            Dice myDice = new Dice(6, 5);

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

          
            IsAdventureValid(null, null);
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
            Dice myDice = new Dice(6, 5);
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

            if (AdventureMakerBorder.Visibility == Visibility.Visible)
            {
                AdventureMakerBorder.Visibility = Visibility.Collapsed;
                AdventureMakerMenu.Visibility = Visibility.Collapsed;
                DarkUI.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                DarkUI.Visibility = System.Windows.Visibility.Visible;
                AdventureMakerBorder.Visibility = System.Windows.Visibility.Visible;
                AdventureMakerMenu.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public void OpenSavedAdventures(object sender, EventArgs e)
        {
            if (AdventureChooserBorder.Visibility == Visibility.Visible)
            {
                AdventureChooserBorder.Visibility = Visibility.Collapsed;
                AdventureChooserMenu.Visibility = Visibility.Collapsed;
                DarkUI.Visibility = Visibility.Collapsed;
                return;
            }
            else
            {
                DarkUI.Visibility = System.Windows.Visibility.Visible;
                AdventureChooserBorder.Visibility = System.Windows.Visibility.Visible;
                AdventureChooserMenu.Visibility = System.Windows.Visibility.Visible;
            }
            string[] adventureFiles = Directory.GetFiles("./saves", "*.adventure");

            AdventureChooserMenu.Children.Clear();

            for(int i = 0; i < adventureFiles.Length; i += 1)
            {
#pragma warning disable SYSLIB0011
                FileStream fileStream = new FileStream(adventureFiles[i], FileMode.Open);
                BinaryFormatter formatter = new BinaryFormatter();
                Adventure adventure = (Adventure)formatter.Deserialize(fileStream);
#pragma warning restore SYSLIB0011
                fileStream.Close();
                AdventureChooserMenu.Children.Add(CreateAdventureButton(adventure));
            }

        }
       
        private void SliderChanged(object sender, EventArgs e)
        {
            if (changingSliders) { return; }
            changingSliders = true;

            int totalPointsUsed = (int)(bruteForceSlider.Value + dexteritySlider.Value + resistanceSlider.Value + agilitySlider.Value);

            if (totalPointsUsed > DEFAULT_POINTS_ON_HERO_MAKER)
            {
                Slider changedSlider = (Slider)sender;

                int valueWithoutSlider = totalPointsUsed - (int)changedSlider.Value;

                changedSlider.Value = DEFAULT_POINTS_ON_HERO_MAKER - valueWithoutSlider;
                totalPointsUsed = (int)(bruteForceSlider.Value + dexteritySlider.Value + resistanceSlider.Value + agilitySlider.Value);

            }
            remainingPoints = DEFAULT_POINTS_ON_HERO_MAKER - totalPointsUsed;
            remainingPointsText.Text = remainingPoints.ToString();
            UpdateHeroStatGrid(null, null);
            changingSliders = false;
            IsAdventureValid(null,null);

        }



        private void IsAdventureValid(object sender, EventArgs e)
        {
            AboutYourAdventure.Inlines.Clear();
            currentAdventureIsValid = true;
            if (string.IsNullOrWhiteSpace(adventureName.Text)) 
            {  currentAdventureIsValid = false; }
            else
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu aventura se nombrará " + adventureName.Text + "\n"));
            }
            if (!int.TryParse(diceSides.Text, out int diceSidesInt)) 
            { currentAdventureIsValid = false; }
            if (!int.TryParse(diceMinWin.Text, out int diceMinWinInt)) { currentAdventureIsValid = false; }

            if (string.IsNullOrWhiteSpace(characterName.Text)) { currentAdventureIsValid = false; }
            if (string.IsNullOrWhiteSpace(factionName.Text)) { currentAdventureIsValid = false; }
            
            


            if(diceMinWinInt > diceSidesInt) { currentAdventureIsValid = false; } else
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Jugarás con un dado que tiene "
                    + diceSidesInt + " caras, cada vez que saques al menos (" + diceMinWinInt + ")" +
                    ", tendrás un éxito.\n"));
            }

            if (!int.TryParse(startingCats.Text, out int startingCatsInt)) { currentAdventureIsValid = false; } else
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu aventura empezará con " + startingCatsInt + " cats. " +
                    (startingCatsInt > 2000 ? "Eso es una fortuna...\n" : (startingCatsInt > 500 ? "" : "Buena suerte...\n"))));

            }

            if (currentAdventureIsValid)
            {
                Debug.WriteLine((factionColor.SelectedIndex + 1));
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu facción se llama @" + 
                    (factionColor.SelectedIndex + 1) + "@" + factionName.Text + "@\n"));

                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu héroe es " + characterName.Text + ", " +
                    (characterTitle.Text == "" ? " " : characterTitle.Text + ", ") + "un " + characterRace.SelectedItem + "\n"));

                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Su historia es: " + (characterBackgroundStory.Text == "" ? 
                  DEFAULT_HERO_BACKGROUND : characterBackgroundStory.Text) + "\n\n"));

                
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Recuerda, es un mundo hostil al que te enfrentas. " +
                    "Puedes recibir golpes que te dejen paralítico, morirte de hambre, ser secuestrado... \n" +
                    "Elige bien a tus compañeros y mejor a tus enemigos" +
                    "\nProcede con cuidado en el mundo de Kenshi_DnD..."));
                MakeAdventureButton.IsEnabled = true;
                MakeAdventureButton.Background = Brushes.LightGreen;
            }
            else
            {
                MakeAdventureButton.IsEnabled = false;
                MakeAdventureButton.Background = Brushes.Red;
            }
        }
        

        private void UpdateHeroStatGrid(object sender, EventArgs e)
        {

            Hero hero = new Hero((int)bruteForceSlider.Value, (int)dexteritySlider.Value, (int)resistanceSlider.Value, (int)agilitySlider.Value,
                    GetSelectedRace(), GetSeletedSubrace(), GenerateLimbs());
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
            characterSubrace.SelectedIndex = 0;
        }
        private Race GetSelectedRace()
        {
            for (int i = 0; i < allRaces.Length; i += 1)
            {
                if (allRaces[i].GetName() == (string)characterRace.SelectedItem)
                {
                    return allRaces[i];
                }
            }
            return null;
        }
        private Race GetSeletedSubrace()
        {
            for(int i = 0; i < allRaces.Length; i += 1)
            {
                if (allRaces[i].GetName() == (string)characterSubrace.SelectedItem)
                {
                    return allRaces[i];
                }
            }
            return null;
        }
        private void MakeAdventure(object sender, EventArgs e)
        {
            if (currentAdventureIsValid)
            {
                Hero hero = new Hero((int)bruteForceSlider.Value, (int)dexteritySlider.Value, (int)resistanceSlider.Value, (int)agilitySlider.Value,
                    GetSelectedRace(), GetSeletedSubrace(), GenerateLimbs());
                Dice myDice = new Dice(int.Parse(diceSides.Text), int.Parse(diceMinWin.Text));
                Adventure myAdventure = new Adventure(adventureName.Text, hero, rnd, myDice, int.Parse(startingCats.Text));

                if (!Directory.Exists("./saves"))
                {
                    Directory.CreateDirectory("./saves");
                }

                string adventurePath = "./saves/" + myAdventure.GetId() + ".adventure";

                if(File.Exists(adventurePath))
                {
                    return;
                }
                else
                {

                    FileStream fileStream = new FileStream(adventurePath, FileMode.CreateNew);
                    
#pragma warning disable SYSLIB0011 

                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fileStream, myAdventure);

#pragma warning restore SYSLIB0011 

                    fileStream.Close();


                    Debug.WriteLine("Adventure saved in: " + adventurePath);
                }

            }
            else
            {
                MessageBox.Show("Aventura inválida, revisa los campos.");
            }
        }
        private Limb[] GenerateLimbs()
        {
            Limb[] limbs = new Limb[4];

            for (int i = 0; i < limbs.Length; i += 1)
            {
                limbs[i] = new Limb("Extremidad normal", 0, 0, 0, 0, 0, 0);
            }
            return limbs;
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
                    if (char.IsWhiteSpace(text[i]))
                    {
                        result += text[i];
                    }
                    else
                    {
                        caretIndex = i;
                    }
                }
            }
            textBox.Text = result;
            if (caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
            IsAdventureValid(null, null);
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
            IsAdventureValid(null, null);
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
                    if (char.IsWhiteSpace(text[i]))
                    {
                        result += text[i];
                    }
                    else
                    {
                        caretIndex = i;
                    }
                }
            }
            textBox.Text = result;
            if (caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
            IsAdventureValid(null, null);
        }
        private void CloseCurrentMenu(object sender, EventArgs e)
        {
            if (AdventureMakerBorder.Visibility == Visibility.Visible)
            {
                AdventureMakerBorder.Visibility = Visibility.Collapsed;
                AdventureMakerMenu.Visibility = Visibility.Collapsed;
                DarkUI.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (AdventureChooserBorder.Visibility == Visibility.Visible)
                {
                    AdventureChooserBorder.Visibility = Visibility.Collapsed;
                    AdventureChooserMenu.Visibility = Visibility.Collapsed;
                    DarkUI.Visibility = Visibility.Collapsed;
                }
            }
        }
        private StackPanel CreateAdventureButton(Adventure adventure)
        {
            StackPanel adventureStack = new StackPanel();
            adventureStack.Orientation = Orientation.Horizontal;
            adventureStack.Margin = new Thickness(0, 5, 0, 5);
            adventureStack.HorizontalAlignment = HorizontalAlignment.Center;
            adventureStack.VerticalAlignment = VerticalAlignment.Center;
            adventureStack.Background = Brushes.LightGray;

            Button button = new Button();
            button.Content = adventure.GetId();

            Button deleteButton = new Button();
            deleteButton.Content = "🗑️";

            adventureStack.Children.Add(button);
            adventureStack.Children.Add(deleteButton);

            return adventureStack;
        }
    }
}
