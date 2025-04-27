using System;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.VisualBasic;
namespace Kenshi_DnD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer;
        int seconds;
        Combat myCombat;
        Monster monsterTarget;
        PlayerInventory myInventory;
        Hero[] heroes;
        Monster[] monsters;
        ITurnable currentAttacker;
        public MainWindow()
        {
            InitializeComponent();
            //Starts a timer
            seconds = 0;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            timer.Start();


            Limb[] limbs = new Limb[4];
            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            
            Dice myDice = new Dice(6, 5);
            Race human = new Race("Humano", -1, 1, 0, 0, -1, 1);

            Hero hero1 = new Hero("Isaac", "El Sagaal",10,4,2,4,5, human, human, limbs);
            Monster monster1 = new Monster("Ikran", 3, 10, 3, 6, -1, 100, 0);
            StatModifier genericStats = new StatModifier(5, 0, 0, 1, -1);
            StatModifier genericRangedStats = new StatModifier(2, 2, 0, 0, -2);
            myInventory = new PlayerInventory();
            myInventory.AddItem(new MeleeItem("Espada", 10, 2, 2, false, genericStats, false, false));
            myInventory.AddItem(new MeleeItem("Poción de fuerza", 6, 1, 1, false, genericStats, true, false));
            myInventory.AddItem(new RangedItem("Ballesta de principiante", 2,10, 15, 4, 3, genericRangedStats, false));

            heroes = new Hero[]{ hero1 };
            monsters = new Monster[]{ monster1 };
            myCombat = new Combat(heroes, monsters, myDice, myInventory);

            currentAttacker = myCombat.GetCurrentAttacker();

            FillItemTrees();
            UpdateFightersGrid();


            if (currentAttacker is Monster)
            {
                DispatcherTimer timeToAttack = new DispatcherTimer();
                timeToAttack.Interval = TimeSpan.FromMilliseconds(600);
                timeToAttack.Tick += MonsterAttackTimer_Tick;
                timeToAttack.Start();
            }
            

        }
        private void NextTurnTest(object sender, RoutedEventArgs e)
        {
            //myCombat.NextTurn();
            if (currentAttacker is Hero && monsterTarget == null)
            {
                MessageBox.Show("Selecciona un monstruo para atacar");
                return;
            }
            myCombat.NextTurn(monsterTarget);
            currentAttacker = myCombat.GetCurrentAttacker();
            UpdateFightersGrid();
            FillItemTrees();
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
                column.Width = i == 0 ? GridLength.Auto : new GridLength(1,GridUnitType.Star);
                AllHeroes.ColumnDefinitions.Add(column);
            }
            for (int i = 0; i < heroes.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllHeroes.RowDefinitions.Add(row);
            }

            for(int i = 0; i < 2; i++)
            {
                ColumnDefinition column = new ColumnDefinition();
                column.Width = i == 0 ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
                AllMonsters.ColumnDefinitions.Add(column);
            }
            for(int i = 0; i < monsters.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllMonsters.RowDefinitions.Add(row);
            }

            for (int i = 0; i < heroes.Length; i++)
            {
                Button button = new Button();
                if(heroes[i].IsAlive())
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

            if(currentAttacker is Monster)
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
                    if(IsParentX(selectedItem,HeroItems))
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
            NextTurnTest(null, null);
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
            }else
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