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
using MySql.Data.MySqlClient;
using System.Data;

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
        public Menu(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd)
        {
            changingSliders = true;
            InitializeComponent();
            this.allRaces = SqlGetRaces();
            this.cursors = cursors;
            this.controller = controller;
            this.mainWindow = mainWindow;
            this.rnd = rnd;
            TitleText.Inlines.Clear();
            TitleText.Inlines.AddRange(mainWindow.DecorateText("@134@KENSHI_DND@\n@8@Por@ @7@Santiago Cabrero@"));
            remainingPoints = DEFAULT_POINTS_ON_HERO_MAKER - 4 ;
            remainingPointsText.Text = remainingPoints.ToString();


            SqlConnectionTest();
            SqlGetFactions();
            SqlGetItems();

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
            characterSubrace.SelectedItem = "Greenlander";

            bruteForceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            dexteritySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            resistanceSlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            agilitySlider.Maximum = DEFAULT_POINTS_ON_HERO_MAKER - 3;
            

            IsAdventureValid(null, null);
        }
        public void GoToCombat(object sender, EventArgs e)
        {

            controller.Content = new CombatWindow(mainWindow, cursors, rnd, myAdventure, null);
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
        
        private void PlayAdventure(object sender, EventArgs e) 
        {
            Adventure adventure = (Adventure)((Button)sender).Tag;

            if (adventure != null)
            {
                controller.Content = new Map(mainWindow, controller,cursors, rnd, adventure);
            }
            else
            {
                MessageBox.Show("Algo ha ido terriblemente mal.");
            }
        }
        private void DeleteAdventureWarning(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Adventure adventure = (Adventure)button.Tag;
            string adventurePath = "./saves/" + adventure.GetId() + ".adventure";
            if (File.Exists(adventurePath))
            {
                MessageBox.Show("¿Quieres eliminar la aventura " + adventure.GetId() + "? Pulsa de nuevo para eliminarla.");
                button.Click -= DeleteAdventureWarning;
                button.Click += DeleteAdventure;
            } else
            {
                MessageBox.Show("No se ha encontrado la aventura " + adventure.GetId() + ", no se puede eliminar.");
            }
        }
        private void DeleteAdventure(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Adventure adventure = (Adventure)button.Tag;
            string adventurePath = "./saves/" + adventure.GetId() + ".adventure";
            if (File.Exists(adventurePath))
            {
                File.Delete(adventurePath);
                AdventureChooserMenu.Children.Remove((Grid)button.Parent);
                MessageBox.Show("Aventura eliminada.");
            }
            else
            {
                MessageBox.Show("No se ha encontrado la aventura " + adventure.GetId() + ", no se puede eliminar.");
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
    
    
        private void ChangeSubraces(object sender, EventArgs e)
        {
            string race = (string)characterRace.SelectedItem;
            characterSubrace.ItemsSource = RacesDictionary.RacesAndSubraces[race];
            characterSubrace.SelectedIndex = 0;
            UpdateHeroStatGrid(null, null);
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
        private void MakeAdventure(object sender, EventArgs e)
        {
            if (currentAdventureIsValid)
            {


                Faction[] factions = SqlGetFactions();
                Region[] regions = SqlGetRegions();
                Limb[] limbs = SqlGetLimbs();
                Item[] items = SqlGetItems();
                Monster[] enemies = SqlGetEnemies();
                string[] titles = SqlGetTitles();
                string[] backgrounds = SqlGetBackgrounds();
                string[] names = SqlGetNames();




                Hero hero = new Hero((int)bruteForceSlider.Value, (int)dexteritySlider.Value, (int)resistanceSlider.Value, (int)agilitySlider.Value,
                    GetSelectedRace(), GetSeletedSubrace(), GenerateLimbs());

                Dice myDice = new Dice(int.Parse(diceSides.Text), int.Parse(diceMinWin.Text));
                Adventure myAdventure = new Adventure(adventureName.Text, hero, rnd, myDice, int.Parse(startingCats.Text)
                    ,factions,regions,enemies,titles,backgrounds,names,items,allRaces,limbs);

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
                    CloseCurrentMenu(null, null);
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
                limbs[i] = new Limb("Extremidad normal", 0, 0, 0, 0, 0);
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
            button.Content = adventure.GetId() + "\nDado: " + adventure.GetDice().ToString() + "\nCats: " + adventure.GetCats() + "$";
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
        private void SqlConnectionTest()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("SELECT * FROM factions;", connection);
            MySqlDataReader reader;


            try
            {
                connection.Open();
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Debug.WriteLine(reader.GetString(1) + " " + reader.GetString(2));
                }
                reader.Close();

            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySql wanted to crash");
                connection.Close();
            }
            connection.Close();

        }
        private Faction[] SqlGetFactions()
        {

            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("SELECT count(*) FROM factions;", connection);
            MySqlDataReader reader;
            int numberOfFactions = 0;
            Faction[] factions = null;

            try
            {
                connection.Open();
                reader = command.ExecuteReader();
                reader.Read();
                numberOfFactions = reader.GetInt32(0);
                Debug.WriteLine("Number of factions: " + numberOfFactions);
                reader.Close();
            
            
                factions = new Faction[numberOfFactions];
                command = new MySqlCommand("SELECT * FROM factions;", connection);
                reader = command.ExecuteReader();

                for(int i = 0; i < numberOfFactions; i += 1)
                {
                    reader.Read();
                    factions[i] = new Faction(reader.GetInt32(0), reader.GetString(1), reader.GetString(2),reader.GetInt32(3), reader.GetInt32(4));
                }
                reader.Close();

                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySql wanted to crash");
                connection.Close();
                return null;
            }
            return factions;
        }
        private Item[] SqlGetItems()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command;
            MySqlDataReader reader;
            int numberOfItems = 0;

            try
            {
                // Abre la conexión
                connection.Open();

                // Obtener la cantidad de items
                command = new MySqlCommand("SELECT count(*) FROM items;", connection);
                reader = command.ExecuteReader();
                reader.Read();
                numberOfItems = reader.GetInt32(0);
                Debug.WriteLine("Number of items: " + numberOfItems);
                reader.Close(); // Cierra el reader

                // Crear el arreglo de items
                Item[] items = new Item[numberOfItems];
                int iteration = 0;

                // Obtener MeleeItems
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
                reader.Close(); // Cierra el reader después de leer MeleeItems

                // Obtener RangedItems
                command = new MySqlCommand("SELECT i.name, i.description, i.value, i.resellValue, i.weight, ri.ammo, ri.difficulty, stats.bruteForce, " +
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
                reader.Close(); // Cierra el reader después de leer RangedItems

                // Cierra la conexión al final
                connection.Close();

                return items;
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
        }
        private Limb[] SqlGetLimbs()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from limbs;",connection);
            MySqlDataReader reader;
            Limb[] limbs;

            try
            {
                
                connection.Open();
                reader = command.ExecuteReader();
                reader.Read();
                int numOfLimbs = reader.GetInt32(0);

                reader.Close();

                command = new MySqlCommand("SELECT limbs.name, stats.bruteForce, stats.dexterity, stats.hp, stats.resistance, stats.agility FROM limbs inner join stats" +
                    " on limbs.stats_id = stats.id;", connection);

                reader = command.ExecuteReader();
                limbs = new Limb[numOfLimbs];

                for(int i = 0; i < numOfLimbs; i += 1)
                {
                    reader.Read();
                    limbs[i] = new Limb(reader.GetString(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3),
                        reader.GetInt32(4), reader.GetInt32(5));
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return limbs;
        }
        private Region[] SqlGetRegions()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from regions;", connection);
            MySqlDataReader reader;
            Region[] regions;
            Faction[] factions = SqlGetFactions();
            try
            {
                connection.Open();

                reader = command.ExecuteReader();

                reader.Read();
                int numOfRegions = reader.GetInt32(0);
                regions = new Region[numOfRegions];
                reader.Close();

                command = new MySqlCommand("select * from regions", connection);
                for (int i = 0; i < numOfRegions; i += 1)
                {
                    reader.Read();
                    regions[i] = new Region(reader.GetString(1), reader.GetString(2));
                }
                reader.Close();

                command = new MySqlCommand("select regions.id, factions.id from regions inner join region_faction on regions.id = region_faction.RegionId " +
                    "inner join factions on factions.id = region_faction.factionId;", connection);
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    int regionId = reader.GetInt32(0);
                    int factionId = reader.GetInt32(1);

                    regions[regionId].AddFaction(factions[factionId]);
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return regions;

        }
        private Race[] SqlGetRaces()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from races;", connection);
            MySqlDataReader reader;
            Race[] races;

            try
            {

                connection.Open();
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
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return races;
        }
        private Monster[] SqlGetEnemies()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from enemies;", connection);
            MySqlDataReader reader;
            Faction[] factions = SqlGetFactions();
            Monster[] enemies;

            try
            {

                connection.Open();
                reader = command.ExecuteReader();
                reader.Read();
                int numOfEnemies = reader.GetInt32(0);

                reader.Close();

                command = new MySqlCommand("SELECT name, health, factionId, strength, resistance,agility, immunity,xp, maxCatDrop, canDropItem " +
                    "FROM enemies;", connection);

                reader = command.ExecuteReader();
                enemies = new Monster[numOfEnemies];

                for (int i = 0; i < numOfEnemies; i += 1)
                {
                    reader.Read();
                    enemies[i] = new Monster(reader.GetString(0), reader.GetInt32(1), factions[reader.GetInt32(2)],reader.GetInt32(3),
                        reader.GetInt32(4),reader.GetInt32(5),reader.GetString(6),reader.GetInt32(7),reader.GetInt32(8),reader.GetBoolean(9)) ;
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Debug.WriteLine("MySQL error: " + ex.Message);
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return enemies;
        }
        private string[] SqlGetNames()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from names;", connection);
            MySqlDataReader reader;
            string[] names;

            try
            {
                connection.Open();
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
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return names;
        }
        private string[] SqlGetTitles()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from titles;", connection);
            MySqlDataReader reader;
            string[] titles;

            try
            {
                connection.Open();
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
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return titles;
        }
        private string[] SqlGetBackgrounds()
        {
            MySqlConnection connection = new MySqlConnection("server=localhost;user=root;database=kenshi_dnd_db;port=3306;password=root");
            MySqlCommand command = new MySqlCommand("select count(*) from backgrounds;", connection);
            MySqlDataReader reader;
            string[] backgrounds;

            try
            {
                connection.Open();
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
                connection.Close(); // Cierra la conexión en caso de error
                return null; // En caso de error, retorna null
            }
            return backgrounds;
        }
    }
}
