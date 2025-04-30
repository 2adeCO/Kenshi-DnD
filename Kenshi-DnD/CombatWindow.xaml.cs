using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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


            for (int i = 0; i < 2; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = i == 0 ? GridLength.Auto : new GridLength(1, GridUnitType.Star);
                AllHeroes.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < heroes.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllHeroes.RowDefinitions.Add(row);
            }

            for (int i = 0; i < 2; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = i == 0 ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
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
                if (heroes[i].IsAlive())
                {
                    button.Content = heroes[i].GetName();
                    button.Background = new SolidColorBrush(Colors.LightGreen);
                    button.Tag = heroes[i];
                }
                else
                {
                    button.Content = heroes[i].GetName() + " (Inconsciente)";
                    button.Background = new SolidColorBrush(Colors.DarkRed);
                }
                if (heroes[i] == currentAttacker)
                {
                    Debug.WriteLine("Current attacker is " + heroes[i].GetName());
                    button.Content = "[ " + button.Content + " ]";
                }
                button.ToolTip = heroes[i].ToString();
                button.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetRow(button, i);
                Grid.SetColumn(button, 0);
                AllHeroes.Children.Add(button);
            }
            for (int i = 0; i < heroes.Length; i++)
            {
                Border border = new Border();
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.IndianRed);
                border.Margin = new Thickness(5, 0, 0, 0);
                border.HorizontalAlignment = HorizontalAlignment.Left;

                ProgressBar progressBar = new ProgressBar();
                progressBar.Minimum = 0;
                progressBar.Maximum = heroes[i].GetToughness();
                progressBar.Value = heroes[i].GetHp();
                progressBar.Background = new SolidColorBrush(Colors.DarkGray);
                progressBar.Foreground = new SolidColorBrush(Colors.Red);
                progressBar.Height = 20;
                progressBar.Width = 150;
                progressBar.Margin = new Thickness(1, 1, 1, 1);
                progressBar.ToolTip = heroes[i].GetHp() + "/" + heroes[i].GetToughness();
                Grid.SetRow(border, i);
                Grid.SetColumn(border, 1);
                border.Child = progressBar;
                AllHeroes.Children.Add(border);
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                Button button = new Button();
                if (monsters[i].IsAlive())
                {
                    button.Content = monsters[i].GetName();
                    button.Background = new SolidColorBrush(Colors.PaleVioletRed);
                    button.Tag = monsters[i];
                    button.Click += SelectMonster;
                }
                else
                {
                    button.Content = monsters[i].GetName() + " (muerto)";
                    button.Background = new SolidColorBrush(Colors.DarkRed);
                }
                if (monsters[i] == currentAttacker)
                {
                    button.Content = "[ " + button.Content + " ]";
                }
                if (monsters[i] == monsterTarget)
                {
                    button.Content = "-->" + button.Content;
                }
                button.ToolTip = monsters[i].ToString();
                button.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(button, i);
                Grid.SetColumn(button, 1);
                AllMonsters.Children.Add(button);
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                Label label = new Label();
                label.Content = monsters[i].GetFaction().GetFactionName();
                label.HorizontalAlignment = HorizontalAlignment.Right;
                label.ToolTip = monsters[i].GetFaction().GetFactionDescription();
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);
                AllMonsters.Children.Add(label);
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
                    Tag = rangedArray[i]
                };
                item.MouseLeftButtonUp += UseItem;
                ranged.Items.Add(item);
            }
            for (int i = 0; i < meleeArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = meleeArray[i].GetName(),
                    Tag = meleeArray[i]
                };
                item.MouseLeftButtonUp += UseItem;
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = consumableArray[i].GetName(),
                    Tag = consumableArray[i]
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
                    Tag = rangedArray[i]
                };
                item.MouseLeftButtonUp += UseItem;
                ranged.Items.Add(item);
            }
            for (int i = 0; i < meleeArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = meleeArray[i].GetName(),
                    Tag = meleeArray[i]
                };
                item.MouseLeftButtonUp += UseItem;
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem
                {
                    Header = consumableArray[i].GetName(),
                    Tag = consumableArray[i]
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
            CombatTest.Content = message;
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
    }
}

