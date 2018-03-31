namespace Start.Game_Classes
{
    public class Point
    {
        private int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                if (value >= 0 && value < Board.Width)
                {
                    x = value;
                }
            }
        }

        private int y;
        public int Y
        {
            get
            {
                return y;
            }
            set
            {
                if (value >= 0 && value < Board.Heigth)
                {
                    y = value;
                }
            }
        }
        public Point()
        {
            X = 0;
            Y = 0;
        }

        public Point(Point other)
        {
            this.X = other.X;
            this.Y = other.Y;
        }

        //Checks if two points are the same
        public bool isEqualTo(Point other)
        {
            if (this.X == other.X && this.Y == other.Y)
            {
                return true;
            }
            return false;
        }
    }
}
