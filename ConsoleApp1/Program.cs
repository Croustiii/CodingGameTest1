using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    /// <summary>
    /// There is No Spoon Solution
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {

            //// Mars Lander
            //// Test

            //FieldMapper FM = new FieldMapper(5);

            Lander lander = new Lander();
            //lander.Site = new point[] { new point(500, 1000), new point(2000, 1000) };
            lander.X = 2000;
            lander.Y = 1000;

            lander.CalcPosition();
            


            Console.ReadLine();
        }
    }


    public class point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public point(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// Class de Detection de zone d'aterrissage
    public class FieldMapper
    {
        // Nombre de points qui composent la surface
        public List<point> Field = new List<point>();

        public int surface;

        public List<point> Site = new List<point>();

        public FieldMapper(int surfacepts)
        {
            surface = surfacepts;
        }

        public void Map (int x, int y)
        {
            Field.Add(new point(x, y));
        }

        public void FindSite ()
        {

            for (int i = 0; i < Field.Count - 1; i++)
            {
                var split = Field[i + 1].X - Field[i].X;

                if (split >=1000 && Field[i + 1].Y == Field[i].Y)
                {
                    // Debut du Landing Site
                    Site.Add(new point(Field[i].X, Field[i].Y));

                    // Fin du Landing Site
                    Site.Add(new point(Field[i+1].X, Field[i+1].Y));
                }
            }
            Console.Error.WriteLine($"Début {Site[0].X} Fin {Site[1].X} ");
        }

    }
    public enum RelativePosition { Above, Below, SiteLvl, Left, Right, InSite };
    public enum Edge {SiteLeft, SiteRight, FieldLeft, FieldRight, Neutral };

    public class Lander
    {
        // 0 - 7000
        public int X;

        // 0 - 3000
        public int Y;

        // + Up - Down
        public int Vspeed;

        // - Left + Right
        public int HSpeed;

        public int Fuel;

        // + Left - Right (0 - 90)
        public int Angle;
        public int AngleChange;

        // Thrust 0-4
        public int Power;
        public int ThrustChange;

        public List<point> Site = new List<point>();

        public RelativePosition XPosition;
        public RelativePosition YPosition;
        public Edge edge;

        public Lander()
        {
        }

        public void ActualizeLander (int x, int y, int h, int v, int fuel, int angle, int power)
        {
            X = x;
            Y = y;
            HSpeed = h;
            Vspeed = v;
            Fuel = fuel;
            Angle = angle;
            Power = power;
        }

        public void CalcPosition ()
        {
            edge = Edge.Neutral;
            if (X <= 1000)
                edge = Edge.FieldLeft;
            if (X >= 6000)
                edge = Edge.FieldRight;
            if (X >= Site[0].X && X <= Site[0].X + 250)
                edge = Edge.SiteLeft;
            if (X >= Site[1].X -250 && X <= Site[1].X)
                edge = Edge.SiteRight;


            // Lander is left or right from site
            if (Site[0].X <= X && Site[1].X >= X)
                XPosition = RelativePosition.InSite;
            if (X > Site[1].X)
                XPosition = RelativePosition.Right;
            if (X < Site[0].X)
                XPosition = RelativePosition.Left;

            // Lander is above or below from site
            if (Y == Site[0].Y)
                YPosition = RelativePosition.SiteLvl;
            if (Site[0].Y < Y)
                YPosition = RelativePosition.Above;
            else if (Site[0].Y > Y)
                YPosition = RelativePosition.Below;

            Console.Error.WriteLine($"{XPosition} {YPosition} {edge}");
        }

        public void Decide()
        {
            if (XPosition == RelativePosition.Right && Angle == 0)
                AngleChange = 15;
            if (XPosition == RelativePosition.Left && Angle == 0)
                AngleChange = -15;

            if (Vspeed < -10)
                ThrustChange = 1;

            if (XPosition == RelativePosition.InSite)
                Stabilize();

            if (YPosition == RelativePosition.Above && Vspeed >= 100)
                ThrustChange = 1;

            if (edge == Edge.FieldRight || edge == Edge.SiteRight && HSpeed > 0 && Angle <=30)
            {
                ThrustChange = 1;
                AngleChange = 15;
            }
            if (edge == Edge.FieldLeft ||edge == Edge.SiteLeft && HSpeed < 0)
            {
                ThrustChange = 1;
                AngleChange = 15;
            }


            Console.Error.WriteLine($"Angle decision : {AngleChange} Power decision : {ThrustChange}");
        }

        public void Stabilize()
        {
            if (HSpeed > 50) // droite rapide
            {
                if (Angle < 0) // orientation à droite
                {
                    AngleChange = 15;

                    if (Power >= 2)
                        ThrustChange = -1;
                }
                if (Angle > 0) // Orientation à gauche
                {
                    if (true)
                        AngleChange = 15;

                    ThrustChange = 1;
                }

            }
            if (HSpeed < -50) // Gauche Rapide
            {

                if (Angle > 0) // orientation à gauche
                {
                    AngleChange = -15;

                    if (Power >= 2)
                        ThrustChange = -1;
                }

                if (Angle < 0) // orientation à droite

                {
                    if (Angle > -60)
                        AngleChange = -15;

                    ThrustChange = 1;
                }
            }

            if (HSpeed < 20 && HSpeed > 0) // Droite lente
            {
                if (Angle < 0) // orientation à droite
                {
                    AngleChange = 15;

                    if (Power >= 2)
                        ThrustChange = -1;
                }
                if (Angle > 0) // Orientation à gauche
                {
                    AngleChange = 15;
                    ThrustChange = 1;
                }

            }

            if (Vspeed < 60)
            {
                if (Angle > 0) // Orientation à gauche
                    AngleChange = -15;

                if (Angle < 0) // Orientation à droite
                    AngleChange = 15;

                ThrustChange = 1;
            }

        }

        public void Order()
        {
            if (Power == 0 || Power == 4)
            {
                ThrustChange = 0;
            }
            if (Angle == -90 || Angle == 90)
            {
                AngleChange = 0;
            }
            Console.WriteLine($"{Angle + AngleChange} {Power + ThrustChange}");
        }

    }


}
