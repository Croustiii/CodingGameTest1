using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp2
{
    class Program
    {
        static void Main(string[] args)
        {

            //// Code VS Zombies

            Mapper map = new Mapper();

            // 2 Humans position and mapping
            map.AddHuman(0, new Human(0, 2, 4));
            map.AddHuman(1, new Human(1, 5, 5));
            //map.AddHuman(2, new Human(2, 4, 3));

            // Zombie Position and mapping
            Zombie Z0 = new Zombie(2, 2);
            Zombie Z1 = new Zombie(5, 2);
            Zombie Z2 = new Zombie(6, 4);

            map.AddZombie(0, Z0);
            map.AddZombie(1, Z1);
            map.AddZombie(2, Z2);

            map.Targetting();
            map.FindVictims();
            map.OrderVictims();
            Console.WriteLine(map.Victims.Count);
            map.Display();



            Console.ReadKey();
            Console.ReadLine();
        }
    }

    public class Mapper
    {
        // Liste des zombies présents sur la map
        public Dictionary<int,Zombie> Zombies { get; set; }

        //Liste des humains présents sur la map
        public Dictionary<int, Human> Humans { get; set; }

        // Liste des Humains ID qui sont target par des zombies
        public List<Human> Victims;

        // Constructeur du mapper
        public Mapper()
        {
            Zombies = new Dictionary<int, Zombie>();
            Humans = new Dictionary<int, Human>();
            Victims = new List<Human>();
        }

        // Ajoute un zombie à la listede zombies existants
        public void AddZombie(int id,Zombie zomb)
        {
            Zombies.Add(id,zomb);
        }

        // Ajoute un humains à la liste d'humains existants
        public void AddHuman (int id, Human h)
        {
            Humans.Add(id, h);
        }

        // Nettoie les listes de zombies et humains
        public void Clear()
        {
            Zombies.Clear();
            Humans.Clear();
        }

        // Calcule la distance entre 2 points
        public double CalcDistance(Point A, Point B)
        {
            var termA = B.X - A.X;
            var termB = B.Y - A.Y;
            var result = Math.Sqrt(Math.Pow(termA, 2) + Math.Pow(termB, 2));
            return result;
        }

        // Find human target for each zombie
        public void Targetting ()
        {
            //pour chaque zombie calcule la distance a chaque humains et affecte l'ID de l'humain le plus proche
            foreach (Zombie z in Zombies.Values)
            {
                var TargetDistance = double.MaxValue;
                int TargetID = 0;
                foreach (KeyValuePair<int,Human> h in Humans)
                {
                    var distance = CalcDistance(z.Position, h.Value.Position);
                    if (distance <= TargetDistance)
                    {
                        TargetDistance = distance;
                        z.Target = h.Key;
                        z.TargetDistance = TargetDistance;
                        TargetID = h.Value.Id;
                    }
                }
                Humans[TargetID].DeathDistance = TargetDistance;
            }
        }

        // Add all humans id who are targeted in the Victims List
        public void FindVictims()
        {
            List<int> Added = new List<int>();
            foreach (KeyValuePair<int,Zombie> z in Zombies)
            {
                if (Added.Contains(z.Value.Target))
                {
                    break;
                }
                else
                {
                    Victims.Add(new Human(z.Value.Target, z.Value.Position.X, z.Value.Position.Y));
                }
            }
        }

        /// Find next human victim among all zombies target and return its identifier
        public void OrderVictims()
        {
            var NextVictims = Victims.OrderBy(i => i.DeathDistance).ToList();
            Victims.Clear();
            Victims = NextVictims.ToList();
        }

        // Affiche les zombies et humains et leur position dans le flux d'erreur
        public void Display()
        {
            foreach (KeyValuePair<int, Zombie> z in Zombies)
            {
                Console.Error.WriteLine($"Z {z.Key} is at position {z.Value.Position.X} {z.Value.Position.Y} \n target is {z.Value.Target} Distance : {z.Value.TargetDistance}");
            }

            foreach (KeyValuePair<int, Human> h in Humans)
            {
                Console.Error.WriteLine($"H {h.Key} is at position {h.Value.Position.X} {h.Value.Position.Y} and distance to danger is {h.Value.DeathDistance}");
            }

            foreach (Human h in Victims)
            {
                Console.Error.WriteLine($"Victim: Human {h.Id}");
            }
        }

    }
    public class Zombie
    {
        public Point Position { get; set; }

        public int Target { get; set; }

        public double TargetDistance { get; set; }

        public Zombie(double x, double y)
        {
            Position = new Point(x, y);
        }
        public void Move (double x, double y)
        {
            Position.X = x;
            Position.Y = y;
        }
    }
    public class Human
    {
        public int Id { get; set; }

        public double? DeathDistance { get; set; }

        public Point Position { get; set; }

        public Human(int id, double x, double y)
        {
            Id = id;
            Position = new Point(x, y);
        }

        //Distance to closest zombie

    }
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point()
        {
        }
        public Point (double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
