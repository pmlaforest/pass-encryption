using System;
using WebTP1;

namespace ConsoleApp.SQLite
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandManager cmdManager = new CommandManager();
           
            cmdManager.parseCommand(args);
        }
    }
}
