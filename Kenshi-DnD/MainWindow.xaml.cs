using System.Windows;
namespace Kenshi_DnD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            PageController.Content = new Menu(PageController);
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            PageController.Content = new CombatWindow();

        }
    }
}