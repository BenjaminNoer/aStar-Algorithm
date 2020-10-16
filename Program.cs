using System;
using System.Linq;
using System.Collections.Generic;

namespace aStar
{
    class Program
    {
        static void Main(string[] args)
        {
            // A = start, B = target, X = obstacle
            string[] map = new string[]
            {
                "+-----------------------+",
                "|        XXX            |",
                "|   X      X A XXXXXXXX |",
                "|   X      XXXXXX       |",
                "|   XXXXXX X B X    X   |",
                "|        X     X XXXXXXX|",
                "| XXXXXXXXXXXXXX        |",
                "|                   X   |",
                "+-----------------------+",
            };
            Location current = null;
            var start = new Location { X = 13, Y = 2 };
            var target = new Location { X = 13, Y = 4 };
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;

            //add the starting position to the open list to start searching from there
            openList.Add(start);    

            //print the maze and legend
            foreach (string line in map)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("A = Start, B = Destination, X = Obstacle, . = Searched Tiles");

            //algorithm
            while (openList.Count > 0)
            {
                //get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                //add the current square to the closed list
                closedList.Add(current);
                //remove it from the open list
                openList.Remove(current);

                //show the current position on the map
                Console.SetCursorPosition(current.X, current.Y);
                if ((current.X == start.X && current.Y == start.Y) || (current.X == target.X && current.Y == target.Y))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write('.');
                Console.SetCursorPosition(current.X, current.Y);
                System.Threading.Thread.Sleep(25);

                //if the destination is in the closed list it has been reached
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null)
                    break;

                //search possible adjacent squares
                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, map);

                //distance from starting point is increased by 1
                g++;

                //compute adjacent square scores and add them to the open list
                foreach (var adjacentSquare in adjacentSquares)
                {
                    //if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    //if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null)
                    {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeH(adjacentSquare.X,
                            adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        //and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    }
                    else
                    {
                        //test if using the current G score makes the adjacent square's F score
                        //if lower update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F)
                        {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }

            //show path in text
            while (current != null)
            {
                Console.SetCursorPosition(current.X, current.Y);
                if ((current.X == start.X && current.Y == start.Y) || (current.X == target.X && current.Y == target.Y))
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.White;
                }
                Console.Write("\u2588");
                Console.SetCursorPosition(current.X, current.Y);
                current = current.Parent;
                System.Threading.Thread.Sleep(10);
            }
            Console.ReadKey();
        }

        static int ComputeH(int x, int y, int targetX, int targetY)
        {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        static List<Location> GetWalkableAdjacentSquares(int x, int y, string[] map)
        {
            //Get possible moves
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };
            return proposedLocations.Where(l => map[l.Y][l.X] == ' ' || map[l.Y][l.X] == 'B').ToList();
        }
    }

    public class Location
    {
        public int X;               //Coordinate
        public int Y;               //Coordinate
        public int F;               //|G| + |H|
        public int G;               //Distance from startin point to current point
        public int H;               //Estimated istance from current point to destination
        public Location Parent;     //Previous location, keeps track of path
    }
}