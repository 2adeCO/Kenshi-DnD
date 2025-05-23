using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        MainWindow mainWindow;
        ContentControl controller;
        Region selectedRegion;
        Rectangle[] zones;
        Adventure adventure;
        Cursor[] cursors;
        Random rnd;

        public Map(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd, Adventure myAdventure)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            this.controller = controller;
            this.cursors = cursors;
            this.rnd = rnd;
            this.adventure = myAdventure;


            Region[] regions = adventure.GetAllRegions();
            zones = new Rectangle[regions.Length];

            NorthZone.Tag = regions[0];
            zones[0] = NorthZone;
            NorthZone.ToolTip = mainWindow.ToolTipThemer(regions[0].ToString());

            HolyNationZone.Tag = regions[1];
            zones[1] = HolyNationZone;
            HolyNationZone.ToolTip = mainWindow.ToolTipThemer(regions[1].ToString());

            UnitedCitiesZone.Tag = regions[2];
            zones[2] = UnitedCitiesZone;
            UnitedCitiesZone.ToolTip = mainWindow.ToolTipThemer(regions[2].ToString());

            WestHiveZone.Tag = regions[3];
            zones[3] = WestHiveZone;
            WestHiveZone.ToolTip = mainWindow.ToolTipThemer(regions[3].ToString());

            ShekKingdomZone.Tag = regions[4];
            zones[4] = ShekKingdomZone;
            ShekKingdomZone.ToolTip = mainWindow.ToolTipThemer(regions[4].ToString());

            SwampZone.Tag = regions[5];
            zones[5] = SwampZone;
            SwampZone.ToolTip = mainWindow.ToolTipThemer(regions[5].ToString());


        }
        private void SelectZone(object sender, MouseButtonEventArgs e)
        {
            
            Debug.WriteLine("Tried to select zone");
            Rectangle rectangle = (Rectangle)sender;
            Region region = (Region)rectangle.Tag;
            if (region == selectedRegion)
            {
                selectedRegion.AffectsRelations(adventure);
                // Player rolls average relations / 10 (1-10), and if is higher than 3, he avoids the encounter
                if (adventure.GetDice().PlayDice(selectedRegion.GetRelations() / 10,rnd) > 3)
                {
                    controller.Content = new Zone(mainWindow, controller, cursors, rnd, adventure, selectedRegion);
                }
                else
                {
                    controller.Content = new CombatWindow(mainWindow, controller, cursors, rnd, adventure, GenerateMonsters(selectedRegion,rnd));
                }
            }
            else
            {
                rectangle.Stroke = Brushes.Red;
                rectangle.StrokeThickness = 5;
                selectedRegion = region;
                UpdateZoneSelected();
            }

        }
        private void UpdateZoneSelected()
        {
            for (int i = 0; i < zones.Length; i++)
            {
                if (zones[i].Tag != selectedRegion)
                {
                    zones[i].Stroke = Brushes.Black;
                    zones[i].StrokeThickness = 1;
                }
            }
        }
        private Monster[] GenerateMonsters(Region region, Random rnd)
        {
            MySqlConnection connection = new MySqlConnection(mainWindow.GetSqlConnectionString());
            MySqlCommand command = new MySqlCommand("SELECT count(*) " +
                "FROM enemies e JOIN factions f ON e.factionId = f.id JOIN region_faction rf ON f.id = rf.factionId JOIN regions r ON rf.regionId = r.id " +
                "WHERE r.name = \"" + region.GetName() + "\"", connection);
            connection.Open();
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            Monster[] monsters = new Monster[reader.GetInt32(0)];
            reader.Close();
            command = new MySqlCommand(
                "SELECT e.name,e.health,e.factionId,e.strength,e.resistance,e.agility,e.immunity,e.maxCatDrop,e.xp,e.canDropItem,r.id " +
                "FROM enemies e JOIN factions f ON e.factionId = f.id JOIN region_faction rf ON f.id = rf.factionId JOIN regions r ON rf.regionId = r.id " +
                "WHERE r.name = \"" + region.GetName() + "\"", connection);
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
                int regionId = reader.GetInt32(10);

                monsters[i] = new Monster(name, hp, adventure.GetAllFactions()[factionId-1], strength, resistance, agility, immunity, cats, xpDrop, canDropItem);
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
