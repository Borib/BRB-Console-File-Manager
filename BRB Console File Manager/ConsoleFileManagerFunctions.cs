using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace BRB_Console_File_Manager
{
    public static class ConsoleFileManagerFunctions
    {
        /// <summary>
        /// Установить курсор в начальную позицию для ввода команд
        /// </summary>
        public static void SetCursorDefaultPosition()
        {
            ClearRowOnScreen(48);
            Console.SetCursorPosition(1, 48);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(">>>");
            Console.SetCursorPosition(5, 48);
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = true;
        }

        /// <summary>
        /// Написать текст в консоли в указанной позиции с заданными параметрами
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="text"></param>
        /// <param name="symbolColor"></param>
        /// <param name="backColor"></param>
        public static void DrawText(int positionX, int positionY, string text, ConsoleColor symbolColor = ConsoleColor.Green, ConsoleColor backColor = ConsoleColor.Black)
        {
            Console.ForegroundColor = symbolColor;
            Console.BackgroundColor = backColor;
            Console.SetCursorPosition(positionX, positionY);
            Console.WriteLine(text);
        }

        public static int GetPagesCount(List<string> folderList, List<string> fileList, int elementsOnPage = 32)
        {
            int pages = (folderList.Count + fileList.Count) / elementsOnPage;
            if (((folderList.Count + fileList.Count) % elementsOnPage) > 0)
            {
                pages++;
            }
            return pages;
        }

        private static string GetIteratedString(string sourceString, uint iterationsCount)
        {
            string result = sourceString;
            for(int i = 2; i <= iterationsCount; i++)
            {
                result += sourceString;
            }
            return result;
        }

        private static void ClearRowOnScreen(uint RowNumber)
        {
            Console.SetCursorPosition(1, (int)RowNumber);
            Console.WriteLine(GetIteratedString(" ", 198));
        }

        /// <summary>
        /// Написать символ в консоли в указанной позиции с заданными параметрами
        /// </summary>
        /// <param name="positionX"></param>
        /// <param name="positionY"></param>
        /// <param name="symbol"></param>
        /// <param name="symbolColor"></param>
        /// <param name="backColor"></param>
        public static void DrawSymbol(int positionX, int positionY, char symbol, ConsoleColor symbolColor = ConsoleColor.Green, ConsoleColor backColor = ConsoleColor.Black)
        {
            Console.SetCursorPosition(positionX, positionY);
            Console.ForegroundColor = symbolColor;
            Console.BackgroundColor = backColor;
            Console.Write(symbol);
        }

        public static void ClearMainArea()
        {
            for (uint j = 3; j < 35; j++)
            {
                ClearRowOnScreen(j);
            }
        }

        public static void ClearOutputArea()
        {
            for (uint j = 36; j < 45; j++)
            {
                ClearRowOnScreen(j);
            }
        }

        /// <summary>
        /// Очистить строку с информацией
        /// </summary>
        public static void ClearInfoArea()
        {
            ClearRowOnScreen(1);
        }

        public static void ClearPathArea()
        {
            ClearRowOnScreen(46);
        }

        public static void SetTextIntoOutputArea(string text)
        {
            ClearOutputArea();            
            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for(int i=36; i<=45; i++)
            {
                if (i - 36 < lines.Length)
                {
                    if (lines[i - 36].Length > 198)
                    {
                        DrawText(1, i, lines[i - 36].Substring(0, 195) + "...");
                    }
                    else
                    {
                        DrawText(1, i, lines[i - 36]);
                    }
                }
            }
        }

        public static void SetTextIntoMainArea(string text)
        {
            ClearMainArea();
            string[] lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            for (int i = 3; i <= 34; i++)
            {
                while (i - 3 <= lines.Length)
                {
                    if (lines[i - 3].Length > 198)
                    {
                        DrawText(1, i - 3, lines[i - 3].Substring(0, 190) + "..." + lines[i - 3].Substring(lines[i - 3].Length - 6, 5));
                    }
                }
            }
        }

        public static void SetTextIntoPathArea(string text)
        {
            ClearPathArea();
            if (text.Length > 198)
            {
                DrawText(1, 46, text.Substring(0, 190) + "..." + text.Substring(text.Length - 6, 5));
            }
            else
            {
                DrawText(1, 46, text);
            }
        }

        public static void SetTextIntoInfoArea(string text)
        {
            ClearInfoArea();
            if (text.Length > 198)
            {
                DrawText(1, 1, text.Substring(0, 190) + "..." + text.Substring(text.Length - 6, 5));
            }
            else
            {
                DrawText(1,1, text);
            }
        }

        /// <summary>
        /// Отобразить список команд
        /// </summary>
        public static void ShowCommandsList()
        {
            string HelpText = string.Empty;
            HelpText += $"help - выводит список команд{Environment.NewLine}";
            HelpText += $"exit - выход из программы{Environment.NewLine}";
            HelpText += $"cd <имя папки> - перейти в папку{Environment.NewLine}";
            HelpText += $"fi <имя папки или файла> - получить информацию о папке или файле{Environment.NewLine}";
            HelpText += $"del <имя папки или файла> - удалить папку или файл{Environment.NewLine}";
            HelpText += $"copy <имя папки или файла источника> <имя папки или файла приёмника> - скопировать папку или файл{Environment.NewLine}";
            HelpText += $"list <номер страницы> - отобразить указанную страницу списка";
            SetTextIntoOutputArea(HelpText);
        }

        public static void ShowPageFromList(int page, List<string> folderList, List<string> fileList, int elementOnPage = 32)
        {
            List<string> FFList = new List<string>();
            FFList.AddRange(folderList);
            FFList.AddRange(fileList);
            int y = 3;
            if (page > GetPagesCount(folderList, fileList))
            {
                SetTextIntoOutputArea($"Указан номер страницы, превышающий количество страниц.");
            }
            else if (page == GetPagesCount(folderList, fileList))
            {
                ClearMainArea();
                foreach (string str in FFList.GetRange((elementOnPage * page) - elementOnPage, FFList.Count - ((elementOnPage * page) - elementOnPage)))
                {
                    DrawText(1, y, $"{str.Substring(str.LastIndexOf("\\") + 1)}", Directory.Exists(str) ? ConsoleColor.Yellow : ConsoleColor.Gray);
                    y++;
                }
                SetTextIntoInfoArea($"Страница {page} из {GetPagesCount(folderList, fileList)}");
            }
            else
            {
                ClearOutputArea();
                foreach (string str in FFList.GetRange((elementOnPage * page) - elementOnPage, elementOnPage))
                {
                    DrawText(1, y, $"{str.Substring(str.LastIndexOf("\\") + 1)}", Directory.Exists(str) ? ConsoleColor.Yellow : ConsoleColor.Gray);
                    y++;
                }
                SetTextIntoInfoArea($"Страница {page} из {GetPagesCount(folderList, fileList)}");
            }
        }

        /// <summary>
        /// Вывод интерфейса программы
        /// </summary>
        public static void DrawInterface()
        {
            //used symbols: ╚ ╝ ║ ═ ╗ ╔ ► ◄ ╣ ╠ ╩ ╦ ↑ → ← ↓ ╬

            DrawText(0, 0, $"╔{GetIteratedString("═", 198)}╗");
            //Vertical border lines
            for (int i = 1; i < 49; i++)
            {
                if (i == 2 || i == 35 || i == 45 || i == 47)
                {
                    DrawSymbol(0, i, '╠');
                    DrawSymbol(199, i, '╣');
                }
                else
                {
                    DrawSymbol(0, i, '║');
                    DrawSymbol(199, i, '║');
                }
            }
            DrawText(1, 2, $"{GetIteratedString("═", 198)}");
            DrawText(1, 35, $"{GetIteratedString("═", 198)}");
            DrawText(1, 45, $"{GetIteratedString("═", 198)}");
            DrawText(1, 47, $"{GetIteratedString("═", 198)}");
            DrawText(0, 49, $"╚{GetIteratedString("═", 198)}╝");
        }
    }
}
