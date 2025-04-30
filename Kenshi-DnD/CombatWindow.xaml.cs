using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para UserControl.xaml
    /// </summary>
    public partial class CombatWindow : UserControl
    {
        MainWindow mainWindow;
        DispatcherTimer timer;
        int seconds;
        Combat myCombat;
        Monster monsterTarget;
        PlayerInventory myInventory;
        Hero[] heroes;
        Monster[] monsters;
        ITurnable currentAttacker;
        public CombatWindow()
        {
            InitializeComponent();
            //Starts a timer
            seconds = 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();


            Limb[] limbs = new Limb[4];
            Limb[] limbs2 = new Limb[4];
            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            for (int i = 0; i < limbs2.Length; i++)
            {
                limbs2[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            Faction faction1 = new Faction(1, "DAW", "Dawer");
            Dice myDice = new Dice(6, 5);
            Race human = new Race("Humano", -1, 1, 0, 0, -1, 1);

            Hero hero1 = new Hero("Héroe bruto", "El PartePiedras", 10, 4, 1, 4, 5, human, human, limbs);
            Hero hero2 = new Hero("Héroe habilidoso", "El Arquero", 8, 2, 2, 3, 8, human, human, limbs2);
            Monster monster1 = new Monster("Monstruo genérico", 3, faction1, 10, 3, 30, -1, 100, 0);
            StatModifier genericStats = new StatModifier(5, 0, 0, 1, -1);
            StatModifier genericRangedStats = new StatModifier(2, 2, 0, 0, -2);
            myInventory = new PlayerInventory();
            myInventory.AddItem(new MeleeItem("Espada", 10, 2, 2, false, genericStats, false, false));
            myInventory.AddItem(new MeleeItem("Poción de fuerza", 6, 1, 1, false, genericStats, true, false));
            myInventory.AddItem(new RangedItem("Ballesta de principiante", 2, 10, 15, 4, 3, genericRangedStats, false));

            heroes = new Hero[] { hero1, hero2 };
            monsters = new Monster[] { monster1 };
            myCombat = new Combat(heroes, monsters, myDice, myInventory, this);

            currentAttacker = myCombat.GetCurrentAttacker();

            FillItemTrees();
            UpdateGameStateUI();
            UpdateFightersGrid();


            if (currentAttacker is Monster)
            {
                DispatcherTimer timeToAttack = new DispatcherTimer();
                timeToAttack.Interval = TimeSpan.FromMilliseconds(600);
                timeToAttack.Tick += MonsterAttackTimer_Tick;
                timeToAttack.Start();
            }

            this.mainWindow = mainWindow;
        }
        private void NextTurn(object sender, RoutedEventArgs e)
        {
            if (myCombat.GetGameState() != 0)
            {
                UpdateGameStateUI();
                MessageBox.Show("Ya terminó el combate");
                return;
            }
            if (currentAttacker is Hero && monsterTarget == null)
            {
                MessageBox.Show("Selecciona un monstruo para atacar");
                return;
            }
            myCombat.NextTurn(monsterTarget);
            currentAttacker = myCombat.GetCurrentAttacker();

            UpdateFightersGrid();
            FillItemTrees();
            if (myCombat.GetGameState() != 0)
            {
                UpdateGameStateUI();
                MessageBox.Show("Ya terminó el combate");
                return;
            }
            if (currentAttacker is Monster)
            {
                DispatcherTimer timeToAttack = new DispatcherTimer();
                timeToAttack.Interval = TimeSpan.FromMilliseconds(600);
                timeToAttack.Tick += MonsterAttackTimer_Tick;
                timeToAttack.Start();
            }
        }
        private void UpdateFightersGrid()
        {

            AllHeroes.Children.Clear();
            AllMonsters.Children.Clear();
            AllHeroes.RowDefinitions.Clear();
            AllMonsters.RowDefinitions.Clear();
            AllHeroes.ColumnDefinitions.Clear();
            AllMonsters.ColumnDefinitions.Clear();


            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                AllHeroes.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < heroes.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllHeroes.RowDefinitions.Add(row);
            }

            for (int i = 0; i < 1; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = new GridLength(1, GridUnitType.Star);
                AllMonsters.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllMonsters.RowDefinitions.Add(row);
            }

            for (int i = 0; i < heroes.Length; i++)
            {
                Button button = new Button();
                //Put content with the hero name and a progress bar with its hp
                button.Content = FighterStackPanel(heroes[i]);
                //The hero object is stored in the button's tag
                button.Tag = heroes[i];
                //This will show the user who the current attacker is                
                button.ToolTip = ToolTipThemer(heroes[i].ToString());
                ToolTipService.SetInitialShowDelay(button, 100);

                button.HorizontalAlignment = HorizontalAlignment.Left;
                if(heroes[i] == currentAttacker)
                {
                    Border border = PutBorderOnCurrentAttacker(button);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, 0);
                    AllHeroes.Children.Add(border);
                }
                else
                {
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, 0);
                    AllHeroes.Children.Add(button);
                }    
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                Button button = new Button();

                button.Content = FighterStackPanel(monsters[i]);


                if (monsters[i].IsAlive())
                {
                    button.Tag = monsters[i];
                    button.Click += SelectMonster;
                }
                
                button.HorizontalAlignment = HorizontalAlignment.Right;

                if (monsters[i] == currentAttacker)
                {
                    Border border = PutBorderOnCurrentAttacker(button);
                    Grid.SetRow(border, i);
                    Grid.SetColumn(border, 0);
                    AllMonsters.Children.Add(border);
                }
                else
                {
                    Grid.SetRow(button, i);
                    Grid.SetColumn(button, 0);
                    AllMonsters.Children.Add(button);
                }
            }

        }
        private void FillItemTrees()
        {

            UnselectedItems.Items.Clear();
            HeroItems.Items.Clear();

            TreeViewItem root = GenerateTreeView();
            TreeViewItem ranged = (TreeViewItem)root.Items[0];
            TreeViewItem melee = (TreeViewItem)root.Items[1];
            TreeViewItem notConsumable = (TreeViewItem)melee.Items[0];
            TreeViewItem consumable = (TreeViewItem)melee.Items[1];

            Item[] rangedArray = myInventory.GetRanged(1);
            Item[] meleeArray = myInventory.GetMelee(1);
            Item[] consumableArray = myInventory.GetConsumables(1);

            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = rangedArray[i].GetName(),
                    Tag = rangedArray[i],
                    ToolTip = ToolTipThemer(rangedArray[i].ToString())
                };
                item.MouseLeftButtonUp += UseItem;
                ranged.Items.Add(item);
            }
            for (int i = 0; i < meleeArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = meleeArray[i].GetName(),
                    Tag = meleeArray[i],
                    ToolTip = ToolTipThemer(meleeArray[i].ToString())
                };
                item.MouseLeftButtonUp += UseItem;
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = consumableArray[i].GetName(),
                    Tag = consumableArray[i],
                    ToolTip = ToolTipThemer(consumableArray[i].ToString())
                };
                item.MouseLeftButtonUp += UseItem;
                consumable.Items.Add(item);
            }
            UnselectedItems.Items.Add(root);

            root = GenerateTreeView();

            ranged = (TreeViewItem)root.Items[0];
            melee = (TreeViewItem)root.Items[1];
            notConsumable = (TreeViewItem)melee.Items[0];
            consumable = (TreeViewItem)melee.Items[1];

            if (currentAttacker is Monster)
            {
                return;
            }
            Hero currentHero = (Hero)currentAttacker;

            HeroInventory heroInventory = currentHero.GetInventory();
            rangedArray = heroInventory.GetRanged(2);
            meleeArray = heroInventory.GetMelee(2);
            consumableArray = heroInventory.GetConsumables(2);
            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = rangedArray[i].GetName(),
                    Tag = rangedArray[i],
                    ToolTip = ToolTipThemer(rangedArray[i].ToString())
                };
                item.MouseLeftButtonUp += UseItem;
                ranged.Items.Add(item);
            }
            for (int i = 0; i < meleeArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = meleeArray[i].GetName(),
                    Tag = meleeArray[i],
                    ToolTip = ToolTipThemer(meleeArray[i].ToString())
                };
                item.MouseLeftButtonUp += UseItem;
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = consumableArray[i].GetName(),
                    Tag = consumableArray[i],
                    ToolTip = ToolTipThemer(consumableArray[i].ToString())
                };
                item.MouseLeftButtonUp += UseItem;
                consumable.Items.Add(item);
            }
            HeroItems.Items.Add(root);

        }
        private TreeViewItem GenerateTreeView()
        {
            TreeViewItem root = new TreeViewItem { Header = "Inventario", IsExpanded = true };

            TreeViewItem ranged = new TreeViewItem { Header = "Objetos a distancia", IsExpanded = true };
            TreeViewItem melee = new TreeViewItem { Header = "Objetos melee", IsExpanded = true };

            TreeViewItem notConsumable = new TreeViewItem { Header = "No consumibles", IsExpanded = true };
            TreeViewItem consumable = new TreeViewItem { Header = "Consumibles", IsExpanded = true };

            melee.Items.Add(notConsumable);
            melee.Items.Add(consumable);

            root.Items.Add(ranged);
            root.Items.Add(melee);

            return root;
        }
        public void UseItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)sender;
            if (selectedItem != null && selectedItem.Tag is Item item)
            {
                Debug.WriteLine("Selected item: " + item.GetName());
                if (currentAttacker is Hero)
                {
                    Debug.WriteLine("Current attacker is a hero");
                    Hero currentHero = (Hero)currentAttacker;
                    if (IsParentX(selectedItem, HeroItems))
                    {
                        Debug.WriteLine("Trying to remove item");
                        RemoveItemFromHero(currentHero, item);
                    }
                    else
                    {
                        Debug.WriteLine("Trying to use item");
                        if (item.CanUseItem(currentHero))
                        {
                            Debug.WriteLine("Item added!");
                            AddItemToHero(currentHero, item);
                        }
                    }
                    FillItemTrees();
                    UpdateFightersGrid();
                }
            }
        }
        private void MonsterAttackTimer_Tick(object sender, EventArgs e)
        {
            DispatcherTimer timer = (DispatcherTimer)sender;
            timer.Stop();
            if (myCombat.GetGameState() != 0)
            {
                MessageBox.Show("Ya terminó el combate");
                UpdateGameStateUI();
                return;
            }
            NextTurn(null, null);
        }
        private void SelectMonster(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            monsterTarget = (Monster)button.Tag;
            UpdateFightersGrid();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            seconds += 1;
            string time;
            if (seconds > 3599)
            {
                time = TimeSpan.FromSeconds(seconds).ToString(@"hh\:mm\:ss");
            }
            else
            {
                time = TimeSpan.FromSeconds(seconds).ToString(@"mm\:ss");
            }
            TimerUI.Content = time;
        }
        private void AddItemToHero(Hero hero, Item item)
        {
            hero.AddItemToInventory(item);
        }
        private void RemoveItemFromHero(Hero hero, Item item)
        {
            hero.RemoveItemFromInventory(item);
        }
        public void UpdateLogUI(string message)
        {
            CurrentCombatInfo.Content = message;
            InfoLog.Text += message + "\n";
        }
        private void UpdateGameStateUI()
        {
            int gameState = myCombat.GetGameState();

            switch (gameState)
            {

                case 0:
                    {
                        GameStateUI.Content = "En combate";
                        break;
                    }
                case 1:
                    {
                        GameStateUI.Content = "¡Combate ganado!";
                        timer.Stop();
                        break;
                    }
                case -1:
                    {
                        GameStateUI.Content = "¡Has sido abandonado a tu suerte!";
                        timer.Stop();
                        break;
                    }
            }
        }
        private TreeViewItem CloneTreeViewItem(TreeViewItem original)
        {
            if (original == null)
            {
                return null;
            }

            TreeViewItem copy = new TreeViewItem
            {
                Header = original.Header,
                Tag = original.Tag,
                IsExpanded = original.IsExpanded
            };

            for (int i = 0; i < original.Items.Count; i++)
            {
                TreeViewItem child = (TreeViewItem)original.Items[i];
                TreeViewItem childCopy = CloneTreeViewItem(child);
                copy.Items.Add(childCopy);
            }

            return copy;
        }
        private bool IsParentX(TreeViewItem item, TreeView parentToCheck)
        {
            do
            {
                if (item.Parent is TreeViewItem)
                {
                    item = (TreeViewItem)item.Parent;
                }
                else
                {
                    if (item.Parent == parentToCheck)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            } while (item.Parent != null);
            return false;
        }
        private StackPanel FighterStackPanel(Monster monster)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = monster.GetName();
            label.ToolTip = ToolTipThemer(monster.ToString());
            ToolTipService.SetInitialShowDelay(label, 100);
            stackPanel.Children.Add(label);


            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1,GridUnitType.Auto)});
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto)});


            label = new System.Windows.Controls.Label();
            label.Content = monster.GetFaction().GetFactionName();
            label.ToolTip = ToolTipThemer(monster.GetFaction().GetFactionDescription());
            ToolTipService.SetInitialShowDelay(label, 100);
            Grid.SetColumn(label, 1);
            grid.Children.Add(label);
            if (monster.GetFaction().GetFactionImage() != null)
            {
                BitmapImage bitmapImage  
                = monster.GetFaction().GetFactionImage();
                Image image = new Image();
                image.Source = bitmapImage;
                Grid.SetColumn(image, 0);
                grid.Children.Add(image);
            }
            if (!monster.IsAlive())
            {
                label = new System.Windows.Controls.Label();
                label.Content = "Muerto";
                label.ToolTip = ToolTipThemer("Podrías haber sido tú");
                ToolTipService.SetInitialShowDelay(label, 100);
                grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                Grid.SetColumn(label, 2);
                grid.Children.Add(label);
            }
            else
            {
                if(monster == monsterTarget)
                {
                    stackPanel.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#8a7327"));
                }
            }


                stackPanel.Children.Add(grid);

            return stackPanel;
        }
        private StackPanel FighterStackPanel(Hero hero)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Vertical;
            System.Windows.Controls.Label label = new System.Windows.Controls.Label();
            label.Content = hero.GetName();
            label.ToolTip = ToolTipThemer(hero.ToString());
            ToolTipService.SetInitialShowDelay(label, 100);
            stackPanel.Children.Add(label);

            if (hero.IsAlive())
            {
                ProgressBar progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = hero.GetToughness();
                progressBar.Value = hero.GetHp();
                progressBar.Background = new SolidColorBrush(Colors.DarkGray);
                progressBar.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#69b14f"));
                progressBar.Height = 20;
                progressBar.Width = 150;
                progressBar.Margin = new Thickness(1, 1, 1, 1);

                progressBar.ToolTip = ToolTipThemer(hero.GetHp() + "/" + hero.GetToughness());
                ToolTipService.SetInitialShowDelay(progressBar, 100);
                stackPanel.Children.Add(progressBar);
            }
            else
            {
                System.Windows.Controls.Label deadLabel = new System.Windows.Controls.Label();
                deadLabel.Content = "Inconsciente...";
                deadLabel.ToolTip = ToolTipThemer("Solo una buena cura podrá salvarlo ahora...\n \nEsperemos ganes el combate");
                deadLabel.HorizontalAlignment = HorizontalAlignment.Center;
                stackPanel.Children.Add(deadLabel);
            }
            

            return stackPanel;
        }
        private Border PutBorderOnCurrentAttacker(Button button)
        {
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Red);
            border.BorderThickness = new Thickness(2);
            border.CornerRadius = new CornerRadius(5);
            button.Padding = new Thickness(2);
            border.HorizontalAlignment = (button.Tag is Hero? HorizontalAlignment.Left: HorizontalAlignment.Right);
            border.Child = button;
            return border;
        }
        private ToolTip ToolTipThemer(string content)
        {
            ToolTip toolTip = new ToolTip();
            toolTip.FontSize = 20;
            toolTip.Content = content;
            toolTip.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#e6e5d5"));
            toolTip.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#2b2b2b"));
            return toolTip;
        }
    }
}

