using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
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

            ziroRacuni.Add(1, (-5.00, ziroTransakcije)); //testiram pocetni ispis
            ziroRacuni.Add(2, (0.00, ziroTransakcije));
            ziroRacuni.Add(3, (0.00, ziroTransakcije));
            ziroRacuni.Add(4, (0.00, ziroTransakcije));
            ziroRacuni.Add(5, (0.00, ziroTransakcije));

            prepaidRacuni.Add(1, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(2, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(3, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(4, (0.00, prepaidTransakcije));
            prepaidRacuni.Add(5, (0.00, prepaidTransakcije));


            start(users, ziroRacuni, tekuciRacuni, prepaidRacuni, ziroTransakcije, tekuciTransakcije, prepaidTransakcije);

            Console.ReadLine();
        }
    }
}