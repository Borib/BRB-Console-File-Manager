using System;

namespace BRB_Console_File_Manager
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "BRB Console File Manager";
            DrawInterface();
            while (true)
            {
                KeyPress(Console.ReadKey(true));
            }
        }

        private static void DrawInterface()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.SetCursorPosition(0, 0);
            Console.Write('╔');  //╚ ╝ ║ ═ ╗ ╔ ► ◄
            Console.SetCursorPosition(Console.WindowHeight-1, Console.WindowWidth-1);
            Console.Write('╝');
            Console.SetCursorPosition(Console.WindowHeight-1, 0);
            Console.Write('╗');
            Console.SetCursorPosition(0, Console.WindowWidth-1);
            Console.Write('╚');
            for (int i=1;i<Console.WindowWidth-2;i++)
            {
                Console.SetCursorPosition(i, 0);
                Console.Write('═');
                Console.SetCursorPosition(i, Console.WindowHeight-1);
                Console.Write('═');
            }
            for (int i=1;i<Console.WindowHeight-2;i++)
            {
                Console.SetCursorPosition(0, i);
                Console.Write('║');
                Console.SetCursorPosition(Console.WindowWidth-1, i);
                Console.Write('║');
            }
        }

        private static void KeyPress(ConsoleKeyInfo consoleKeyInfo)
        {
            switch (consoleKeyInfo.Key)
            {
                
            }
        }
    }
}