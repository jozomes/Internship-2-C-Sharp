using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DrugoPredavanje
{
    class Program
    {
        static void newUser(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users) 
        {
            Console.Clear();
            int numberOfUsers = users.Count;

            Console.WriteLine("Unesi ime novog korisnika: ");
            string newName = Console.ReadLine();

            Console.WriteLine("Unesi prezime novog korisnika: ");
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
            Console.WriteLine("Korisnik uspješno unešen!");
            userDropDown(users);
        }
        static void listUsers() { }
        static void userDropDown(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users) 
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
                """;
            Console.Clear();
            Console.WriteLine(startMessaage);

            int choice = 0;
            do
            {
                Console.WriteLine("Enter a choice between 1 and 4:");
                var input = Console.ReadLine();

                if (int.TryParse(input, out choice))
                {
                    if (choice >= 1 && choice <= 4)
                    {
                        Console.WriteLine("Valid choice: " + choice);
                        break; // Exit the loop if the choice is valid
                    }
                    else
                    {
                        Console.WriteLine("Please enter a choice between 1 and 4.");
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
                    newUser(users);
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }
        static void start(Dictionary<int, (string name, string surname, DateTime dateOfBirth)> users) 
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
                    userDropDown(users);
                    break;
            }
        }
        static void Main(string[] args)
        {
            var users = new Dictionary<int, (string name, string surname, DateTime dateOfBirth)>();

            users.Add(1, ("Alice", "Johnson", new DateTime(1990, 1, 1)));
            users.Add(2, ("Bob", "Smith", new DateTime(1985, 5, 15)));
            users.Add(3, ("Charlie", "Brown", new DateTime(1972, 10, 22)));
            users.Add(4, ("David", "Lee", new DateTime(1995, 3, 7)));
            users.Add(5, ("Emily", "Davis", new DateTime(2002, 12, 25)));

            start(users);

            Console.ReadLine();
        }
    }
}