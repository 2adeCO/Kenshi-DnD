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
namespace Kenshi_DnD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Combat myCombat;
        public MainWindow()
        {
            InitializeComponent();
            Limb[] limbs = new Limb[4];
            Dice myDice = new Dice(6, 5);
            Race human = new Race("Humano", -1, 1, 0, 0, -1, 1);
            Hero hero1 = new Hero("Isaac", "El Sagaal", 7, 2, 10, 3, 10, human, human, limbs);
            Monster monster1 = new Monster("Ikran", 2, 40, 3, 2, 6, 100, 0);

            Hero[] heroes = { hero1 };
            Monster[] monsters = { monster1 };
            Inventory myInventory = new Inventory();

            myCombat = new Combat(heroes, monsters, myDice, myInventory);

            myCombat.NextTurn();
        }
        private void NextTurnTest(object sender, RoutedEventArgs e)
        {
            //myCombat.NextTurn();
            myCombat.NextTurn();
        }
        private void UpdateItemGrids(Inventory myInventory)
        {
            //for (int i = 0; i < myInventory; i++)
            //{

            //}
        }
    }
}