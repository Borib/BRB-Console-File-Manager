using System;
using System.Diagnostics;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using CFMF = BRB_Console_File_Manager.ConsoleFileManagerFunctions;
using System.Security.Principal;

namespace BRB_Console_File_Manager
{
    internal class Program
    {
        /// <summary>
        /// Флаг для работы обработчика изменения размера окна
        /// </summary>
        private static bool ThreadWork;
        /// <summary>
        /// Текст введенной пользователем команды.
        /// </summary>
        private static string Command;
        /// <summary>
        /// Словарь, описывающий наполнение выбранной папки. string - имя вложенной папки или файла, bool - флаг, указывающий на тип элемента (true = папка, false = файл).
        /// </summary>
        private static List<string> FolderList;
        private static List<string> FileList;
        //private static int ElementsOnPage = 10; //32
        private static string CurrentDir;

        public static void Main(string[] args)
        {
            Console.Title = "BRB Console File Manager";
            Console.CursorVisible = false;
            Console.SetWindowSize(200, 50);
            CFMF.DrawInterface();
            ParseCommand(@"cd c:\");
            Console.SetCursorPosition(0, 0);
            Thread WindowSizeUpdater = new Thread(UpdateWindowSize);
            WindowSizeUpdater.Start();
            CFMF.SetCursorDefaultPosition();
            Command = string.Empty;
            while (Command.ToLower() != "exit")
            {
                Command = Console.ReadLine();
                ParseCommand(Command);
            }

            ThreadWork = false;
            WindowSizeUpdater.Join();
        }
        /// <summary>
        /// Контроль изменения размера окна
        /// </summary>
        static void UpdateWindowSize()
        {
            ThreadWork = true;
            while (ThreadWork)
            {
                if (Console.WindowHeight != 50 || Console.WindowWidth != 200)
                {
                    //Thread.Sleep(500);
                    Console.SetWindowSize(200, 50);
                    //Console.SetCursorPosition(0, 0);
                }
            }
        }        
        
        /// <summary>
        /// Обработка команд пользователя
        /// </summary>
        /// <param name="cmd"></param>
        private static void ParseCommand(string cmd)
        {
            if (cmd.ToLower() == "help")
            {
                CFMF.ShowCommandsList();
            }
            else if (cmd.ToLower() == "exit")
            {
                return;
            }
            else if (cmd.ToLower().StartsWith("cd "))
            {
                CFMF.ClearOutputArea();
                CFMF.ClearMainArea();
                cmd = cmd.Remove(0, 3).ToLower();
                if (cmd.StartsWith(@"c:\"))
                {
                    if (Directory.Exists(cmd))
                    {
                        CurrentDir = cmd;
                        FolderList = new List<string>();
                        FileList = new List<string>();
                        foreach (string str in Directory.EnumerateDirectories(cmd))
                        {                            
                            FolderList.Add(str);
                        }
                        foreach (string str in Directory.EnumerateFiles(cmd))
                        {                            
                            FileList.Add(str);
                        }
                        CFMF.ShowPageFromList(1, FolderList, FileList);
                    }
                    else
                    {
                        CFMF.SetTextIntoOutputArea($"Указанная папка ({cmd}) не существует.");
                    }
                }
                else
                {
                    CFMF.SetTextIntoOutputArea($"Указанная папка ({cmd}) не существует.");
                }
            }
            else if (cmd.ToLower().StartsWith("list "))
            {
                CFMF.ClearOutputArea();
                cmd = cmd.Remove(0, 5).ToLower();
                try
                {
                    Convert.ToInt32(cmd);
                    CFMF.ShowPageFromList(Convert.ToInt32(cmd), FolderList, FileList);
                }
                catch
                {
                    CFMF.SetTextIntoOutputArea($"Указан некорректный номер страницы в команде list.");
                }
            }
            else if (cmd.ToLower().StartsWith("fi "))
            {
                cmd = cmd.Remove(0, 3).ToLower();
                if (Directory.Exists(CurrentDir + "\\" + cmd))
                {
                    string result = string.Empty;
                    result += $"Дата создания: {Directory.GetCreationTime(CurrentDir + "\\" + cmd).ToString()} Дата изменения: {Directory.GetLastWriteTime(CurrentDir + "\\" + cmd).ToString()}{Environment.NewLine}";
                    result += $"Количество файлов: {Directory.GetFiles(CurrentDir + "\\" + cmd).Length.ToString()} Количество папок: {Directory.GetDirectories(CurrentDir + "\\" + cmd).Length.ToString()}{Environment.NewLine}";
                    result += $"Владелец: {Directory.GetAccessControl(CurrentDir + "\\" + cmd, System.Security.AccessControl.AccessControlSections.Owner).GetOwner(typeof(NTAccount)).Value}";
                    CFMF.SetTextIntoOutputArea(result);
                }
                else if (File.Exists(CurrentDir + "\\" + cmd))
                {
                    string result = string.Empty;
                    result += $"Дата создания: {File.GetCreationTime(CurrentDir + "\\" + cmd).ToString()} Дата изменения: {File.GetLastWriteTime(CurrentDir + "\\" + cmd).ToString()}{Environment.NewLine}";
                    result += $"Размер файла (в байтах): {new FileInfo(CurrentDir + "\\" + cmd).Length.ToString()} Только для чтения: {(new FileInfo(CurrentDir + "\\" + cmd).IsReadOnly ? "да" : "нет")}{Environment.NewLine}";
                    result += $"Размер файла (в мегабайтах): {(Convert.ToSingle((new FileInfo(CurrentDir + "\\" + cmd).Length / 1024)) / 1024f).ToString()}{Environment.NewLine}";
                    result += $"Владелец: {File.GetAccessControl(CurrentDir + "\\" + cmd, System.Security.AccessControl.AccessControlSections.Owner).GetOwner(typeof(NTAccount)).Value}";
                    CFMF.SetTextIntoOutputArea(result);
                }
                else
                {
                    CFMF.SetTextIntoOutputArea($"Указанный файл или папка ({cmd}) не существует.");
                }
            }
            else if (cmd.ToLower().StartsWith("del "))
            {
                cmd = cmd.Remove(0, 4).ToLower();
                if (Directory.Exists(CurrentDir + "\\" + cmd))
                {
                    try
                    {
                        Directory.Delete(CurrentDir + "\\" + cmd, true);                        
                        ParseCommand($"cd {CurrentDir}");
                        CFMF.SetTextIntoOutputArea($"Папка {cmd} удалёна.");
                    }
                    catch (Exception ex)
                    {
                        CFMF.SetTextIntoOutputArea($"Не удалось удалить папку {cmd}.{Environment.NewLine}Описание проблемы:{Environment.NewLine}{ex.Message}");
                    }
                }
                else if (File.Exists(CurrentDir + "\\" + cmd))
                {
                    try
                    {
                        File.Delete(CurrentDir + "\\" + cmd);                        
                        ParseCommand($"cd {CurrentDir}");
                        CFMF.SetTextIntoOutputArea($"Файл {cmd} удалён.");
                    }
                    catch(Exception ex)
                    {
                        CFMF.SetTextIntoOutputArea($"Не удалось удалить файл {cmd}.{Environment.NewLine}Описание проблемы:{Environment.NewLine}{ex.Message}");
                    }
                }
                else
                {
                    CFMF.SetTextIntoOutputArea($"Указанный файл или папка ({cmd}) не существует.");
                }
            }
            else if (cmd.ToLower().StartsWith("copy "))
            {
                cmd = cmd.Remove(0, 5).ToLower();
                
            }
            else
            {
                CFMF.SetTextIntoOutputArea("Команда не распознана. Введите help для получения списка команд.");
            }            
            CFMF.SetTextIntoPathArea(CurrentDir);
            CFMF.SetCursorDefaultPosition();
        } 
    }
}