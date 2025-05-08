using System.ComponentModel.DataAnnotations;
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
        Random rnd;
        public MainWindow()
        {
            InitializeComponent();
            LoadCursors();
            rnd = new Random();
            PageController.Content = new Menu(this,PageController, cursors, rnd);
        }
        public void LoadCursors()
        {
            Debug.WriteLine(Directory.GetCurrentDirectory());
            string[] cursorCurFiles = Directory.GetFiles("./Resources/cursors", "*.cur");
            string[] cursorAniFiles = Directory.GetFiles("./Resources/cursors", "*.ani");
            int arrayNum = cursorCurFiles.Length + cursorAniFiles.Length;
            string[] cursorFiles = new string[arrayNum];

            Debug.WriteLine($"Loading {arrayNum} cursors...");
            for (int i = 0; i < cursorCurFiles.Length; i+=1)
            {
                cursorFiles[i] = cursorCurFiles[i];
            }
            for (int i = 0; i < cursorAniFiles.Length; i+=1)
            {
                cursorFiles[i + cursorCurFiles.Length] = cursorAniFiles[i];
            }


            cursors = new Cursor[arrayNum];
            for (int i = 0; i < arrayNum; i+=1)
            {
                Debug.WriteLine($"Loading cursor: {cursorFiles[i]}");
                cursors[i] = new Cursor(cursorFiles[i]);
            }
        }
        public List<Inline> DecorateText(string message)
        {
            string txt = "";
            List<Inline> inlines = new List<Inline>();
            Run inlineRun;
            bool foundDecoration = false;
            bool startDecoration = false;
            int size = 0;
            int color = 0;

            for (int i = 0; i < message.Length; i+=1)
            {
                //If it's normal text, just write it normally
                if (!foundDecoration && ! startDecoration)
                {
                    //If the special character is found, then save the bool
                    if (message[i] == '@')
                    {
                        foundDecoration = true;
                    }
                    else
                    {
                        //Write it
                        txt += message[i];
                    }
                }
                else
                {
                    //Because the decoration has been found, see what to do next

                    //If just found the decoration, then read it
                    if (!startDecoration)
                    {
                        foundDecoration = false;
                        startDecoration = true;
                        color = int.Parse(message[i].ToString());
                        inlineRun = new Run();
                        inlineRun.Text = txt;
                        inlines.Add(inlineRun);
                        txt = "";
                        i += 1;
                        while (message[i] != '@')
                        {
                            txt += message[i];
                            i += 1;
                        }
                        if (txt != "")
                        {
                            size = int.Parse(txt);
                        }
                        txt = "";
                    }
                    else
                    {
                        //If the decoration is known, then, start it until the special character is found again
                        if (message[i] != '@')
                        { 
                            txt += message[i];
                        }
                        else
                        {
                            startDecoration = false;

                            inlineRun = new Run();
                            inlineRun.Text = txt;
                            inlineRun.FontWeight = FontWeights.Bold;
                            inlineRun.Foreground = GetBrushByNum(color);
                            if(size != 0)
                            {
                                inlineRun.FontSize = size;
                            }

                            inlines.Add(inlineRun);

                            txt = "";
                            size = 0;
                        }
                    }
                    
                }    
            }
            //If some text at the end of the loop and not special, these lines of code will save them
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
                        //Normal blue is really hard to read
                        return Brushes.CornflowerBlue;
                    }
                case 4:
                    {
                        return Brushes.Magenta;
                    }
                case 5:
                    {
                        return Brushes.Goldenrod;
                    }
                case 6:
                    {
                        return Brushes.Orange;
                    }
                case 7:
                    {
                        return Brushes.LightSlateGray;
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