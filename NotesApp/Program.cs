using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NoteApp
{
    class Program
    {
        static readonly string NoteDirectory = 
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Notes\";

        static void Main(string[] args)
        {
            Directory.CreateDirectory(NoteDirectory);
            CheckNotes();
            HandleUserCommand();
        }

        private static void HandleUserCommand()
        {
            while (true)
            {
                CommandAvailable();
                Console.WriteLine("Ready! ");
                Console.Write("Your command:  ");
                ReadCommand();
            }
        }
        private static bool TryGetFileInfo(out FileInfo[] fileInfo)
        {
            DirectoryInfo dir = new DirectoryInfo(NoteDirectory);
            fileInfo = dir.GetFiles("*.txt");
            return fileInfo.Length != 0;
        }

        private static void CheckNotes()
        {
            var checkFiles = TryGetFileInfo(out _);
            if (!checkFiles)
            {
                WriteNote();
            }
        }

        private static void ReadCommand()
        {
            string commandInput = Console.ReadLine();

            if (TryGetNoteCommand(commandInput, out Action command))
            {
                command();
            }
        }

        private static bool TryGetNoteCommand(string commandInput, out Action command)
        {
            return Commands.TryGetValue(commandInput.ToLower(), out command);
        }

        private static void WriteNote()
        {
            Console.WriteLine("Enter note: ");
            string input = Console.ReadLine();

            Console.WriteLine("Enter file name: ");
            string filename = Console.ReadLine() + ".txt";

            using StreamWriter writer = File.CreateText(NoteDirectory + filename);
            writer.WriteLine(input);
            writer.Flush();
            writer.Close();
        }
        private static List<string> ShowNoteContent(string notePath)
        {
            List<string> lines = File.ReadAllLines(notePath).ToList();
            Console.WriteLine("+------------");
            foreach (string line in lines)
            {
                Console.WriteLine(line);
            }
            Console.WriteLine("+------------");
            return lines;
        }

        private static string GetNotePath()
        {
            string path = NoteDirectory + Console.ReadLine().ToLower() + ".txt";

            return path;
        }

        private static void ShowNote()
        {
            var getFiles = TryGetFileInfo(out FileInfo[] fileInfo);
            if (getFiles)
            {
                Console.WriteLine("+------------+");
                foreach (var item in fileInfo)
                {
                    Console.WriteLine("   " + item.Name);
                }
                Console.WriteLine("+------------+");
            }
            else
            {
                Console.WriteLine("+------------+\nNo notes found!\n+------------+");
            }
        }

        private static void EditNote()
        {
            Console.WriteLine("Please enter file name:  ");
            string fileName = Console.ReadLine().ToLower() + ".txt";
            string notePath = NoteDirectory + fileName;
            Console.WriteLine("Content: ");
            if (File.Exists(notePath))
            {
                var lines = ShowNoteContent(notePath);
                Console.WriteLine("Input more text: ");
                string input = Console.ReadLine();
                lines.Add(input);
                File.WriteAllLines(notePath, lines);
            }
            else
            {
                Console.WriteLine("File doesn't exist !");
            }
        }

        private static void ReadNote()
        {
            Console.WriteLine("Enter file name you wish to read: ");
            string notePath = GetNotePath();
            Console.WriteLine("Content: ");

            if (File.Exists(notePath))
            {
                ShowNoteContent(notePath);
            }
            else
            {
                Console.WriteLine("File doesn't exist !");
            }
        }

        private static void DeleteNote()
        {
            Console.WriteLine("Enter file name you wish to delete: ");
            string notePath = GetNotePath();
            if (File.Exists(notePath))
            {
                Console.Write("Are you sure you wish to delete this file?? Y/N: ");
                string confirmation = Console.ReadLine().ToLower();

                if (confirmation == "y")
                {
                    File.Delete(notePath);
                    Console.WriteLine("File has been deleted !");
                }
                else if (confirmation == "n")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Invalid command, please re-enter!");
                    DeleteNote();
                }
            }
            else
            {
                Console.WriteLine("File doesn't exist !");
            }
        }

        private static void Exit()
        {
            Environment.Exit(0);
        }
        private static void CommandAvailable()
        {
            Console.WriteLine("  end - Close the application\n" +
                "  new - Write a new note\n" +
                "  read - Read a note\n" +
                "  edit - Edit note\n" +
                "  delete - Delete a note\n" +
                "  show - Show all notes\n" +
                "  dir - Open note directory\n" +
                "  cls - Clear screen\n");
        }
        private static void ShowNoteDirectory()
        {
            Process.Start("explorer.exe", NoteDirectory);
        }

        private static readonly Dictionary<string, Action> Commands = new()
        {
            { "read", ReadNote },
            { "new", WriteNote },
            { "edit", EditNote },
            { "show", ShowNote },
            { "delete", DeleteNote },
            { "dir", ShowNoteDirectory },
            { "cls", Console.Clear },
            { "end", Exit }
        };
    }
}




