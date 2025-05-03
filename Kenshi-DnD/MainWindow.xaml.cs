using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Schema;
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
            PageController.Content = new Menu(this,PageController, cursors);
        }
        public void GoToCombat(object sender, EventArgs e)
        {
            PageController.Content = new CombatWindow(this, cursors);
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
        public List<Inline> DecorateText(string msg)
        {
            string txt = "";
            List<Inline> inlines = new List<Inline>();
            Run inlineRun;
            bool foundDecoration = false;
            bool startDecoration = false;
            int color = 0;

            for (int i = 0; i < msg.Length; i++)
            {
                if (!foundDecoration && ! startDecoration)
                {
                    if (msg[i] == '@')
                    {
                        foundDecoration = true;
                    }
                    else
                    {
                        txt += msg[i];
                    }
                }
                else
                {
                    if (!startDecoration)
                    {
                        foundDecoration = false;
                        color = int.Parse(msg[i].ToString());
                        startDecoration = true;
                        inlineRun = new Run();
                        inlineRun.Text = txt;
                        inlines.Add(inlineRun);
                        txt = "";
                    }
                    else
                    {
                        if (msg[i] != '@')
                        { 
                            txt += msg[i];
                        }
                        else
                        {
                            startDecoration = false;

                            inlineRun = new Run();
                            inlineRun.Text = txt;
                            inlineRun.FontWeight = FontWeights.Bold;
                            txt = "";
                            inlineRun.Foreground = GetBrushByNum(color);
                            inlines.Add(inlineRun);
                        }
                    }
                    
                }    
            }
            inlineRun = new Run();
            inlineRun.Text = txt;
            inlines.Add(inlineRun);
            return inlines;
        }
        public SolidColorBrush GetBrushByNum(int num)
        {
            switch (num)
            {
                case 1:
                    {
                        return Brushes.Red;
                    }
                case 2:
                    {
                        return Brushes.Green;
                    }
                case 3:
                    {
                        return Brushes.Blue;
                    }
                case 4:
                    {
                        return Brushes.Magenta;
                    }
                case 5:
                    {
                        return Brushes.Yellow;
                    }
                case 6:
                    {
                        return Brushes.Orange;
                    }
                case 7:
                    {
                        return Brushes.WhiteSmoke;
                    }
                case 8:
                    {
                        return Brushes.Gray;
                    }
                default:
                    {
                        return Brushes.Black;
                    }
            }
        }
    }
}