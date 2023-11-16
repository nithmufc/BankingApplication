using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Runtime.Remoting.Messaging;
using System.Net;

namespace BankingApplication
{
    class authorization
    {
        
        private readonly int attemptsAllowed = 3;

        private readonly char delimiter = ',';

        private readonly string adminFileName = "adminDetails.txt";
        private readonly string userFileName = "userDetails.txt";
        public bool checkAdminCredFromFile()
        {
            Console.Write("Enter your email: ");
            string email = Console.ReadLine();
            Console.Write("Enter your password: ");
            string password = Console.ReadLine();
            StreamReader adminFile = new StreamReader(adminFileName);
            string actualId = adminFile.ReadLine();
            string actualPassword = adminFile.ReadLine();
            if (actualId == email && actualPassword == password)
                return true;
            else
                return false;
        }

        
        public bool checkUserFromFile(string email, string password)
        {
            
            //A new function can be created which asks the user for email id and returns it if it exists in the userDetails files. 

            bool existingUser = false;

            userClass user = new userClass();
            
            //Code to reading CSV files and storing values into an Array
            //Using StreamReader and lists using Split method, Stack OverFlow --> Highest rated answer
            //https://stackoverflow.com/questions/5282999/reading-csv-file-and-storing-values-into-an-array#comment100654489_5283044
            using (var reader = new StreamReader(userFileName))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    listA.Add(values[user.emailIndex]); //storing all email_ids in a list
                    listB.Add(values[user.passwordIndex]); //storing all passwords in a list
                }

                for (int i = 0; i < listA.Count; i++)
                {
                    if (listA[i].Contains(email) && listB[i].Contains(password))
                    {
                        existingUser = true;
                    }
                }
            }

            return existingUser;
        }

        public string authorizeUser() 
        {

            bool userAuthorised = false;
            int noOfAttempts = 0;
            string email;

            do
            {
                Console.Write("Enter your email: ");
                email = Console.ReadLine();
                Console.Write("Enter your password: ");
                string password = Console.ReadLine();

                if (checkUserFromFile(email, password))
                {
                    userAuthorised = true;
                    Console.WriteLine("Login successful!");
                }
                else
                {
                    noOfAttempts++;
                    if (noOfAttempts == attemptsAllowed)
                    {
                        Console.WriteLine("Details are incorrect and crossed the number of attempts. Exiting..");
                        break;
                    }
                    Console.WriteLine("Details are incorrect. Try again!");
                }
            } while (!userAuthorised);
            return email;
        }

        public bool authorizeAdmin()
        {
            bool adminAuthorised = false;
            int noOfAttempts = 0;

            do
            {
                if (checkAdminCredFromFile())
                {
                    adminAuthorised = true;
                    Console.WriteLine("Login successful!");
                    break;
                }
                else
                {
                    noOfAttempts++;
                    if (noOfAttempts == attemptsAllowed)
                    {
                        Console.WriteLine("Details are incorrect and crossed the number of attempts. Exiting..");
                        break;
                    }
                    Console.WriteLine("Details are incorrect. Try again!");
                }
            } while (!adminAuthorised);
            return adminAuthorised;
        }
        

    }

    class userClass
    {
        private string firstName;
        private string lastName;
        private string address;
        private long phone;
        private string email;
        private string password;
        private decimal balance;

        public readonly int firstNameIndex = 0;
        public readonly int lastNameIndex = 1;
        private readonly int addressIndex = 2;
        private readonly int phoneIndex = 3;
        public readonly int emailIndex = 4;
        public readonly int passwordIndex = 5;
        private readonly int balanceIndex = 6;

        private readonly char delimiter = ',';
        private readonly string delimiterString = ",";
        public void setFields()
        {
            bool error = true;

            do
            {
                try
                {
                    Console.Write("Enter first name: ");
                    firstName = Console.ReadLine();
                    Console.Write("Enter last name: ");
                    lastName = Console.ReadLine();
                    Console.Write("Enter address: ");
                    address = Console.ReadLine();
                    Console.Write("Enter phone number: ");
                    phone = long.Parse(Console.ReadLine());
                    Console.Write("Enter email: ");
                    email = Console.ReadLine();
                    createPassword(password);
                    balance = 0.00m; //Since we are creating a new account, the balance is 0 by default
                    error = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }while(error == true);
        }

        private readonly string userFileName = "userDetails.txt";
        public void storeInFile()
        {
            List<string> user = new List<string>
            {
                firstName,
                lastName,
                address,
                Convert.ToString(phone),
                email,
                password,
                Convert.ToString(balance)
            };

            //Code to write an array of string into a CSV file
            //Using String.Join, Stack Overflow – Highest Rated answer
            //https://stackoverflow.com/questions/8666518/how-can-i-write-a-general-array-to-csv-file
            StreamWriter userFile = new StreamWriter(userFileName, true);
            userFile.WriteLine(string.Join(delimiterString, user));
            userFile.Close(); 
        }

        /*public void getFields()
        {
            Console.WriteLine("Name: " + firstName + " " + lastName + "\n" +
                "Address: " + address + "\n" +
                "Email: " + email + "\n" +
                "Password: " + password + "\n" +
                "Balance: " + balance);
        }*/
        private string createPassword(string pw)
        {
            Console.Write("Create new password: ");
            string password1 = Console.ReadLine();
            Console.Write("Re-enter the password: ");
            string password2 = Console.ReadLine();
            while (password1 != password2)
            {
                Console.Write("Passwords do not match. Create new password: ");
                password1 = Console.ReadLine();
                Console.Write("Re-enter the password: ");
                password2 = Console.ReadLine();
            }
            Console.WriteLine("Password created successfully");
            return password = password1;
        }
        

        private int indexofLine = 0;
        //Function which returns balance for a particular user
        public decimal getBalance(string email_id)
        {

            string[] lines = File.ReadAllLines(userFileName);
            
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(email_id))
                {
                    indexofLine = i;
                }
            }
            var lineToSplit = lines[indexofLine].Split(delimiter);
            decimal accountBalance = Convert.ToDecimal(lineToSplit[balanceIndex]);
            return accountBalance;
        }
        //Function which returns address for a particular user
        public string getAddress(string email_id)
        {
            string[] lines = File.ReadAllLines(userFileName);

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(email_id))
                {
                    indexofLine = i;
                }
            }
            var lineToSplit = lines[indexofLine].Split(delimiter);
            string accAddress = lineToSplit[addressIndex];
            return accAddress;
        }

        //Procedure to debit and credit money into the account
        private void moneyTransactions(decimal ammount, string email)
        {
            List<string> newList = new List<string>();
            
            //To read a file 
            string[] lines = File.ReadAllLines(userFileName);
            List<string> linesList = new List<string>(lines);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Contains(email))
                    indexofLine = i;
            }
            //Console.WriteLine(lines[indexofLine]);

            var lineToSplit = lines[indexofLine].Split(delimiter);

            //code to delete row in a file according to a condition
            //Stack OverFlow --> highest score answer
            //https://stackoverflow.com/questions/48951959/delete-rows-in-a-csv-file
            File.WriteAllLines(userFileName, File.ReadAllLines(userFileName).Where(line => !email.Contains(line.Split(delimiter)[emailIndex])));

            newList.Add(lineToSplit[firstNameIndex]);
            newList.Add(lineToSplit[lastNameIndex]);
            newList.Add(lineToSplit[addressIndex]);
            newList.Add(lineToSplit[phoneIndex]);
            newList.Add(lineToSplit[emailIndex]);
            newList.Add(lineToSplit[passwordIndex]);
            newList.Add(Convert.ToString(Convert.ToDecimal(lineToSplit[balanceIndex]) + ammount));

            Console.WriteLine("Current available balance: {0:00.00}", (Convert.ToDecimal(lineToSplit[balanceIndex]) + ammount));

            //Writing the new list to the file
            StreamWriter userFile = new StreamWriter(userFileName, true);
            userFile.WriteLine(string.Join(delimiterString, newList));
            userFile.Close();
        }

        public void depositMoney(string email_id)
        {
            bool error = true;
            do
            {
                try
                {
                    Console.WriteLine("Available balance: {0:00.00}", getBalance(email_id));
                    Console.Write("Enter the ammount to deposit: ");
                    decimal ammount = decimal.Parse(Console.ReadLine());
                    moneyTransactions(ammount, email_id);
                    error = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (error == true);

        }

        public void withdrawMoney(string email_id)
        {
            bool error = true;
            do
            {
                try
                {
                    Console.WriteLine("Available balance: {0:00.00}", getBalance(email_id));

                    Console.Write("Enter the ammount to be withdrawn: ");
                    decimal ammount = decimal.Parse(Console.ReadLine());

                    if (getBalance(email_id) > ammount)
                    {
                        moneyTransactions(-ammount, email_id); 
                    }
                    else
                    {
                        Console.WriteLine("Failed!");
                        Console.WriteLine("Available balance: {0:00.00}", getBalance(email_id));
                    }
                    error = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (error == true);
            
 
        }
        private char userAnswerYes = 'y';
        public void newUser()
        {
            Console.WriteLine("Register a new account:");
            setFields(); //Prompts to fill in essential user details
            storeInFile(); 

            Console.WriteLine("Initial Balance set to 0.00 by default. Do you want to add money? [y/n]");
            char addMoney;

            try
            {
                addMoney = char.Parse(Console.ReadLine().ToLower());
                if (addMoney == userAnswerYes)
                    depositMoney(email);

                else
                    Console.WriteLine("Balance set to default. Taking you back to the menu.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
    }
    
    class displayMenu
    {
        authorization auth = new authorization();

        userClass user = new userClass();

        public string[] bankMainMenu = new string[] { "Admin ", "User ", "Exit" };
        public string[] userMenu = new string[] { "New User - Register and create new account", "Existing User", "Main Menu" };
        public string[] existingUserMenu = new string[] { "Pay money into the account", "Draw money out of the account", "Find the balance", "Print out the address", "Previous Menu" };

        private int zero = 0;

        //Function to display menu options and return the user's choice
        public int displayOptions(string[] array)
        {

            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine((i+1) + ". " + array[i]);
            }
            int userChoice = 0;
            do
            {
                try
                {
                    Console.Write(">> ");
                    userChoice = int.Parse(Console.ReadLine());
                    if (userChoice <= zero || userChoice > array.Length)
                        throw new Exception("Invalid choice entered.");
                }
                catch (Exception e)
                {
                    //Console.Write("Invalid input, enter a valid number: ");
                    Console.WriteLine(e.Message + " Enter a valid choice: ");
                }

            } while (userChoice <= zero || userChoice > array.Length);
            return userChoice;
        }

        public void existingUserOptions(string email)
        {
            string confirmEmail;
            int choice;

            do
            {
                Console.WriteLine("\n\nWelcome to the Existing User Menu: ");
                choice = displayOptions(existingUserMenu);
                Console.WriteLine("Confirm your email again: ");
                confirmEmail = Console.ReadLine();
                if(confirmEmail == email)
                {
                    switch (choice)
                    {
                        case 1:
                            Console.WriteLine("\n");
                            user.depositMoney(confirmEmail);
                            break;
                        case 2:
                            user.withdrawMoney(confirmEmail);
                            break;
                        case 3:
                            Console.WriteLine("\n");

                            Console.WriteLine("Available balance: {0:00.00}", user.getBalance(confirmEmail));
                            break;
                        case 4:
                            Console.WriteLine(user.getAddress(confirmEmail));
                            break;
                        case 5:
                            break;
                        default:
                            Console.WriteLine("Enter a valid option: ");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Entered email does not match.");
                }
                
            } while (choice != existingUserMenu.Length);
            
        }

        public void userOptions()
        {
            int choice;

            do
            {
                Console.WriteLine("\n\nWelcome to the User Menu: ");
                //To display user menu:
                choice = displayOptions(userMenu);
                switch (choice)
                {
                    case 1:
                        Console.WriteLine("\n");
                        user.newUser();
                        break;

                    case 2:
                        Console.WriteLine("\n");
                        //(bool existingUser, string email) = auth.checkUserFromFile();
                        string email_id = auth.authorizeUser();
                        existingUserOptions(email_id);
                        break;

                    case 3:
                        break;

                    default:
                        Console.WriteLine("Enter a valid option: ");
                        break;
                }
            } while (choice!= userMenu.Length);
            
        }
    }
    class adminClass
    {
        private readonly string userFileName = "userDetails.txt";
        private readonly char delimiter = ',';
        userClass user = new userClass();

        //Code to return multiple values
        //A Tuple can be used to return multiple values from a method. --> C# 7 Tuples: StackOver Flow
        //https://stackoverflow.com/questions/748062/return-multiple-values-to-a-method-caller/36436255#36436255
        private (bool, string) displayUsersAndCheckEmail()
        {
            bool emailExists = false;
            string email_id;

            using (var reader = new StreamReader(userFileName))
            {
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                List<string> listC = new List<string>();

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(delimiter);

                    listA.Add(values[user.firstNameIndex]); //storing all firstnames in a list
                    listB.Add(values[user.lastNameIndex]); //storing all lastnames in a list
                    listC.Add(values[user.emailIndex]); //storing all emails in a list
                }

                Console.WriteLine("These are the available users- with their names and email-ids.");
                Console.WriteLine("\nFirstName  LastName\t\tEmail");

                for (int i = 0; i < listA.Count; i++)
                {
                    Console.WriteLine(listA[i] + "  " + listB[i] + "\t\t" + listC[i]);
                }
                
                Console.WriteLine("\nEnter the email of the user you want to remove:");
                email_id = Console.ReadLine();
                for (int i = 0; i < listA.Count; i++)
                {
                    if (listC[i].Contains(email_id))
                    {
                        emailExists = true;
                    }
                    else
                        emailExists = false;
                }
            }
            return (emailExists, email_id);
        }      
        public void removeUser()
        {
            try
            {
                //Displays all the existing users and if the entered email is existing in the file
                (bool emailExists, string email_id) = displayUsersAndCheckEmail(); 

                if(emailExists)
                {
                    //using a lamda expression to remove a line from the file corresponding to the email --> reference mentioned above
                    File.WriteAllLines(userFileName, File.ReadAllLines(userFileName).Where(line => !email_id.Contains(line.Split(delimiter)[user.emailIndex])));
                    Console.WriteLine("User with " + email_id + " has been removed.\n");
                } 
                else
                {
                    
                    Console.WriteLine("Email not found.");

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    internal class Program
    {
        
        static void Main(string[] args)
        {
            
            int choice;

            authorization auth = new authorization();

            adminClass admin = new adminClass();
            //userClass user = new userClass();

            displayMenu menu = new displayMenu();

            Console.WriteLine("\tBANKING APPLICATION: ");

            do
            {
                Console.WriteLine("\nWELCOME TO THE BANK - Main Menu: ");

                //To display bank main menu: 
                choice = menu.displayOptions(menu.bankMainMenu);
                switch (choice)
                {
                    case 1:
                        if (auth.authorizeAdmin())
                        {
                            Console.WriteLine("As an admin, you can remove a user: ");
                            admin.removeUser();
                        }   
                        break;
                    case 2:
                        menu.userOptions();
                        break;
                    case 3:
                        break;
                    default:
                        Console.WriteLine("Enter a valid option: ");
                        break;
                }
            } while (choice != menu.bankMainMenu.Length); 
        }
    }
}
