namespace Start.Game_Classes
{
    using System;
    using System.Collections.Generic;
    using Enums;

    public class Snake
    {
        //Where the snake is moving towards
        private Direction lookingAt;
        private Direction LookingAt
        {
            get
            {
                return lookingAt;
            }
            set
            {
                lookingAt = value;
            }
        }

        //Length of the snake
        private int length;
        public int Length
        {
            get
            {
                return length;
            }
            set
            {
                // Between 3 and number of Board tiles. 3 comes from the game rules. 
                if (value >= 3 && value < Board.Width * Board.Heigth)
                {
                    length = value;
                }
            }
        }

        //List of all the positions of the body parts : (1,0),(1,1) ...
        //First point is the head
        private IList<Point> bodyPartsPositions;
        public IList<Point> BodyPartsPositions
        {
            get
            {
                return bodyPartsPositions;
            }
            private set
            {
                bodyPartsPositions = value;
            }
        }
        
        public Snake()
        {
            //Could be changed. Game will still work. Minimum is 3
            Length = 10;

            LookingAt = Direction.Right;

            BodyPartsPositions = new List<Point>();

            generateStartingPositions();

        }

        //Updates the direction variable lookingAt
        public void UpdateDirection()
        {
            Direction directionFromKeyboard = InputHandler.GetDirection();
            //check for bad input. If the player tries to reverse the direction
            
            if (this.lookingAt == Direction.Up && directionFromKeyboard == Direction.Down)
            {
                return;
            }
            if (this.lookingAt == Direction.Down && directionFromKeyboard == Direction.Up)
            {
                return;
            }
            if (this.lookingAt == Direction.Left && directionFromKeyboard == Direction.Right)
            {
                return;
            }
            if (this.lookingAt == Direction.Right && directionFromKeyboard == Direction.Left)
            {
                return;
            }
            if(directionFromKeyboard == Direction.None)
            {
                return;
            }
            lookingAt = directionFromKeyboard;
        }

        //Updates the whole snake. Moves it. Makes it bigger if it hits a fruit. 
        //Signals the board if the game is over because it bit itself or is out of bounds.
        public void Update()
        {
            UpdateDirection();

            Point head = bodyPartsPositions[0];
            Point newPositionForHead = new Point(head);
            if (LookingAt == Direction.Right)
            {
                newPositionForHead.X += 1;
            }
            else if (LookingAt == Direction.Up)
            {
                newPositionForHead.Y -= 1;
            }
            else if (LookingAt == Direction.Down)
            {
                newPositionForHead.Y += 1;
            }
            else if (LookingAt == Direction.Left)
            {
                newPositionForHead.X -= 1;
            }

            if (pointIsSnakeBodyPart(newPositionForHead))
            {
                Board.gameIsOver = true;
                return;
            }

            if (Board.pointIsOutsideOfTheBoard(newPositionForHead))
            {
                Board.gameIsOver = true;
                return;
            }

            Point tail = new Point(bodyPartsPositions[bodyPartsPositions.Count - 1]);
            
            //Moves the body parts from the tail to the head
            for (int i = bodyPartsPositions.Count - 1; i > 0; i--)
            {
                bodyPartsPositions[i] = bodyPartsPositions[i - 1];
            }
            //The head is moved last
            bodyPartsPositions[0] = newPositionForHead;

            //Checks if the new head position is on a fruit. If it is it makes it longer.
            if (newPositionForHead.isEqualTo(Board.Fruit))
            {
                Board.FruitIsEaten = true;
                bodyPartsPositions.Add(new Point(tail));
                Length++;
            }
        }

        //Initializes the snakes array of body parts. Works for any length that is less than the Board Width
        private void generateStartingPositions()
        {
            //So we can change the starting length without a problem later on
            int minimalPositionForHeadOnXAxis = 5 + Length;

            for (int i = 0; i < Length; i++)
            {
                Point bodyPart = new Point();

                bodyPart.X = minimalPositionForHeadOnXAxis - i;
                bodyPart.Y = 5;

                BodyPartsPositions.Add(bodyPart);
            }

        }
        
        //Checks if the given point contains a body part of the snake
        private bool pointIsSnakeBodyPart(Point askedForPoint)
        {
            for (int i = 0; i < BodyPartsPositions.Count; i++)
            {
                if (BodyPartsPositions[i].isEqualTo(askedForPoint))
                {
                    return true;
                }
            }
            return false;
        }
        
    }
}
