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
        MainWindow mainWindow;
        ContentControl controller;
        Region selectedRegion;
        Rectangle[] zones;
        Adventure adventure;
        Cursor[] cursors;
        Random rnd;

        const int PEACE_ROLLS_NEEDED = 3;
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
                adventure.SetCurrentRegion(region);
                selectedRegion.AffectsRelations(adventure, rnd);
                // Player rolls average relations / 10 (1-10), and if is higher than 3, he avoids the encounter
                if (adventure.GetDice().PlayDice(selectedRegion.GetRelations() / 10,rnd) >= PEACE_ROLLS_NEEDED)
                {
                    controller.Content = new Zone(mainWindow, controller, cursors, rnd, adventure, selectedRegion);
                }
                else
                {
                    controller.Content = new CombatWindow(mainWindow, controller, cursors, rnd, adventure, 
                        mainWindow.GenerateMonsters(adventure,SelectAggressor(adventure,rnd,selectedRegion),rnd));
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
        private void ExitGame(object sender, RoutedEventArgs e)
        {

            if(MessageBox.Show("Estás a punto de salir del juego.\nPerderás el progreso no guardado", "Salir del juego", MessageBoxButton.YesNo, MessageBoxImage.Exclamation) == MessageBoxResult.No)
            {
                return;
            }
            mainWindow.StopPlaying();
            controller.Content = new Menu(mainWindow, controller, cursors, rnd);
        }
        private void SaveAdventure(object sender, EventArgs e)
        {
            mainWindow.SaveAdventure(adventure);
        }
    }
}
