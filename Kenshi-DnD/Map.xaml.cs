using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
        // Mainwindow
        MainWindow mainWindow;
        // Page controller
        ContentControl controller;
        // Region that is selected
        Region selectedRegion;
        // Rectangles that can be clicked to select a region
        Rectangle[] zones;
        // Current adventure
        Adventure adventure;
        // Cursors
        Cursor[] cursors;
        // Random
        Random rnd;
        // Rolls needed to not fight
        const int PEACE_ROLLS_NEEDED = 3;
        // Constructor
        public Map(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd, Adventure myAdventure)
        {
            InitializeComponent();

            this.mainWindow = mainWindow;
            this.controller = controller;
            this.cursors = cursors;
            this.rnd = rnd;
            this.adventure = myAdventure;

            // Fills an array of regions, and using the fact we know their position, saves the regions in the rectangle's tags
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
        // When a zone is selected, it will highlight it, and if it was already highlighted, tries to travel to that zone
        private void SelectZone(object sender, MouseButtonEventArgs e)
        {
            // Casting of region
            Rectangle rectangle = (Rectangle)sender;
            Region region = (Region)rectangle.Tag;
            // If the selectedRegion was highlighted, try to travel there
            if (region == selectedRegion)
            {
                // Sets current region to the selected
                adventure.SetCurrentRegion(region);
                // Affects the relations with factions from the region
                selectedRegion.AffectsRelations(adventure, rnd);

                // Player rolls average relations / 10 (1-10), and if is higher than 3, he avoids the encounter
                if (adventure.GetDice().PlayDice(selectedRegion.GetRelations() / 10,rnd) >= PEACE_ROLLS_NEEDED)
                {
                    // Travel there
                    controller.Content = new Zone(mainWindow, controller, cursors, rnd, adventure, selectedRegion);
                }
                else
                {
                    // Have combat, if successful, travel to the region, else, go back to Map.xaml
                    controller.Content = new CombatWindow(mainWindow, controller, cursors, rnd, adventure, 
                        mainWindow.SqlGenerateMonsters(adventure,SelectAggressor(adventure,rnd,selectedRegion),rnd));
                }
            }
            else
            {
                // Highlight the rectangle
                rectangle.Stroke = Brushes.Red;
                rectangle.StrokeThickness = 5;
                selectedRegion = region;
                // Un-highlight all the others
                UpdateZoneSelected();
            }

        }
        // Un-highlights non selected zones
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
        // If player encounters a fight, an agressor is chosen, if relations are lower than 50 and not enough successful rolls are gotten, that faction will fight the player
        // else, it will fight the most aggresive faction
        private Faction SelectAggressor(Adventure adventure,Random rnd,Region region)
        {
            List<Faction> factions = region.GetFactions();

            for (int i = 0; i < factions.Count; i++)
            {
                if (factions[i].GetRelation() < 50)
                {
                    if (adventure.GetDice().PlayDice(factions[i].GetRelation() / 10, rnd) < PEACE_ROLLS_NEEDED)
                    {
                        return factions[i];
                    }
                }
            }
            Faction mostHostileFaction = factions[0];
            for (int i = 1; i < factions.Count; i++)
            {
                if (factions[i].GetRelation() < mostHostileFaction.GetRelation())
                {
                    mostHostileFaction = factions[i];
                }
            }
            return mostHostileFaction;
        }
        // Exits current adventure and stops the timer
        private void ExitGame(object sender, RoutedEventArgs e)
        {

            if(MessageBox.Show("Estás a punto de salir del juego.\nPerderás el progreso no guardado", "Salir del juego", MessageBoxButton.YesNo, MessageBoxImage.Exclamation)
                == MessageBoxResult.No)
            {
                return;
            }
            mainWindow.StopPlaying();
            controller.Content = new Menu(mainWindow, controller, cursors, rnd);
        }
        // Saves current adventure
        private void SaveAdventure(object sender, EventArgs e)
        {
            mainWindow.SaveAdventure(adventure);
        }
    }
}
