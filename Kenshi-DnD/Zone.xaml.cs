using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para Zone.xaml
    /// </summary>
    [Serializable]
    public partial class Zone : UserControl
    {
        MainWindow mainWindow;
        ContentControl controller;
        Cursor[] cursors;
        Random rnd;
        Adventure myAdventure;
        Region region;

        ComboBox factionToFight;

        Hero[] heroesInBar;
        Button selectedHeroButton;

        Item[] itemsInShop;
        Button selectedShopItemButton;

        Button selectedItemToSellButton;

        Item[] itemsInContrabandMarket;

        Item[] itemsInRangedShop;

        Limb[] limbsInHospital;


        const int CONTRABAND_MARKET_ACCESS_PRICE = 1000;
        const int RANGED_SHOP_ACCESS_PRICE = 500;
        public Zone(MainWindow mainWindow, ContentControl controller, Cursor[] cursors, Random rnd, Adventure myAdventure, Region currentRegion)
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
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys.ToList();
            SquadAlignmentsCombobox.SelectedItem = myAdventure.GetCurrentSquadName();
            Debug.WriteLine("Selected item: " + (string)SquadAlignmentsCombobox.SelectedItem);
            UpdatePlayerGrid();
            UpdateInventoryGrid();

            CreateActions();
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("Descansar en el bar\n@2@" + region.GetSleepCost(myAdventure) + "$@"));

            SleepButton.Content = textBlock;


        }
        private void CreateActions()
        {
            int relations = region.GetRelations();
            switch (relations)
            {
                case < 20:
                    {
                        relations = 1;
                        break;
                    }
                case < 40:
                    {
                        relations = 6;
                        break;
                    }
                default:
                    {
                        relations = 2;
                        break;
                    }
            }


            ZoneName.Inlines.AddRange(mainWindow.DecorateText("@" + relations + "@" + region.GetName() + "@"));
            ZoneName.ToolTip = mainWindow.ToolTipThemer(region.GetDescription());

            bool reloadShops = region.ConsumeToken();

            if (region.HasBar())
            {
                if (reloadShops)
                {
                    region.GoToBar(myAdventure, rnd);
                }

                Button button = new Button();
                button.Content = "Ir al bar";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += GoToBarButton;
                heroesInBar = region.GetHeroesInBar();
                ActionsPanel.Children.Add(button);

            }
            if (region.HasShop())
            {
                if (reloadShops)
                {
                    region.GoToShop(myAdventure, rnd);
                }
                Button button = new Button();
                button.Content = "Ir a la tienda";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += GoToShopButton;
                itemsInShop = region.GetShop();
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
                button.Click += GoToContrabandMarketButton;
                ActionsPanel.Children.Add(button);

            }
            if (region.HasLimbHospital())
            {
                if(reloadShops)
                {
                    region.GoToHospital(myAdventure, rnd);
                }
                Button button = new Button();
                button.Content = "Ir al mecánico de extremidades";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
               
                button.Click += GoToLimbHospitalButton;
                limbsInHospital = region.GetLimbHospital();
                ActionsPanel.Children.Add(button);

            }
            if (region.HasRangedShop())
            {
                if (reloadShops)
                {
                    region.SetAccessToRangedShop(false);
                }
                else
                {
                    if (region.HasAccessToRangedShop())
                    {
                        RangedShopItems.Visibility = Visibility.Visible;
                        AccessRangedShopButton.IsEnabled = false;
                        TextBlock textBlock = new TextBlock();
                        if (region.CanBuyRangedItems())
                        {
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@Venga pasa.@\n@111@Nada de delatarme@"));
                        }
                        else
                        {
                            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@Ya tienes lo que querías.@\n@111@¿No?@"));
                        }
                        AccessRangedShopButton.Content = textBlock;
                    }
                    
                }

                Button button = new Button();
                button.Content = "Ir al club de tiro";
                button.Margin = new Thickness(4, 30, 4, 30);
                button.Height = 50;
                button.FontSize = 18;
                button.HorizontalAlignment = HorizontalAlignment.Stretch;
                button.Click += GoToRangedShopButton;
                itemsInRangedShop = region.GetRangedShop();
                ActionsPanel.Children.Add(button);

            }
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            
            Button buttonFight = new Button();
            buttonFight.Content = "Buscar pelea";
            buttonFight.ToolTip = mainWindow.ToolTipThemer("Perderás relaciones con las facciones de la zona.");
            buttonFight.Margin = new Thickness(4, 30, 4, 4);
            buttonFight.Height = 50;
            buttonFight.FontSize = 18;
            buttonFight.HorizontalAlignment = HorizontalAlignment.Stretch;
            buttonFight.Click += LookForAFight;
            stackPanel.Children.Add(buttonFight);


            ComboBox comboBox = new ComboBox();
            comboBox.Margin = new Thickness(4, 0, 4, 0);
            comboBox.Height = 30;
            for (int i = 0; i < region.GetFactions().Count; i += 1)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.Tag = region.GetFactions()[i];
                textBlock.Inlines.AddRange(mainWindow.DecorateText(region.GetFactions()[i].GetFactionName()));
                comboBox.Items.Add(textBlock);
            }
            comboBox.SelectedIndex = 0;
            factionToFight = comboBox;
            stackPanel.Children.Add(comboBox);
            ActionsPanel.Children.Add(stackPanel);
            comboBox.SelectionChanged += LookFaction;
        }
        private void GoToBarButton(object sender, EventArgs e)
        {
            UpdateLog("Entras en el bar local");
            GoToBar(sender,e);
        }
        private void GoToShopButton(object sender, EventArgs e)
        {
            UpdateLog("Entras en la tienda local");
            GoToShop(sender, e);
        }
        private void GoToLimbHospitalButton(object sender, EventArgs e)
        {
            UpdateLog("Entras a la clínica, un olor intenso a aceite y sangre te empuja");
            GoToLimbHospital(sender, e);
        }
        private void GoToContrabandMarketButton(object sender, EventArgs e)
        {
            UpdateLog("Buscas entre los barrios bajos de " + region.GetName() + " y encuentras un mercado ilícito");
            GoToContrabandMarket(sender, e);
        }
        private void GoToRangedShopButton(object sender, EventArgs e)
        {
            if (region.HasAccessToRangedShop())
            {
                UpdateLog("Vuelves a entrar al club de tiro, el segurata vuelve a ser tu sombra");
            }
            else
            {
                UpdateLog("Entras al club de tiro, el segurata ante tí, te mira por encima del hombro, y te para en seco");
            }
            GoToRangedShop(sender, e);
        }
        private void LookFaction(object sender, EventArgs e)
        {
            Faction faction = (Faction)((TextBlock)factionToFight.SelectedItem).Tag;
            UpdateLog(faction.GetFactionDescription());
        }
        private void LookForAFight(object sender, EventArgs e)
        {
            UpdateLog("Buscas bronca por la zona..");

            Faction faction = (Faction)((TextBlock)factionToFight.SelectedItem).Tag;
            faction.AddOrSubtractRelation(-20);
            controller.Content = new CombatWindow(mainWindow, controller, cursors, rnd, myAdventure,mainWindow.SqlGenerateMonsters(myAdventure,faction,rnd));
        }
        public void UpdateLog(string message)
        {
            InfoLog.Inlines.AddRange(mainWindow.DecorateText(message + "\n"));
            InfoLogScroll.ScrollToEnd();
        }
        private void GoToBar(object sender, EventArgs e)
        {
            CloseCurrentGrid(null, null);
            BarGrid.Visibility = Visibility.Visible;

            HireButton.IsEnabled = false;
            TextBlock textBlockSalute = new TextBlock();
            textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@Bienvenido al bar de " + region.GetName() +"@"));
            HireButton.Content = textBlockSalute;
            BarItems.Children.Clear();
            if (heroesInBar != null)
            {
                for (int i = 0; i < heroesInBar.Length; i++)
                {

                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Auto);
                    BarItems.RowDefinitions.Add(row);
                    Button button = new Button();
                    button.Tag = heroesInBar[i];
                    TextBlock textBlock = new TextBlock();
                    textBlock.Inlines.AddRange(mainWindow.DecorateText(heroesInBar[i].GetNameAndTitle() + " - " +
                        heroesInBar[i].CompetencyToString() + "\n@2@" + heroesInBar[i].GetCompetencyCost() + "$@"));
                    button.Content = textBlock;
                    button.FontSize = 17;
                    button.Margin = new Thickness(10, 20, 30, 20);
                    button.MinHeight = 40;
                    button.Padding = new Thickness(5);
                    button.ToolTip = mainWindow.HeaderToolTipThemer(heroesInBar[i].GetNameAndTitle(), heroesInBar[i].Meet());
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
                UpdateLog("Las tensiones en el barrio hacen que el bar no sea muy rentable");
                Grid.SetRow(textBlock, 0);
                BarItems.Children.Add(textBlock);
            }

        }

        private void GoToShop(object sender, EventArgs e)
        {
            CloseCurrentGrid(null, null);
            ShopGrid.Visibility = Visibility.Visible;

            BuyButton.IsEnabled = false;
            TextBlock textBlockSalute = new TextBlock();
            textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@¡Bienvenido a la tienda de " + region.GetName() + "!@"));
            BuyButton.Content = textBlockSalute;
            ShopItems.Children.Clear();
            bool boughtEverything = true;
            if (itemsInShop != null)
            {
                for (int i = 0; i < itemsInShop.Length; i++)
                {

                    if (itemsInShop[i] != null)
                    {
                        RowDefinition row = new RowDefinition();
                        row.Height = new GridLength(1, GridUnitType.Auto);
                        ShopItems.RowDefinitions.Add(row);
                        Button button = new Button();
                        button.Tag = itemsInShop[i];
                        TextBlock textBlock = new TextBlock();
                        textBlock.FontSize = 17;
                        textBlock.Inlines.AddRange(mainWindow.DecorateText(itemsInShop[i].GetName() + " - " + itemsInShop[i].RarityToString()
                            + "\n@2@" + itemsInShop[i].GetValue() + "$@"));
                        button.Content = textBlock;
                        button.Margin = new Thickness(10, 20, 30, 20);
                        button.MinHeight = 40;
                        button.Padding = new Thickness(5);
                        button.ToolTip = mainWindow.HeaderToolTipThemer(itemsInShop[i].GetName(), itemsInShop[i].ToString());
                        button.Click += SelectShopItem;
                        button.Background = new SolidColorBrush(Colors.WhiteSmoke);

                        boughtEverything = false;

                        Grid.SetRow(button, i);
                        ShopItems.Children.Add(button);
                    }

                }
                if (boughtEverything)
                {
                    RowDefinition row = new RowDefinition();
                    row.Height = new GridLength(1, GridUnitType.Star);
                    ShopItems.RowDefinitions.Add(row);

                    TextBlock textBlock = new TextBlock();
                    textBlock.Inlines.AddRange(mainWindow.DecorateText("Lo compraste @916@TODO@, vuelva pronto..."));
                    textBlock.Margin = new Thickness(4, 10, 4, 10);
                    UpdateLog("El vendedor de la tienda sonríe calidamente");
                    Grid.SetRow(textBlock, 0);
                    ShopItems.Children.Add(textBlock);

                }
            }
        }
        private void SellItem(object sender, EventArgs e) 
        { 
            if(selectedItemToSellButton == null)
            {
                MessageBox.Show("Primero selecciona un objeto a vender.");
                return;
            }

            Item item = (Item)selectedItemToSellButton.Tag;
            UpdateLog("Vendiste " + item.GetName() + " por @2@" + item.GetResellValue() + "$@");
            myAdventure.SellItem(item);
            SellButton.IsEnabled = false;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText($"@9@Un placer hacer negocios con usted@\n@2@+{item.GetResellValue}$@"));
            SellButton.Content = textBlock;
            selectedItemToSellButton = null;
            region.ImproveRelations(1,this);
            UpdateCats();
            UpdateInventoryGrid();
        }

        private void SelectSellItem(object sender, EventArgs e)
        {
            selectedItemToSellButton = (Button)sender;
            SellButton.IsEnabled = true;
            Item selectedItem = (Item)selectedItemToSellButton.Tag;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText($"@9@¿Vender@ @916@{selectedItem.GetName()}@ @9@?@"));
            SellButton.Content = textBlock;
            UpdateLog("Ponderas la posibilidad de vender " + selectedItem.GetName() + " por @2@" + selectedItem.GetResellValue() + "$@");
            UpdateInventoryGrid();
        }
        private void GoToLimbHospital(object sender, EventArgs e)
        {
            CloseCurrentGrid(null,null);
            HospitalGrid.Visibility = Visibility.Visible;
            PutLimbButton.IsEnabled = false;
            TextBlock textBlockSalute = new TextBlock();

            if (myAdventure.GetAmputees().Length == 0)
            {
                textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@¿Qué haces aquí?\nYa vendrás@"));
                HeroComboBox.Visibility = Visibility.Collapsed;
                PutLimbButton.IsEnabled = false;
                HospitalItems.Visibility = Visibility.Collapsed;
            }
            else
            {
                textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@¿Necesitas un parche, guerrero?@"));
                HeroComboBox.Items.Clear();
                Hero[] amputees = myAdventure.GetAmputees();
                for (int i = 0; i < amputees.Length; i += 1)
                {
                    ComboBoxItem comboItem = new ComboBoxItem();
                    comboItem.Content = amputees[i].GetNameAndTitle();
                    comboItem.Tag = amputees[i];
                    HeroComboBox.Items.Add(comboItem);
                }
                HeroComboBox.SelectedIndex = 0;
            }
            PutLimbButton.Content = textBlockSalute;
            HospitalItems.Children.Clear();
            if(limbsInHospital != null)
            {
                for (int i = 0; i < limbsInHospital.Length; i++)
                {
                    Debug.WriteLine("Limb: " + limbsInHospital[i].GetName());
                    if (limbsInHospital[i] != null)
                    {
                        RowDefinition row = new RowDefinition();
                        row.Height = new GridLength(1, GridUnitType.Auto);
                        HospitalItems.RowDefinitions.Add(row);
                        Button button = new Button();
                        button.Tag = limbsInHospital[i];
                        TextBlock textBlock = new TextBlock() {FontSize = 17 };
                        textBlock.Inlines.AddRange(mainWindow.DecorateText(limbsInHospital[i].GetName() + " - " +
                            limbsInHospital[i].RarityToString() + "\n@2@" + limbsInHospital[i].GetValue() + "$@"));
                        button.Content = textBlock;
                        button.Margin = new Thickness(10, 20, 30, 20);
                        button.MinHeight = 40;
                        button.Padding = new Thickness(5);
                       
                        button.ToolTip = mainWindow.HeaderToolTipThemer(limbsInHospital[i].GetName(), limbsInHospital[i].ToString());
                        button.Click += SelectLimb;
                        button.Background = new SolidColorBrush(Colors.WhiteSmoke);
                        Grid.SetRow(button, i);
                        HospitalItems.Children.Add(button);
                    }
                }
            }
        }
        private void SelectLimb(object sender,EventArgs e)
        {
            if (selectedShopItemButton != null)
            {
                selectedShopItemButton.ClearValue(Button.BorderBrushProperty);
                selectedShopItemButton.ClearValue(Button.BorderThicknessProperty);
                selectedShopItemButton.ClearValue(Button.BackgroundProperty);
                selectedShopItemButton.Background = new SolidColorBrush(Colors.WhiteSmoke);
            }
            selectedShopItemButton = (Button)sender;
            PutLimbButton.IsEnabled = true;
            Limb selectedLimb = (Limb)selectedShopItemButton.Tag;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText($"@9@¿Quieres 'instalarte'@ @916@{selectedLimb.GetName()}@ @9@?@"));
            PutLimbButton.Content = textBlock;
            UpdateLog("Miras " + selectedLimb.GetName() + " (@2@" + selectedLimb.GetValue() + "$@)");
            selectedShopItemButton.BorderBrush = Brushes.SteelBlue;
            selectedShopItemButton.BorderThickness = new Thickness(3);
            selectedShopItemButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334455"));
        }
        private void PutLimb(object sender, EventArgs e)
        {
            ComboBoxItem comboItem = (ComboBoxItem)HeroComboBox.SelectedItem;
            Hero hero = (Hero)comboItem.Tag;
            Limb limb = (Limb)selectedShopItemButton.Tag;
            if (!myAdventure.SpendIfHasEnough(limb.GetValue()))
            {
                MessageBox.Show("No tienes suficiente dinero");
                return;
            }
            UpdateLog(hero.GetName() + " sufre de dolor durante unos largos 20 minutos, ¡Pero ahora tiene su " + limb.GetName() + " bien reluciente!");
            hero.PutLimb(limb);
            HospitalItems.Children.Remove(selectedShopItemButton);
            PutLimbButton.IsEnabled = false;
            selectedShopItemButton = null;
            UpdateCats();
            GoToLimbHospital(null,null);
            UpdatePlayerGrid();
        }
        private void GoToContrabandMarket(object sender, EventArgs e)
        {
            CloseCurrentGrid(null, null);
            ContrabandMarketGrid.Visibility = Visibility.Visible;
            BuyContrabandItemButton.IsEnabled = false;
            TextBlock textBlockSalute = new TextBlock();
            textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@Bienvenido al mercado de " + region.GetName() + "... nadie te ha visto entrar.@"));
            BuyContrabandItemButton.Content = textBlockSalute;
            ContrabandMarketItems.Children.Clear();



            if (itemsInContrabandMarket != null)
            {
                for (int i = 0; i < itemsInContrabandMarket.Length; i++)
                {
                    if (itemsInContrabandMarket[i] != null)
                    {
                        RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
                        ContrabandMarketItems.RowDefinitions.Add(row);

                        Button button = new Button
                        {
                            Tag = itemsInContrabandMarket[i],
                            Margin = new Thickness(10, 20, 30, 20),
                            MinHeight = 40,
                            Padding = new Thickness(5),
                            Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), 
                            Foreground = Brushes.WhiteSmoke,
                            ToolTip = mainWindow.HeaderToolTipThemer(itemsInContrabandMarket[i].GetName(), itemsInContrabandMarket[i].ToString())
                        };

                        TextBlock textBlock = new TextBlock { FontSize = 17 };
                        textBlock.Inlines.AddRange(mainWindow.DecorateText(
                            itemsInContrabandMarket[i].GetName() + " - " + itemsInContrabandMarket[i].RarityToString() +
                            "\n@2@" + itemsInContrabandMarket[i].GetValue() + " cats@"));

                        button.Content = textBlock;
                        button.Click += SelectContrabandItem;

                        Grid.SetRow(button, i);
                        ContrabandMarketItems.Children.Add(button);


                    }
                }


            }
        }
        private void FillAmmo(object sender, EventArgs e)
        {
            if (!myAdventure.SpendIfHasEnough((int)FillAmmoButton.Tag))
            {
                MessageBox.Show("No tienes suficiente dinero para recargar munición.");
                return;
            }
            UpdateLog("Recargaste tu munición por @2@" + (int)FillAmmoButton.Tag + "$@");
            region.ImproveRelations(1, this);
            UpdateCats();
            
            myAdventure.GetInventory().FillAmmo();
            FillAmmoButton.Visibility = Visibility.Collapsed;
        }
        private void GoToRangedShop(object sender, EventArgs e)
        {
            // Closes all other grids
            CloseCurrentGrid(null, null);
            // Makes the grid visible
            RangedShopGrid.Visibility = Visibility.Visible;
            // Disables the buy button
            BuyRangedItemButton.IsEnabled = false;
            // Sets the content of the buy button
            TextBlock textBlockSalute = new TextBlock();
            if(region.HasAccessToRangedShop())
            {
                if (region.CanBuyRangedItems())
                {
                    textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@Pues... hazlo rápido...@"));
                    if (myAdventure.GetInventory().GetRanged(0).Length != 0)
                    {
                        FillAmmoButton.Visibility = Visibility.Visible;
                        int price = 0;
                        Item[] rangedItems = myAdventure.GetInventory().GetRanged(0);
                        for (int i = 0; i < rangedItems.Length; i++)
                        {
                            price += ((RangedItem)rangedItems[i]).IsFull() ? 0 : 200;
                        }
                        if(price != 0)
                        {
                            FillAmmoButton.IsEnabled = true;
                            FillAmmoButton.Tag = price;
                            FillAmmoButton.Content = mainWindow.GenerateTextblock("Recargar munición (@2@" + price + "$@)");

                        }
                        else
                        {
                            FillAmmoButton.IsEnabled = false;
                            FillAmmoButton.Tag = price;
                            FillAmmoButton.Content = mainWindow.GenerateTextblock("@9@No te falta munición@");
                        }
                    }
                }
                else
                {
                    textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@2@No tardes en irte.@"));
                }
            }
            else
            {
                textBlockSalute.Inlines.AddRange(mainWindow.DecorateText("@9@No puedes entrar aquí, plebeyo.@"));
                FillAmmoButton.Visibility = Visibility.Collapsed;
            }
            BuyRangedItemButton.Content = textBlockSalute;

            
            // Resets the grid's items
            RangedShopItems.Children.Clear();

            //Checks if the array of items is null
            if (itemsInRangedShop != null)
            {
                //Generates one row and button for each item
                for (int i = 0; i < itemsInRangedShop.Length; i++)
                {
                    if (itemsInRangedShop[i] != null)
                    {
                        // Creates a row for each item
                        RowDefinition row = new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) };
                        RangedShopItems.RowDefinitions.Add(row);
                        // Creates a button for each item
                        Button button = new Button
                        {
                            Tag = itemsInRangedShop[i],
                            Margin = new Thickness(10, 20, 30, 20),
                            MinHeight = 40,
                            Padding = new Thickness(5),
                            Background = new SolidColorBrush(Color.FromRgb(50, 50, 50)), // Oscuro, sucio
                            Foreground = Brushes.WhiteSmoke,
                            ToolTip = mainWindow.HeaderToolTipThemer(itemsInRangedShop[i].GetName(), itemsInRangedShop[i].ToString())
                        };
                        // Fills the button with the item name, rarity and value
                        TextBlock textBlock = new TextBlock { FontSize = 17 };
                        textBlock.Inlines.AddRange(mainWindow.DecorateText(
                            itemsInRangedShop[i].GetName() + " - " + itemsInRangedShop[i].RarityToString() +
                            "\n@2@" + itemsInRangedShop[i].GetValue() + " $@"));
                        button.Content = textBlock;
                        // Sets the button's click event
                        if (region.CanBuyRangedItems())
                        {
                            button.Click += SelectRangedItem;
                        }
                        // Places the button in the grid
                        Grid.SetRow(button, i);
                        RangedShopItems.Children.Add(button);

                        Debug.WriteLine("Button added");

                    }
                }


            }

        }
        private void SelectRangedItem(object sender, EventArgs e)
        {
            if (selectedShopItemButton != null)
            {
                selectedShopItemButton.ClearValue(Button.BorderBrushProperty);
                selectedShopItemButton.ClearValue(Button.BorderThicknessProperty);
                selectedShopItemButton.ClearValue(Button.BackgroundProperty);
                selectedShopItemButton.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            }

            selectedShopItemButton = (Button)sender;

            BuyRangedItemButton.IsEnabled = true;

            Item selectedItem = (Item)selectedShopItemButton.Tag;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText($"@9@¿Comprar@ @914@{selectedItem.GetName()}@ @9@del arsenal?@"));
            BuyRangedItemButton.Content = textBlock;
            UpdateLog("Miras " + selectedItem.GetName() + " (@2@" + selectedItem.GetValue() + "$@)");
            selectedShopItemButton.BorderBrush = Brushes.SteelBlue;
            selectedShopItemButton.BorderThickness = new Thickness(3);
            selectedShopItemButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#334455"));
        }

        private void BuyRangedItem(object sender, EventArgs e)
        {
            if (selectedShopItemButton == null)
            {
                MessageBox.Show("Primero selecciona un arma.");
                return;
            }

            Item selectedItem = (Item)selectedShopItemButton.Tag;

            if (myAdventure.GetCats() < selectedItem.GetValue())
            {
                MessageBox.Show($"No tienes suficientes cats para comprar {selectedItem.GetName()}.");
                return;
            }

            BuyRangedItemButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55334455"));
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@Ya tienes lo que querías.@\n@111@¿No?@"));
            BuyRangedItemButton.Content = textBlock;
            myAdventure.BuyItem(selectedItem);
            RangedShopItems.Children.Remove(selectedShopItemButton);
            UpdateLog("Compraste " + selectedItem.GetName() + " por @2@" + selectedItem.GetValue() + "$@");
            UpdateLog("El segurata suspira...");
            BuyRangedItemButton.IsEnabled = false;
            selectedShopItemButton = null;

            for (int i = 0; i < itemsInRangedShop.Length; i++)
            {
                if (itemsInRangedShop[i] == selectedItem)
                {
                    itemsInRangedShop[i] = null;
                    break;
                }
            }

            region.ImproveRelations(1, this);
            UpdateInventoryGrid();
            UpdateCats();

           
            textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("@920@Muy bien.@\n@112@Pero no tocarás nada más@."));
            AccessRangedShopButton.Content = textBlock;
            region.SetCanBuyRangedItems(false);
            region.GoToRangedShop(myAdventure, rnd);
            GoToRangedShop(null,null);
            Debug.WriteLine("Ranged item bought");
        }

        private void AccessRangedShop(object sender, EventArgs e)
        {
            // If the player has enough money, access the ranged shop
            if (!myAdventure.SpendIfHasEnough(RANGED_SHOP_ACCESS_PRICE))
            {
                MessageBox.Show("No tienes suficiente dinero para acceder al arsenal.");
                return;
            }
            // Open the access to the shop as open
            region.SetAccessToRangedShop(true);
            // Update the cats on screen
            UpdateCats();
            // Set the shop items as visible
            RangedShopItems.Visibility = Visibility.Visible;
            // Disable the access button
            AccessRangedShopButton.IsEnabled = false;
            // Reset the items in the shop
            region.GoToRangedShop(myAdventure,rnd);
            itemsInRangedShop = region.GetRangedShop();
            // Set the content of the access button
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@Haré la vista gorda.@\n@111@Pero no te quitaré los ojos@"));
            AccessRangedShopButton.Content = textBlock;
            UpdateLog("El segurata se hace a un lado y te deja pasar, pero no dejas de notar su respiración");
            // Make the player able to buy one item
            region.SetCanBuyRangedItems(true);
            // Generate the item buttons in the shop
            GoToRangedShop(null, null);
        }

        private void SelectContrabandItem(object sender, EventArgs e)
        {
            if (selectedShopItemButton != null)
            {
                selectedShopItemButton.ClearValue(Button.BorderBrushProperty);
                selectedShopItemButton.ClearValue(Button.BorderThicknessProperty);
                selectedShopItemButton.ClearValue(Button.BackgroundProperty);
                selectedShopItemButton.Background = new SolidColorBrush(Color.FromRgb(50, 50, 50));
            }

            selectedShopItemButton = (Button)sender;

            BuyContrabandItemButton.IsEnabled = true;

            Item selectedItem = (Item)selectedShopItemButton.Tag;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText($"@914@¿Adquirir@ @916@\"{selectedItem.GetName()}\"@ @914@del mercado negro?@"));
            BuyContrabandItemButton.Content = textBlock;
            UpdateLog("Miras " + selectedItem.GetName() + " (@2@" + selectedItem.GetValue() + "$@)");
            selectedShopItemButton.BorderBrush = Brushes.DarkOliveGreen;
            selectedShopItemButton.BorderThickness = new Thickness(3);
            selectedShopItemButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#4B3B2A")); 
        }

        private void BuyContrabandItem(object sender, EventArgs e)
        {
            if (selectedShopItemButton == null)
            {
                MessageBox.Show("Primero elige algo del mercado.");
                return;
            }

            Item selectedItem = (Item)selectedShopItemButton.Tag;

            if (myAdventure.GetCats() < selectedItem.GetValue())
            {
                MessageBox.Show($"No tienes suficientes cats para conseguir {selectedItem.GetName()}.");
                return;
            }

            BuyContrabandItemButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#55222222"));
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("@218@✔ Intercambio hecho@\n@9@Ahora vete...@"));

            BuyContrabandItemButton.Content = textBlock;
            myAdventure.BuyItem(selectedItem);
            UpdateLog("Compraste " + selectedItem.GetName() + " por @2@" + selectedItem.GetValue() + "$@");

            ContrabandMarketItems.Children.Remove(selectedShopItemButton);
            BuyContrabandItemButton.IsEnabled = false;
            selectedShopItemButton = null;

            for (int i = 0; i < itemsInShop.Length; i++)
            {
                if (itemsInShop[i] == selectedItem)
                {
                    itemsInShop[i] = null;
                    break;
                }
            }
            region.ImproveRelations(1, this);
            UpdateInventoryGrid();
            UpdateCats();

            ContrabandMarketItems.Visibility = Visibility.Collapsed;
            AccessContrabandMarketButton.IsEnabled = true;
            textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("@912@Ahora vete.@\n@111@A no ser de que tengas más cats(" + CONTRABAND_MARKET_ACCESS_PRICE + ") de sobra@."));
            AccessContrabandMarketButton.Content = textBlock;
            UpdateLog("El 'mercader' te señala la salida");

            Debug.WriteLine("Contraband item bought");
        }
        private void AccessContrabandMarket(object sender, EventArgs e)
        {
            if (!myAdventure.SpendIfHasEnough(CONTRABAND_MARKET_ACCESS_PRICE))
            {
                MessageBox.Show("No tienes suficiente dinero para acceder al mercado clandestino.");
                return;
            }
            UpdateCats();
            UpdateLog("Agarran tus cats con fuerza mientras te llevan adentro");

            ContrabandMarketItems.Visibility = Visibility.Visible;
            AccessContrabandMarketButton.IsEnabled = false;
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("@9@Ya sabes el trato,@ @111@1 solo \nobjeto@ @9@por acceso.@"));
            AccessContrabandMarketButton.Content = textBlock;
            region.GoToContrabandMarket(myAdventure, rnd);
            itemsInContrabandMarket = region.GetContrabandMarket();
            GoToContrabandMarket(null, null);
        }
        private void SelectShopItem(object sender, EventArgs e)
        {
            if (selectedShopItemButton != null)
            {
                selectedShopItemButton.ClearValue(Button.BorderBrushProperty);
                selectedShopItemButton.ClearValue(Button.BorderThicknessProperty);
                selectedShopItemButton.ClearValue(Button.BackgroundProperty);
            }

            selectedShopItemButton = ((Button)sender);

            BuyButton.IsEnabled = true;
            Item selectedItem = (Item)selectedShopItemButton.Tag;
            BuyButton.Content = "¿Comprar " + selectedItem.GetName() + "?";
            UpdateLog("Miras " + selectedItem.GetName() + " (@2@" + selectedItem.GetValue() + "$@)");
            selectedShopItemButton.BorderBrush = Brushes.DarkGreen;
            selectedShopItemButton.BorderThickness = new Thickness(3);
            selectedShopItemButton.Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#D2B48C")));
        }
        private void BuyShopItem(object sender, EventArgs e)
        {
            if (selectedShopItemButton == null)
            {
                MessageBox.Show("Debes elegir un objeto que comprar");
                return;
            }
            Item selectedItem = (Item)selectedShopItemButton.Tag;

            if (myAdventure.GetCats() < selectedItem.GetValue())
            {
                MessageBox.Show("No tienes suficiente dinero para comprar " + selectedItem.GetName());
                return;
            }
            UpdateLog("Compraste " + selectedItem.GetName() + " por @2@" + selectedItem.GetValue() + "$@");
            BuyButton.Background = new SolidColorBrush(Colors.LightGreen);
            BuyButton.Content = "✔ Compra realizada\n¡Muchas gracias!";
            myAdventure.BuyItem(selectedItem);
            ShopItems.Children.Remove(selectedShopItemButton);
            BuyButton.IsEnabled = false;
            selectedShopItemButton = null;

            for (int i = 0; i < itemsInShop.Length; i += 1)
            {
                if (itemsInShop[i] == selectedItem)
                {
                    itemsInShop[i] = null;
                    break;
                }
            }
            region.ImproveRelations(1, this);
            UpdateInventoryGrid();
            UpdateCats();

            if (ShopItems.Children.Count == 0)
            {
                Debug.WriteLine("Shop is empty");
                ShopItems.RowDefinitions.Clear();
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(1, GridUnitType.Star);
                ShopItems.RowDefinitions.Add(row);

                TextBlock textBlock = new TextBlock();
                textBlock.Inlines.AddRange(mainWindow.DecorateText("Lo compraste @916@TODO@, vuelva pronto..."));
                textBlock.Margin = new Thickness(4, 10, 4, 10);
                Grid.SetRow(textBlock, 0);
                ShopItems.Children.Add(textBlock);
            }


            Debug.WriteLine("Item bought");
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
            Hero hero = (Hero)selectedHeroButton.Tag;
            UpdateLog(hero.GetNameAndTitle() + "\n" + hero.Meet());
            HireButton.Content = "¿Contratar a " + hero.GetName() +"?";
            selectedHeroButton.BorderBrush = Brushes.DarkGreen;
            selectedHeroButton.BorderThickness = new Thickness(3);
            selectedHeroButton.Background = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#D2B48C")));

        }
        private void HireHero(object sender, EventArgs e)
        {
            if (selectedHeroButton == null)
            {
                MessageBox.Show("Debes elegir un héroe al que contratar");
                return;
            }
            if (!myAdventure.CanHire())
            {
                MessageBox.Show("Has superado el límite de héroes");
                return;
            }
            Hero selectedHero = (Hero)selectedHeroButton.Tag;


            if (!myAdventure.SpendIfHasEnough(selectedHero.GetCompetencyCost()))
            {
                MessageBox.Show("No tienes suficiente dinero para contratar a " + selectedHero.GetName());
                return;
            }
            HireButton.Background = new SolidColorBrush(Colors.LightGreen);
            HireButton.Content = "✔ Contratado";
            myAdventure.HireHero(selectedHero);
            HireButton.IsEnabled = false;
            UpdateSquadEditor(null, null);
            region.ImproveRelations(1, this);
            UpdateCats();
            selectedHeroButton.Background = new SolidColorBrush(Colors.LightGreen);
            selectedHeroButton.Content = selectedHero.GetName() + " (Contratado)";
            selectedHeroButton.IsEnabled = false;
            selectedHeroButton = null;

            UpdateLog("¡" +selectedHero.GetNameAndTitle() + " se unió a la aventura!");
            Debug.WriteLine("Hired");
        }
        private void SleepInBar(object sender, EventArgs e)
        {
            // Checks if the player has enough money to sleep in the bar
            if (myAdventure.GetCats() < region.GetSleepCost(myAdventure))
            {
                MessageBox.Show("No tienes suficiente dinero para dormir en el bar");
                return;
            }
            // Cures 10 HP to all heroes with less than max HP
            region.SleepInBar(myAdventure, region.GetSleepCost(myAdventure),this);
            // Updates the money on the screen
            UpdateCats();
            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.AddRange(mainWindow.DecorateText("Descansar en el bar\n@2@" + region.GetSleepCost(myAdventure) + "$@"));

            SleepButton.Content = textBlock;
            UpdatePlayerGrid();

            Debug.WriteLine("Slept in bar");
        }
        private void UpdateInventoryGrid()
        {
            InventoryItems.Children.Clear();

            Item[] consumableItems = myAdventure.GetInventory().GetConsumables(0);
            Item[] meleeItems = myAdventure.GetInventory().GetMelee(0);
            Item[] rangedItems = myAdventure.GetInventory().GetRanged(0);

            AddItemsToInventoryGrid(consumableItems);
            AddItemsToInventoryGrid(meleeItems);
            AddItemsToInventoryGrid(rangedItems);
        }

        private void AddItemsToInventoryGrid(Item[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                Item currentItem = items[i];

                StackPanel stackPanel = new StackPanel { Orientation = Orientation.Vertical };

                TextBlock nameTextBlock = new TextBlock();
                nameTextBlock.ToolTip = mainWindow.HeaderToolTipThemer(currentItem.GetName(), currentItem.ToString());

                if (selectedItemToSellButton != null && currentItem == selectedItemToSellButton.Tag)
                {
                    nameTextBlock.Inlines.AddRange(mainWindow.DecorateText("@916@" + currentItem.GetName() + " a la venta@"));
                }
                else
                {
                    nameTextBlock.Inlines.AddRange(mainWindow.DecorateText("@916@" + currentItem.GetName() + "@"));
                }

                ToolTipService.SetInitialShowDelay(nameTextBlock, 100);
                stackPanel.Children.Add(nameTextBlock);

                TextBlock valueTextBlock = new TextBlock();
                valueTextBlock.Inlines.AddRange(mainWindow.DecorateText("Valor: @2@" + currentItem.GetResellValue() + "$@"));
                valueTextBlock.ToolTip = "Valor de reventa";

                if (selectedItemToSellButton != null && currentItem == selectedItemToSellButton.Tag)
                {
                    valueTextBlock.FontSize = 20;
                }

                stackPanel.Children.Add(valueTextBlock);

                Button button = new Button
                {
                    Content = stackPanel,
                    BorderThickness = new Thickness(0),
                    Tag = currentItem
                };

                LinearGradientBrush linearBrush = new LinearGradientBrush
                {
                    StartPoint = new Point(0, 0),
                    EndPoint = new Point(1, 0)
                };

                linearBrush.GradientStops.Add(new GradientStop(mainWindow.GetBrushByNum(currentItem.GetRarityColor()).Color, 0));
                linearBrush.GradientStops.Add(new GradientStop(Colors.White, 0.3));
                linearBrush.GradientStops.Add(new GradientStop(Colors.White, 0.7));
                linearBrush.GradientStops.Add(new GradientStop(mainWindow.GetBrushByNum(currentItem.GetRarityColor()).Color, 1));

                button.Background = linearBrush;

                if (region.HasShop())
                {
                    button.Click += SelectSellItem;
                }


                Border border = new Border
                {
                    Margin = new Thickness(2, 8, 20, 8),
                    BorderThickness = new Thickness(3),
                    VerticalAlignment = VerticalAlignment.Center,
                    CornerRadius = new CornerRadius(5),
                    Child = button
                };

                if (selectedItemToSellButton != null && currentItem == selectedItemToSellButton.Tag)
                {
                    border.Padding = new Thickness(2);
                    border.BorderBrush = Brushes.Black;
                    border.Background = Brushes.Black;
                }
                else
                {
                    border.BorderBrush = mainWindow.GetBrushByNum(currentItem.GetRarityColor());
                }

                InventoryItems.Children.Add(border);
            }
        }

        private void OpenInventory(object sender, EventArgs e)
        {
            if (InventoryScroll.Visibility == Visibility.Visible)
            {
                InventoryScroll.Visibility = Visibility.Collapsed;
                InventoryButton.Content = "Abrir inventario";
                return;
            }
            InventoryScroll.Visibility = Visibility.Visible;
            InventoryButton.Content = "Cerrar inventario";
            UpdateInventoryGrid();
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
                if (currentHeroes[i].HasPointsToSpend())
                {
                    ComboBox comboBox = new ComboBox();
                    comboBox.Tag = currentHeroes[i];
                    comboBox.Margin = new Thickness(4, 5, 4, 1);

                    ComboBoxItem comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = "Fuerza bruta";
                    comboBoxItem.Tag = Stats.Stat.BruteForce;
                    comboBox.Items.Add(comboBoxItem);

                    comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = "Destreza";
                    comboBoxItem.Tag = Stats.Stat.Dexterity;
                    comboBox.Items.Add(comboBoxItem);

                    comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = "Constitución";
                    comboBoxItem.Tag = Stats.Stat.HP;
                    comboBox.Items.Add(comboBoxItem);

                    comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = "Resistencia";
                    comboBoxItem.Tag = Stats.Stat.Resistance;
                    comboBox.Items.Add(comboBoxItem);

                    comboBoxItem = new ComboBoxItem();
                    comboBoxItem.Content = "Agilidad";
                    comboBoxItem.Tag = Stats.Stat.Agility;
                    comboBox.Items.Add(comboBoxItem);

                    comboBox.SelectedIndex = 0;

                    Button button = new Button();
                    button.Content = "Mejorar estadística";
                    button.ToolTip = mainWindow.ToolTipThemer(currentHeroes[i].GetName()
                        + " tiene " + currentHeroes[i].GetXpPoints() + " puntos restantes");
                    button.Tag = comboBox;
                    button.Click += LevelHero;
                    button.Margin = new Thickness(5, 0, 5, 1);

                    stackPanel.Children.Add(comboBox);
                    stackPanel.Children.Add(button);
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
        private void LevelHero(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ComboBox comboBox = (ComboBox)button.Tag;
            Hero hero = (Hero)comboBox.Tag;
            ComboBoxItem comboBoxItem = (ComboBoxItem)comboBox.SelectedItem;
            Stats.Stat statToUpgrade = (Stats.Stat)comboBoxItem.Tag;

            hero.UpgradeStat(statToUpgrade);
            UpdatePlayerGrid();
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

            squadEditorTextBox.TextChanged -= OnlyNumsAndLetters;
            squadEditorTextBox.Text = myAdventure.GetCurrentSquadName();
            squadEditorTextBox.TextChanged += OnlyNumsAndLetters;
            Hero[] heroes = myAdventure.GetHeroes();
            int index = 0;
            for (int i = 0; i < (myAdventure.GetHeroesCount() / 2) + 1; i += 1)
            {
                if (heroes[index] != null)
                {
                    Debug.WriteLine("col created");

                    ColumnDefinition col = new ColumnDefinition();
                    col.Width = new GridLength(1, GridUnitType.Star);

                    SquadEditorItems.ColumnDefinitions.Add(col);

                    Button button = new Button();
                    button.ToolTip = mainWindow.HeaderToolTipThemer(heroes[index].GetName(), heroes[index].ToString());
                    ToolTipService.SetInitialShowDelay(button, 100);
                    button.HorizontalAlignment = HorizontalAlignment.Stretch;
                    button.MaxWidth = 300;
                    button.FontSize = 20;
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
                    button.MouseRightButtonUp += UnhireHero;
                    button.BorderBrush = new SolidColorBrush(Colors.Black);
                    button.BorderThickness = new Thickness(1);
                    button.Tag = heroes[index];
                    index += 1;
                    Grid.SetRow(button, 0);
                    Grid.SetColumn(button, i);

                    SquadEditorItems.Children.Add(button);
                    if (heroes[index] != null)
                    {
                        Button button2 = new Button();
                        button2.ToolTip = mainWindow.HeaderToolTipThemer(heroes[index].GetName(), heroes[index].ToString());
                        ToolTipService.SetInitialShowDelay(button2, 100);
                        button2.HorizontalAlignment = HorizontalAlignment.Stretch;
                        button2.MaxWidth = 300;
                        button2.FontSize = 20;
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
                        button2.MouseRightButtonUp += UnhireHero;
                        button2.BorderBrush = new SolidColorBrush(Colors.Black);
                        button2.BorderThickness = new Thickness(1);
                        button2.Tag = heroes[index];
                        index += 1;
                        Grid.SetRow(button2, 1);

                        Grid.SetColumn(button2, i);

                        SquadEditorItems.Children.Add(button2);
                    }
                    else
                    {
                        break;
                    }

                }
                else { break; }

            }
        }
        private void UnhireHero(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Hero hero = (Hero)button.Tag;
            if(myAdventure.GetHeroesCount() == 1 || myAdventure.IsInCurrentSquad(hero) && myAdventure.GetCurrentSquad().Length == 1)
            {
                MessageBox.Show("No puedes despedir a tu único héroe.");
                return;
            }
            if (MessageBox.Show("¿Quieres realmente echar a " + hero.GetName() + "?", "Echar a héroe", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes)
            {
                myAdventure.UnhireHero(hero);

                SquadAlignmentsCombobox.ItemsSource = null;
                SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys.ToList();
                SquadAlignmentsCombobox.SelectedItem = myAdventure.GetCurrentSquadName();
                UpdateSquadEditor(null,null);
                UpdatePlayerGrid();
                UpdateLog(hero.GetName() + " se marchó... Pero te dejó un regalo de @2@" + hero.GetCompetencyCost() + "@");
                UpdateCats();
            }

        }
        private void AddOrRemoveFromSquad(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Hero hero = (Hero)button.Tag;

            if (myAdventure.IsInCurrentSquad(hero))
            {
                if (myAdventure.GetCurrentSquad().Length == 1)
                {
                    MessageBox.Show("No puedes eliminar el último héroe de la escuadra");
                    return;
                }
                myAdventure.RemoveHeroFromSquad(hero);
            }
            else
            {
                if (myAdventure.GetCurrentSquad().Length < 4)
                {
                    myAdventure.AddHeroInSquad(hero);
                }
                else
                {
                    MessageBox.Show("No puedes hacer una alineación con más de 4 héroes");
                }
            }
            UpdateSquadEditor(null, null);
            UpdatePlayerGrid();
        }
        private void CreateSquad(object sender, EventArgs e)
        {

            string squadName = squadEditorTextBox.Text + " copia";
            if (myAdventure.GetSquadCount() < 10)
            {
                myAdventure.CreateSquad(squadName);
            }
            else
            {
                MessageBox.Show("No puedes crear más de 10 escuadrones");
                return;
            }

            SquadAlignmentsCombobox.ItemsSource = null;
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys;
            Debug.WriteLine("selected item : " + myAdventure.GetCurrentSquadName());
            SquadAlignmentsCombobox.SelectedItem = squadName;
            UpdatePlayerGrid();
            UpdateSquadEditor(null, null);
        }
        private void DeleteSquad(object sender, EventArgs e)
        {
            if (myAdventure.GetSquadCount() == 1)
            {
                MessageBox.Show("No puedes borrar tu única squad\nCrea otra primero.");
                return;
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
                squadEditorTextBox.Text = squadKey;
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

            UpdateTextbox(result);
        }
        private void UpdateTextbox(string result)
        {
            string squadKey = (string)SquadAlignmentsCombobox.SelectedItem;

            Hero[] squadToChangeName = myAdventure.GetSavedSquads()[squadKey];
            myAdventure.DeleteSquad(squadKey);
            myAdventure.CreateSquad(result);
            for (int i = 0; i < myAdventure.GetSquadCount(); i += 1)
            {
                Debug.WriteLine(myAdventure.GetSavedSquads().ElementAt(i));
            }
            SquadAlignmentsCombobox.ItemsSource = null;
            SquadAlignmentsCombobox.ItemsSource = myAdventure.GetSavedSquads().Keys;
            SquadAlignmentsCombobox.SelectedItem = result;
        }
    }
}
