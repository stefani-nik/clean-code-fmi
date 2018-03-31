namespace Start.Game_Classes
{
    using Enums;
    using System.Windows.Input;
    public static class InputHandler
    {
        //Returns a Direction Enum from the keyboard
        public static Direction GetDirection()
        {

            if (Keyboard.IsKeyDown(Key.Down))
            {
                return Direction.Down;
            }
            else if (Keyboard.IsKeyDown(Key.Up))
            {
                return Direction.Up;
            }
            else if (Keyboard.IsKeyDown(Key.Right))
            {
                return Direction.Right;
            }
            else if (Keyboard.IsKeyDown(Key.Left))
            {
                return Direction.Left;
            }
            else
            {
                return Direction.None;
            }
        }


    }
}
