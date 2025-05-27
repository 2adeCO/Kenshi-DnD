
using System.Diagnostics;
using System.IO;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Schema;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Threading;
namespace Kenshi_DnD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
public partial class MainWindow : Window
    {
        Cursor[] cursors;
        Random rnd;
        DispatcherTimer timer;
        public MainWindow()
        {
            InitializeComponent();
            LoadCursors();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            rnd = new Random();
            
            PageController.Content = new Menu(this,PageController, cursors, rnd);
        }
        private void AddSecondsToAdventureTime(Adventure adventure)
        {
            adventure.AddSecondToAdventure();
        }
        public void StartPlaying(Adventure adventure)
        {
            timer.Start();
            timer.Tick += (s, e) =>
            {
                AddSecondsToAdventureTime(adventure);
            };
        }
        public void StopPlaying()
        {
            timer.Stop();
            timer.Tick -= (s, e) =>
            {
                AddSecondsToAdventureTime(null);
            };
        }
        public void LoadCursors()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());
            string[] cursorCurFiles = Directory.GetFiles("./Resources/cursors", "*.cur");
            string[] cursorAniFiles = Directory.GetFiles("./Resources/cursors", "*.ani");
            int arrayNum = cursorCurFiles.Length + cursorAniFiles.Length;
            string[] cursorFiles = new string[arrayNum];

            Debug.WriteLine($"Loading {arrayNum} cursors...");
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
                Debug.WriteLine($"Loading cursor: {cursorFiles[i]}");
                cursors[i] = new Cursor(cursorFiles[i]);
            }
        }
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
                //If it's normal text, just write it normally
                if (!foundDecoration && ! startDecoration)
                {
                    //If the special character is found, then save the bool
                    if (message[i] == '@')
                    {
                        foundDecoration = true;
                    }
                    else
                    {
                        //Write it
                        txt += message[i];
                    }
                }
                else
                {
                    //Because the decoration has been found, see what to do next

                    //If just found the decoration, then read it
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
                        //If the decoration is known, then, start it until the special character is found again
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
            //If some text at the end of the loop and not special, these lines of code will save them
            inlineRun = new Run();
            inlineRun.Text = txt;
            inlines.Add(inlineRun);
            return inlines;
        }
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
        public string GetSqlConnectionString()
        {
            try
            {
                if (Path.Exists("./Resources/config/sqlconfig.txt"))
                {
                    string[] lines = File.ReadAllLines("./Resources/config/sqlconfig.txt");
                    StreamReader sr = new StreamReader("./Resources/config/sqlconfig.txt");
                    string mySqlConnectionString = sr.ReadLine();
                    while (mySqlConnectionString[0] == '#')
                    {
                        mySqlConnectionString = sr.ReadLine();
                    }
                    sr.Close();
                    return mySqlConnectionString;
                }
                else
                {
                    throw new Exception("File not found");
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
                MessageBox.Show("La base de datos no está bien configuarada. El programa fallará si intentas guardar una nueva partida.");
                return "";
            }
            
            
        }
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
        public Monster[] GenerateMonsters(Adventure adventure,Faction faction, Random rnd)
        {
            MySqlConnection connection = new MySqlConnection(GetSqlConnectionString());
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
    }
}