using System.Windows.Controls;
using System.Windows.Input;

namespace Kenshi_DnD
{
    /// <summary>
    /// Lógica de interacción para Menu.xaml
    /// </summary>
    public partial class Menu : UserControl
    {
        MainWindow mainWindow;
        ContentControl controller;
        Cursor[] cursors;
        public Menu(MainWindow mainWindow, ContentControl controller, Cursor[] cursors)
        {
            InitializeComponent();
            this.cursors = cursors;
            this.controller = controller;
            this.mainWindow = mainWindow;
            test.Inlines.Clear();
            test.Inlines.AddRange(mainWindow.DecorateText("Esto es @3una@ prueba @2HOLA@ que pasa"));
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            controller.Content = new CombatWindow(mainWindow, cursors);
        }
    }
}
