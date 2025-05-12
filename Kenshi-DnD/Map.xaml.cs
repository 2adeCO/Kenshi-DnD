using System;
using System.Collections.Generic;
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
            
            NorthZone.Tag = regions[0];
            HolyNationZone.Tag = regions[1];
            UnitedCitiesZone.Tag = regions[2];
            WestHiveZone.Tag = regions[3];
            ShekKingdomZone.Tag = regions[4];
            SwampZone.Tag = regions[5];
            UpdateZoneSelected();

        }
        private void SelectZone(object sender, MouseButtonEventArgs e)
        {
            UpdateZoneSelected();
            Rectangle rectangle = (Rectangle)sender;
            rectangle.Stroke = Brushes.Red;
            rectangle.StrokeThickness = 5;
            rectangle.MouseLeftButtonDown -= SelectZone;
            rectangle.MouseLeftButtonDown += GoToZone;
            Region region = (Region)rectangle.Tag;

            selectedRegion = region;

        }
        private void UpdateZoneSelected()
        {
            NorthZone.MouseLeftButtonDown -= GoToZone;
            NorthZone.MouseLeftButtonDown += SelectZone;
            NorthZone.Stroke = Brushes.Black;
            NorthZone.StrokeThickness = 1;
            HolyNationZone.MouseLeftButtonDown -= GoToZone;
            HolyNationZone.MouseLeftButtonDown += SelectZone;
            HolyNationZone.Stroke = Brushes.Black;
            HolyNationZone.StrokeThickness = 1;
            UnitedCitiesZone.MouseLeftButtonDown -= GoToZone;
            UnitedCitiesZone.MouseLeftButtonDown += SelectZone;
            UnitedCitiesZone.Stroke = Brushes.Black;
            UnitedCitiesZone.StrokeThickness = 1;
            WestHiveZone.MouseLeftButtonDown -= GoToZone;
            WestHiveZone.MouseLeftButtonDown += SelectZone;
            WestHiveZone.Stroke = Brushes.Black;
            WestHiveZone.StrokeThickness = 1;
            ShekKingdomZone.MouseLeftButtonDown -= GoToZone;
            ShekKingdomZone.MouseLeftButtonDown += SelectZone;
            ShekKingdomZone.Stroke = Brushes.Black;
            ShekKingdomZone.StrokeThickness = 1;
            SwampZone.MouseLeftButtonDown -= GoToZone;
            SwampZone.MouseLeftButtonDown += SelectZone;
            SwampZone.Stroke = Brushes.Black;
            SwampZone.StrokeThickness = 1;
        }
        private void GoToZone(object sender, EventArgs e)
        {
            Rectangle rectangle = (Rectangle)sender;
            Region region = (Region)rectangle.Tag;


        }
    }
}
