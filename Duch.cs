using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace pacman
{
    internal class Duch
    {
        public Ellipse textura;
        protected int[,] waypointy;

        int velX = 0;
        int velY = 0;
        public double moveDelay = 5;
        private double sDelay = 0;



        private void prolez(int souradniceX, int souradniceY, int pocetKroku, bool[,] mapa)
        {

            if (mapa[souradniceX, souradniceY]) 
            { 
                if (waypointy[souradniceX, souradniceY] > pocetKroku) 
                {
                    waypointy[souradniceX, souradniceY] = pocetKroku;

                    prolez(souradniceX, souradniceY + 1, pocetKroku + 1, mapa);
                    prolez(souradniceX, souradniceY - 1, pocetKroku + 1, mapa);
                    prolez(souradniceX + 1, souradniceY, pocetKroku + 1, mapa);
                    prolez(souradniceX - 1, souradniceY, pocetKroku + 1, mapa);
                }
            }
        }

        public virtual void nastavWaypointy(bool[,] mapa, Point souradnicePacmana)
        {
            waypointy = new int[mapa.GetLength(0), mapa.GetLength(1)];

            for (int i = 0; i < waypointy.GetLength(0); i++)
            {
                for (int j = 0; j < waypointy.GetLength(1); j++)
                {
                    waypointy[i, j] = int.MaxValue;
                }
            }

            prolez((int)souradnicePacmana.X, (int)souradnicePacmana.Y, 0, mapa);
        }

        public virtual void pohyb() //bool[,] mapa, Point souradnicePacmana
        {
            if (this.waypointy != null)
            {
                sDelay++;

                if (sDelay == moveDelay)
                { 
                    sDelay = 0;

                } else {
                    Thickness pozice = this.textura.Margin;

                    if ((pozice.Left % 20 == 0) && (pozice.Top % 20 == 0))
                    {
                        int pSloupec = Convert.ToInt32(pozice.Left / 20);
                        int pRadek = Convert.ToInt32(pozice.Top / 20);

                        //MessageBox.Show(pSloupec + " " + pRadek);

                        int[] smery = new int[4];
                        smery[0] = waypointy[pSloupec , pRadek - 1];
                        smery[1] = waypointy[pSloupec + 1, pRadek];
                        smery[2] = waypointy[pSloupec , pRadek + 1];
                        smery[3] = waypointy[pSloupec - 1, pRadek ];

                        int minimum = 0;

                        for (int i = 0; i < smery.Length; i++)
                            if (smery[i] < smery[minimum])
                                minimum = i;

                        switch(minimum)
                        {
                            case 0: this.velX = 0; this.velY = -1; break; //nahoru
                            case 1: this.velX = 1; this.velY = 0; break; //doprava
                            case 2: this.velX = 0; this.velY = 1; break; //dolů
                            case 3: this.velX = -1; this.velY = 0; break; //doleva
                        }
                    }

                    pozice.Left += this.velX;
                    pozice.Top += this.velY;

                    this.textura.Margin = pozice;
                
                }
            }
        }
        
        public Duch(int col, int row)
        {
            this.textura = new Ellipse();
            this.textura.Width = 20;
            this.textura.Height = 20;
            this.textura.Fill = new SolidColorBrush(Color.FromRgb(255, 0, 0));
            this.textura.VerticalAlignment = VerticalAlignment.Top;
            this.textura.HorizontalAlignment = HorizontalAlignment.Left;
            this.textura.Margin = new Thickness(col * 20, row * 20, 0, 0);
            Canvas.SetZIndex(this.textura, 11);
        }
    }

    class JinyDuch:Duch
    {
        public JinyDuch(int sloupec, int radek):base(sloupec, radek)
        {

        }

        public override void nastavWaypointy(bool[,] mapa, Point souradnicePacmana)
        {
            waypointy = new int[mapa.GetLength(0), mapa.GetLength(1)];

            for (int i = 0; i < waypointy.GetLength(0); i++)
            {
                for (int j = 0; j < waypointy.GetLength(1); j++)
                {
                    waypointy[i, j] = int.MaxValue;
                }
            }

            prolez((int)souradnicePacmana.X, (int)souradnicePacmana.Y, 0, mapa);
        }

        private void prolez(int souradniceX, int souradniceY, int pocetKroku, bool[,] mapa)
        {

            if (mapa[souradniceX, souradniceY])
            {
                if (waypointy[souradniceX, souradniceY] > pocetKroku)
                {
                    waypointy[souradniceX, souradniceY] = pocetKroku;
                    prolez(souradniceX, souradniceY - 1, pocetKroku + 1, mapa);
                    prolez(souradniceX, souradniceY + 1, pocetKroku + 1, mapa);
                    prolez(souradniceX - 1, souradniceY, pocetKroku + 1, mapa);
                    prolez(souradniceX + 1, souradniceY, pocetKroku + 1, mapa);
                }
            }
        }
    }
    class DementDuch : Duch
    {
        int inteligence = 0;
        Random rand = new Random();


        public DementDuch(int sloupec, int radek, int inteligence) : base(sloupec, radek)
        {
            this.inteligence = inteligence;
        }

        public override void pohyb()
        {

        }
    }
}
