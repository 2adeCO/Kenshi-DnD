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
        Button selectedHeroButton;

        public Zone(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd, Adventure myAdventure,Region currentRegion)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;
            this.controller = controller;
            this.cursors = cursors;
            this.rnd = rnd;
            this.myAdventure = myAdventure;
            this.region = currentRegion;
            UpdateCats();
            PlayerFaction.Inlines.AddRange(mainWindow.DecorateText(myAdventure.GetFactionName()));
            PlayerGrid.Background = mainWindow.GetBrushByNum(myAdventure.GetColor());
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys;
            SquadAlignmentsCombobox.SelectedItem = myAdventure.GetCurrentSquadName();
            Debug.WriteLine("Selected item: " + (string)SquadAlignmentsCombobox.SelectedItem);
            UpdatePlayerGrid();
            
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

            HireButton.IsEnabled = false;
            HireButton.Content = "Elige a quién contratar";
            if (heroesInBar != null)
            {
                for (int i = 0; i < heroesInBar.Length; i++)
                {

                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Auto);
                    BarItems.RowDefinitions.Add(row);
                    Button button = new Button();
                    button.Tag = heroesInBar[i];
                    button.Content = heroesInBar[i].GetNameAndTitle();
                    button.FontSize = 17;
                    button.Margin = new Thickness(10, 20, 30, 20);
                    button.MinHeight = 40;
                    button.Padding = new Thickness(5);
                    button.ToolTip = mainWindow.HeaderToolTipThemer(heroesInBar[i].GetNameAndTitle(),heroesInBar[i].Meet());
                    button.Click += SelectHero;
                    if (heroesInBar[i].IsHired())
                    {
                        button.IsEnabled = false;
                        button.Content = heroesInBar[i].GetName() + " (Contratado)";
                        button.Background = new SolidColorBrush(Colors.LightGreen);
                        button.BorderThickness = new Thickness(3);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.WhiteSmoke);
                    }


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
                textBlock.Margin = new Thickness(4, 10, 4, 10);
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

            if (selectedHeroButton != null)
            {
                selectedHeroButton.ClearValue(Button.BorderBrushProperty);
                selectedHeroButton.ClearValue(Button.BorderThicknessProperty);
                selectedHeroButton.ClearValue(Button.BackgroundProperty);
            }

            selectedHeroButton = ((Button)sender);
            if (((Hero)(selectedHeroButton.Tag)).IsHired())
            {
                HireButton.Content = "Ya has contratado a " + ((Hero)(selectedHeroButton.Tag)).GetName();
                HireButton.IsEnabled = false;
                return;
            }
            HireButton.IsEnabled = true;

            HireButton.Content = "Contratar a " + ((Hero)(selectedHeroButton.Tag)).GetName();
            selectedHeroButton.BorderBrush = Brushes.DarkGreen;
            selectedHeroButton.BorderThickness = new Thickness(3);
            selectedHeroButton.Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#D2B48C"))); 

        }
        private void HireHero(object sender, EventArgs e)
        {
            if(selectedHeroButton == null) 
            {
                MessageBox.Show("Debes elegir un héroe al que contratar");
                return; 
            }
            Hero selectedHero = (Hero)selectedHeroButton.Tag;
            

            if (myAdventure.GetCats() < selectedHero.GetCompetencyCost())
            {
                MessageBox.Show("No tienes suficiente dinero para contratar a " + selectedHero.GetName());
                return;
            }
            if (selectedHero.IsHired())
            {
                MessageBox.Show("Ya has contratado a " + selectedHero.GetName());
                return;
            }
            HireButton.Background = new SolidColorBrush(Colors.LightGreen);
            HireButton.Content = "✔ Contratado";
            myAdventure.HireHero(selectedHero);
            HireButton.IsEnabled = false;
            UpdateSquadEditor(null,null);
            UpdateCats();
            selectedHeroButton.Background = new SolidColorBrush(Colors.LightGreen);
            selectedHeroButton.Content = selectedHero.GetName() + " (Contratado)";
            selectedHeroButton.IsEnabled = false;
            selectedHeroButton = null;
            
            Debug.WriteLine("Hired");
        }
        private void UpdatePlayerGrid()
        {
            PlayerSquad.Children.Clear();
            Hero[] currentHeroes = myAdventure.GetCurrentSquad();
            for (int i = 0; i < currentHeroes.Length; i += 1)
            {
                StackPanel stackPanel = new StackPanel();
                stackPanel.Orientation = Orientation.Vertical;
                System.Windows.Controls.Label label = new System.Windows.Controls.Label();
                label.Content = currentHeroes[i].GetName();
                label.ToolTip = mainWindow.HeaderToolTipThemer(currentHeroes[i].GetName(), currentHeroes[i].ToString());
                ToolTipService.SetInitialShowDelay(label, 100);
                stackPanel.Children.Add(label);

                if (currentHeroes[i].IsAlive())
                {
                    ProgressBar progressBar = new ProgressBar();
                    progressBar.Minimum = 0;
                    progressBar.Maximum = currentHeroes[i].GetToughness();
                    progressBar.Value = currentHeroes[i].GetHp();
                    progressBar.Background = new SolidColorBrush(Colors.DarkGray);
                    progressBar.Foreground = new SolidColorBrush(Colors.DarkRed);
                    progressBar.Height = 20;
                    progressBar.Width = 150;
                    progressBar.Margin = new Thickness(4, 2, 4, 2);
                    progressBar.ToolTip = mainWindow.ToolTipThemer(currentHeroes[i].GetHp() + "/" + currentHeroes[i].GetToughness());
                    ToolTipService.SetInitialShowDelay(progressBar, 100);
                    stackPanel.Background = Brushes.WhiteSmoke;
                    stackPanel.Children.Add(progressBar);
                }
                else
                {
                    stackPanel.Background = Brushes.Gray;
                }
                Border border = new Border();
                border.Margin = new Thickness(2, 8, 20, 8);
                border.BorderBrush = Brushes.Black;
                border.BorderThickness = new Thickness(1);
                border.Child = stackPanel;
                border.VerticalAlignment = VerticalAlignment.Center;
                PlayerSquad.Children.Add(border);
            }



        }
        private void OpenSquadEditor(object sender, EventArgs e)
        {
            if (SquadEditor.Visibility == Visibility.Visible)
            {
                SquadEditor.Visibility = Visibility.Collapsed;
                SquadEditorButton.Content = "Abrir editor de escuadrones";
                return;
            }
            SquadEditor.Visibility = Visibility.Visible;
            SquadEditorButton.Content = "Cerrar editor";
            UpdateSquadEditor(sender, e);
        }
        private void UpdateSquadEditor(object sender, EventArgs e)
        {
            SquadEditorItems.Children.Clear();
            SquadEditorItems.ColumnDefinitions.Clear();
            squadEditorTextBox.Text = myAdventure.GetCurrentSquadName();
            Hero[] heroes =  myAdventure.GetHeroes();
            int index = 0;
            for (int i = 0; i < (heroes.Length / 2) + 1; i+=1  )
            { 
                ColumnDefinition col = new ColumnDefinition();
                col.Width = new GridLength(1, GridUnitType.Star);

                SquadEditorItems.ColumnDefinitions.Add(col);
               

                if (heroes[index] != null)
                {
                    Button button = new Button();
                    button.ToolTip = mainWindow.HeaderToolTipThemer(heroes[index].GetName(), heroes[index].ToString());
                    ToolTipService.SetInitialShowDelay(button, 100);
                    button.Content = heroes[index].GetName();
                    button.Padding = new Thickness(5);
                    button.Margin = new Thickness(5, 2, 5, 2);
                    if (myAdventure.IsInCurrentSquad(heroes[index]))
                    {
                        button.Background = new SolidColorBrush(Colors.LightGreen);
                    }
                    else
                    {
                        button.Background = new SolidColorBrush(Colors.WhiteSmoke);
                    }
                    button.Click += AddOrRemoveFromSquad;
                    button.BorderBrush = new SolidColorBrush(Colors.Black);
                    button.BorderThickness = new Thickness(1);
                    button.Tag = heroes[index];
                    index += 1;
                    Grid.SetRow(button, 0);
                    Grid.SetColumn(button, i);

                    SquadEditorItems.Children.Add(button);
                    if (heroes[index]!= null)
                    {
                        Button button2 = new Button();
                        button2.ToolTip = mainWindow.HeaderToolTipThemer(heroes[index].GetName(), heroes[index].ToString());
                        ToolTipService.SetInitialShowDelay(button2, 100);
                        button2.Content = heroes[index].GetName();
                        button2.Padding = new Thickness(5);
                        button2.Margin = new Thickness(5, 2, 5, 2);
                        if (myAdventure.IsInCurrentSquad(heroes[index]))
                        {
                            button2.Background = new SolidColorBrush(Colors.LightGreen);
                        }
                        else
                        {
                            button2.Background = new SolidColorBrush(Colors.WhiteSmoke);
                        }
                        button2.Click += AddOrRemoveFromSquad;
                        button2.BorderBrush = new SolidColorBrush(Colors.Black);
                        button2.BorderThickness = new Thickness(1);
                        button2.Tag = heroes[index];
                        index += 1;
                        Grid.SetRow(button2, 1);

                        Grid.SetColumn(button2, i);

                        SquadEditorItems.Children.Add(button2);
                    }
                    
                }
                
            }
        }

      

        private void AddOrRemoveFromSquad(object sender,EventArgs e)
        {
            Button button = (Button)sender;
            Hero hero = (Hero)button.Tag;

            if (myAdventure.IsInCurrentSquad(hero))
            {
                myAdventure.RemoveHeroFromSquad(hero);
            }
            else
            {
                if(myAdventure.GetCurrentSquad().Length < 4)
                {
                    myAdventure.AddHeroInSquad(hero);
                }
                else
                {
                    MessageBox.Show("No puedes hacer una alineación con más de 4 héroes");
                }
            }
            UpdateSquadEditor(null,null);
            UpdatePlayerGrid();
        }
        private void CreateSquad(object sender, EventArgs e)
        {
            
            string squadName = squadEditorTextBox.Text + " copia";
            if(myAdventure.GetSquadCount() < 12)
            {
                myAdventure.CreateSquad(squadName);
            }
            else
            {
                MessageBox.Show("No puedes crear más de 12 escuadrones");
                return; 
            }
            
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys;
            SquadAlignmentsCombobox.SelectedItem = myAdventure.GetCurrentSquadName();
            UpdatePlayerGrid();
        }
        private void DeleteSquad(object sender, EventArgs e)
        {
            if(myAdventure.GetSquadCount() == 1)
            {
                MessageBox.Show("No puedes borrar tu única squad\nCrea otra primero.");
                return ;
            }
            string squadKey = (string)SquadAlignmentsCombobox.SelectedItem;
            myAdventure.DeleteSquad(squadKey);

            SquadAlignmentsCombobox.ItemsSource = null;
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys;
            SquadAlignmentsCombobox.SelectedItem = myAdventure.GetSavedSquads().First().Key;
            
            myAdventure.SetCurrentSquad((string)SquadAlignmentsCombobox.SelectedItem);
            UpdatePlayerGrid();
            UpdateSquadEditor(null, null);
        }
        private void ChangeSquad(object sender, EventArgs e)
        {
            if (SquadAlignmentsCombobox.ItemsSource != null)
            {
                string squadKey = (string)SquadAlignmentsCombobox.SelectedItem;
                myAdventure.SetCurrentSquad(squadKey);
                UpdatePlayerGrid();
                UpdateSquadEditor(null, null);
            }
        }
        private void GoToMap(object sender, EventArgs e)
        {
            controller.Content = new Map(mainWindow, controller, cursors, rnd, myAdventure);
        }
        private void UpdateCats()
        {
            PlayerCats.Inlines.Clear();
            PlayerCats.Inlines.AddRange(mainWindow.DecorateText("@218@" + myAdventure.GetCats() + "$@"));
        }
        private void CloseCurrentGrid(object sender, EventArgs e)
        {
            ShopGrid.Visibility = Visibility.Collapsed;
            BarGrid.Visibility = Visibility.Collapsed;
            HospitalGrid.Visibility = Visibility.Collapsed;
            RangedShopGrid.Visibility = Visibility.Collapsed;
            ContrabandMarketGrid.Visibility = Visibility.Collapsed;
        }
        private void OnlyNumsAndLetters(object sender, EventArgs e)
        {
            Debug.WriteLine("Text changed");
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
            squadEditorTextBox.TextChanged -= OnlyNumsAndLetters;
            textBox.Text = result;

            squadEditorTextBox.TextChanged += OnlyNumsAndLetters;
            if (caretIndex != -1)
            {
                textBox.CaretIndex = caretIndex;
            }
            else
            {
                textBox.CaretIndex = result.Length;
            }

            string squadKey = (string)SquadAlignmentsCombobox.SelectedItem;

            Hero[] squadToChangeName = myAdventure.GetSavedSquads()[squadKey];
            myAdventure.DeleteSquad(squadKey);
            myAdventure.CreateSquad(result) ;
            for(int i = 0; i < myAdventure.GetSquadCount(); i += 1)
            {
                Debug.WriteLine(myAdventure.GetSavedSquads().ElementAt(i));
            }
            SquadAlignmentsCombobox.ItemsSource = null;
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys;
            SquadAlignmentsCombobox.SelectedItem = result;
        }
    }
}
