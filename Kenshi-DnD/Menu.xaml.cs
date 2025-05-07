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

            Limb[] limbs = new Limb[4];
            Limb[] limbs2 = new Limb[4];
            for (int i = 0; i < limbs.Length; i++)
            {
                limbs[i] = new Limb("Limb", 0, 0, 0, 0, 0, 0);
            }
            for (int i = 0; i < limbs2.Length; i++)
            {
                limbs2[i] = new Limb("Limb2", 3, 3, 0, 0, 0, 0);
            }
            Race human = new Race("Humano", -1, 1, 10, -1, 1);

            Hero hero1 = new Hero("Héroe bruto", "El PartePiedras", 10, 4, 1, 4, 5, human, human, limbs);

            this.cursors = cursors;
            this.controller = controller;
            this.mainWindow = mainWindow;
            Adventure adventure = new Adventure("Prueba", hero1, new Random());
            test.Inlines.Clear();
            test.Inlines.AddRange(mainWindow.DecorateText("Esto es @320@una@ prueba @2@HOLA@ que pasa"));
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            controller.Content = new CombatWindow(mainWindow, cursors);
        }
    }
}
