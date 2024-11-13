using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
namespace DrugoPredavanje
{
    class Program
    {
        static string GetValidInput(string[] validInputs)
        {
            string userInput;

            do
            {
                Console.Write("Please enter one of the following options: ");
                Console.WriteLine(string.Join(", ", validInputs));
                userInput = Console.ReadLine().ToLower().Trim();

                if (Array.IndexOf(validInputs, userInput) == -1)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
            while (Array.IndexOf(validInputs, userInput) == -1);

            return userInput;
        }
        static void newUser(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni)
                    
        {
            Console.Clear();
            int numberOfUsers = users.Count;

            Console.WriteLine("Enter the name: ");
            string newName = Console.ReadLine();

            Console.WriteLine("Enter the surname: ");
            string newSurname = Console.ReadLine();

            DateTime newDateOfBirth;
            do
            {
                Console.WriteLine("Enter your date of birth (dd/MM/yyyy):");
                string inputDate = Console.ReadLine();

                if (DateTime.TryParseExact(inputDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDateOfBirth))
                {
                    if (newDateOfBirth <= DateTime.Today)
                    {
                        Console.WriteLine("Valid date of birth: " + newDateOfBirth.ToShortDateString());
                        break; // Exit the loop if the date is valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid date of birth. Please enter a date in the past.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please use dd/MM/yyyy format.");
                }
            } while (true); // Loop until a valid date is entered
            users[numberOfUsers + 1] = ((newName.Substring(0,1).ToUpper() + newName.Substring(1).ToLower()), (newSurname.Substring(0, 1).ToUpper() + newSurname.Substring(1).ToLower()), newDateOfBirth);
            Console.WriteLine("User successfully entered!");
            tekuciRacuni.Add(numberOfUsers + 1, (100.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            ziroRacuni.Add(numberOfUsers + 1, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            prepaidRacuni.Add(numberOfUsers + 1, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            Console.WriteLine("Press any key to go back to the start");
            Console.ReadKey();

            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void listUsers(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni)
                   
        {
            Console.Clear();
            string startMessaage = """
                a) ispis svih korisnika abecedno po prezimenu
                b) svih onih koji imaju više od 30 godina
                c) svih oni koji imaju barem jedan račun u minusu
                """;
            
            Console.WriteLine(startMessaage);

            char input = 'd';
            bool isValidInput = false;
            Console.Write("Enter 'a', 'b', or 'c': ");
            do
            {
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                isValidInput = input == 'a' || input == 'b' || input == 'c' || input == 'A' || input == 'B' || input == 'C';

                if (!isValidInput)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            } while (!isValidInput);

            switch (input.ToString().ToLower())
            {
                case "a":
                    foreach (var item in users.Keys)
                    {
                        Console.WriteLine($"{item} {users[item].name} {users[item].surname} {users[item].dateOfBirth.Year}");
                    }
                    break;
                case "b":
                    foreach (var item in users.Keys) 
                    {
                        int yearsOfAge = DateTime.Now.Year - users[item].dateOfBirth.Year;
                        if (yearsOfAge > 30)
                        {
                            Console.WriteLine($"{item} {users[item].name} {users[item].surname} {users[item].dateOfBirth.Year}");
                        }
                    }
                    break;
                case "c":
                    foreach (var item in users.Keys) 
                    {
                        if ((ziroRacuni[item].balance < 0) || (prepaidRacuni[item].balance < 0) || (tekuciRacuni[item].balance < 0)) 
                        {
                            Console.WriteLine($"{item} {users[item].name} {users[item].surname} {users[item].dateOfBirth.Year}");
                        }

                    }
                    break;
            }
            Console.WriteLine("Press any key to get back to the start.");
            Console.ReadKey();
            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static void userDropDown(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni)
               
        {
            string startMessaage = """
                 1 - Unos novog korisnika
                 2 - Brisanje korisnika
                        a) po id-u
                        b) po imenu i prezimenu
                 3 - Uređivanje korisnika
                        a) po id-u
                 4 - Pregled korisnika
                        a) ispis svih korisnika abecedno po prezimenu
                        b) svih onih koji imaju više od 30 godina
                        c) svih oni koji imaju barem jedan račun u minusu
                5 - Povratak na start
                0 - Izlaz iz aplikacije
                """;
            Console.Clear();
            Console.WriteLine(startMessaage);

            int choice = 10;
            do
            {
                Console.WriteLine("Enter a choice between 1 and 5(or 0 to leave the app):");
                var input = Console.ReadLine();

                if (int.TryParse(input, out choice))
                {
                    if (choice >= 1 && choice <= 5)
                    {
                        Console.WriteLine("Valid choice: " + choice);
                        break; // Exit the loop if the choice is valid
                    }
                    else if (choice == 0)
                    {
                        //Console.WriteLine("Please enter a choice between 1 and 5.");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a valid choice between 1 and 5.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            } while (true); // Loop indefinitely until a valid choice is provided

            switch (choice)
            {
                case 1:
                    newUser(users,ziroRacuni,tekuciRacuni, prepaidRacuni);
                    break;
                case 2:
                    deleteUsers(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case 3:
                    modifyUser(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case 4:
                    listUsers(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case 5:
                    start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case 0:
                    Console.Clear();
                    Console.WriteLine("THANKS FOR VISITING");
                    break;
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void deleteUsers(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni)
                   
        {
            Console.Clear();
            string startMessaage = """
                a) izbrisi po id-u
                b) izbrisi po imenu i prezimenu
                """;

            Console.WriteLine(startMessaage);

            char input = 'd';
            bool isValidInput = false;
            Console.Write("Enter 'a' or 'b': ");
            do
            {
                input = Console.ReadKey().KeyChar;
                Console.WriteLine();

                isValidInput = input == 'a' || input == 'b' || input == 'A' || input == 'B';

                if (!isValidInput)
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            } while (!isValidInput);
            switch (input.ToString().ToLower())
            {
                case "a":
                    int inputNumber = 0;
                    Console.Write("Enter the ID of a user you want to remove: ");
                    do
                    {
                        Console.Write("Enter an integer greater than 0: ");
                        if (!int.TryParse(Console.ReadLine(), out inputNumber) || inputNumber <= 0)
                        {
                            Console.WriteLine("Invalid input. Please enter a positive integer.");
                        }
                        else if (!users.ContainsKey(inputNumber))
                        {
                            Console.WriteLine("The entered integer is not a valid key in the dictionary.");
                        }
                    } while (!users.ContainsKey(inputNumber) || inputNumber <= 0);

                    users.Remove(inputNumber);
                    Console.WriteLine("User deleted succesfully, press any key to return to the start page.");
                    Console.ReadKey();
                    start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case "b":
                    string name, surname;

                    do
                    {
                        Console.Write("Enter your name: ");
                        name = Console.ReadLine();

                        Console.Write("Enter your surname: ");
                        surname = Console.ReadLine();

                        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname))
                        {
                            Console.WriteLine("Both name and surname are required. Please try again.");
                        }
                    } while (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(surname));
                    int targetID = 0;
                    foreach (var item in users.Keys)
                    {
                        if ((users[item].name.ToLower() == name.ToLower()) && (users[item].surname.ToLower() == surname.ToLower()))
                        {
                            targetID = item;
                            break;
                        }
                    }

                    if (targetID != 0)
                    {
                        users.Remove(targetID);
                        Console.WriteLine("Successfully deleted user.");
                        Console.Write("Press any key to move back to start.");
                        Console.ReadKey();
                        start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    }
                    else
                    {
                        Console.WriteLine("User with given name and surname does not exist.");
                        Console.Write("Press any key to move back to start.");
                        Console.ReadKey();
                        start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    }
                    break;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static bool GetYesOrNo()
        {
            string userInput;

            do
            {
                Console.Write("Please enter 'y' for yes or 'n' for no: ");
                userInput = Console.ReadLine().ToLower().Trim();

                if (userInput != "y" && userInput != "n")
                {
                    Console.WriteLine("Invalid input. Please try again.");
                }
            }
            while (userInput != "y" && userInput != "n");

            return userInput == "y";
        }


        static void modifyUser(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni
                   )
        {
            Console.Clear();
            int inputNumber = 0;
            Console.Write("Enter the ID of a user you want to modify: ");
            do
            {
                Console.Write("Enter an integer greater than 0: ");
                if (!int.TryParse(Console.ReadLine(), out inputNumber) || inputNumber <= 0)
                {
                    Console.WriteLine("Invalid input. Please enter a positive integer.");
                }
                else if (!users.ContainsKey(inputNumber))
                {
                    Console.WriteLine("The entered integer is not a valid key in the dictionary.");
                }
            } while (!users.ContainsKey(inputNumber) || inputNumber <= 0);

            Console.WriteLine("Enter valid new details: ");
            Console.WriteLine("Enter the name: ");
            string newName = Console.ReadLine();

            Console.WriteLine("Enter the surname: ");
            string newSurname = Console.ReadLine();

            DateTime newDateOfBirth;
            do
            {
                Console.WriteLine("Enter your date of birth (dd/MM/yyyy):");
                string inputDate = Console.ReadLine();

                if (DateTime.TryParseExact(inputDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out newDateOfBirth))
                {
                    if (newDateOfBirth <= DateTime.Today)
                    {
                        Console.WriteLine("Valid date of birth: " + newDateOfBirth.ToShortDateString());
                        break; // Exit the loop if the date is valid
                    }
                    else
                    {
                        Console.WriteLine("Invalid date of birth. Please enter a date in the past.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please use dd/MM/yyyy format.");
                }
            } while (true); // Loop until a valid date is entered
            users[inputNumber] = (newName, newSurname, newDateOfBirth);
            Console.WriteLine($"User with the id {inputNumber} successfully modified!");
            Console.Write("Press any key to move back to the start");
            Console.ReadKey();
            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);

        }
        static void accountManipulation(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)> prepaidRacuni,
                    char signalLetter, int userID)
        {
            int numberOfTransactions = 0;
            foreach (var item in users.Keys)
            {
                numberOfTransactions += ziroRacuni[item].transactions.Count();
                numberOfTransactions += prepaidRacuni[item].transactions.Count();
                numberOfTransactions += tekuciRacuni[item].transactions.Count();
            }
            Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)> accounts = ziroRacuni;
            switch (signalLetter)
            {
                case 'z':
                    break;
                case 'p':
                    accounts = prepaidRacuni;
                    break;
                case 't':
                    accounts = tekuciRacuni;
                    break;
            }
            string startMessaage = """
                 1 - Unos nove transakcije
                 2 - Brisanje transakcije
                 3 - Uređivanje transakcije
                 4 - Pregled transakcija
                 5 - FInancijsko izvješće
                """;
            Console.Clear();
            Console.WriteLine(startMessaage);
            int input = 0;

            do
            {
                Console.Write("Enter an integer (1, 2, 3, 4, or 5): ");

                if (!int.TryParse(Console.ReadLine(), out input))
                {
                    Console.WriteLine("Invalid input. Please enter an integer.");
                    continue;
                }

                if (input < 1 || input > 5)
                {
                    Console.WriteLine("Invalid input. Please enter a number between 1 and 5.");
                }
            } while (input < 1 || input > 5);
            switch (input)
            {
                case 1:
                    string inputMessage = """
                 a) trenutno izvršena transakcija
                 b) ranije izvršena transakcija
                """;
                    Console.Clear();
                    Console.WriteLine(inputMessage);
                    string inputTaskA = "";

                    do
                    {
                        Console.Write("Enter 'a' or 'b': ");
                        inputTaskA = Console.ReadLine().ToLower().Trim();
                    } while (inputTaskA != "a" && inputTaskA!= "b");

                    switch (inputTaskA)
                    {
                        case "a":
                            double amount;
                            string type = "";
                            string category = "";


                            bool isValidInputD = false;

                            do
                            {
                                Console.Write("Enter amount: ");
                                if (!double.TryParse(Console.ReadLine(), out amount))
                                {
                                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                                    continue;
                                }

                                Console.Write("Enter type(Must be prihod or rashod): ");
                        
                                type = Console.ReadLine();
                                if (type.ToLower().Trim() == "prihod")
                                {
                                    Console.Write("""
                                     a) salary
                                     b) friend-transfer
                                     c) item-sold
                                    """);
                                    string letter = "d";
                                    Console.Write("Enter 'a', 'b', or 'c': ");
                                    do
                                    {
                                        
                                        letter = Console.ReadLine().ToLower().Trim();
                                    } while (letter != "a" && letter != "b" && letter != "c");
                                    switch (letter)
                                    {
                                        case "a":
                                            category = "Salary";
                                            break;
                                        case "b":
                                            category = "Friend transfer";
                                            break;
                                        case "c":
                                            category = "Item sold";
                                            break;

                                    }
                                }
                                else if (type.ToLower().Trim() == "rashod")
                                {
                                    Console.Write("""
                                     a) Food
                                     b) Transport
                                     c) Sport
                                    """);
                                    string letter = "d";

                                    do
                                    {
                                        Console.Write("Enter 'a', 'b', or 'c': ");
                                        letter = Console.ReadLine().ToLower().Trim();
                                    } while (letter != "a" && letter != "b" && letter != "c");
                                    switch (letter)
                                    {
                                        case "a":
                                            category = "Food";
                                            break;
                                        case "b":
                                            category = "Transport";
                                            break;
                                        case "c":
                                            category = "Sport";
                                            break;
                                    }


                                }
                                else 
                                {
                                    Console.WriteLine("You didn't select the right type. (prihod or rashod). Try again");
                                }

                                
                                isValidInputD = true;
                            } while (!isValidInputD);
                            
                            
                                accounts[userID].transactions.Add(numberOfTransactions + 1, (amount, "Standard", type, category, DateTime.Now));
                                accounts[userID] = (accounts[userID].balance + amount, accounts[userID].transactions);
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                            
                            
                            Console.WriteLine("Transaction successfully inputted. Press any key to move back to the start: ");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                            case "b":
                            double amountB;

                            string typeB = "";
                            string categoryB = "";
                            DateTime date = DateTime.Now;
                            bool isValidInputB = false;

                            do
                            {
                                Console.Write("Enter amount: ");
                                if (!double.TryParse(Console.ReadLine(), out amountB))
                                {
                                    Console.WriteLine("Invalid amount. Please enter a valid number.");
                                    continue;
                                }

                                Console.Write("Enter type (must be prihod or rashod): ");
                                typeB = Console.ReadLine();


                                if (typeB.ToLower().Trim() == "prihod")
                                {
                                    Console.Write("""
                                     a) salary
                                     b) friend-transfer
                                     c) item-sold
                                    """);
                                    string letter = "d";

                                    do
                                    {
                                        Console.WriteLine("Enter 'a', 'b', or 'c': ");
                                        letter = Console.ReadLine().ToLower().Trim();
                                    } while (letter != "a" && letter != "b" && letter != "c");
                                    switch (letter)
                                    {
                                        case "a":
                                            categoryB = "Salary";
                                            break;
                                        case "b":
                                            categoryB = "Friend transfer";
                                            break;
                                        case "c":
                                            categoryB = "Item sold";
                                            break;

                                    }
                                }
                                else if (typeB.ToLower().Trim() == "rashod")
                                {
                                    Console.Write("""
                                     a) Food
                                     b) Transport
                                     c) Sport
                                    """);
                                    string letter = "d";

                                    do
                                    {
                                        Console.WriteLine("Enter 'a', 'b', or 'c': ");
                                        letter = Console.ReadLine().ToLower().Trim();
                                    } while (letter != "a" && letter != "b" && letter != "c");
                                    switch (letter)
                                    {
                                        case "a":
                                            categoryB = "Food";
                                            break;
                                        case "b":
                                            categoryB = "Transport";
                                            break;
                                        case "c":
                                            categoryB = "Sport";
                                            break;
                                    }


                                }
                                else
                                {
                                    Console.WriteLine("You didn't select the right type. (prihod or rashod). Try again");
                                }

                                Console.Write("Enter date (DD/MM/YYYY): "); // Changed format to DD/MM/YYYY
                                string dateString = Console.ReadLine();

                                if (!DateTime.TryParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))

                                {
                                    Console.WriteLine("Invalid date format. Please enter the date in this format: DD/MM/YYYY  ");
                            
                             
                                    continue;
                                }


                                isValidInputB = true;
                            } while (!isValidInputB);

                           
                             accounts[userID].transactions.Add(numberOfTransactions + 1, (amountB, "Standard", typeB, categoryB, date));
                             accounts[userID] = (accounts[userID].balance + amountB, accounts[userID].transactions);
                            
                            Console.WriteLine("Transaction successfully inputted. Press any key to move back to the start: ");
                            switch (signalLetter)
                            {
                                case 'z':
                                    ziroRacuni = accounts;
                                    break;
                                case 'p':
                                    prepaidRacuni = accounts;
                                    break;
                                case 't':
                                    tekuciRacuni = accounts;
                                    break;
                            }
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                    }

                    break;
                case 2:
                    string deleteMessage = """
                 a) po id-u
                 b) ispod unesenog iznosa
                 c) iznad unesenog iznosa
                 d) svih prihoda
                 e) svih rashoda
                 f) svih transakcija za odabranu kategoriju
                """;
                    Console.Clear();
                    Console.WriteLine(deleteMessage);
                    string inputDelete = "z";
                    string[] validInputs = { "a", "b", "c", "d", "e", "f" };

                    while (true)
                    {
                        Console.Write("Please enter a letter (a, b, c, d, e, or f): ");
                        inputDelete = Console.ReadLine().ToLower().Trim();

                        if (Array.Exists(validInputs, element => element == inputDelete))
                        {
                            Console.WriteLine($"You entered a valid option: {inputDelete}");
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Invalid input. Please try again.");
                        }
                    }
                    switch (inputDelete)
                    {
                        case "a":
                            int deleteID;
                            Console.Write("Please enter a positive integer that is a valid key in the dictionary: ");
                            do
                            {
                                
                                bool isValidInteger = int.TryParse(Console.ReadLine(), out deleteID);

                                if (!isValidInteger || deleteID <= 0 || !users.ContainsKey(deleteID))
                                {
                                    Console.WriteLine("Invalid input. Please try again.");
                                }
                                else
                                {
                                    Console.WriteLine($"You entered a valid key: {deleteID}");
                                }
                            }
                            while (deleteID <= 0 || !users.ContainsKey(deleteID));
                            Console.WriteLine("Are you sure you want to delete this transaction y or n?");
                            bool decisionDelete = GetYesOrNo();
                            if (decisionDelete)
                            {
                                Console.WriteLine("Successfully deleted transaction. Press any key to go back to the start");
                                accounts[userID].transactions.Remove(deleteID);
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            else
                            {
                                Console.WriteLine("Aborted user deletion. Press any key to move back to the start");
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);

                            }

                            break;


                        case "b":
                            double deletionNumber = 0;
                            bool isValidInputB = false;

                            do
                            {
                                Console.Write("Enter a double number: ");
                                string deleteInput = Console.ReadLine();

                                isValidInputB = double.TryParse(deleteInput, out deletionNumber);

                                if (!isValidInputB)
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid double number.");
                                }
                            } while (!isValidInputB);
                            List<int> deleteIDS = new List<int>();
                            foreach (var item in accounts.Keys)
                            {
                                foreach (var item1 in accounts[item].transactions.Keys)
                                {
                                    if (accounts[item].transactions[item1].amount < deletionNumber)
                                    {
                                        deleteIDS.Add(item1);
                                    }
                                }
                            }
                            bool decisionDeleteB = GetYesOrNo();
                            if (decisionDeleteB)
                            {
                                for (global::System.Int32 i = 0; i < deleteIDS.Count; i++)
                                {
                                    accounts[userID].transactions.Remove(deleteIDS[i]);
                                }
                                Console.WriteLine("Transactions successfully deleted. Press any key to move back to start");
                                Console.ReadKey();
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            else
                            {
                                Console.WriteLine("Aborted deletion. Press any key to move back to start.");
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }

                            break;


                        case "c":
                            double deletionNumberC = 0;
                            bool isValidInputC = false;

                            do
                            {
                                Console.Write("Enter a double number: ");
                                string deleteInput = Console.ReadLine();

                                isValidInputC = double.TryParse(deleteInput, out deletionNumberC);

                                if (!isValidInputC)
                                {
                                    Console.WriteLine("Invalid input. Please enter a valid double number.");
                                }
                            } while (!isValidInputC);
                            List<int> deleteIDSC = new List<int>();
                            foreach (var item in accounts.Keys)
                            {
                                foreach (var item1 in accounts[userID].transactions.Keys)
                                {
                                    if (accounts[userID].transactions[item1].amount > deletionNumberC)
                                    {
                                        deleteIDSC.Add(item1);
                                    }
                                }
                            }
                            bool decisionDeleteC = GetYesOrNo();
                            if (decisionDeleteC)
                            {
                                for (global::System.Int32 i = 0; i < deleteIDSC.Count; i++)
                                {
                                    accounts[userID].transactions.Remove(deleteIDSC[i]);
                                }
                                Console.WriteLine("Transactions successfully deleted. Press any key to move back to start");
                                Console.ReadKey();
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            else
                            {
                                Console.WriteLine("Aborted deletion. Press any key to move back to start.");
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            break;


                        case "d":
                            List<int> prihodIDS = new List<int>();
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].type.ToLower() == "prihod")
                                {
                                    prihodIDS.Add(item);
                                }
                            }
                            bool decisionPrihod = GetYesOrNo();
                            if (decisionPrihod)
                            {
                                for (global::System.Int32 i = 0; i < prihodIDS.Count; i++)
                                {
                                    accounts[userID].transactions.Remove(prihodIDS[i]);
                                }
                                Console.WriteLine("Transactions successfully deleted. Press any key to move back to start");
                                Console.ReadKey();
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            else 
                            {
                                Console.WriteLine("Aborted deletion. Press any key to move back to start.");
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            break;



                        case "e":
                            List<int> rashodIDS = new List<int>();
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].type.ToLower() == "rashod")
                                {
                                    rashodIDS.Add(item);
                                }
                            }
                            bool decisionRashod = GetYesOrNo();
                            if (decisionRashod)
                            {
                                for (global::System.Int32 i = 0; i < rashodIDS.Count; i++)
                                {
                                    accounts[userID].transactions.Remove(rashodIDS[i]); 
                                }
                                Console.WriteLine("Transactions successfully deleted. Press any key to move back to start");
                                Console.ReadKey();
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            else
                            {
                                Console.WriteLine("Aborted deletion. Press any key to move back to start.");
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                        
                            break;


                        case "f":
                            string category = "";
                            bool validCategory = false;
                            Console.Write("Enter a category(food, transport, sport, salary, friend transfer, item sold): ");

                            do
                            {
                                category = Console.ReadLine();
                                if (!string.IsNullOrEmpty(category))
                                {
                                                                   
                                    foreach (var item in accounts[userID].transactions.Keys)
                                        {
                                            if (accounts[userID].transactions[item].category.ToLower().Trim() == category.ToLower().Trim())
                                            {
                                                Console.WriteLine("Inputted category is valid.");
                                                validCategory = true;
                                                break;
                                            }
                                        }
                                }
                            } while (validCategory == false);
                            List<int> categoryIDS = new List<int>();
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].category.ToLower() == category.ToLower().Trim())
                                {
                                    categoryIDS.Add(item);
                                }
                            }
                            bool deleteCategory = GetYesOrNo();
                            if (deleteCategory)
                            {
                                for (global::System.Int32 i = 0; i < categoryIDS.Count; i++)
                                {
                                    accounts[userID].transactions.Remove(categoryIDS[i]);
                                }
                                Console.WriteLine("Transactions successfully deleted. Press any key to move back to start");
                                Console.ReadKey();
                                switch (signalLetter)
                                {
                                    case 'z':
                                        ziroRacuni = accounts;
                                        break;
                                    case 'p':
                                        prepaidRacuni = accounts;
                                        break;
                                    case 't':
                                        tekuciRacuni = accounts;
                                        break;
                                }
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }
                            else
                            {
                                Console.WriteLine("Aborted deletion. Press any key to move back to start.");
                                Console.ReadKey();
                                start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            }

                            break;
                    }
                    break;
                case 3:
                    int transactionID;
                    bool isValidInput = false;

                    do
                    {
                        Console.Write("Enter transaction ID: ");
                        if (!int.TryParse(Console.ReadLine(), out transactionID) || transactionID <= 0 || !accounts[userID].transactions.ContainsKey(transactionID))
                        {
                            Console.WriteLine("Invalid transaction ID. Please enter a valid positive integer.");
                        }
                        else
                        {
                            isValidInput = true;
                        }
                    } while (!isValidInput);
                    double newAmount;
                    string newType = "";
                    bool isValidInputModify = false;

                    do
                    {
                        Console.Write("Enter the new amount: ");
                        if (!double.TryParse(Console.ReadLine(), out newAmount))
                        {
                            Console.WriteLine("Invalid amount. Please enter a valid number.");
                            continue;
                        }

                        Console.Write("Enter the new type (prihod or rashod): ");
                        newType = Console.ReadLine().ToLower();

                        if (newType != "prihod" && newType != "rashod")
                        {
                            Console.WriteLine("Invalid type. Please enter 'prihod' or 'rashod'.");
                            continue;
                        }

                        isValidInputModify = true;
                    } while (!isValidInputModify);
                    string categoryModify = "";
                    bool validCategoryModify = false;
                    Console.Write("Enter a category(food, transport, sport, salary, friend transfer, item sold): ");

                    do
                    {
                        categoryModify = Console.ReadLine();
                        if (!string.IsNullOrEmpty(categoryModify))
                        {

                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].category.ToLower().Trim() == categoryModify.ToLower().Trim())
                                {
                                    Console.WriteLine("Inputted category is valid.");
                                    validCategoryModify = true;
                                    break;
                                }
                            }
                        }
                    } while (validCategoryModify == false);

                    bool modifyTransaction = GetYesOrNo();
                    if (modifyTransaction)
                    {
                        accounts[userID].transactions[transactionID] = (newAmount, "Standard", newType, categoryModify, DateTime.Now);
                    }
                    break;
                case 4:
                    string summaryMessage = """
                 a) sve transakcije kako su spremljene
                 b) sve transakcije sortirane po iznosu uzlazno
                 c) sve transakcije sortitane po iznosu silazno
                 d) sve transakcije sortirane po opisu abecedno
                 e) sve transackije opisane po datumu uzlazno
                 f) sve transakcije opisane po datumu silazno
                 g) svi prihodi
                 h) svi rashodi
                 i) sve transakcije za odabranu kategoriju
                 j) sve transakcije za tip i kategoriju

                """;

                    Console.Clear();
                    Console.WriteLine(summaryMessage);
                    string summaryInput = GetValidInput(["a", "b", "c", "d", "e", "f", "g", "h", "i", "j"]);
                    switch (summaryInput)
                    {
                        case "a":
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                Console.WriteLine($"{accounts[userID].transactions[item].type}  {accounts[userID].transactions[item].amount}  {accounts[userID].transactions[item].description}  {accounts[userID].transactions[item].category} {accounts[userID].transactions[item].dateTime}");
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "b":
                            var amountUp = accounts[userID].transactions.OrderBy(x => x.Value.amount);
                            foreach (var item in amountUp)
                            {
                                Console.WriteLine($"{item.Value.type} - {item.Value.amount} - {item.Value.description} - {item.Value.category} - {item.Value.dateTime}");
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "c":
                            var amountDown = accounts[userID].transactions.OrderByDescending(x => x.Value.amount);
                            foreach (var item in amountDown)
                            {
                                Console.WriteLine($"{item.Value.type} - {item.Value.amount} - {item.Value.description} - {item.Value.category} - {item.Value.dateTime}");
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "d":
                            var alphabetical = accounts[userID].transactions.OrderBy(x => x.Value.category);
                            foreach (var item in alphabetical)
                            {
                                Console.WriteLine($"{item.Value.type} - {item.Value.amount} - {item.Value.description} - {item.Value.category} - {item.Value.dateTime}");
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "e":
                            var dateUp = accounts[userID].transactions.OrderBy(x => x.Value.dateTime);
                            foreach (var item in dateUp)
                            {
                                Console.WriteLine($"{item.Value.type} - {item.Value.amount} - {item.Value.description} - {item.Value.category} - {item.Value.dateTime}");
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "f":
                            var dateDown = accounts[userID].transactions.OrderByDescending(x => x.Value.dateTime);
                            foreach (var item in dateDown)
                            {
                                Console.WriteLine($"{item.Value.type} - {item.Value.amount} - {item.Value.description} - {item.Value.category} - {item.Value.dateTime}");
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "g":
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].type.ToLower() == "prihod") 
                                {
                                    Console.WriteLine($"{accounts[userID].transactions[item].type}  {accounts[userID].transactions[item].amount}  {accounts[userID].transactions[item].description}  {accounts[userID].transactions[item].category} {accounts[userID].transactions[item].dateTime}");
                                }
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "h":
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].type.ToLower() == "rashod")
                                {
                                    Console.WriteLine($"{accounts[userID].transactions[item].type}  {accounts[userID].transactions[item].amount}  {accounts[userID].transactions[item].description}  {accounts[userID].transactions[item].category} {accounts[userID].transactions[item].dateTime}");
                                }
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "i":
                            string category = "";
                            bool validCategory = false;
                            Console.Write("Enter a category(food, transport, sport, salary, friend transfer, item sold): ");

                            do
                            {
                                category = Console.ReadLine();
                                if (!string.IsNullOrEmpty(category))
                                {

                                    foreach (var item in accounts[userID].transactions.Keys)
                                    {
                                        if (accounts[userID].transactions[item].category.ToLower().Trim() == category.ToLower().Trim())
                                        {
                                            Console.WriteLine("Inputted category is valid.");
                                            validCategory = true;
                                            break;
                                        }
                                    }
                                }
                            } while (validCategory == false);
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].category.ToLower().Trim() == category.ToLower().Trim()) 
                                {
                                    Console.WriteLine($"{accounts[userID].transactions[item].type}  {accounts[userID].transactions[item].amount}  {accounts[userID].transactions[item].description}  {accounts[userID].transactions[item].category} {accounts[userID].transactions[item].dateTime}");
                                }
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "j":
                            string category2 = "";
                            bool validCategory2 = false;
                            Console.Write("Enter a category(food, transport, sport, salary, friend transfer, item sold): ");

                            do
                            {
                                category2 = Console.ReadLine();
                                if (!string.IsNullOrEmpty(category2))
                                {

                                    foreach (var item in accounts[userID].transactions.Keys)
                                    {
                                        if (accounts[userID].transactions[item].category.ToLower().Trim() == category2.ToLower().Trim())
                                        {
                                            Console.WriteLine("Inputted category is valid.");
                                            validCategory2 = true;
                                            break;
                                        }
                                    }
                                }
                            } while (validCategory2 == false);
                            string userInput = "";
                            Console.Write("Enter 'prihod' or 'rashod': ");
                            do
                            {
                                
                                userInput = Console.ReadLine().ToLower().Trim();
                            } while (userInput != "prihod" && userInput != "rashod");

                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if ((accounts[userID].transactions[item].category.ToLower().Trim() == category2.ToLower().Trim()) && (accounts[userID].transactions[item].type.ToLower().Trim() == userInput))
                                {
                                    Console.WriteLine($"{accounts[userID].transactions[item].type}  {accounts[userID].transactions[item].amount}  {accounts[userID].transactions[item].description}  {accounts[userID].transactions[item].category} {accounts[userID].transactions[item].dateTime}");
                                }
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);

                            break;
                            
                    }
                    break;
                case 5:
                    string financialReportMessage = """
                 a) trenutno stanje računa
                 b) broj ukupnih transakcija
                 c) ukupan iznos prihoda i rashoda za odabrani mjesec i godinu
                 d) postotak udjela rashoda za odabranu kategoriju
                 e) prosjecni iznos transakcije za odabrani mjesec i godinu
                 f) prosjecni ishod transakcije za odabranu kategoriju

                """;
                    Console.Clear();
                    Console.WriteLine(financialReportMessage);
                    string validInput = GetValidInput(["a", "b", "c", "d", "e", "f"]);
                    switch (validInput)
                    {
                        case "a":
                            Console.Clear();
                            Console.WriteLine("Trenutno stanje korisničkih računa: ");
                            Console.WriteLine($"Žiro račun: {ziroRacuni[userID].balance}, Tekući račun: {tekuciRacuni[userID].balance}, Prepaid račun: {prepaidRacuni[userID].balance}");
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "b":
                            Console.WriteLine("Broj ukupnih transakcija: ");
                            Console.WriteLine((ziroRacuni[userID].transactions.Count()) + (tekuciRacuni[userID].transactions.Count()) + (prepaidRacuni[userID].transactions.Count()));
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "c":
                            int month, year;

                            do
                            {
                                Console.Write("Enter the month (1-12): ");
                                if (!int.TryParse(Console.ReadLine(), out month) || month < 1 || month > 12)
                                {
                                    Console.WriteLine("Invalid month. Please enter a number between 1 and 12.");
                                    continue;
                                }

                                Console.Write("Enter the year: ");
                                if (!int.TryParse(Console.ReadLine(), out year))
                                {
                                    Console.WriteLine("Invalid year. Please enter a valid year.");
                                    continue;
                                }

                                // If we reach this point, both month and year are valid
                                break;
                            } while (true);
                            double sumRashod = 0;
                            double sumPrihod = 0;
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].dateTime.Month == month && accounts[userID].transactions[item].dateTime.Year == year)
                                {
                                    if (accounts[userID].transactions[item].type.ToLower() == "prihod")
                                    {
                                        sumPrihod += accounts[userID].transactions[item].amount;
                                    }
                                    else
                                    {
                                        sumRashod += accounts[userID].transactions[item].amount;
                                    }
                                }
                            }
                            Console.WriteLine("Suma rashoda je: "+ sumRashod);
                            Console.WriteLine("Suma prihoda je: " + sumPrihod);
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;


                        case "d":
                            string category = "";
                            bool validCategory = false;
                            Console.Write("Enter a category(food, transport, sport, salary, friend transfer, item sold): ");

                            do
                            {
                                category = Console.ReadLine();
                                if (!string.IsNullOrEmpty(category))
                                {

                                    foreach (var item in accounts[userID].transactions.Keys)
                                    {
                                        if (accounts[userID].transactions[item].category.ToLower().Trim() == category.ToLower().Trim())
                                        {
                                            Console.WriteLine("Inputted category is valid.");
                                            validCategory = true;
                                            break;
                                        }
                                    }
                                }
                            } while (validCategory == false);
                            int rashodCount = 0;
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].category.ToLower().Trim() == category.ToLower().Trim())
                                {
                                    
                                
                                    if (accounts[userID].transactions[item].type.ToLower() == "rashod")
                                    {
                                        rashodCount += 1;
                                    }
                                }

                            }
                            Console.WriteLine("Percentage of rashod for given category is: " + (rashodCount / accounts[userID].transactions.Count()));
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "e":
                            int monthE, yearE;

                            do
                            {
                                Console.Write("Enter the month (1-12): ");
                                if (!int.TryParse(Console.ReadLine(), out monthE) || monthE < 1 || monthE > 12)
                                {
                                    Console.WriteLine("Invalid month. Please enter a number between 1 and 12.");
                                    continue;
                                }

                                Console.Write("Enter the year: ");
                                if (!int.TryParse(Console.ReadLine(), out yearE))
                                {
                                    Console.WriteLine("Invalid year. Please enter a valid year.");
                                    continue;
                                }

                                // If we reach this point, both month and year are valid
                                break;
                            } while (true);
                            double sumAll = 0;
                            double sumMonth = 0;
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].dateTime.Month == monthE && accounts[userID].transactions[item].dateTime.Year == yearE)
                                {
                                    sumMonth += accounts[userID].transactions[item].amount;
                                }
                                sumAll += accounts[userID].transactions[item].amount;
                            }
                            Console.WriteLine("Average amount of the transaction for given month and year is: "+(sumMonth/sumAll));
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                        case "f":
                            string category2 = "";
                            bool validCategory2 = false;
                            Console.Write("Enter a category(food, transport, sport, salary, friend transfer, item sold): ");

                            do
                            {
                                category2 = Console.ReadLine();
                                if (!string.IsNullOrEmpty(category2))
                                {

                                    foreach (var item in accounts[userID].transactions.Keys)
                                    {
                                        if (accounts[userID].transactions[item].category.ToLower().Trim() == category2.ToLower().Trim())
                                        {
                                            Console.WriteLine("Inputted category is valid.");
                                            validCategory2 = true;
                                            break;
                                        }
                                    }
                                }
                            } while (validCategory2 == false);
                            string userInput = "";
                            Console.Write("Enter 'prihod' or 'rashod': ");
                            do
                            {

                                userInput = Console.ReadLine().ToLower().Trim();
                            } while (userInput != "prihod" && userInput != "rashod");
                            double sumCategory = 0;
                            double sumOverall = 0;
                            foreach (var item in accounts[userID].transactions.Keys)
                            {
                                if (accounts[userID].transactions[item].category.ToLower().Trim() == userInput)
                                {
                                    sumCategory += accounts[userID].transactions[item].amount;
                                }
                                sumOverall += accounts[userID].transactions[item].amount;
                            }
                            Console.WriteLine("Press any key to move back to the start.");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                            break;
                    }
                    break;

            }
        }
        static void accountDropDown(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni)
                   
        {
            string nameTest = "";
            string surnameTest = "";
            bool flag = false;
            int userID = 0;
            Console.Clear();
            Console.WriteLine("Enter your name and surname: ");
            do
            {
                Console.Write("Enter name: ");
                nameTest = Console.ReadLine();

                Console.Write("Enter surname: ");
                surnameTest = Console.ReadLine();

                // Check if name and surname are not null or empty
                if (!string.IsNullOrEmpty(nameTest) && !string.IsNullOrEmpty(surnameTest))
                {
                    // Check if the combination exists in the dictionary
                    foreach (var item in users.Keys)
                    {
                        if ((users[item].name.ToLower() == nameTest.ToLower().Trim()) && (users[item].surname.ToLower() == surnameTest.ToLower().Trim()))
                        {
                            Console.WriteLine("Accessing user's info...");
                            userID = item;
                            flag = true;
                            break;
                        }
                      
                    }
                    
                }
                else
                {
                    Console.WriteLine("Name and surname cannot be empty. Please try again.");
                }


                if (!flag)
                {
                    Console.WriteLine("Such an user does not exist in our current database. Please try again.");
                }
                else 
                {
                    flag = false;
                    break;
                } 
                    
                

            } while (true); // Loop until valid input is provided

            string input = "";

            do
            {
                Console.Write("Enter a letter (z for ziro, p for prepaid, or t for tekuci): ");
                input = Console.ReadLine().ToLower().Trim(); ;

                if (input != "z" && input != "p" && input != "t")
                {
                    Console.WriteLine("Invalid input. Please enter a letter.");
                }
            } while (input != "z" && input != "p" && input != "t");

            switch (input) 
            {
                case "z":
                    Console.WriteLine("Accessing ziro account. Press any key to continue.");
                    accountManipulation(users, ziroRacuni, tekuciRacuni, prepaidRacuni, 'z', userID);
                    break;
                case "p":
                    Console.WriteLine("Accessing prepaid account. Press any key to continue.");
                    accountManipulation(users, ziroRacuni, tekuciRacuni, prepaidRacuni, 'p', userID);
                    break;
                case "t":
                    accountManipulation(users, ziroRacuni, tekuciRacuni, prepaidRacuni, 't', userID);
                    Console.WriteLine("Accessing tekuci account. Press any key to continue.");
                    break;
            }




        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void start(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni)
                   
        {
            string startMessaage = """
                 1 - Korisnici
                 2 - Računi
                 3 - Izlaz iz aplikacije
                """;
            Console.Clear();
            Console.WriteLine(startMessaage);

            int choice = 0;
            do
            {
                Console.WriteLine("Enter a choice between 1 and 3:");
                var input = Console.ReadLine();

                if (int.TryParse(input, out choice))
                {
                    if (choice >= 1 && choice <= 3)
                    {
                        Console.WriteLine("Valid choice: " + choice);
                        break; // Exit the loop if the choice is valid
                    }
                    else
                    {
                        Console.WriteLine("Please enter a choice between 1 and 3.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
            } while (true); // Loop indefinitely until a valid choice is provided

            switch (choice) 
            {
                case 1:
                    userDropDown(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case 2:
                    accountDropDown(users, ziroRacuni, tekuciRacuni, prepaidRacuni);
                    break;
                case 3:
                    Console.Clear();
                    Console.WriteLine("THANKS FOR VISITING");

                    break;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void Main(string[] args)
        {
            var users = new Dictionary<int, (string name, string surname, DateTime dateOfBirth)>();

            users.Add(1, ("Alice", "Johnson", new DateTime(1990, 1, 1)));
            users.Add(2, ("Bob", "Smith", new DateTime(1985, 5, 15)));
            users.Add(3, ("Charlie", "Brown", new DateTime(1972, 10, 22)));
            users.Add(4, ("David", "Lee", new DateTime(1995, 3, 7)));
            users.Add(5, ("Emily", "Davis", new DateTime(2002, 12, 25)));


            var ziroRacuni = new Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)>();
            var tekuciRacuni = new Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)>();
            var prepaidRacuni = new Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)>();

            ziroRacuni.Add(1, (-500000.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>())); // Test negative balance case
            ziroRacuni.Add(2, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            ziroRacuni.Add(3, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            ziroRacuni.Add(4, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            ziroRacuni.Add(5, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));

            tekuciRacuni.Add(1, (100.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            tekuciRacuni.Add(2, (100.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            tekuciRacuni.Add(3, (100.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            tekuciRacuni.Add(4, (100.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            tekuciRacuni.Add(5, (100.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));

            prepaidRacuni.Add(1, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            prepaidRacuni.Add(2, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            prepaidRacuni.Add(3, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            prepaidRacuni.Add(4, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            prepaidRacuni.Add(5, (0.00, new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>()));
            int transactionId = 0;
            foreach (var userId in users.Keys)
            {
                // Adding transactions to ziroRacuni
                if (ziroRacuni.ContainsKey(userId))
                {
                    var account = ziroRacuni[userId];

                    // Prihod (Income) transaction - Salary
                    account.transactions[transactionId++] = (1000.00, "Standard", "Prihod", "Salary", DateTime.Now);
                    account.balance += 1000.00;

                    // Rashod (Expense) transaction - Food
                    account.transactions[transactionId++] = (-200.00, "Standard", "Rashod", "Food", DateTime.Now);
                    account.balance -= 200.00;

                    // Rashod (Expense) transaction - Transport
                    account.transactions[transactionId++] = (-50.00, "Standard", "Rashod", "Transport", DateTime.Now);
                    account.balance -= 50.00;

                    ziroRacuni[userId] = account; // Update ziroRacuni with new balance and transactions
                }

                // Adding transactions to tekuciRacuni
                if (tekuciRacuni.ContainsKey(userId))
                {
                    var account = tekuciRacuni[userId];

                    // Prihod (Income) transaction - Salary
                    account.transactions[transactionId++] = (1500.00, "Standard", "Prihod", "Friend transfer", DateTime.Now);
                    account.balance += 1500.00;

                    // Rashod (Expense) transaction - Food
                    account.transactions[transactionId++] = (-300.00, "Standard", "Rashod", "Food", DateTime.Now);
                    account.balance -= 300.00;

                    // Rashod (Expense) transaction - Utilities
                    account.transactions[transactionId++] = (-100.00, "Standard", "Rashod", "Sport", DateTime.Now);
                    account.balance -= 100.00;

                    tekuciRacuni[userId] = account; // Update tekuciRacuni with new balance and transactions
                }

                // Adding transactions to prepaidRacuni
                if (prepaidRacuni.ContainsKey(userId))
                {
                    var account = prepaidRacuni[userId];

                    // Prihod (Income) transaction - Salary
                    account.transactions[transactionId++] = (500.00, "Standard", "Prihod", "Salary", DateTime.Now);
                    account.balance += 500.00;

                    // Rashod (Expense) transaction - Food
                    account.transactions[transactionId++] = (-100.00, "Standard", "Rashod", "Food", DateTime.Now);
                    account.balance -= 100.00;

                    // Rashod (Expense) transaction - Entertainment
                    account.transactions[transactionId++] = (-50.00, "Standard", "Rashod", "Food", DateTime.Now);
                    account.balance -= 50.00;

                    prepaidRacuni[userId] = account; // Update prepaidRacuni with new balance and transactions
                }
            }



            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni);

        }
    }
}