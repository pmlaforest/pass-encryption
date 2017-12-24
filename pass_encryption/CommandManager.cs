using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApp.SQLite;

namespace WebTP1
{
    class CommandManager
    {
        private string cmdToPerform = "";
        private string username = "";
        private string masterPassword = "";
        private string tag = "";
        private string tagPassword = "";

        SQLiteController dbController = new SQLiteController();
        PasswordManager passManager = new PasswordManager();

        public CommandManager() {
            //create the connection with the database
            dbController.CreateSQLiteConnection();
        }

        public void parseCommand(string[] args) {
            //Empty array ?
            if (args.Length < 2) {
                Console.WriteLine("ERROR");
                return;
            }

            //The commmand is always the first argument
            cmdToPerform = args[0];

            //The username is always the second argument
            username = args[1];

            //register a user
            if (cmdToPerform == "-r") {

                if (args.Length != 3) {
                    Console.WriteLine("ERROR");
                    return;
                }

                //The masterPassword is always the third argument
                masterPassword = args[2];

                /*
                 *FIRST HASH THE PASSWORD BEFORE SAVING IT 
                 * 
                 */
                string[] hashedAndSalt = passManager.hashPassword(masterPassword);

                dbController.AddUser(username, hashedAndSalt[0], hashedAndSalt[1]);
                //dbController.PrintUsersInDB();
                Console.WriteLine("OK");
            }

            // add a password/tag couple
            else if (cmdToPerform == "-a") {
                if (args.Length != 5)
                {
                    Console.WriteLine("ERROR");
                    return;
                }

                //The masterPassword is always the third argument, tag fourth and tag's password fifth
                masterPassword = args[2];
                tag = args[3];
                tagPassword = args[4];

                /*
                 * Compare the main password with the password entered
                 * if they match, call ConnectUserToDB() and addPassword()
                 * else ERROR
                 * IF MATCH -­>
                 * dbController.ConnectUserToDB(username);
                 * dbController.AddPassword(tag, tagPassword);
                 * ELSE ->
                 * Console.WriteLine("ERROR");
                 */
                string[] passAndSalt = dbController.GetMainPassword(username);
                string[] mainPassTemp = passManager.hashPassword(masterPassword, passAndSalt[1]);
                string mainPassword = mainPassTemp[0];

                if (passManager.compareHash(passAndSalt[0], mainPassword))
                {
                    dbController.connectUserToDB(username);
                    
                    string tagPassCrypted = passManager.cryptTagPassword(tagPassword,new string[]{masterPassword, passAndSalt[1]});

                    dbController.AddPassword(tag, tagPassCrypted);
                    Console.WriteLine("OK");
                }
                else
                {
                    Console.WriteLine("ERROR");
                    return;
                }
            }
            // get a password
            else if (cmdToPerform == "-g") {
                if (args.Length != 4)
                {
                    Console.WriteLine("ERROR");
                    return;
                }

                //The masterPassword is always the third argument
                masterPassword = args[2];
                tag = args[3];

                string[] passAndSalt = dbController.GetMainPassword(username);
                dbController.connectUserToDB(username);
                string passwordEncrypted = dbController.GetPassword(tag);
                var passwordToPrint = passManager.decryptTagPassword(passwordEncrypted,new string[] { masterPassword, passAndSalt[1] });
                 Console.WriteLine("{0}", passwordToPrint);
            }
            // delete a password
            else if (cmdToPerform == "-d") {
                if (args.Length != 4)
                {
                    Console.WriteLine("ERROR");
                    return;
                }

                //The masterPassword is always the third argument
                masterPassword = args[2];
                tag = args[3];

              dbController.connectUserToDB(username);
              dbController.DeletePassword(tag);
                Console.WriteLine("OK");
            }
            // return an hashed password with a salt
            else if (cmdToPerform == "-t") {
                if (args.Length == 2)
                {
                    // get mainPassword + salt
                    string[] passAndSalt = dbController.GetMainPassword(username);
                    Console.WriteLine(passAndSalt[1]+":"+passAndSalt[0]);
                }
                else if (args.Length == 3)
                {
                    tag = args[2];
                    // return hashed password associated with tag
                    dbController.connectUserToDB(username);
                    string passwordToPrint =  dbController.GetPassword(tag);
                    Console.WriteLine("{0}", passwordToPrint);

                }
                else
                {
                    Console.WriteLine("ERROR");
                    return;
                }
            }
            // something went wrong
            else {
                Console.WriteLine("ERROR");
            }
        }
        private void login(string user, string pass) { }
        private void register(string user, string pass) { }

        private void addUser(string user, string masterPassword) { }

        private void addTag(string tag, string hashedPass) { }
        private void getPassword(string tag) { }
        private void delete(string tag) { }
        private void getHash(string user) { }
        private void getHash(string user, string tag) { }
        private void display() { }

    }
}
