using System;
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
        Inventory myInventory;
        public MainWindow()
        {
            InitializeComponent();
            Limb[] limbs = new Limb[4];
            Dice myDice = new Dice(6, 5);
            Race human = new Race("Humano", -1, 1, 0, 0, -1, 1);
            Hero hero1 = new Hero("Isaac", "El Sagaal", 7, 2, 10, 3, 10, human, human, limbs);
            Monster monster1 = new Monster("Ikran", 2, 40, 3, 2, 6, 100, 0);
            StatModifier genericStats = new StatModifier(5, 0, 0, 1, -1);
            StatModifier genericRangedStats = new StatModifier(2, 4, 0, 0, -2);
            myInventory = new Inventory();
            myInventory.AddItem(new MeleeItem("Espada", 10, 2, 2, false, genericStats, false, false));
            myInventory.AddItem(new MeleeItem("Poción de fuerza", 6, 1, 1, false, genericStats, true, false));
            myInventory.AddItem(new RangedItem("Ballesta de principiante", 1, 15, 4, 3, genericStats, false));

            Hero[] heroes = { hero1 };
            Monster[] monsters = { monster1 };

            
            LoadTreeView(myInventory);


            myCombat = new Combat(heroes, monsters, myDice, myInventory);
            
            myCombat.NextTurn();
        }
        private void NextTurnTest(object sender, RoutedEventArgs e)
        {
            //myCombat.NextTurn();
            myCombat.NextTurn();
        }
        private void LoadTreeView(Inventory myInventory)
        {

            TreeViewItem root = new TreeViewItem();
            root.Header = "Inventario";

            TreeViewItem ranged = new TreeViewItem();
            ranged.Header = "Objetos a distancia";

            TreeViewItem melee = new TreeViewItem();
            melee.Header = "Objetos melee";

            TreeViewItem consumable = new TreeViewItem();
            consumable.Header = "Consumibles";

            TreeViewItem notConsumable = new TreeViewItem();
            notConsumable.Header = "No consumibles";

            root.Items.Add(ranged);
            root.Items.Add(melee);

            melee.Items.Add(notConsumable);
            melee.Items.Add(consumable);


            SelectedItemsGrid.Items.Add(CloneTreeViewItem(root));

            Item[] rangedArray = myInventory.GetRanged(1);
            Item[] notConsumableArray = myInventory.GetMelee(1);
            Item[] consumableArray = myInventory.GetConsumables(1);

            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = rangedArray[i].GetName();
                item.Tag = rangedArray[i];
                ranged.Items.Add(item);
            }
            for (int i = 0; i < notConsumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = notConsumableArray[i].GetName();
                item.Tag = notConsumableArray[i];
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = consumableArray[i].GetName();
                item.Tag = consumableArray[i];
                consumable.Items.Add(item);
            }
            UnSelectedItemsGrid.Items.Add(root);

        }
        public void TestRemoveItem(object sender, RoutedEventArgs e)
        {
            TreeViewItem selectedItem = (TreeViewItem)UnSelectedItemsGrid.SelectedItem;
            if (selectedItem != null && selectedItem.Tag is Item item)
            {
                // Remove the item from the inventory
                myInventory.SellItem(item);
                // Update the TreeView
                UpdateItemGrids(myInventory);
            }
        }
        private void UpdateItemGrids(Inventory myInventory)
        {

            TreeViewItem root = (TreeViewItem)UnSelectedItemsGrid.Items.GetItemAt(0);
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
                item.Header = rangedArray[i].GetName();
                item.Tag = rangedArray[i];
                ranged.Items.Add(item);
            }
            for (int i = 0; i < notConsumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = notConsumableArray[i].GetName();
                item.Tag = notConsumableArray[i];
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = consumableArray[i].GetName();
                item.Tag = consumableArray[i];
                consumable.Items.Add(item);
            }


            root = (TreeViewItem)SelectedItemsGrid.Items.GetItemAt(0);
            ranged = (TreeViewItem)root.Items.GetItemAt(0);
            melee = (TreeViewItem)root.Items.GetItemAt(1);
            consumable = (TreeViewItem)melee.Items.GetItemAt(1);
            notConsumable = (TreeViewItem)melee.Items.GetItemAt(0);

            rangedArray = myInventory.GetRanged(2);
            consumableArray = myInventory.GetMelee(2);
            notConsumableArray = myInventory.GetConsumables(2);

            ranged.Items.Clear();
            consumable.Items.Clear();
            notConsumable.Items.Clear();

            for (int i = 0; i < rangedArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = rangedArray[i].GetName();
                item.Tag = rangedArray[i];
                ranged.Items.Add(item);
            }
            for (int i = 0; i < notConsumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = notConsumableArray[i].GetName();
                item.Tag = notConsumableArray[i];
                notConsumable.Items.Add(item);
            }
            for (int i = 0; i < consumableArray.Length; i++)
            {
                TreeViewItem item = new TreeViewItem();
                item.Header = consumableArray[i].GetName();
                item.Tag = consumableArray[i];
                consumable.Items.Add(item);
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
    }
}