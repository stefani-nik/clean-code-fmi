
namespace Start
{
    using System;
    using Game_Classes;
    using System.Windows.Input;
    public class Start
    {
        [STAThread]
        static void Main(string[] args)
        {
            Board.Run();
            Console.WriteLine("Press Escape to exit");
            while (true)
            {
                if (Keyboard.IsKeyDown(Key.Escape))
                {
                    break;
                }
            }
        }
    }
}
