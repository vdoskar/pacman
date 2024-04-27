using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace pacman
{
    public partial class MainWindow : Window
    {

        DispatcherTimer casovac = new DispatcherTimer();
        Random rand = new Random();


        double RYCHLOST_PACMAN = 2.5;

        bool nahoru = false;
        bool dolu = false;
        bool doleva = false;
        bool doprava = false;

        int pixelNaPolicko = 20;
        int pocetPuntiku;
        int pocetSezranychPuntiku = 0;

        int smer = 2;
        int uzivatelSmer = 1;

        static int rows = 0;
        static int cols = 0;

        bool[,] plocha_log = new bool[rows, cols];
        Ellipse[,] puntiky = new Ellipse[rows, cols];

        List<Duch> Duchove = new List<Duch>();

        public MainWindow()
        {
            InitializeComponent();

            // VYTVOŘENÍ POLE

            StreamReader sr = new StreamReader("mapa.txt");

            cols = Convert.ToInt32(sr.ReadLine());
            rows = Convert.ToInt32(sr.ReadLine());
            
            plocha_log = new bool[rows, cols];
            puntiky = new Ellipse[rows, cols];

            for (int j = 0; j < cols; j++)
                for (int i = 0; i < rows; i++)
                {
                    switch (Convert.ToInt32(sr.ReadLine()))
                    {
                        case 1: plocha_log[i, j] = true;

                            pocetPuntiku++;
                            puntiky[i, j] = new Ellipse();
                            puntiky[i, j].Width = 3;
                            puntiky[i, j].Height = 3;
                            puntiky[i, j].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            puntiky[i, j].VerticalAlignment = VerticalAlignment.Top;
                            puntiky[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                            puntiky[i, j].Margin = new Thickness(pixelNaPolicko * i + 9, pixelNaPolicko * j + 9, 0, 0);

                            Canvas.SetZIndex(puntiky[i, j], 10);
                            plocha.Children.Add(puntiky[i, j]);

                            break;

                        case 2: plocha_log[i, j] = true;

                            pacman.Margin = new Thickness(pixelNaPolicko * i, pixelNaPolicko * j, 0, 0);
                            Canvas.SetZIndex(pacman, 10);
                            puntiky[i, j] = new Ellipse();
                            puntiky[i, j].Visibility = Visibility.Hidden;

                            break;

                        case 3: plocha_log[i, j] = true;

                            Duch novyDuch = new Duch(i, j);
                            novyDuch.moveDelay = rand.Next(2, 6);
                            Duchove.Add(novyDuch);

                            pocetPuntiku++;
                            puntiky[i, j] = new Ellipse();
                            puntiky[i, j].Width = 3;
                            puntiky[i, j].Height = 3;
                            puntiky[i, j].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            puntiky[i, j].VerticalAlignment = VerticalAlignment.Top;
                            puntiky[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                            puntiky[i, j].Margin = new Thickness(pixelNaPolicko * i + 9, pixelNaPolicko * j + 9, 0, 0);

                            Canvas.SetZIndex(puntiky[i, j], 10);
                            plocha.Children.Add(puntiky[i, j]);

                            break;

                        case 4:
                            plocha_log[i, j] = true;

                            JinyDuch JinyDuch = new JinyDuch(i, j);
                            JinyDuch.moveDelay = rand.Next(2, 6);
                            Duchove.Add(JinyDuch);

                            pocetPuntiku++;
                            puntiky[i, j] = new Ellipse();
                            puntiky[i, j].Width = 3;
                            puntiky[i, j].Height = 3;
                            puntiky[i, j].Fill = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                            puntiky[i, j].VerticalAlignment = VerticalAlignment.Top;
                            puntiky[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                            puntiky[i, j].Margin = new Thickness(pixelNaPolicko * i + 9, pixelNaPolicko * j + 9, 0, 0);

                            Canvas.SetZIndex(puntiky[i, j], 10);
                            plocha.Children.Add(puntiky[i, j]);

                            break;

                        default: 

                            plocha_log[i, j] = false; 
                            break;
                    } 
                }

            currentPointsTextBlock.Text = "Zbývá sežrat : " + pocetPuntiku;

            for (int i = 0; i < plocha_log.GetLength(0); i++)
                for (int j = 0; j < plocha_log.GetLength(1); j++)
                {
                    Rectangle policko = new Rectangle();
                    policko.Width = pixelNaPolicko;
                    policko.Height = pixelNaPolicko;
                    policko.VerticalAlignment = VerticalAlignment.Top;
                    policko.HorizontalAlignment = HorizontalAlignment.Left;
                    policko.Margin = new Thickness(i * 20, j * 20, 0, 0);

                    if (plocha_log[i, j] == true) {
                        policko.Fill = new SolidColorBrush(Color.FromArgb(255,0,0,255));
                    } else {
                        policko.Fill = new SolidColorBrush(Color.FromArgb(255,0,0,0));
                    }

                    plocha.Children.Add(policko);
                }

            foreach (Duch jeden in Duchove)
            {
                plocha.Children.Add(jeden.textura);
            }

            // ČASOVAČ
            casovac.Interval = new TimeSpan(0, 0, 0, 0, 1);
            casovac.Tick += akce;
            casovac.Start();
        }

        bool kolize(Duch duch)
        {
            double vzdalenost_x = duch.textura.Margin.Left - pacman.Margin.Left;
            double vzdalenost_y = duch.textura.Margin.Top - pacman.Margin.Top;
            double vzdalenost = Math.Sqrt(Math.Pow(vzdalenost_x, 2) + Math.Pow(vzdalenost_y, 2)); // pythagorovka

            if (vzdalenost < (pacman.Width / 2 + duch.textura.Width / 2))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void akce(object sender, EventArgs e)
        {

            // pohyb pacmana

            foreach(Duch duch in Duchove)
            {
                duch.pohyb();
            }

            Thickness pozicePackmana = pacman.Margin;

            if ((pozicePackmana.Left % pixelNaPolicko == 0) && (pozicePackmana.Top % pixelNaPolicko == 0)) 
            {
                int pacmanSloupec = Convert.ToInt32(pozicePackmana.Left / pixelNaPolicko);
                int pacmanRadek = Convert.ToInt32(pozicePackmana.Top / pixelNaPolicko);

                foreach (Duch duch in Duchove)
                {
                    duch.nastavWaypointy(plocha_log, new Point(pacmanSloupec, pacmanRadek));
                }

                if (puntiky[pacmanSloupec, pacmanRadek].Visibility == Visibility.Visible)
                {
                    puntiky[pacmanSloupec, pacmanRadek].Visibility = Visibility.Hidden;
                    pocetSezranychPuntiku++;
                    currentPointsTextBlock.Text = "Zbývá sežrat : " + (pocetPuntiku - pocetSezranychPuntiku);
                }

                if (pocetSezranychPuntiku == pocetPuntiku)
                {
                    casovac.Stop();
                    MessageBox.Show("win win");
                }

                // tady se nastavuje smer uzivatele
                switch (uzivatelSmer)
                {
                    case 1  : if (plocha_log[pacmanSloupec, pacmanRadek - 1]) smer = 1;  break;
                    case 2  : if (plocha_log[pacmanSloupec + 1, pacmanRadek]) smer = 2;  break; 
                    case 3  : if (plocha_log[pacmanSloupec, pacmanRadek + 1]) smer = 3;  break;
                    default : if (plocha_log[pacmanSloupec - 1, pacmanRadek]) smer = 4; break;
                }

                // 1 = nahoru, 2 = doprava, 3 = dolů, 4 = doleva
                switch (smer)
                {
                    case 1 : if (plocha_log[pacmanSloupec, pacmanRadek - 1]) pozicePackmana.Top--; break;
                    case 2 : if (plocha_log[pacmanSloupec + 1, pacmanRadek]) pozicePackmana.Left++; break;
                    case 3 : if (plocha_log[pacmanSloupec, pacmanRadek + 1]) pozicePackmana.Top++; break;
                    default: if (plocha_log[pacmanSloupec - 1, pacmanRadek]) pozicePackmana.Left--; break;
                }

                pacman.Margin = pozicePackmana; // ted to jede samo  
            } 
            else
            {
                switch (smer)  
                {
                    case 1 : pozicePackmana.Top--; break;
                    case 2 : pozicePackmana.Left++; break;
                    case 3 : pozicePackmana.Top++; break;
                    default: pozicePackmana.Left--; break;
                }
                pacman.Margin = pozicePackmana;
            } 

            // test kolize

            foreach (Duch duch in Duchove)
            {
                if (kolize(duch))
                {
                    casovac.Stop();
                    MessageBox.Show("Game over lol");
                }
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)    { uzivatelSmer = 1;   }
            if (e.Key == Key.Down)  { uzivatelSmer = 3;   }
            if (e.Key == Key.Right) { uzivatelSmer = 2;   }
            if (e.Key == Key.Left)  { uzivatelSmer = 4;   }
        }
        
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up)    { nahoru = false;   }
            if (e.Key == Key.Down)  { dolu = false;     }
            if (e.Key == Key.Right) { doprava = false;  }
            if (e.Key == Key.Left)  { doleva = false;   }
        }
        
    }
}