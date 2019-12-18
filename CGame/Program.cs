using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CGame
{
    public class point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public bool IsWall;
        public bool IsNode;
        public List<point> Nodes;

        public int BlastResult;

        public point(int x, int y)
        {
            X = x;
            Y = y;

            BlastResult = 0;
        }
        public point(int x, int y, bool wall, bool node)
            : this(x, y)
        {
            IsWall = wall;
            IsNode = node;
        }
    }

    public class BlastCalculator
    {
        //largeur du terrain = nombre de colonnes
        public int width { get; set; }
        // hauteur du terrain = nombre de lignes
        public int height { get; set; }

        // Line to fill
        public int Line { get; set; }

        //Timer Fields
        public int RoundsLeft;
        public int RoundShoot;
        public int Bombs;

        public bool BombPlaced;
        //public bool shootable;

        // possible positions and blast results
        public List<point> Results = new List<point>();
        public List<point> final;
        // Terrain
        public point[,] field;

        public BlastCalculator(int _width, int _height)
        {
            //shootable = true;

            width = _width;
            height = _height;
            Line = 0;
            field = new point[height, width];
        }

        public void MapLine(string row)
        {
            for (int j = 0; j < width; j++)
            {
                if (row[j] == '@')
                    field[Line, j] = new point(Line, j, false, true);
                if (row[j] == '#')
                    field[Line, j] = new point(Line, j, true, false);
                if (row[j] == '.')
                    field[Line, j] = new point(Line, j, false, false);
            }

            Line += 1;
        }

        public void ChooseTarget()
        {

            Results = new List<point>();

            //clean des valeurs de blast des points du terrain
            foreach (point p in field)
            {
                p.BlastResult = 0;
            }

            foreach (point p in field)
            {
                p.Nodes = new List<point>();

                if (p.IsNode == false && p.IsWall == false)
                {
                    // calcul horizontal
                    for (int i = 1; i <= 3; i++)
                    {
                        if (p.Y + i >= width)
                            break;
                        if (field[p.X, p.Y + i].IsNode)
                        {
                            p.BlastResult += 1;
                            p.Nodes.Add(field[p.X, p.Y + i]);
                        }
                        if (field[p.X, p.Y + i].IsWall == true)
                            break;
                    }

                    for (int i = 1; i <= 3; i++)
                    {
                        if (p.Y - i < 0)
                            break;
                        if (field[p.X, p.Y - i].IsNode == true)
                        {
                            p.BlastResult += 1;
                            p.Nodes.Add(field[p.X, p.Y - i]);
                        }
                        if (field[p.X, p.Y - i].IsWall == true)
                            break;
                    }

                    // Calcul Vertical
                    for (int i = 1; i <= 3; i++)
                    {
                        if (p.X + i > height - 1)
                            break;
                        if (field[p.X + i, p.Y].IsNode == true)
                        {
                            p.BlastResult += 1;
                            p.Nodes.Add(field[p.X + i, p.Y]);
                        }
                        if (field[p.X + i, p.Y].IsWall == true)
                            break;
                    }

                    for (int i = 1; i <= 3; i++)
                    {
                        if (p.X - i < 0)
                            break;
                        if (field[p.X - i, p.Y].IsNode == true)
                        {
                            p.BlastResult += 1;
                            p.Nodes.Add(field[p.X - i, p.Y]);
                        }
                        if (field[p.X - i, p.Y].IsWall == true)
                            break;
                    }
                    Results.Add(p);
                }
            }
            Console.Error.WriteLine("nb vide " + Results.Count());
            this.final = Results.OrderByDescending(p => p.BlastResult).ToList();


        }

        public void UpdateWorld(int round)
        {
            if (Bombs == 0)
            {
                Console.Error.WriteLine("No Bombs Left!");

                Console.WriteLine("wait");

            }
            else if (Bombs >= 0)
            {
                RoundShoot = round;
                Bombs -= 1;

                Console.WriteLine($"{final.First().Y} {final.First().X}");

                foreach (point node in final.First().Nodes)
                {
                    node.IsNode = false;
                }
                final.RemoveAt(0);

            }
        }

        public void Display()
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (field[i, j].IsNode)
                        Console.Error.Write("@");
                    else if (field[i, j].IsWall)
                        Console.Error.Write("#");
                    else if (final != null && field[i, j].X == final.First().X && field[i, j].Y == final.First().Y)
                        Console.Error.Write("X");
                    else
                        Console.Error.Write(".");
                }
                Console.Error.WriteLine();
            }
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            //// VoxCodei Solution 
            //// Test
            
            BlastCalculator BC = new BlastCalculator(8, 6);

            string s1  = "....@@@.";
            string s2  = ".@@@...@";
            string s3  = "@...@..@";
            string s4  = "@...@..@";
            string s5  = "@...@...";
            string s6  = ".@@@.@@@";

            //Mapper le terrain
            BC.MapLine(s1);
            BC.MapLine(s2);
            BC.MapLine(s3);
            BC.MapLine(s4);
            BC.MapLine(s5);
            BC.MapLine(s6);

            // Paramètres du niveau
            int rounds = 15;
            int bombs = 3;


            // Afficher chaque point du terrain, ses valeurs et coordonnées
            BC.Bombs = bombs;
            BC.Display();


            // Boucle de jeu
            while (rounds >= 0)
            {
                Console.Error.WriteLine();
                BC.ChooseTarget();
                BC.Display();
                BC.UpdateWorld(rounds);
                Console.Error.WriteLine();

                rounds--;
            }

            Console.ReadLine();

        }
    }
}
