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
using Microsoft.VisualBasic;
namespace Kenshi_DnD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Combat myCombat;
        PlayerInventory myInventory;
        Hero[] heroes;
        Monster[] monsters;
        ITurnable currentAttacker;
        public MainWindow()
        {
            InitializeComponent();
            Limb[] limbs = new Limb[4];
            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            
            Dice myDice = new Dice(6, 5);
            Race human = new Race("Humano", -1, 1, 0, 0, -1, 1);

            Hero hero1 = new Hero("Isaac", "El Sagaal", 7, 2, 10, 3, 10, human, human, limbs);
            Monster monster1 = new Monster("Ikran", 2, 40, 3, 2, 6, 100, 0);
            StatModifier genericStats = new StatModifier(5, 0, 0, 1, -1);
            StatModifier genericRangedStats = new StatModifier(2, 4, 0, 0, -2);
            myInventory = new PlayerInventory();
            myInventory.AddItem(new MeleeItem("Espada", 10, 2, 2, false, genericStats, false, false));
            myInventory.AddItem(new MeleeItem("Poción de fuerza", 6, 1, 1, false, genericStats, true, false));
            myInventory.AddItem(new RangedItem("Ballesta de principiante", 1, 15, 4, 3, genericStats, false));

            heroes = new Hero[]{ hero1 };
            monsters = new Monster[]{ monster1 };
            myCombat = new Combat(heroes, monsters, myDice, myInventory);

            currentAttacker = myCombat.GetCurrentAttacker();

            LoadTreeView(myInventory);
            UpdateFightersGrid();


        }
        private void NextTurnTest(object sender, RoutedEventArgs e)
        {
            //myCombat.NextTurn();
            myCombat.NextTurn();
            currentAttacker = myCombat.GetCurrentAttacker();
            UpdateFightersGrid();
            UpdateItemGrids(myInventory);
        }
        private void UpdateFightersGrid()
        {

            AllHeroes.Children.Clear();
            AllMonsters.Children.Clear();
            AllHeroes.RowDefinitions.Clear();
            AllMonsters.RowDefinitions.Clear();

            for (int i = 0; i < heroes.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllHeroes.RowDefinitions.Add(row);
            }
            for(int i = 0; i < monsters.Length; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                AllMonsters.RowDefinitions.Add(row);
            }

            for (int i = 0; i < heroes.Length; i++)
            {
                Label label = new Label();
                if(heroes[i].IsAlive())
                {
                    label.Content = heroes[i].GetName();
                    label.Background = new SolidColorBrush(Colors.LightGreen);
                }
                else
                {
                    label.Content = heroes[i].GetName() + " (Inconsciente)";
                    label.Background = new SolidColorBrush(Colors.DarkRed);
                }
                if (heroes[i] == currentAttacker)
                {
                    Debug.WriteLine("Current attacker is " + heroes[i].GetName());
                    label.Content = "[ " + label.Content + " ]";
                }
                label.HorizontalAlignment = HorizontalAlignment.Left;
                Grid.SetRow(label, i);

                AllHeroes.Children.Add(label);
            }
            for (int i = 0; i < monsters.Length; i++)
            {
                Label label = new Label();
                if (monsters[i].IsAlive())
                {
                    label.Content = monsters[i].GetName();
                    label.Background = new SolidColorBrush(Colors.PaleVioletRed);
                }
                else
                {
                    label.Content = monsters[i].GetName() + " (muerto)";
                    label.Background = new SolidColorBrush(Colors.DarkRed);
                }
                if (monsters[i] == currentAttacker)
                {
                    label.Content = "[ " + label.Content + " ]";
                }
                    label.HorizontalAlignment = HorizontalAlignment.Right;
                Grid.SetRow(label, i);
                AllMonsters.Children.Add(label);
            }

        }
        private void LoadTreeView(Inventory myInventory)
        {

            TreeViewItem root = new TreeViewItem();
            root.IsExpanded = true;
            root.Header = "Inventario";

            TreeViewItem ranged = new TreeViewItem();
            ranged.IsExpanded = true;
            ranged.Header = "Objetos a distancia";

            TreeViewItem melee = new TreeViewItem();
            melee.IsExpanded = true;
            melee.Header = "Objetos melee";

            TreeViewItem consumable = new TreeViewItem();
            consumable.IsExpanded = true;
            consumable.Header = "Consumibles";

            TreeViewItem notConsumable = new TreeViewItem();
            notConsumable.IsExpanded = true;
            notConsumable.Header = "No consumibles";

            root.Items.Add(ranged);
            root.Items.Add(melee);

            melee.Items.Add(notConsumable);
            melee.Items.Add(consumable);


            HeroItems.Items.Add(CloneTreeViewItem(root));

            Item[] rangedArray = myInventory.GetRanged(1);
            Item[] notConsumableArray = myInventory.GetMelee(1);
            Item[] consumableArray = myInventory.GetConsumables(1);

            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = rangedArray[i].GetName();
                item.Tag = rangedArray[i];
                ranged.Items.Add(item);
            }
            for (int i = 0; i < notConsumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = notConsumableArray[i].GetName();
                item.Tag = notConsumableArray[i];
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = consumableArray[i].GetName();
                item.Tag = consumableArray[i];
                consumable.Items.Add(item);
            }
            UnselectedItems.Items.Add(root);
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
                    UpdateItemGrids(myInventory);
                }
            }
        }
        
        
        private void UpdateItemGrids(PlayerInventory myInventory)
        {

            TreeViewItem root = (TreeViewItem)UnselectedItems.Items.GetItemAt(0);
            TreeViewItem ranged = (TreeViewItem)root.Items.GetItemAt(0);
            TreeViewItem melee = (TreeViewItem)root.Items.GetItemAt(1);
            TreeViewItem consumable = (TreeViewItem)melee.Items.GetItemAt(1);
            TreeViewItem notConsumable = (TreeViewItem)melee.Items.GetItemAt(0);

            Item[] rangedArray = myInventory.GetRanged(1);
            Item[] notConsumableArray = myInventory.GetMelee(1);
            Item[] consumableArray = myInventory.GetConsumables(1);

            ranged.Items.Clear();
            consumable.Items.Clear();
            notConsumable.Items.Clear();    

            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = rangedArray[i].GetName();
                item.Tag = rangedArray[i];
                ranged.Items.Add(item);
            }
            for (int i = 0; i < notConsumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = notConsumableArray[i].GetName();
                item.Tag = notConsumableArray[i];
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = consumableArray[i].GetName();
                item.Tag = consumableArray[i];
                consumable.Items.Add(item);
            }

            
            root = (TreeViewItem)HeroItems.Items.GetItemAt(0);
            ranged = (TreeViewItem)root.Items.GetItemAt(0);
            melee = (TreeViewItem)root.Items.GetItemAt(1);
            consumable = (TreeViewItem)melee.Items.GetItemAt(1);
            notConsumable = (TreeViewItem)melee.Items.GetItemAt(0);

            ranged.Items.Clear();
            consumable.Items.Clear();
            notConsumable.Items.Clear();
            if (currentAttacker is Monster)
            {
                return;
            }
            Hero currentHero = (Hero)currentAttacker;
            HeroInventory heroInventory = currentHero.GetInventory();
            // Update the items in the hero's inventory
            rangedArray = heroInventory.GetRanged(2);
            notConsumableArray = heroInventory.GetMelee(2);
            consumableArray = heroInventory.GetConsumables(2);

            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = rangedArray[i].GetName();
                item.Tag = rangedArray[i];
                ranged.Items.Add(item);
            }
            for (int i = 0; i < notConsumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = notConsumableArray[i].GetName();
                item.Tag = notConsumableArray[i];
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.MouseLeftButtonUp
                    += UseItem;
                item.Header = consumableArray[i].GetName();
                item.Tag = consumableArray[i];
                consumable.Items.Add(item);
            }
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