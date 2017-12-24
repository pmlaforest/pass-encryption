/*
 * 1. Create an instance of a passManagerContext by calling createSQLiteConnection();
 *  
 * 2. If you want to get/delete an info related to an account first you must log-in with that account.
 *    To do that, use the connectUserToDB() function.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp.SQLite
{
class SQLiteController
    {
        passManagerContext dbConnection;
        int connectedUserId = -1;

        public SQLiteController() { }

        public void CreateSQLiteConnection() {
            dbConnection = new passManagerContext();
        }
        public void connectUserToDB(string user) {
            //setting the id of the connected user to the correct one
            foreach (var username in dbConnection.Usernames)
            {
                if (username.Name == user)
                {
                    connectedUserId = username.UsernameId;
                    return;
                }
            }
            Console.WriteLine("Could not find this user");
            Console.WriteLine("ERROR");
            return;
        }
        public string GetPassword(string tag) {
            //Verify that a user is connected
            if (connectedUserId == -1) {
                Console.WriteLine("A user must be connected to retrieve a password");
                return "";
            }

            //get a password's tag for a given user
            foreach (var password in dbConnection.Passwords)
            {
                if (password.UsernameId == connectedUserId && password.tag == tag)
                {
                    return password.hashedPass;
                }
            }
            Console.WriteLine("Could not find this tag");
            return "";
        }
        public string[] GetMainPassword(string user) {
            //Getting the main password corresponding to the connected user
            string[] hashedAndSalt = new string[2];
            foreach (var username in dbConnection.Usernames) {
                if (username.Name == user) {
                    hashedAndSalt[0] = username.MainPassword;
                    hashedAndSalt[1] = username.Salt;
                    return hashedAndSalt;
                }  
            }
            Console.WriteLine("Could not find this user");
            return null;
        }
        public void AddUser(string user, string hashedPass, string salt) {
            dbConnection.Usernames.Add(new Username { Name = user, MainPassword = hashedPass, Salt = salt});
            dbConnection.SaveChanges();
        }
        public void AddPassword(string tagToAdd, string pass) {
            //Verify that a user is connected
            if (connectedUserId == -1) {
                Console.WriteLine("A user must be connected to retrieve a password");
                return;
            }

            dbConnection.Passwords.Add(new Password { hashedPass = pass, tag = tagToAdd, UsernameId = connectedUserId });
            dbConnection.SaveChanges();
        }
        public void DeletePassword(string tagToSearchFor) {

            //Verify that a user is connected
            if (connectedUserId == -1) {
                Console.WriteLine("A user must be connected to retrieve a password");
                return;
            }

            // Deleting the password
            var stud = (from s1 in dbConnection.Passwords
                        where s1.tag == tagToSearchFor && s1.UsernameId == connectedUserId
                        select s1).FirstOrDefault();

            //Delete it from memory
            dbConnection.Remove(stud);
            //Save to database
            dbConnection.SaveChanges();
        }

        public void DeleteUser() {
            //Verify that a user is connected
            if (connectedUserId == -1) {
                Console.WriteLine("A user must be connected to retrieve a password");
                return;
            }

            // Deleting all password associated with the user
            dbConnection.Passwords.DefaultIfEmpty(null);
            while (true)
            {
                var pass = (from s1 in dbConnection.Passwords
                        where s1.UsernameId == connectedUserId
                        select s1).FirstOrDefault();
                if (pass == null) {
                    break;
                }
                //Delete it from memory
                dbConnection.Remove(pass);
                //Save to database
                dbConnection.SaveChanges();

            }

            //Deleting the user
            var user = (from s1 in dbConnection.Usernames
                        where s1.UsernameId == connectedUserId
                        select s1).FirstOrDefault();
            dbConnection.Remove(user);
            dbConnection.SaveChanges();
            connectedUserId = -1;
        }
        public void PrintUsersInDB()
        {
            Console.WriteLine();
            Console.WriteLine("All users in database:");
            foreach (var username in dbConnection.Usernames)
            {
                Console.WriteLine(" - {0}.{1}:{2}", username.UsernameId, username.Name, username.MainPassword);
            }
        }
        public void PrintPasswordInDB()
        {
            Console.WriteLine();
            Console.WriteLine("All passwords in database:");
            foreach (var password in dbConnection.Passwords)
            {
                Console.WriteLine(" - {0}.{1}:{2}-{3}", password.PasswordId, password.tag, password.hashedPass, password.UsernameId);
            }
        }
    }
}
