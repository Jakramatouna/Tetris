using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Tetris
{
    static class Game_Program
    {
        public static string playerName;
        public static int theme;
        public static string sqr = "◼";
        public static int[,] grid = new int[23, 10];
        public static int[,] droppedtetrominoeLocationGrid = new int[23, 10];
        public static Stopwatch timer = new Stopwatch();
        public static Stopwatch dropTimer = new Stopwatch();
        public static Stopwatch inputTimer = new Stopwatch();
        public static int dropRate, dropTime = 300;
        public static bool isDropped = false;
        // Tetris block.
        static TetrisObject.Tetrominoe tet;
        // Tetris block that will be spawn next.
        static TetrisObject.Tetrominoe nexttet;
        public static ConsoleKeyInfo key;
        public static bool isKeyPressed = false;
        public static int linesCleared = 0, score = 0, hiScore = 0, level = 1;

        static void Main(string[] args)
        {
            // For using unicode.
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            try
            {
                if (args.Length != 2)
                {
                    Console.WriteLine("Program can work with two args (player name,theme)\ntheme\nCyan   => Press 1\nGreen => Press 2\nMagenta   => Press 3\nRed   => Press 4\nYellow   => Press any numbers\n");
                    return;
                }
                if (args.Length == 2)
                {
                    playerName = args[0];
                    theme = Convert.ToInt32(args[1]);
                    switch (theme)
                    {
                        case 1:
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            break;
                        case 2:
                            Console.ForegroundColor = ConsoleColor.Green;
                            break;
                        case 3:
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            break;
                        case 4:
                            Console.ForegroundColor = ConsoleColor.Red;
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            break;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Wrong input !\nPlease enter again with player name and theme ....\ntheme\nCyan   => Press 1\nGreen => Press 2\nMagenta   => Press 3\nRed   => Press 4\nYellow   => Press any numbers\n");
                Console.ReadKey(true);
                return;
            }
            Console.Clear();
            // For game border
            drawBorder();
            Console.SetCursorPosition(5, 5);
            Console.WriteLine("Press any key");
            Console.SetCursorPosition(7, 6);
            Console.WriteLine("to start");
            Console.SetCursorPosition(6, 8);
            Console.WriteLine("☆Control☆");
            Console.SetCursorPosition(2, 10);
            Console.WriteLine("spacebar → rotate");
            Console.SetCursorPosition(2, 11);
            Console.WriteLine("left/right → move");
            Console.SetCursorPosition(2, 12);
            Console.WriteLine("up → quick drop");
            Console.SetCursorPosition(2, 13);
            Console.WriteLine("down → drop faster");
            Console.ReadKey(true);
            // Time start.
            timer.Start();
            dropTimer.Start();
            long time = timer.ElapsedMilliseconds;
            Console.SetCursorPosition(25, 0);
            Console.WriteLine($"Player {playerName} Level {level}");
            Console.SetCursorPosition(25, 1);
            Console.WriteLine($"Score {score}   ♚ Hi score {hiScore}");
            Console.SetCursorPosition(25, 2);
            Console.WriteLine($"LinesCleared {linesCleared}");
            // Creating tetris block by instantiation an object from TetrisObject namespace and Tetrominoe class
            nexttet = new TetrisObject.Tetrominoe();
            tet = nexttet;
            tet.Spawn();
            nexttet = new TetrisObject.Tetrominoe();
            Update();
            Console.SetCursorPosition(0, 26);
            Console.WriteLine("Game Over");
            Console.SetCursorPosition(0, 27);
            Console.WriteLine("Replay? (Y/N)");
            string input = Console.ReadLine();
            // Press Y or y to replay.
            if (input == "y" || input == "Y")
            {
                int[,] grid = new int[23, 10];
                droppedtetrominoeLocationGrid = new int[23, 10];
                timer = new Stopwatch();
                dropTimer = new Stopwatch();
                inputTimer = new Stopwatch();
                dropRate = 300;
                isDropped = false;
                isKeyPressed = false;
                linesCleared = 0;
                score = 0;
                level = 1;
                GC.Collect();
                Console.Clear();
                Main(args);
            }
            else return;
        }
        private static void Update()
        {
            //Update loop.
            while (true)
            {
                dropTime = (int)dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0;
                    dropTimer.Restart();
                    tet.Drop();
                }
                if (isDropped == true)
                {
                    tet = nexttet;
                    nexttet = new TetrisObject.Tetrominoe();
                    tet.Spawn();

                    isDropped = false;
                }
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[0, j] == 1)
                        return;
                }
                Input();
                ClearBlock();
            }   //End Update.
        }
        // When a horizontal line of blocks is created, it gets cleared and any block above the deleted line will fall.
        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (droppedtetrominoeLocationGrid[i, j] == 0)
                        break;
                }
                if (j == 10)
                {
                    linesCleared++;
                    combo++;
                    for (j = 0; j < 10; j++)
                    {
                        droppedtetrominoeLocationGrid[i, j] = 0;
                    }
                    int[,] newdroppedtetrominoeLocationGrid = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            newdroppedtetrominoeLocationGrid[k + 1, l] = droppedtetrominoeLocationGrid[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            droppedtetrominoeLocationGrid[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 10; l++)
                            if (newdroppedtetrominoeLocationGrid[k, l] == 1)
                                droppedtetrominoeLocationGrid[k, l] = 1;
                    Draw();
                }
            }
            if (combo == 1)
                score += 40 * level;
            else if (combo == 2)
                score += 100 * level;
            else if (combo == 3)
                score += 300 * level;
            else if (combo > 3)
                score += 300 * combo * level;
            if (hiScore <= score)
                hiScore = score;
            // Level.
            if (linesCleared < 5) level = 1;
            else if (linesCleared < 10) level = 2;
            else if (linesCleared < 15) level = 3;
            else if (linesCleared < 25) level = 4;
            else if (linesCleared < 35) level = 5;
            else if (linesCleared < 50) level = 6;
            else if (linesCleared < 70) level = 7;
            else if (linesCleared < 90) level = 8;
            else if (linesCleared < 110) level = 9;
            else if (linesCleared < 150) level = 10;


            if (combo > 0)
            {
                Console.SetCursorPosition(25, 0);
                Console.WriteLine($"Player {playerName} Level {level}");
                Console.SetCursorPosition(25, 1);
                Console.WriteLine($"Score {score}   ♚ Hi score {hiScore}");
                Console.SetCursorPosition(25, 2);
                Console.WriteLine($"LinesCleared {linesCleared}");

            }
            dropRate = 300 - 22 * level;
        }
        // For controling tetris blocks.
        private static void Input()
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                isKeyPressed = true;
            }
            else
                isKeyPressed = false;

            if (Game_Program.key.Key == ConsoleKey.LeftArrow & !tet.isSomethingLeft() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] -= 1;
                }
                tet.Update();
            }
            else if (Game_Program.key.Key == ConsoleKey.RightArrow & !tet.isSomethingRight() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] += 1;
                }
                tet.Update();
            }
            if (Game_Program.key.Key == ConsoleKey.DownArrow & isKeyPressed)
            {
                tet.Drop();
            }
            if (Game_Program.key.Key == ConsoleKey.UpArrow & isKeyPressed)
            {
                for (; tet.isSomethingBelow() != true;)
                {
                    tet.Drop();
                }
            }
            if (Game_Program.key.Key == ConsoleKey.Spacebar & isKeyPressed)
            {
                //rotate
                tet.Rotate();
                tet.Update();
            }
        }
        public static void Draw()
        {
            for (int i = 0; i < 23; ++i)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.SetCursorPosition(1 + 2 * j, i);
                    if (grid[i, j] == 1 | droppedtetrominoeLocationGrid[i, j] == 1)
                    {
                        Console.SetCursorPosition(1 + 2 * j, i);
                        Console.Write(sqr);
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }
            }
        }
        public static void drawBorder()
        {
            for (int lengthCount = 0; lengthCount < 23; ++lengthCount)
            {
                Console.SetCursorPosition(0, lengthCount);
                Console.Write("*");
                Console.SetCursorPosition(21, lengthCount);
                Console.Write("*");
            }
            Console.SetCursorPosition(0, 23);
            for (int widthCount = 0; widthCount < 11; widthCount++)
            {
                Console.Write("*-");
            }
        }
    }
}
namespace TetrisObject
{
    public class Tetrominoe
    {
        // Tetris block'shapes.
        public static int[,] I = { { 1, 1, 1, 1 } };//3
        public static int[,] O = { { 1, 1 }, { 1, 1 } };
        public static int[,] T = { { 0, 1, 0 }, { 1, 1, 1 } };//3
        public static int[,] S = { { 0, 1, 1 }, { 1, 1, 0 } };//4
        public static int[,] Z = { { 1, 1, 0 }, { 0, 1, 1 } };//3
        public static int[,] J = { { 1, 0, 0 }, { 1, 1, 1 } };//3
        public static int[,] L = { { 0, 0, 1 }, { 1, 1, 1 } };//3
        public static List<int[,]> tetrominoes = new List<int[,]>() { I, O, T, S, Z, J, L };
        private bool isErect = false;
        private int[,] shape;
        private int[] pix = new int[2];
        public List<int[]> location = new List<int[]>();
        public Tetrominoe()
        {
            Random rnd = new Random();
            shape = tetrominoes[rnd.Next(0, 7)];
            for (int i = 23; i < 33; ++i)
            {
                for (int j = 3; j < 10; j++)
                {
                    Console.SetCursorPosition(i, j);
                    Console.Write("  ");
                }

            }
            Tetris.Game_Program.drawBorder();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        Console.SetCursorPosition(((10 - shape.GetLength(1)) / 2 + j) * 2 + 20, i + 5);
                        Console.Write(Tetris.Game_Program.sqr);
                    }
                }
            }
        }
        public void Spawn()
        {
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        location.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            Update();
        }
        public void Drop()
        {

            if (isSomethingBelow())
            {
                for (int i = 0; i < 4; i++)
                {
                    Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] = 1;
                }
                Tetris.Game_Program.isDropped = true;

            }
            else
            {
                for (int numCount = 0; numCount < 4; numCount++)
                {
                    location[numCount][0] += 1;
                }
                Update();
            }
        }
        public void Rotate()
        {
            List<int[]> templocation = new List<int[]>();
            for (int i = 0; i < shape.GetLength(0); i++)
            {
                for (int j = 0; j < shape.GetLength(1); j++)
                {
                    if (shape[i, j] == 1)
                    {
                        templocation.Add(new int[] { i, (10 - shape.GetLength(1)) / 2 + j });
                    }
                }
            }
            if (shape == tetrominoes[0])
            {
                if (isErect == false)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                    }
                }
                else
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i] = TransformMatrix(location[i], location[2], "Counterclockwise");
                    }
                }
            }

            else if (shape == tetrominoes[3])
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[3], "Clockwise");
                }
            }

            else if (shape == tetrominoes[1]) return;
            else
            {
                for (int i = 0; i < location.Count; i++)
                {
                    templocation[i] = TransformMatrix(location[i], location[2], "Clockwise");
                }
            }
            for (int count = 0; isOverlayLeft(templocation) != false | isOverlayRight(templocation) != false | isOverlayBelow(templocation) != false; count++)
            {
                if (isOverlayLeft(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] += 1;
                    }
                }
                if (isOverlayRight(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][1] -= 1;
                    }
                }
                if (isOverlayBelow(templocation) == true)
                {
                    for (int i = 0; i < location.Count; i++)
                    {
                        templocation[i][0] -= 1;
                    }
                }
                if (count == 3)
                {
                    return;
                }
            }
            location = templocation;
        }
        // Rotate tetris block.
        public int[] TransformMatrix(int[] coord, int[] axis, string dir)
        {
            int[] pcoord = { coord[0] - axis[0], coord[1] - axis[1] };
            if (dir == "Counterclockwise")
            {
                pcoord = new int[] { -pcoord[1], pcoord[0] };
            }
            else if (dir == "Clockwise")
            {
                pcoord = new int[] { pcoord[1], -pcoord[0] };
            }

            return new int[] { pcoord[0] + axis[0], pcoord[1] + axis[1] };
        }

        public bool isSomethingBelow()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][0] + 1 >= 23)
                    return true;
                if (location[i][0] + 1 < 23)
                {
                    if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0] + 1, location[i][1]] == 1)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public bool? isOverlayBelow(List<int[]> location)
        {
            List<int> ycoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                ycoords.Add(location[i][0]);
                if (location[i][0] >= 23)
                    return true;
                if (location[i][0] < 0)
                    return null;
                if (location[i][1] < 0)
                {
                    return null;
                }
                if (location[i][1] > 9)
                {
                    return null;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                if (ycoords.Max() - ycoords.Min() == 3)
                {
                    if (ycoords.Max() == location[i][0] | ycoords.Max() - 1 == location[i][0])
                    {
                        if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
                else
                {
                    if (ycoords.Max() == location[i][0])
                    {
                        if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool isSomethingLeft()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 0)
                {
                    return true;
                }
                else if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1] - 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool? isOverlayLeft(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] < 0)
                {
                    return true;
                }
                if (location[i][1] > 9)
                {
                    return false;
                }
                if (location[i][0] >= 23)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Min() == location[i][1] | xcoords.Min() + 1 == location[i][1])
                    {
                        if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (xcoords.Min() == location[i][1])
                    {
                        if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public bool isSomethingRight()
        {
            for (int i = 0; i < 4; i++)
            {
                if (location[i][1] == 9)
                {
                    return true;
                }
                else if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1] + 1] == 1)
                {
                    return true;
                }
            }
            return false;
        }
        public bool? isOverlayRight(List<int[]> location)
        {
            List<int> xcoords = new List<int>();
            for (int i = 0; i < 4; i++)
            {
                xcoords.Add(location[i][1]);
                if (location[i][1] > 9)
                {
                    return true;
                }
                if (location[i][1] < 0)
                {
                    return false;
                }
                if (location[i][0] >= 23)
                    return null;
                if (location[i][0] < 0)
                    return null;
            }
            for (int i = 0; i < 4; i++)
            {
                if (xcoords.Max() - xcoords.Min() == 3)
                {
                    if (xcoords.Max() == location[i][1] | xcoords.Max() - 1 == location[i][1])
                    {
                        if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }

                }
                else
                {
                    if (xcoords.Max() == location[i][1])
                    {
                        if (Tetris.Game_Program.droppedtetrominoeLocationGrid[location[i][0], location[i][1]] == 1)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        // For updating new tetris block.
        public void Update()
        {
            for (int i = 0; i < 23; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    Tetris.Game_Program.grid[i, j] = 0;
                }
            }
            for (int i = 0; i < 4; i++)
            {
                Tetris.Game_Program.grid[location[i][0], location[i][1]] = 1;
            }
            Tetris.Game_Program.Draw();
        }
    }
}
