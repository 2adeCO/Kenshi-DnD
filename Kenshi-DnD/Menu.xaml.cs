using System.Windows.Controls;
using System.Windows.Input;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        ContentControl controller;
        Cursor[] cursors;
        public Menu(ContentControl controller, Cursor[] cursors)
        {
            InitializeComponent();
            this.cursors = cursors;
            this.controller = controller;
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            controller.Content = new CombatWindow(cursors);
        }
    }
}
