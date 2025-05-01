using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
namespace Kenshi_DnD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Cursor[] cursors;
        public MainWindow()
        {
            InitializeComponent();
            LoadCursors();
            PageController.Content = new Menu(PageController, cursors);
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            PageController.Content = new CombatWindow(cursors);
        }
        public void LoadCursors()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());
            string[] cursorCurFiles = Directory.GetFiles("./Resources/cursors", "*.cur");
            string[] cursorAniFiles = Directory.GetFiles("./Resources/cursors", "*.ani");
            int arrayNum = cursorCurFiles.Length + cursorAniFiles.Length;
            string[] cursorFiles = new string[arrayNum];

            Debug.WriteLine($"Loading {arrayNum} cursors...");
            for (int i = 0; i < cursorCurFiles.Length; i++)
            {
                cursorFiles[i] = cursorCurFiles[i];
            }
            for (int i = 0; i < cursorAniFiles.Length; i++)
            {
                cursorFiles[i + cursorCurFiles.Length] = cursorAniFiles[i];
            }


            cursors = new Cursor[arrayNum];
            for (int i = 0; i < arrayNum; i++)
            {
                Debug.WriteLine($"Loading cursor: {cursorFiles[i]}");
                cursors[i] = new Cursor(cursorFiles[i]);
            }
        }
    }
}