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
        static void newUser(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
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
            tekuciRacuni.Add(numberOfUsers + 1, (100.00, tekuciTransakcije));
            ziroRacuni.Add(numberOfUsers + 1, (0.00, ziroTransakcije));
            prepaidRacuni.Add(numberOfUsers + 1, (0.00, prepaidTransakcije));
            Console.WriteLine("Press any key to go back to the start");
            Console.ReadKey();

            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void listUsers(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
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
            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        static void userDropDown(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
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
                    newUser(users,ziroRacuni,tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    break;
                case 2:
                    deleteUsers(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    break;
                case 3:
                    modifyUser(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    break;
                case 4:
                    listUsers(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    break;
                case 5:
                    start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
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
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
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
                    start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
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
                        start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    }
                    else
                    {
                        Console.WriteLine("User with given name and surname does not exist.");
                        Console.Write("Press any key to move back to start.");
                        Console.ReadKey();
                        start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    }
                    break;
            }
        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void modifyUser(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
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
            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);

        }
        static void accountManipulation(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije, char signalLetter, int userID)
        {
            int numberOfTransactions = 0;
            foreach (var item in users.Keys)
            {
                numberOfTransactions += ziroRacuni[item].ziroTransakcije.Count();
                numberOfTransactions += prepaidRacuni[item].prepaidTransakcije.Count();
                numberOfTransactions += tekuciRacuni[item].tekuciTransakcije.Count();
            }
            Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions = ziroTransakcije;
            Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)> accounts = ziroRacuni;
            switch (signalLetter)
            {
                case 'z':
                    break;
                case 'p':
                    transactions = prepaidTransakcije;
                    accounts = prepaidRacuni;
                    break;
                case 't':
                    transactions = tekuciTransakcije;
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


                            bool isValidInput = false;

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

                                    do
                                    {
                                        Console.Write("Enter 'a', 'b', or 'c': ");
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

                                
                                isValidInput = true;
                            } while (!isValidInput);
                            if (amount < 0)
                            {
                                transactions[numberOfTransactions + 1] = (amount, "Standard", type, category, DateTime.Now);
                                accounts[userID] = (accounts[userID].balance - amount, accounts[userID].transactions);
                            }
                            else
                            {
                                transactions[numberOfTransactions + 1] = (amount, "Standard", type, category, DateTime.Now);
                                accounts[userID] = (accounts[userID].balance + amount, accounts[userID].transactions);
                            }
                            Console.WriteLine("Transaction successfully inputted. Press any key to move back to the start: ");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
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

                                Console.Write("Enter type: ");
                                typeB = Console.ReadLine() + "B";


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

                                Console.Write("Enter date (YYYY-MM-DD): ");
                                string dateString = Console.ReadLine();

                                if (!DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                                {
                                    
                                    continue;
                                }

                                isValidInputB = true;
                            } while (!isValidInputB);
                            if (amountB < 0)
                            {
                                transactions[numberOfTransactions + 1] = (amountB, "Standard", typeB, categoryB, date);
                                accounts[userID] = (accounts[userID].balance - amountB, accounts[userID].transactions);
                            }
                            else
                            {
                                transactions[numberOfTransactions + 1] = (amountB, "Standard", typeB, categoryB, date);
                                accounts[userID] = (accounts[userID].balance + amountB, accounts[userID].transactions);
                            }
                            Console.WriteLine("Transaction successfully inputted. Press any key to move back to the start: ");
                            Console.ReadKey();
                            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
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
                    break;
                case 3:
                    
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
                 i) sve transakcije za tip i kategoriju

                """;
                    Console.Clear();
                    Console.WriteLine(summaryMessage);
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
                    break;

            }
        }
        static void accountDropDown(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
        {
            string nameTest = "";
            string surnameTest = "";
            bool flag = false;
            int userID = 0;
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
                            Console.WriteLine(users[item].name.ToLower() + users[item].surname.ToLower());
                            Console.WriteLine(nameTest.ToLower() + surnameTest.ToLower());
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
                Console.Write("Enter a letter (z, p, or t): ");
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
                    accountManipulation(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije, 'z', userID);
                    break;
                case "p":
                    Console.WriteLine("Accessing prepaid account. Press any key to continue.");
                    accountManipulation(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije, 'p', userID);
                    break;
                case "t":
                    accountManipulation(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije, 't', userID);
                    Console.WriteLine("Accessing tekuci account. Press any key to continue.");
                    break;
            }




        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void start(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije)> ziroRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije)> tekuciRacuni,
                    Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)> prepaidRacuni,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> ziroTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> tekuciTransakcije,
                    Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> prepaidTransakcije)
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
                    userDropDown(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
                    break;
                case 2:
                    accountDropDown(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);
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

            var ziroTransakcije = new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>();
            var tekuciTransakcije = new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>();
            var prepaidTransakcije = new Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)>();

            var ziroRacuni = new Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)>();
            var tekuciRacuni = new Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)>();
            var prepaidRacuni = new Dictionary<int, (double balance, Dictionary<int, (double amount, string description, string type, string category, DateTime dateTime)> transactions)>();

            tekuciRacuni.Add(1, (100.00, tekuciTransakcije));
            tekuciRacuni.Add(2, (100.00, tekuciTransakcije));
            tekuciRacuni.Add(3, (100.00, tekuciTransakcije));
            tekuciRacuni.Add(4, (100.00, tekuciTransakcije));
            tekuciRacuni.Add(5, (100.00, tekuciTransakcije));

            ziroRacuni.Add(1, (-500000.00, ziroTransakcije)); //testiram pocetni ispis
            ziroRacuni.Add(2, (0.00, ziroTransakcije));
            ziroRacuni.Add(3, (0.00, ziroTransakcije));
            ziroRacuni.Add(4, (0.00, ziroTransakcije));
            ziroRacuni.Add(5, (0.00, ziroTransakcije));

            prepaidRacuni.Add(1, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(2, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(3, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(4, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(5, (0.00, prepaidTransakcije));
            int transactionId = 0;


            foreach (var userId in users.Keys)
            {
                // Three transactions for each user in the 'tekuci' account
                tekuciTransakcije[transactionId++] = (50.0, $"User {userId} Standard", "prihod", "Salary", DateTime.Now.AddDays(-1));
                tekuciTransakcije[transactionId++] = (20.0, $"User {userId} Standard", "prihod", "Salary", DateTime.Now.AddDays(-2));
                tekuciTransakcije[transactionId++] = (15.0, $"User {userId} Standard", "prihod", "Friend transfer", DateTime.Now.AddDays(-3));
                tekuciRacuni[userId] = (tekuciRacuni[userId].balance + 85, tekuciRacuni[userId].transactions);
                // Three transactions for each user in the 'ziro' account
                ziroTransakcije[transactionId++] = (150.0, $"User {userId} Standard", "prihod", "Salary", DateTime.Now.AddDays(-1));
                ziroTransakcije[transactionId++] = (40.0, $"User {userId} Standard", "prihod", "Item sold", DateTime.Now.AddDays(-2));
                ziroTransakcije[transactionId++] = (10.0, $"User {userId} Standard", "prihod", "Salary", DateTime.Now.AddDays(-3));
                ziroRacuni[userId] = (ziroRacuni[userId].balance + 200, tekuciRacuni[userId].transactions);
                // Three transactions for each user in the 'prepaid' account
                prepaidTransakcije[transactionId++] = (100.0, $"User {userId} Standard", "prihod", "Salary", DateTime.Now.AddDays(-1));
                prepaidTransakcije[transactionId++] = (25.0, $"User {userId} Standard", "prihod", "Item sold", DateTime.Now.AddDays(-2));
                prepaidTransakcije[transactionId++] = (30.0, $"User {userId} Standard", "prihod", "Salary", DateTime.Now.AddDays(-3));
                prepaidRacuni[userId] = (prepaidRacuni[userId].balance + 155, prepaidRacuni[userId].transactions);
            }
            
            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);

            Console.ReadLine();
        }
    }
}