
namespace Start.Game_Classes
{
    using System;
    using System.Threading;

    public static class Board
    {
        //Points as in positions
        private static char[,] positions;

        private static string title;

        private static int refreshRate = 200;

        private static char fruitSymbol;


        //UI
        private static char horizontalWall = '|';

        private static char verticalWall = '-';


        public static Snake Snake;

        public static bool gameIsOver;

        public static int Width;

        public static int Heigth;

        public static Point Fruit;

        public static bool FruitIsEaten;

        //Set up the Board
        static Board()
        {
            Board.FruitIsEaten = true;

            GenerateFruit();

            Board.fruitSymbol = '*';

            Board.gameIsOver = false;

            //Set Heigth and Width
            Heigth = 25;
            Width = 50;

            //Configure Console.
            title = "Snake";
            Console.SetWindowSize(Width + 10, Heigth + 10);
            Console.SetBufferSize(Width + 10, Heigth + 10);
            Console.CursorVisible = false;
            Console.Title = title;

            positions = new char[Heigth, Width];
            Clear();

            Snake = new Snake();
        }

        //Clears the board with white spaces
        private static void Clear()
        {
            for (int i = 0; i < Heigth; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    positions[i, j] = ' ';
                }
            }
        }

        //Updates the Board and repositions the fruit/snake in the array
        private static void Update()
        {
            Clear();

            if (Board.FruitIsEaten)
            {
                GenerateFruit();
                Board.FruitIsEaten = false;
            }

            for (int i = 0; i < Snake.BodyPartsPositions.Count; i++)
            {
                int x = Snake.BodyPartsPositions[i].X;
                int y = Snake.BodyPartsPositions[i].Y;

                positions[y, x] = '0';
            }

            positions[Board.Fruit.Y, Board.Fruit.X] = Board.fruitSymbol;
        }

        //Draws a line with a the given message in the middle of it
        private static void drawMessage(string message)
        {
            System.Console.Write(horizontalWall);
            for (int j = 0; j < (Width - message.Length) / 2; j++)
            {
                System.Console.Write(' ');
            }
            Console.Write(message);
            for (int j = 0; j < (Width - message.Length) / 2; j++)
            {
                System.Console.Write(' ');
            }
            System.Console.WriteLine(horizontalWall);
        }

        //Draws a vertical line for the top and bottom portions of the board
        private static void drawVerticalLine()
        {
            for (int i = 0; i < Width + 2; i++)
            {
                System.Console.Write(verticalWall);
            }
            System.Console.WriteLine();
        }

        //Checks if the given tile is inside the board 
        public static bool tileIsInside()
        {
            return true;//TODO
        }

        //Draws the whole board
        public static void Draw()
        {
            //Draw top BoundingBox
            drawVerticalLine();

            for (int i = 0; i < Heigth; i++)
            {
                System.Console.Write(horizontalWall);
                for (int j = 0; j < Width; j++)
                {
                    System.Console.Write(positions[i, j]);
                }
                System.Console.WriteLine(horizontalWall);
            }

            //Draw bottom BoundingBox
            drawVerticalLine();
        }

        //Draws the board with a message GAME OVER in the middle of it
        public static void DrawGameOver()
        {
            string gameOver = "GAME OVER!";

            //Draw top BoundingBox
            drawVerticalLine();

            for (int i = 0; i < Heigth / 2; i++)
            {
                System.Console.Write(horizontalWall);
                for (int j = 0; j < Width; j++)
                {
                    System.Console.Write(' ');
                }
                System.Console.WriteLine(horizontalWall);
            }

            drawMessage(gameOver);


            for (int i = 0; i < Heigth / 2; i++)
            {
                System.Console.Write(horizontalWall);
                for (int j = 0; j < Width; j++)
                {
                    System.Console.Write(' ');
                }
                System.Console.WriteLine(horizontalWall);
            }
            //Draw bottom BoundingBox
            drawVerticalLine();
        }

        //Generates a random position for the fruit
        public static void GenerateFruit()
        {

            Random rnd = new Random();
            Board.Fruit = new Point();
            bool generateFruitAgain = false;
            do
            {
                Board.Fruit.X = rnd.Next(0, Board.Width);
                Board.Fruit.Y = rnd.Next(0, Board.Heigth);

                generateFruitAgain = false;

                //Checks if the fruit is generated on top of the snake
                if (Snake != null)
                {
                    for (int i = 0; i < Snake.Length; i++)
                    {
                        if (Snake.BodyPartsPositions[i].isEqualTo(Board.Fruit))
                        {
                            generateFruitAgain = true;
                        }
                    }
                }

            } while (generateFruitAgain == true);


        }

        //Game loop. Starting point
        public static void Run()
        {
            while (true)
            {
                Snake.Update();
                Board.Update();

                if (gameIsOver == true)
                {
                    DrawGameOver();
                    break;
                }
                Draw();
                Console.WriteLine("Snake Length : " + Snake.Length);

                //Console.WriteLine("Refresh rate in ms : " + refreshRate);
                //Thread.Sleep(refreshRate);

                //Alternative is above. If we use the Thread.Sleep it doesnt take any input because the program is 
                //sleeping and it gets laggy. Could be fixed with multiple threading and shared variable for direction.

                //50000 is a magic number
                for (int i = 0; i < 50000; i++)
                {
                    Snake.UpdateDirection();
                }

                Console.Clear();
            }
        }

        //Checks if the given point is outside of the board
        public static bool pointIsOutsideOfTheBoard(Point point)
        {
            if (point.X < 0 || point.X >= Width || point.Y < 0 || point.Y >= Heigth)
            {
                return true;
            }
            return false;
        }
    }
}
