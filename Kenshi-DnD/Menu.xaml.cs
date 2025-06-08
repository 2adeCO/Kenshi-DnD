using MySql.Data.MySqlClient;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace Kenshi_DnD
{
    // Menu of the game, used to create or access adventures, change configuration of Sql connection, or leave the game. - Santiago Cabrero
    public partial class Menu : UserControl
    {
        // Window
        MainWindow mainWindow;
        // Random
        Random rnd;
        // Page controller
        ContentControl controller;
        // Cursors
        Cursor[] cursors;
        // Flag used to avoid an infinite loop of SlidersChanged
        bool changingSliders;
        // Tells if the current adventure being made is not possible
        bool currentAdventureIsValid = false;

        // Gets all races from MySQL or XML, for the purpose of generating the first character at the start
        Race[] allRaces;

        // Points left on the character creation sliders
        int remainingPoints;

        // Const values used to place default values on adventure creation
        const int DEFAULT_DICE_SIDES = 6;
        const int DEFAULT_DICE_MIN_WIN = 4;
        // The points on the hero maker have to take into account the 4 points that are already assigned to the hero
        const int DEFAULT_POINTS_ON_HERO_MAKER = 8;
        const string DEFAULT_FACTION_NAME = "El club famelico";
        const string DEFAULT_HERO_NAME = "Beep";
        const string DEFAULT_HERO_TITLE = "El Elegido";
        const string DEFAULT_HERO_BACKGROUND = "Una cualquiera... Abusado por las terribles condiciones de estas tierras, se escabulle ganándose la vida como puede," +
            " realizando ilícitas actividades, tratando de ganarse un nombre sin morir en el intento";
        const int DEFAULT_CATS = 2000;
        // Constructor
        public Menu(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd)
        {
            changingSliders = true;
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.allRaces = SqlGetRaces();
            this.cursors = cursors;
            this.controller = controller;
            this.rnd = rnd;
            // Title text
            TitleText.Inlines.Clear();
            TitleText.Inlines.AddRange(mainWindow.DecorateText("@134@KENSHI_DND@\n@8@Por@ @7@Santiago Cabrero@"));
            remainingPoints = DEFAULT_POINTS_ON_HERO_MAKER - 4;
            remainingPointsText.Text = remainingPoints.ToString();


            adventureName.Text = "Aventura De " + DEFAULT_HERO_NAME;
            factionName.Text = DEFAULT_FACTION_NAME;
            // Valid colors
            factionColor.ItemsSource = new string[] { "Rojo", "Verde", "Azul", "Morado", "Dorado", "Naranja", "Gris azulado", "Gris", "Negro" };
            factionColor.SelectedItem = "Rojo";
            diceMinWin.Text = DEFAULT_DICE_MIN_WIN.ToString();
            diceSides.Text = DEFAULT_DICE_SIDES.ToString();
            characterName.Text = DEFAULT_HERO_NAME;
            characterTitle.Text = DEFAULT_HERO_TITLE;
            startingCats.Text = DEFAULT_CATS.ToString();
            characterBackgroundStory.Text = "";

            changingSliders = false;
            // Default character race and subrace
            characterRace.ItemsSource = RacesDictionary.RacesAndSubraces.Keys;
            characterRace.SelectedItem = "Enjambre";
            characterSubrace.ItemsSource = RacesDictionary.RacesAndSubraces["Enjambre"];
            characterSubrace.SelectedItem = "Dron Soldado";

            // Maximum values of sliders
            bruteForceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            dexteritySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            resistanceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            agilitySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            // Checks if the adventure is valid
            IsAdventureValid(null, null);
        }
        // Valid way of closing the game
        private void LeaveGame(object sender, EventArgs e)
        {
            mainWindow.Close();
        }
        // Opens the game's config folder in file explorer
        private void OpenConfig(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Path.GetFullPath("./Resources/config"));
        }
        // Opens the adventure maker
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
                // Dark UI is the semi transparent background when a menu is opened, it can be clicked to close that menu
                DarkUI.Visibility = System.Windows.Visibility.Visible;
                AdventureMakerBorder.Visibility = System.Windows.Visibility.Visible;
                AdventureMakerMenu.Visibility = System.Windows.Visibility.Visible;
            }
        }
        // Opens the saved adventures menu
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
                // Dark UI is the semi transparent background when a menu is opened, it can be clicked to close that menu
                DarkUI.Visibility = System.Windows.Visibility.Visible;
                AdventureChooserBorder.Visibility = System.Windows.Visibility.Visible;
                AdventureChooserMenu.Visibility = System.Windows.Visibility.Visible;
            }
            // Loads the paths of the files
            string[] adventureFiles = Directory.GetFiles("./saves", "*.adventure");

            AdventureChooserMenu.Children.Clear();

            // Deserialises each adventure, and adds a new grid to a stackpanel representing each adventure
            for (int i = 0; i < adventureFiles.Length; i += 1)
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
        // When an slider is changed, check if it's valid, if not, revert to the last state
        private void SliderChanged(object sender, EventArgs e)
        {
            // Use of flag to avoid loops
            if (changingSliders) { return; }
            changingSliders = true;

            // All points combined
            int totalPointsUsed = (int)(bruteForceSlider.Value + dexteritySlider.Value + resistanceSlider.Value + agilitySlider.Value);
            // If it isn't valid, revert the slider changed to all the possible points it can actually take
            if (totalPointsUsed > DEFAULT_POINTS_ON_HERO_MAKER)
            {
                Slider changedSlider = (Slider)sender;

                int valueWithoutSlider = totalPointsUsed - (int)changedSlider.Value;

                changedSlider.Value = DEFAULT_POINTS_ON_HERO_MAKER - valueWithoutSlider;
                totalPointsUsed = (int)(bruteForceSlider.Value + dexteritySlider.Value + resistanceSlider.Value + agilitySlider.Value);

            }
            // Updates remaining points
            remainingPoints = DEFAULT_POINTS_ON_HERO_MAKER - totalPointsUsed;
            remainingPointsText.Text = remainingPoints.ToString();
            // Updates the hero stat grid
            UpdateHeroStatGrid(null, null);
            // Reverts the flag
            changingSliders = false;
            // Checks if the adventure is valid
            IsAdventureValid(null, null);

        }


        // Checks if the adventure is valid and doesn't have any empty values
        private void IsAdventureValid(object sender, EventArgs e)
        {
            AboutYourAdventure.Inlines.Clear();
            currentAdventureIsValid = true;
            if (string.IsNullOrWhiteSpace(adventureName.Text))
            { currentAdventureIsValid = false; }
            else
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu aventura se nombrará " + adventureName.Text + "\n"));
            }
            if (!int.TryParse(diceSides.Text, out int diceSidesInt))
            { currentAdventureIsValid = false; }
            if (!int.TryParse(diceMinWin.Text, out int diceMinWinInt)) { currentAdventureIsValid = false; }

            if (string.IsNullOrWhiteSpace(characterName.Text)) { currentAdventureIsValid = false; }
            if (string.IsNullOrWhiteSpace(factionName.Text)) { currentAdventureIsValid = false; }




            if (diceMinWinInt > diceSidesInt) { currentAdventureIsValid = false; }
            else
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Jugarás con un dado que tiene "
                    + diceSidesInt + " caras, cada vez que saques al menos (" + diceMinWinInt + ")" +
                    ", tendrás un éxito.\n"));
            }

            if (!int.TryParse(startingCats.Text, out int startingCatsInt)) { currentAdventureIsValid = false; }
            else
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu aventura empezará con " + startingCatsInt + " cats. " +
                    (startingCatsInt > 2000 ? "Eso es una fortuna...\n" : (startingCatsInt > 500 ? "" : "Buena suerte...\n"))));

            }

            if (currentAdventureIsValid)
            {
                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu facción se llama @" +
                    (factionColor.SelectedIndex + 1) + "@" + factionName.Text + "@\n"));

                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Tu héroe es " + characterName.Text + ", " +
                    (characterTitle.Text == "" ? " " : characterTitle.Text + ", ") + "un " + characterRace.SelectedItem + "\n"));

                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Su historia es: " + (characterBackgroundStory.Text == "" ?
                  DEFAULT_HERO_BACKGROUND : characterBackgroundStory.Text) + "\n\n"));


                AboutYourAdventure.Inlines.AddRange(mainWindow.DecorateText("Recuerda, es un mundo hostil al que te enfrentas. " +
                    "Puedes recibir golpes que te dejen paralítico, ser rechazado por pobre, o por tus creencias... \n" +
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
        // Used to play the adventure in the button
        private void PlayAdventure(object sender, EventArgs e)
        {
            Adventure adventure = (Adventure)((Button)sender).Tag;

            if (adventure != null)
            {
                mainWindow.StartPlaying(adventure);
                controller.Content = new Map(mainWindow, controller, cursors, rnd, adventure);
            }
            else
            {
                MessageBox.Show("Algo ha ido terriblemente mal.");
            }
        }
        // Used to delete an adventure, but first gives off a warning
        private void DeleteAdventureWarning(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Adventure adventure = (Adventure)button.Tag;
            string adventurePath = "./saves/" + adventure.GetId() + ".adventure";
            if (File.Exists(adventurePath))
            {
                if (MessageBox.Show("¿Quieres eliminar la aventura " + adventure.GetId() + "?",
                    "Borrar " + adventure.GetId(), MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    File.Delete(adventurePath);
                    AdventureChooserMenu.Children.Remove((Grid)button.Parent);
                    MessageBox.Show("Aventura eliminada.");
                }
            }
            else
            {
                MessageBox.Show("No se ha encontrado la aventura " + adventure.GetId() + ", no se puede eliminar.");
            }
        }
        // Updates the visualization of stats in the create adventure menu
        private void UpdateHeroStatGrid(object sender, EventArgs e)
        {

            Hero hero = new Hero((int)bruteForceSlider.Value, (int)dexteritySlider.Value, (int)resistanceSlider.Value, (int)agilitySlider.Value,
                    GetSelectedRace(), GetSeletedSubrace(), GenerateLimbs());
            double width = statsGrid.ActualWidth;

            statsGrid.Children.Clear();
            statsGrid.ColumnDefinitions.Clear();
            statsGrid.RowDefinitions.Clear();


            double biggestNum;
            double hp = hero.GetToughness();
            biggestNum = hp;
            double bruteForce = hero.GetStat(Stats.Stat.BruteForce);
            if (biggestNum < bruteForce)
            {
                biggestNum = bruteForce;
            }
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


            for (int i = 0; i < 5; i += 1)
            {
                RowDefinition rowDefinition = new RowDefinition();
                rowDefinition.Height = new GridLength(1, GridUnitType.Star);
                statsGrid.RowDefinitions.Add(rowDefinition);
            }
            for (int i = 0; i < 5; i += 1)
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
                            rectangle.Width = proportion * hp < 0 ? 0 : proportion * hp;
                            rectangle.Fill = Brushes.DarkRed;
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@DRZ@: " + hp));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("DUREZA: " + hp,
                                "La dureza representa tu vida máxima.");
                            ToolTipService.SetInitialShowDelay(textBlock, 100);
                            break;
                        }
                    case 1:
                        {
                            rectangle.Width = proportion * bruteForce < 0 ? 0 : proportion * bruteForce;
                            rectangle.Fill = Brushes.SaddleBrown;
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@FBT@: " + bruteForce));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Fuerza Bruta: " + bruteForce,
                                "La fuerza bruta es usada para ataques físicos, y para definir el nivel de artes marciales.");
                            ToolTipService.SetInitialShowDelay(textBlock, 100);
                            break;
                        }
                    case 2:
                        {
                            rectangle.Width = proportion * dexterity < 0 ? 0 : proportion * dexterity;
                            rectangle.Fill = Brushes.SteelBlue;

                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@DST@: " + dexterity));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Destreza: " + dexterity,
                                "La destreza es usada para apuntar en ataques a distancia, y para definir el nivel de artes marciales.");

                            break;
                        }
                    case 3:
                        {
                            rectangle.Width = proportion * resistance < 0 ? 0 : proportion * resistance;
                            rectangle.Fill = Brushes.Olive;
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@RES@: " + resistance));
                            textBlock.ToolTip = mainWindow.HeaderToolTipThemer("Resistencia " + resistance,
                                "La resistencia es usada para defenderse de los ataques enemigos, se necesita tener dos puntos por \nencima de la fuerza del atacante " +
                                "para cada posibilidad de defenderse un punto de ataque.");
                            break;
                        }
                    case 4:
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

        // Changes the subraces in the character subrace combobox
        private void ChangeSubraces(object sender, EventArgs e)
        {
            string race = (string)characterRace.SelectedItem;
            characterSubrace.ItemsSource = RacesDictionary.RacesAndSubraces[race];
            characterSubrace.SelectedIndex = 0;
            UpdateHeroStatGrid(null, null);
        }

        // Gets the selected race and subrace
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
            if (characterSubrace.SelectedItem == null) { characterSubrace.SelectedIndex = 0; }
            for (int i = 0; i < allRaces.Length; i += 1)
            {
                if (allRaces[i].GetName() == (string)characterSubrace.SelectedItem)
                {
                    return allRaces[i];
                }
            }
            return null;
        }
        // Creates an adventure with all the values chosen
        private void MakeAdventure(object sender, EventArgs e)
        {
            if (currentAdventureIsValid)
            {
                // Gets all the data of the game
                Faction[] factions = SqlGetFactions();
                Region[] regions = SqlGetRegions();
                Limb[] limbs = SqlGetLimbs();
                Item[] items = SqlGetItems();
                string[] titles = SqlGetTitles();
                string[] backgrounds = SqlGetBackgrounds();
                string[] names = SqlGetNames();

                Hero hero = new Hero(characterName.Text, characterTitle.Text, characterBackgroundStory.Text,
                    (int)bruteForceSlider.Value, (int)dexteritySlider.Value, (int)resistanceSlider.Value, (int)agilitySlider.Value, 1,
                    GetSelectedRace(), GetSeletedSubrace(), GenerateLimbs(), Competency.StartCompetency.Apprentice);
                hero.Hire();
                Dice myDice = new Dice(int.Parse(diceSides.Text), int.Parse(diceMinWin.Text));
                Adventure myAdventure = new Adventure(adventureName.Text, hero, rnd, myDice, int.Parse(startingCats.Text),
                    factionName.Text, factionColor.SelectedIndex + 1, factions, regions, titles, backgrounds, names, items, allRaces, limbs);

                // Gives a random item at the start to the hero
                Item randomItemAtStart = null;
                do
                {
                    int rng = rnd.Next(0, items.Length);

                    if (items[rng] is MeleeItem)
                    {
                        MeleeItem meleeItem = (MeleeItem)items[rng];
                        if (!meleeItem.BreaksOnUse())
                        {
                            meleeItem = (MeleeItem)meleeItem.GetCopy();
                            meleeItem.UpgradeRarity(Rarity.Rarities.Junk);
                            randomItemAtStart = meleeItem;
                        }
                    }
                } while (randomItemAtStart == null);
                myAdventure.GetInventory().AddItem(randomItemAtStart);
                // Creates saves directory if it doesn't exist
                if (!Directory.Exists("./saves"))
                {
                    Directory.CreateDirectory("./saves");
                }
                // Creates new path for the adventure
                string adventurePath = "./saves/" + myAdventure.GetId() + ".adventure";
                // If it's new, it serializes the adventure in that path
                if (File.Exists(adventurePath))
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

                    // Closes the menu
                    CloseCurrentMenu(null, null);
                }

            }
            else
            {
                MessageBox.Show("Aventura inválida, revisa los campos.");
            }
        }
        // Generates default limbs for the character creator
        private Limb[] GenerateLimbs()
        {
            Limb[] limbs = new Limb[4];

            for (int i = 0; i < limbs.Length; i += 1)
            {
                limbs[i] = new Limb("Extremidad normal", 0, 0, 0, 0, 0, 0);
            }
            return limbs;
        }
        // Lets only numbers and letters in a textbox
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
        // Lets only numbers in a textbox
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
        // Lets only letters in a textbox
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
        // Closes the opened menu
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
        // Creates a grid with two buttons, that play or erase an adventure
        private Grid CreateAdventureButton(Adventure adventure)
        {
            Grid adventureGrid = new Grid();
            ColumnDefinition columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(1, GridUnitType.Star);
            adventureGrid.ColumnDefinitions.Add(columnDefinition);

            columnDefinition = new ColumnDefinition();
            columnDefinition.Width = new GridLength(0, GridUnitType.Auto);
            adventureGrid.ColumnDefinitions.Add(columnDefinition);

            LinearGradientBrush linearBrush = new LinearGradientBrush();
            linearBrush.StartPoint = new Point(0, 0);
            linearBrush.EndPoint = new Point(1, 0);

            linearBrush.GradientStops.Add(new GradientStop(Colors.AntiqueWhite, 0.0));
            linearBrush.GradientStops.Add(new GradientStop(Colors.Goldenrod, 0.7));
            linearBrush.GradientStops.Add(new GradientStop(Colors.DarkGoldenrod, 1.0));

            adventureGrid.Margin = new Thickness(10);
            adventureGrid.VerticalAlignment = VerticalAlignment.Center;
            adventureGrid.Background = Brushes.LightGray;
            adventureGrid.Height = 120;


            Button button = new Button();
            button.Content = mainWindow.GenerateTextblock("Aventura: " + adventure.GetId() + "\n" +
                "Horas jugadas: " + adventure.GetTimePlayed() + "\n" +
                "Fecha de empiece: " + adventure.GetStartDate() + "\n" +
                "Dado: " + adventure.GetDice().ToString() + "\n" +
                "Cats: " + adventure.GetCats() + "$");
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            button.Background = linearBrush;
            button.Tag = adventure;
            button.HorizontalContentAlignment = HorizontalAlignment.Left;


            Button deleteButton = new Button();
            deleteButton.Content = "🗑️";
            deleteButton.FontSize = 30;
            deleteButton.Background = Brushes.Red;
            deleteButton.Width = 120;
            deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
            deleteButton.Tag = adventure;

            button.Click += PlayAdventure;
            deleteButton.Click += DeleteAdventureWarning;

            Grid.SetColumn(button, 0);
            Grid.SetColumn(deleteButton, 1);

            adventureGrid.Children.Add(button);
            adventureGrid.Children.Add(deleteButton);

            return adventureGrid;
        }
        // SQL and XML query to get game data
        private Faction[] SqlGetFactions()
        {
            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetFactions();
            }

            MySqlConnection connection = null;
            Faction[] factions = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand("SELECT count(*) FROM factions;", connection);
                MySqlDataReader reader;
                int numberOfFactions = 0;


                connection.Open();
                reader = command.ExecuteReader();
                reader.Read();
                numberOfFactions = reader.GetInt32(0);
                reader.Close();


                factions = new Faction[numberOfFactions];
                command = new MySqlCommand("SELECT * from factions;", connection);
                reader = command.ExecuteReader();

                for (int i = 0; i < numberOfFactions; i += 1)
                {
                    reader.Read();
                    factions[i] = new Faction(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetBoolean(5));
                }
                reader.Close();
                for (int i = 0; i < numberOfFactions; i += 1)
                {
                    command = new MySqlCommand("Select hostilities.hostility from hostilities " +
                        "inner join faction_hostility on faction_hostility.idHostility = hostilities.id " +
                        "where faction_hostility.idFaction = " + factions[i].GetFactionId() + ";", connection);
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        factions[i].AddHostility(reader.GetString(0));
                    }
                    reader.Close();
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySql wanted to crash");
                connection.Close();
                return XmlGetFactions();
            }

            return factions;
        }
        private Faction[] XmlGetFactions()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");
                    Faction[] factions = xmlFile.Root.Elements("factions")
                        .Select((f, index) => new Faction(
                            int.Parse(f.Element("id").Value),
                            f.Element("name").Value,
                            f.Element("description").Value,
                            int.Parse(f.Element("baseRelations").Value),
                            int.Parse(f.Element("color").Value),
                            bool.Parse(f.Element("respectByFighting").Value))
                        ).ToArray();

                    XElement[] fh = xmlFile.Root.Elements("faction_hostility").ToArray();

                    for (int i = 0; i < fh.Length; i += 1)
                    {
                        int factionId = int.Parse(fh[i].Element("idFaction").Value);
                        string hostility = xmlFile.Root.Elements("hostilities").Where(h => int.Parse(h.Element("id").Value) ==
                        int.Parse(fh[i].Element("idHostility").Value)).Select(h => h.Element("hostility").Value).FirstOrDefault();

                        factions[factionId - 1].AddHostility(hostility);
                    }

                    return factions;
                }
                else
                {
                    throw new XMLNotFoundException();
                }

            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }

        }
        private Item[] SqlGetItems()
        {
            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetItems();
            }

            MySqlConnection connection = null;
            MySqlCommand command;
            MySqlDataReader reader;
            int numberOfItems = 0;

            try
            {
                connection = new MySqlConnection(connectionString);

                connection.Open();


                command = new MySqlCommand("SELECT count(*) FROM items;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                numberOfItems = reader.GetInt32(0);
                reader.Close();

                Item[] items = new Item[numberOfItems];
                int iteration = 0;

                command = new MySqlCommand("SELECT i.name, i.description, i.value, i.resellValue, i.weight, mi.canRevive, stats.bruteForce, " +
                    "stats.dexterity, stats.hp, stats.resistance, stats.agility, mi.breaksOnUse " +
                    "FROM items i INNER JOIN stats ON i.stats_id = stats.id " +
                    "INNER JOIN meleeitems mi ON mi.item_id = i.id;", connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items[iteration] = new MeleeItem(
                        reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3),
                        reader.GetInt32(4), reader.GetBoolean(5), new StatModifier(
                            reader.GetInt32(6), reader.GetInt32(7), reader.GetInt32(8),
                            reader.GetInt32(9), reader.GetInt32(10)), reader.GetBoolean(11));
                    iteration++;
                }
                reader.Close();


                command = new MySqlCommand("SELECT i.name, i.description, i.value, i.resellValue, i.weight, ri.difficulty, ri.maxAmmo, stats.bruteForce, " +
                    "stats.dexterity, stats.hp, stats.resistance, stats.agility " +
                    "FROM items i INNER JOIN stats ON i.stats_id = stats.id " +
                    "INNER JOIN rangeditems ri ON ri.item_id = i.id;", connection);
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    items[iteration] = new RangedItem(
                        reader.GetString(0), reader.GetString(1), reader.GetInt32(2), reader.GetInt32(3),
                        reader.GetInt32(4), reader.GetInt32(5), reader.GetInt32(6), new StatModifier(
                            reader.GetInt32(7), reader.GetInt32(8), reader.GetInt32(9),
                            reader.GetInt32(10), reader.GetInt32(11)));
                    iteration++;
                }
                reader.Close();


                connection.Close();

                return items;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetItems();
            }
            catch (DBNotFoundException ex)
            {
                connection.Close();
                return XmlGetItems();
            }
        }
        private Item[] XmlGetItems()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    StatModifier[] stats = xmlFile.Root.Element("stats").Elements("stat")
                        .Select(s => new StatModifier(
                            int.Parse(s.Element("bruteForce").Value),
                            int.Parse(s.Element("dexterity").Value),
                            int.Parse(s.Element("hp").Value),
                            int.Parse(s.Element("resistance").Value),
                            int.Parse(s.Element("agility").Value))).ToArray();

                    Item[] items = xmlFile.Root.Element("items").Elements("item")
                        .Select((i) => i.Element("type").Value == "melee" ?
                            (Item)new MeleeItem(
                                i.Element("name").Value,
                                i.Element("description").Value,
                                int.Parse(i.Element("value").Value),
                                int.Parse(i.Element("resellValue").Value),
                                int.Parse(i.Element("weight").Value),
                                xmlFile.Root.Element("meleeItems").Elements("meleeItem").Where(s => int.Parse(s.Element("item_id").Value) == int.Parse(i.Element("id").Value))
                                .Select(s => bool.Parse(s.Element("canRevive").Value)).FirstOrDefault(),
                                stats[int.Parse(i.Element("stats_id").Value) - 1],
                                xmlFile.Root.Element("meleeItems").Elements("meleeItem").Where(s => int.Parse(s.Element("item_id").Value) == int.Parse(i.Element("id").Value))
                                .Select(s => bool.Parse(s.Element("breaksOnUse").Value)).FirstOrDefault()) :
                            (Item)new RangedItem(
                                i.Element("name").Value,
                                i.Element("description").Value,
                                int.Parse(i.Element("value").Value),
                                int.Parse(i.Element("resellValue").Value),
                                int.Parse(i.Element("weight").Value),
                                xmlFile.Root.Element("rangedItems").Elements("rangedItem").Where(s => int.Parse(s.Element("item_id").Value) == int.Parse(i.Element("id").Value))
                                .Select(s => int.Parse(s.Element("difficulty").Value)).FirstOrDefault(),
                                xmlFile.Root.Element("rangedItems").Elements("rangedItem").Where(s => int.Parse(s.Element("item_id").Value) == int.Parse(i.Element("id").Value))
                                .Select(s => int.Parse(s.Element("maxAmmo").Value)).FirstOrDefault(),
                                stats[int.Parse(i.Element("stats_id").Value) - 1]
                        )).ToArray();
                    return items;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
        private Limb[] SqlGetLimbs()
        {

            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetLimbs();
            }

            MySqlConnection connection = null;
            MySqlDataReader reader;
            Limb[] limbs;

            try
            {
                connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand("select count(*) from limbs;", connection);
                connection.Open();
                reader = command.ExecuteReader();
                reader.Read();
                int numOfLimbs = reader.GetInt32(0);

                reader.Close();

                command = new MySqlCommand("SELECT limbs.name,limbs.value, stats.bruteForce, stats.dexterity, stats.hp, stats.resistance, stats.agility FROM limbs inner join stats" +
                    " on limbs.stats_id = stats.id;", connection);

                reader = command.ExecuteReader();
                limbs = new Limb[numOfLimbs];

                for (int i = 0; i < numOfLimbs; i += 1)
                {
                    reader.Read();
                    limbs[i] = new Limb(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3), reader.GetInt32(4),
                        reader.GetInt32(5), reader.GetInt32(6));
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetLimbs();
            }
            return limbs;
        }
        private Limb[] XmlGetLimbs()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    Limb[] limbs = xmlFile.Root.Element("limbs").Elements("limb").Select(l => new Limb(
                        l.Element("name").Value,
                        int.Parse(l.Element("value").Value),
                        xmlFile.Root.Element("stats").Elements("stat").Where(s => int.Parse(s.Element("id").Value) == int.Parse(l.Element("stats_id").Value))
                        .Select(s => int.Parse(s.Element("bruteForce").Value)).FirstOrDefault(),
                        xmlFile.Root.Element("stats").Elements("stat").Where(s => int.Parse(s.Element("id").Value) == int.Parse(l.Element("stats_id").Value))
                        .Select(s => int.Parse(s.Element("dexterity").Value)).FirstOrDefault(),
                        xmlFile.Root.Element("stats").Elements("stat").Where(s => int.Parse(s.Element("id").Value) == int.Parse(l.Element("stats_id").Value))
                        .Select(s => int.Parse(s.Element("hp").Value)).FirstOrDefault(),
                        xmlFile.Root.Element("stats").Elements("stat").Where(s => int.Parse(s.Element("id").Value) == int.Parse(l.Element("stats_id").Value))
                        .Select(s => int.Parse(s.Element("resistance").Value)).FirstOrDefault(),
                         xmlFile.Root.Element("stats").Elements("stat").Where(s => int.Parse(s.Element("id").Value) == int.Parse(l.Element("stats_id").Value))
                        .Select(s => int.Parse(s.Element("agility").Value)).FirstOrDefault()
                        )).ToArray();
                    return limbs;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
        private Region[] SqlGetRegions()
        {
            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetRegions();
            }

            MySqlConnection connection = null;
            MySqlDataReader reader;
            Region[] regions;
            Faction[] factions = SqlGetFactions();
            try
            {

                connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand("select count(*) from regions;", connection);

                connection.Open();

                reader = command.ExecuteReader();

                reader.Read();
                int numOfRegions = reader.GetInt32(0);
                regions = new Region[numOfRegions];
                reader.Close();

                command = new MySqlCommand("select * from regions", connection);
                reader = command.ExecuteReader();
                for (int i = 0; i < numOfRegions; i += 1)
                {
                    reader.Read();
                    regions[i] = new Region(reader.GetString(1), reader.GetString(2),
                        reader.GetBoolean(3), reader.GetBoolean(4), reader.GetBoolean(5), reader.GetBoolean(6), reader.GetBoolean(7));
                }
                reader.Close();

                command = new MySqlCommand("select regions.id, factions.id from regions inner join region_faction on regions.id = region_faction.RegionId " +
                    "inner join factions on factions.id = region_faction.factionId;", connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int regionId = reader.GetInt32(0) - 1;
                    int factionId = reader.GetInt32(1) - 1;

                    regions[regionId].AddFaction(factions[factionId]);
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetRegions();
            }
            return regions;

        }
        private Region[] XmlGetRegions()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    Region[] regions = xmlFile.Root.Elements("regions").Select(r => new Region(
                        r.Element("name").Value, r.Element("description").Value,
                        bool.Parse(r.Element("hasBar").Value),
                        bool.Parse(r.Element("hasShop").Value),
                        bool.Parse(r.Element("hasLimbHospital").Value),
                        bool.Parse(r.Element("hasContrabandMarket").Value),
                        bool.Parse(r.Element("hasRangedShop").Value)
                        )).ToArray();
                    Faction[] factions = XmlGetFactions();


                    XElement[] rf = xmlFile.Root.Elements("region_faction").ToArray();
                    for (int i = 0; i < rf.Length; i += 1)
                    {

                        int regionId = int.Parse(rf[i].Element("regionId").Value);
                        int factionId = int.Parse(rf[i].Element("factionId").Value);

                        regions[regionId - 1].AddFaction(factions[factionId - 1]);
                    }

                    return regions;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
        private Race[] SqlGetRaces()
        {
            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetRaces();
            }

            MySqlConnection connection = null;
            MySqlDataReader reader;
            Race[] races;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand("select count(*) from races;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                int numOfRaces = reader.GetInt32(0);

                reader.Close();

                command = new MySqlCommand("SELECT races.name, stats.bruteForce, stats.dexterity, stats.hp, stats.resistance, stats.agility FROM races inner join stats" +
                    " on races.stats_id = stats.id;", connection);

                reader = command.ExecuteReader();
                races = new Race[numOfRaces];

                for (int i = 0; i < numOfRaces; i += 1)
                {
                    reader.Read();
                    races[i] = new Race(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3),
                        reader.GetInt32(4), reader.GetInt32(5));
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetRaces();
            }
            return races;
        }
        private Race[] XmlGetRaces()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    StatModifier[] stats = xmlFile.Root.Element("stats").Elements("stat")
                        .Select(s => new StatModifier(
                            int.Parse(s.Element("bruteForce").Value),
                            int.Parse(s.Element("dexterity").Value),
                            int.Parse(s.Element("hp").Value),
                            int.Parse(s.Element("resistance").Value),
                            int.Parse(s.Element("agility").Value))).ToArray();

                    Race[] races = xmlFile.Root.Elements("races")
                        .Select(r =>
                            new Race(
                                r.Element("name").Value,
                                stats[int.Parse(r.Element("stats_id").Value) - 1].GetBruteForce(),
                                stats[int.Parse(r.Element("stats_id").Value) - 1].GetDexterity(),
                                stats[int.Parse(r.Element("stats_id").Value) - 1].GetHp(),
                                stats[int.Parse(r.Element("stats_id").Value) - 1].GetResistance(),
                                stats[int.Parse(r.Element("stats_id").Value) - 1].GetAgility()
                                )).ToArray();
                    return races;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
        private string[] SqlGetNames()
        {

            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetNames();
            }

            MySqlConnection connection = null;
            MySqlDataReader reader;
            string[] names;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand("select count(*) from names;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                int numOfNames = reader.GetInt32(0);
                reader.Close();

                names = new string[numOfNames];
                command = new MySqlCommand("SELECT name FROM names ;", connection);
                reader = command.ExecuteReader();

                for (int i = 0; i < numOfNames; i += 1)
                {
                    reader.Read();
                    names[i] = reader.GetString(0);
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetNames();
            }
            return names;
        }
        private string[] XmlGetNames()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    string[] names = xmlFile.Root.Elements("names").Select(n => n.Element("name").Value).ToArray();

                    return names;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
        private string[] SqlGetTitles()
        {
            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetTitles();
            }

            MySqlConnection connection = null;
            MySqlDataReader reader;
            string[] titles;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand("select count(*) from titles;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                int numOfNames = reader.GetInt32(0);
                reader.Close();

                titles = new string[numOfNames];
                command = new MySqlCommand("SELECT title FROM titles ;", connection);
                reader = command.ExecuteReader();

                for (int i = 0; i < numOfNames; i += 1)
                {
                    reader.Read();
                    titles[i] = reader.GetString(0);
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetTitles();
            }
            return titles;
        }
        private string[] XmlGetTitles()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    string[] titles = xmlFile.Root.Elements("titles").Select(n => n.Element("title").Value).ToArray();

                    return titles;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
        private string[] SqlGetBackgrounds()
        {
            string connectionString = mainWindow.GetSqlConnectionString();
            if (mainWindow.UseXml())
            {
                return XmlGetBackgrounds();
            }

            MySqlConnection connection = null;
            MySqlDataReader reader;
            string[] backgrounds;

            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
                MySqlCommand command = new MySqlCommand("select count(*) from backgrounds;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                int numOfNames = reader.GetInt32(0);
                reader.Close();

                backgrounds = new string[numOfNames];
                command = new MySqlCommand("SELECT background FROM backgrounds ;", connection);
                reader = command.ExecuteReader();
                for (int i = 0; i < numOfNames; i += 1)
                {
                    reader.Read();
                    backgrounds[i] = reader.GetString(0);
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close();
                return XmlGetBackgrounds();
            }
            return backgrounds;
        }
        private string[] XmlGetBackgrounds()
        {
            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    string[] backgrounds = xmlFile.Root.Elements("backgrounds").Select(n => n.Element("background").Value).ToArray();

                    return backgrounds;
                }
                else
                {
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                mainWindow.Close();
                return null;
            }
        }
    }
}
