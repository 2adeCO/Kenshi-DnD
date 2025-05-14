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
    /// Lógica de interacción para Zone.xaml
    /// </summary>
    public partial class Zone : UserControl
    {
        MainWindow mainWindow;
        ContentControl controller;
        Cursor[] cursors;
        Random rnd;
        Adventure myAdventure;
        Region region;

        Hero[] heroesInBar;
        Hero selectedHero;
        public Zone(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd, Adventure myAdventure,Region currentRegion)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.controller = controller;
            this.cursors = cursors;
            this.rnd = rnd;
            this.myAdventure = myAdventure;
            this.region = currentRegion;


            CreateActions();




        }
        private void CreateActions()
        {
            if (region.HasBar())
            {
                Button button = new Button();
                button.Content = "Ir al bar";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += GoToBar;
                heroesInBar = region.GoToBar(myAdventure, rnd);
                ActionsPanel.Children.Add(button);

            }
            if (region.HasShop())
            {
                Button button = new Button();
                button.Content = "Ir a la tienda";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;

                ActionsPanel.Children.Add(button);

            }
            if (region.HasContrabandMarket())
            {
                Button button = new Button();
                button.Content = "Ir al mercado clandestino";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;

                ActionsPanel.Children.Add(button);

            }
            if (region.HasLimbHospital())
            {
                Button button = new Button();
                button.Content = "Ir al mecánico de extremidades";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;

                ActionsPanel.Children.Add(button);

            }
            if (region.HasRangedShop())
            {
                Button button = new Button();
                button.Content = "Ir al campo de tiro";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;

                ActionsPanel.Children.Add(button);

            }

           
        }
        private void GoToBar(object sender, EventArgs e)
        {
            CloseCurrentGrid(null,null);
            BarGrid.Visibility = Visibility.Visible;

            if(heroesInBar != null)
            {
                for (int i = 0; i < heroesInBar.Length; i++)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Auto);
                    BarItems.RowDefinitions.Add(row);

                    Button button = new Button();
                    button.Tag = heroesInBar[i];
                    button.Content = heroesInBar[i].GetNameAndTitle();
                    button.Margin = new Thickness(4, 10, 4, 10);
                    button.ToolTip = mainWindow.ToolTipThemer(heroesInBar[i].GetBackgroundStory());
                    button.Click += SelectHero;


                    Grid.SetRow(button, i);
                    BarItems.Children.Add(button);
                }
            }
            else
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                BarItems.RowDefinitions.Add(row);

                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.AddRange(mainWindow.DecorateText("No hay @916@nadie@ aquí..."));

                Grid.SetRow(textBlock, 0);
                BarItems.Children.Add(textBlock);
            }
            
        }
        
        private void GoToShop(object sender, EventArgs e)
        {

        }
        private void GoToLimbHospital(object sender, EventArgs e)
        {

        }
        private void GoToContrabandMarket(object sender, EventArgs e)
        {

        }
        private void GoToRangedShop(object sender, EventArgs e)
        {

        }
        private void SelectHero(object sender, EventArgs e)
        {
            selectedHero = (Hero)((Button)sender).Tag;
            HireButton.Content = "Contratar a " + selectedHero.GetName(); 
        }
        private void HireHero(object sender, EventArgs e)
        {
            if(selectedHero == null) 
            {
                MessageBox.Show("Debes elegir un héroe al que contratar");
                return; 
            }
            if (selectedHero.IsHired())
            {
                MessageBox.Show("Ya has contratado a " + selectedHero.GetName());
                return;
            }
            Debug.WriteLine("Hired");
            myAdventure.HireHero(selectedHero);
        }
        private void UpdateItems()
        {

        }
        private void GoToMap(object sender, EventArgs e)
        {
            controller.Content = new Map(mainWindow, controller, cursors, rnd, myAdventure);
        }
        private void CloseCurrentGrid(object sender, EventArgs e)
        {
            ShopGrid.Visibility = Visibility.Collapsed;
            BarGrid.Visibility = Visibility.Collapsed;
            HospitalGrid.Visibility = Visibility.Collapsed;
            RangedShopGrid.Visibility = Visibility.Collapsed;
            ContrabandMarketGrid.Visibility = Visibility.Collapsed;
        }
    }
}
