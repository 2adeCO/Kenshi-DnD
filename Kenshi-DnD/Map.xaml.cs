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
            HolyNationZone.Tag = regions[1];
            zones[1] = HolyNationZone;
            UnitedCitiesZone.Tag = regions[2];
            zones[2] = UnitedCitiesZone;
            WestHiveZone.Tag = regions[3];
            zones[3] = WestHiveZone;
            ShekKingdomZone.Tag = regions[4];
            zones[4] = ShekKingdomZone;
            SwampZone.Tag = regions[5];
            zones[5] = SwampZone;
            

        }
        private void SelectZone(object sender, MouseButtonEventArgs e)
        {
            
            Debug.WriteLine("Tried to select zone");
            Rectangle rectangle = (Rectangle)sender;
            Region region = (Region)rectangle.Tag;
            if (region == selectedRegion)
            {
                controller.Content = new Zone(mainWindow,controller,cursors,rnd,adventure,selectedRegion);
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
    }
}
