using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Vaccination
{
    public class Person
    {
        public long IdNumber;
        public string LastName;
        public string FirstName;
        public string HealthCarePro;
        public int HighRisk;
        public int Infected;

        public Person(long idNumber, string lastName, string firstName, string healthCarePro, int highRisk, int infected)
        {
            IdNumber = idNumber;
            LastName = lastName;
            FirstName = firstName;
            HealthCarePro = healthCarePro;
            HighRisk = highRisk;
            Infected = infected;

        }
    }
    public class Program
    {
        public static int vaccineDoses = 0;
        
        public static void Main()
        {
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
            bool running = true;
            while (running)
            {
                MainMenu();
            }
        }

        // Create the lines that should be saved to a CSV file after creating the vaccination order.
        //
        // Parameters:
        //
        // input: the lines from a CSV file containing population information
        // doses: the number of vaccine doses available
        // vaccinateChildren: whether to vaccinate people younger than 18
        public static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine($"Antal tillgängliga doser: {vaccineDoses}");
            int option = ShowMenu("Var god välj:", new[]
            {
                "Skapa Prioritetsordning",
                "Ändra antal vaccindoser",
                "Ändra åldersgräns",
                "Ändra indatafil",
                "Ändra utdatafil",
                "Avsluta"
            });
            if (option == 0)
            {

            }
            else if(option == 1)
            {
                ChangeVaccinDoses();
            }
            else if (option == 2)
            {

            }
            else if (option == 3)
            {
                Console.Clear();
                Console.WriteLine("Please enter the filepath:");
                string filePath = Console.ReadLine();
                PrintList(ReadCSVFile(filePath));
                

            }
            else if (option == 4)
            {

            }
            else if (option == 5)
            {
                Environment.Exit(0);
            }
        }
        public static List<Person> ReadCSVFile(string filepath)
        {
            List<Person> listOfPeople = new List<Person>();
            string[] people = File.ReadAllLines(filepath);
            foreach (string l in people)
            {
                string[] values = l.Split(',');
                long idNumber = long.Parse(values[0]);
                string lastName = values[1];
                string firstName = values[2];
                string healthCarePro = (int.Parse(values[3]) == 1) ? "Yes" : "No";
                int highRisk = int.Parse(values[4]);
                int infected = int.Parse(values[5]);
                
                Person person = new Person(idNumber, lastName, firstName, healthCarePro, highRisk, infected);
                listOfPeople.Add(person);
            }
            return listOfPeople;
        }
        public static void PrintList(List<Person> listOfPeople)
        {
            string eachPerson = "";
            foreach (Person person in listOfPeople)
            {
               eachPerson += ($"Person ID: {person.IdNumber} Name: {person.LastName} {person.FirstName} Healthcare professional: {person.HealthCarePro}\n");
            }
            Console.WriteLine(eachPerson);
            Console.ReadKey();
        }
        public static string[] CreateVaccinationOrder(string[] input, int doses, bool vaccinateChildren)
        {
            // Replace with your own code.
            return new string[0];
        }
        public static int ChangeVaccinDoses()
        {
            int changingDoses = 0;
            Console.WriteLine($"Antal tillgängliga doser: {vaccineDoses}");
            
            while (true)
            {
                Console.WriteLine("Hur många doser vill du ändra till?");
                try
                {     
                  changingDoses = int.Parse(Console.ReadLine());
                    vaccineDoses = changingDoses;
                    Console.Clear();
                    Console.WriteLine($"Tillgängliga doser har ändrats till {changingDoses}.");
                    Console.ReadKey();
                    return vaccineDoses;                   
                }
                catch (Exception e)
                {
                    Console.WriteLine("Var god skriv in ett heltal.");
                    Console.WriteLine($"Error: {e.Message}");       
                }
                
            }

            
        }
        #region
        public static int ShowMenu(string prompt, IEnumerable<string> options)
        {
            if (options == null || options.Count() == 0)
            {
                throw new ArgumentException("Cannot show a menu for an empty list of options.");
            }

            Console.WriteLine(prompt);

            // Hide the cursor that will blink after calling ReadKey.
            Console.CursorVisible = false;

            // Calculate the width of the widest option so we can make them all the same width later.
            int width = options.Max(option => option.Length);

            int selected = 0;
            int top = Console.CursorTop;
            for (int i = 0; i < options.Count(); i++)
            {
                // Start by highlighting the first option.
                if (i == 0)
                {
                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.ForegroundColor = ConsoleColor.White;
                }

                var option = options.ElementAt(i);
                // Pad every option to make them the same width, so the highlight is equally wide everywhere.
                Console.WriteLine("- " + option.PadRight(width));

                Console.ResetColor();
            }
            Console.CursorLeft = 0;
            Console.CursorTop = top - 1;

            ConsoleKey? key = null;
            while (key != ConsoleKey.Enter)
            {
                key = Console.ReadKey(intercept: true).Key;

                // First restore the previously selected option so it's not highlighted anymore.
                Console.CursorTop = top + selected;
                string oldOption = options.ElementAt(selected);
                Console.Write("- " + oldOption.PadRight(width));
                Console.CursorLeft = 0;
                Console.ResetColor();

                // Then find the new selected option.
                if (key == ConsoleKey.DownArrow)
                {
                    selected = Math.Min(selected + 1, options.Count() - 1);
                }
                else if (key == ConsoleKey.UpArrow)
                {
                    selected = Math.Max(selected - 1, 0);
                }

                // Finally highlight the new selected option.
                Console.CursorTop = top + selected;
                Console.BackgroundColor = ConsoleColor.Blue;
                Console.ForegroundColor = ConsoleColor.White;
                string newOption = options.ElementAt(selected);
                Console.Write("- " + newOption.PadRight(width));
                Console.CursorLeft = 0;
                // Place the cursor one step above the new selected option so that we can scroll and also see the option above.
                Console.CursorTop = top + selected - 1;
                Console.ResetColor();
            }

            // Afterwards, place the cursor below the menu so we can see whatever comes next.
            Console.CursorTop = top + options.Count();

            // Show the cursor again and return the selected option.
            Console.CursorVisible = true;
            return selected;
        }
        #endregion
    }

    [TestClass]
    public class ProgramTests
    {
        [TestMethod]
        public void ExampleTest()
        {
            // Arrange
            string[] input =
            {
                "19720906-1111,Elba,Idris,0,0,1",
                "8102032222,Efternamnsson,Eva,1,1,0"
            };
            int doses = 10;
            bool vaccinateChildren = false;

            // Act
            string[] output = Program.CreateVaccinationOrder(input, doses, vaccinateChildren);

            // Assert
            Assert.AreEqual(output.Length, 2);
            Assert.AreEqual("19810203-2222,Efternamnsson,Eva,2", output[0]);
            Assert.AreEqual("19720906-1111,Elba,Idris,1", output[1]);
        }
    }
}