using System.Windows.Controls;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        ContentControl controller;
        public Menu(ContentControl controller)
        {
            InitializeComponent();
            this.controller = controller;
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            controller.Content = new CombatWindow();
        }
    }
}
