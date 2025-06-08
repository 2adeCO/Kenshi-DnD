
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Kenshi_DnD
{
    // Only window in the program - Santiago Cabrero
    public partial class MainWindow : Window
    {
        // Array of cursors used in the program
        Cursor[] cursors;
        // New random created when the window initializes
        Random rnd;
        // Timer used to add seconds to adventure's hoursPlayed
        DispatcherTimer timer;
        // Tells the program if it should use XML instead of MySql
        bool usingXML;
        const string SQL_CONFIG_PATH = "./Resources/config/sqlconfig.txt";
        // Constructor
        public MainWindow()
        {
            InitializeComponent();
            // Loads cursors
            LoadCursors();
            // Makes the window's default cursor the first one
            this.Cursor = cursors[0];
            usingXML = false;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            rnd = new Random();
            // Sets PageController to Menu UserControl
            PageController.Content = new Menu(this,PageController, cursors, rnd);
        }
        // Adds a second to the adventure
        private void AddSecondsToAdventureTime(Adventure adventure)
        {
            adventure.AddSecondToAdventure();
        }
        // Starts the timer
        public void StartPlaying(Adventure adventure)
        {
            timer.Start();
            timer.Tick += (s, e) =>
            {
                AddSecondsToAdventureTime(adventure);
            };
        }
        // Stops the timer
        public void StopPlaying()
        {
            timer.Stop();
            timer.Tick -= (s, e) =>
            {
                AddSecondsToAdventureTime(null);
            };
        }
        // Loads cursors in the array
        public void LoadCursors()
        {
            string[] cursorCurFiles = Directory.GetFiles("./Resources/cursors", "*.cur");
            string[] cursorAniFiles = Directory.GetFiles("./Resources/cursors", "*.ani");
            int arrayNum = cursorCurFiles.Length + cursorAniFiles.Length;
            string[] cursorFiles = new string[arrayNum];

            for (int i = 0; i < cursorCurFiles.Length; i+=1)
            {
                cursorFiles[i] = cursorCurFiles[i];
            }
            for (int i = 0; i < cursorAniFiles.Length; i+=1)
            {
                cursorFiles[i + cursorCurFiles.Length] = cursorAniFiles[i];
            }


            cursors = new Cursor[arrayNum];
            for (int i = 0; i < arrayNum; i+=1)
            {
                cursors[i] = new Cursor(cursorFiles[i]);
            }
        }
        // Decorates the text by parsing strings into inlines with colors and text size changed
        public List<Inline> DecorateText(string message)
        {
            string txt = "";
            List<Inline> inlines = new List<Inline>();
            Run inlineRun;
            bool foundDecoration = false;
            bool startDecoration = false;
            int size = 0;
            int color = 0;

            for (int i = 0; i < message.Length; i+=1)
            {
                // If it's normal text, just write it normally
                if (!foundDecoration && ! startDecoration)
                {
                    // If the special character is found, then save the bool
                    if (message[i] == '@')
                    {
                        foundDecoration = true;
                    }
                    else
                    {
                        // Write it
                        txt += message[i];
                    }
                }
                else
                {
                    // Because the decoration has been found, see what to do next

                    // If just found the decoration, then read it
                    if (!startDecoration)
                    {
                        foundDecoration = false;
                        startDecoration = true;
                        color = int.Parse(message[i].ToString());
                        inlineRun = new Run();
                        inlineRun.Text = txt;
                        inlines.Add(inlineRun);
                        txt = "";
                        i += 1;
                        while (message[i] != '@')
                        {
                            txt += message[i];
                            i += 1;
                        }
                        if (txt != "")
                        {
                            size = int.Parse(txt);
                        }
                        txt = "";
                    }
                    else
                    {
                        // If the decoration is known, then, start it until the special character is found again
                        if (message[i] != '@')
                        { 
                            txt += message[i];
                        }
                        else
                        {
                            startDecoration = false;

                            inlineRun = new Run();
                            inlineRun.Text = txt;
                            inlineRun.FontWeight = FontWeights.Bold;
                            inlineRun.Foreground = GetBrushByNum(color);
                            if(size != 0)
                            {
                                inlineRun.FontSize = size;
                            }

                            inlines.Add(inlineRun);

                            txt = "";
                            size = 0;
                        }
                    }
                    
                }    
            }
            // If some text at the end of the loop and not special, these lines of code will save them
            inlineRun = new Run();
            inlineRun.Text = txt;
            inlines.Add(inlineRun);
            return inlines;
        }
        // Returns a brush used by the program to color text and UI by parsing a num
        public SolidColorBrush GetBrushByNum(int num)
        {
            switch (num)
            {
                case 1:
                    {
                        return Brushes.Red;
                    }
                case 2:
                    {
                        return Brushes.Green;
                    }
                case 3:
                    {
                        //Normal blue is really hard to read
                        return Brushes.CornflowerBlue;
                    }
                case 4:
                    {
                        return Brushes.Purple;
                    }
                case 5:
                    {
                        return Brushes.Goldenrod;
                    }
                case 6:
                    {
                        return Brushes.Orange;
                    }
                case 7:
                    {
                        return Brushes.LightSlateGray;
                    }
                case 8:
                    {
                        return Brushes.Gray;
                    }
                default:
                    {
                        return Brushes.Black;
                    }
            }
        }
        // Generates a standard tooltip throughout the program, with a header
        public ToolTip HeaderToolTipThemer(string header, string content)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            TextBlock textBlock = new TextBlock();

            textBlock.Inlines.AddRange(DecorateText(header));
            textBlock.FontSize = 18;
            stackPanel.Children.Add(textBlock);
            textBlock = new TextBlock();
            textBlock.Inlines.AddRange(DecorateText(content));
            textBlock.FontSize = 14;
            stackPanel.Children.Add(textBlock);

            ToolTip toolTip = new ToolTip();
            toolTip.Content = stackPanel;
            toolTip.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e6e5d5"));
            toolTip.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2b2b2b"));
            toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
            return toolTip;

        }
        // Generates a standard tooltip throughout the program, with a header

        public ToolTip ToolTipThemer(string content)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(DecorateText(content));
            textBlock.FontSize = 18;
            ToolTip toolTip = new ToolTip();
            toolTip.Content = textBlock;
            toolTip.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e6e5d5"));
            toolTip.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2b2b2b"));
            toolTip.Placement = System.Windows.Controls.Primitives.PlacementMode.Top;
            return toolTip;
        }
        // Searches the config file for a valid SQL connection string
        public string GetSqlConnectionString()
        {
            try
            {
                if (Path.Exists(SQL_CONFIG_PATH))
                {
                    StreamReader sr = new StreamReader(SQL_CONFIG_PATH);
                    string mySqlConnectionString = sr.ReadLine();
                    while (mySqlConnectionString[0] == '#' && !String.IsNullOrEmpty(mySqlConnectionString))
                    {
                        mySqlConnectionString = sr.ReadLine();
                    }
                    sr.Close();
                    if (String.IsNullOrEmpty(mySqlConnectionString))
                    {
                        // Throws a exception
                        throw new DBNotFoundException();
                    }
                    return mySqlConnectionString;
                }
                else
                {
                    // Throws a exception
                    throw new DBNotFoundException();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                usingXML = true;
                return "";
            }
        }
        // Returns if the program should use XML
        public bool UseXml()
        {
            return usingXML;
        }
        // Saves the adventure
        public void SaveAdventure(Adventure adventure)
        {

#pragma warning disable SYSLIB0011
            FileStream fileStream = new FileStream("./saves/" + adventure.GetId() + ".adventure", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fileStream, adventure);
#pragma warning restore SYSLIB0011
            fileStream.Close();
            MessageBox.Show("Partida guardada exitosamente");
        }
        // Generates random monsters that are in a faction, using SQL
        public Monster[] SqlGenerateMonsters(Adventure adventure, Faction faction, Random rnd)
        {

            string connectionString = GetSqlConnectionString();
            if (UseXml())
            {
                return XmlGenerateMonsters(adventure, faction, rnd);
            }
            MySqlConnection connection = null;
            try
            {
                connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand("SELECT count(*) " +
                    "FROM enemies e " +
                    "WHERE e.factionId = \"" + faction.GetFactionId() + "\"", connection);
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                reader.Read();
                Monster[] monsters = new Monster[reader.GetInt32(0)];
                reader.Close();
                command = new MySqlCommand(
                    "SELECT e.name,e.health,e.factionId,e.strength,e.resistance,e.agility,e.immunity,e.maxCatDrop,e.xp,e.canDropItem " +
                    "FROM enemies e " +
                    "WHERE e.factionId = \"" + faction.GetFactionId() + "\"", connection);
                reader = command.ExecuteReader();
                for (int i = 0; i < monsters.Length; i++)
                {
                    reader.Read();
                    string name = reader.GetString(0);
                    int hp = reader.GetInt32(1);
                    int factionId = reader.GetInt32(2);
                    int strength = reader.GetInt32(3);
                    int resistance = reader.GetInt32(4);
                    int agility = reader.GetInt32(5);
                    string immunity = reader.GetString(6);
                    int cats = reader.GetInt32(7);
                    int xpDrop = reader.GetInt32(8);
                    bool canDropItem = reader.GetBoolean(9);

                    monsters[i] = new Monster(name, hp, adventure.GetAllFactions()[factionId - 1], strength, resistance, agility, immunity, cats, xpDrop, canDropItem);
                }
                reader.Close();
                connection.Close();
                int numMonsters = rnd.Next(1, 5);
                Monster[] monstersToReturn = new Monster[numMonsters];
                for (int i = 0; i < numMonsters; i++)
                {
                    Monster monster = monsters[rnd.Next(0, monsters.Length)];
                    monstersToReturn[i] = monster.GetCopy();
                }
                return monstersToReturn;
            }
            catch(MySqlException ex)
            {
                connection.Close();
                return XmlGenerateMonsters(adventure, faction, rnd);
            }
            
        }
        // Generates random monsters that are in a faction, using XML
        public Monster[] XmlGenerateMonsters(Adventure adventure, Faction faction, Random rnd)
        {

            try
            {
                if (File.Exists("./Resources/xml/kenshidata.xml"))
                {
                    XDocument xmlFile = XDocument.Load("./Resources/xml/kenshidata.xml");

                    Monster[] monsterFromFaction = xmlFile.Root.Element("enemies").Elements("enemy").Where(e =>
                    int.Parse(e.Element("factionId").Value) == faction.GetFactionId()).Select(m => new Monster(
                    m.Element("name").Value, int.Parse(m.Element("health").Value), faction,
                    int.Parse(m.Element("strength").Value), int.Parse(m.Element("resistance").Value), int.Parse(m.Element("agility").Value),
                    m.Element("immunity").Value, int.Parse(m.Element("maxCatDrop").Value), int.Parse(m.Element("xp").Value),
                    bool.Parse(m.Element("canDropItem").Value)
                    )).ToArray();

                    int numMonsters = rnd.Next(1, 5);
                    Monster[] monstersToReturn = new Monster[numMonsters];
                    for (int i = 0; i < numMonsters; i++)
                    {
                        Monster monster = monsterFromFaction[rnd.Next(0, monsterFromFaction.Length)];
                        monstersToReturn[i] = monster.GetCopy();
                    }
                    return monstersToReturn;
                }
                else
                {
                    // Throws an exception
                    throw new XMLNotFoundException();
                }
            }
            catch (XMLNotFoundException xmlError)
            {
                MessageBox.Show(xmlError.Message);
                this.Close();
                return null;
            }
        }
        // Returns a textblock with the text formatted
        public TextBlock GenerateTextblock(string content)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(DecorateText(content));
            return textBlock;
        }
    }
}