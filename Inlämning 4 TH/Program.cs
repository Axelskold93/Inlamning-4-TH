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
        public DateTime IdNumber;
        public string LastFour;
        public double Age;
        public string LastName;
        public string FirstName;
        public int HealthCarePro;
        public int HighRisk;
        public bool Infected;
        public int GivenDoses;

        public Person(DateTime idNumber, string lastFour, string lastName, string firstName, int healthCarePro, int highRisk, bool infected, int givenDoses)
        {
            IdNumber = idNumber;
            LastFour = lastFour;
            LastName = lastName;
            FirstName = firstName;
            HealthCarePro = healthCarePro;
            HighRisk = highRisk;
            Infected = infected;
            GivenDoses = givenDoses;
        }
    }
    public class Program
    {
        public static int vaccineDoses = 0;
        public static bool vaccinateChildren = false;
        public static List<Person> listOfPeople = new List<Person>();
        public static List<Person> vaccinationOrder = new List<Person>();
        public static string inputFilePath;
        public static string outputFilePath;
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
            Console.Write("Vaccinera personer under 18 år:"); Console.Write(vaccinateChildren == true ? "Ja" : "Nej");
            Console.WriteLine();
            Console.WriteLine($"Indatafil: {inputFilePath}");
            Console.WriteLine($"Utdatafil: {outputFilePath}");
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

                CreateVaccinationOrder(listOfPeople, vaccineDoses, vaccinateChildren);
                SaveCSVFile(vaccinationOrder, outputFilePath);
            }
            else if (option == 1)
            {
                Console.Clear();
                ChangeVaccinDoses();
            }
            else if (option == 2)
            {
                vaccinateChildren = ChangeAgeLimit();
            }
            else if (option == 3)
            {
                ChangeInputCSVFilePath();
            }
            else if (option == 4)
            {

                ChangeOutputCSVFilePath();
            }
            else if (option == 5)
            {
                Console.WriteLine("Tack för denna gång!");
                Environment.Exit(0);
            }
        }
        public static bool IsPerson18(DateTime IdNumber)
        {
            DateTime today = DateTime.Today;
            DateTime eighteenYears = today.AddYears(-18);
            return IdNumber <= eighteenYears;
        }
        public static List<Person> ReadCSVFile()
        {
            Console.Clear();
            if (inputFilePath == null)
            {
                Console.WriteLine("Vänligen lägg till en inputfil.");
                Console.ReadKey();
                MainMenu();
            }
            
            List<List<string>> rowErrors = new List<List<string>>();
            int rowIndex = 0;
            
            
                string[] people = File.ReadAllLines(inputFilePath);
            foreach (string l in people)
            {
                bool columnError;
                rowIndex++;
                List<string> currentRow = new List<string>();

                string[] values = l.Split(',');
                if (values.Length != 6)
                {
                    currentRow.Add($"Fel på rad {rowIndex}: \nOtillräckligt antal kolumner.");
                    rowErrors.Add(currentRow);
                    columnError = true;

                }
                else
                {
                    columnError = false;
                }

                if (!columnError)
                {
                    string[] IdNumberParts = values[0].Split('-');
                    if (IdNumberParts.Length != 2)
                    {
                        currentRow.Add("Ogiltigt format på personnummer.");
                    }
                    string birthYear = IdNumberParts[0];
                    DateTime idNumber;

                    if (!DateTime.TryParseExact(birthYear, "yyyyMMdd", null, DateTimeStyles.None, out idNumber))
                    {
                        currentRow.Add("Felaktigt format på födelsedatum.");
                    }

                    TimeSpan years = DateTime.Today.Subtract(idNumber);
                    string lastFour = IdNumberParts[1];
                    double age = Math.Round(years.TotalDays / 365);
                    string lastName = values[1];
                    if (string.IsNullOrWhiteSpace(lastName))
                    {
                        currentRow.Add("Kolumnen för efternamn är tom.");
                    }
                    string firstName = values[2];
                    if (string.IsNullOrEmpty(firstName))
                    {
                        currentRow.Add("Kolumnen för förnamn är tom.");
                    }
                    int healthCarePro;
                    if (int.TryParse(values[3], out healthCarePro))
                    {
                        if (healthCarePro != 0 && healthCarePro != 1)
                        {
                            currentRow.Add("Ogiltig siffra för Vård/Omsorg.");
                        }
                    }
                    else
                    {
                        currentRow.Add("Ogiltig siffra för Vård/Omsorg.");
                    }
                    int highRisk;
                    if (int.TryParse(values[4], out highRisk))
                    {
                        if (highRisk != 0 && highRisk != 1)
                        {
                            currentRow.Add("Ogiltig siffra för Riskgrupp.");
                        }
                    }
                    else
                    {
                        currentRow.Add("Ogiltig siffra för Riskgrupp.");
                    }

                    bool infected = values[5] == "1";
                    if (int.TryParse(values[5], out int infectedValue))
                    {
                        if (infectedValue != 0 && infectedValue != 1)
                        {
                            currentRow.Add("Ogiltig siffra för Infekterad.");
                        }
                    }
                    else
                    {
                        currentRow.Add("Ogiltig siffra för Infekterad.");
                    }

                    if (currentRow.Count > 0)
                    {
                        currentRow.Insert(0, $"Fel på rad {rowIndex}:");
                        rowErrors.Add(currentRow);
                    }


                    if (rowErrors.Count == 0)
                    {
                        Person person = new Person(idNumber, lastFour, lastName, firstName, healthCarePro, highRisk, infected, 0);
                        listOfPeople.Add(person);
                    }
                }

            }
           

            if (rowErrors.Count > 0)
            {
                for(int i = 0; i < rowErrors.Count; i++)
                {
                    
                    foreach(string error in rowErrors[i]) 
                    {
                        Console.WriteLine(error);
                    }
                    
                }
                
                
                Console.ReadKey();
                MainMenu();
            }

            return listOfPeople;
        }
        public static string ChangeInputCSVFilePath()
        {
            Console.Clear();

            while (true)
            {
                Console.WriteLine("Vänligen ange sökväg:");
                string filePath = Console.ReadLine();
                inputFilePath = filePath;
                if (File.Exists(inputFilePath))
                {
                    Console.WriteLine("Indatafil ändrad.");
                    Console.ReadKey();
                    return inputFilePath;
                }
                else
                {
                    Console.WriteLine("Hittar inte fil: Filen kan inte hittas på den angivna sökvägen.");
                }
            }
        }
        public static string ChangeOutputCSVFilePath()
        {
            Console.Clear();
            while (true)
            {
                Console.WriteLine("Vänligen ange sökväg:");
                string filePath = Console.ReadLine();

                string directoryPath = Path.GetDirectoryName(filePath);


                if (Directory.Exists(directoryPath))
                {
                    outputFilePath = filePath;
                    Console.WriteLine("Utdatafil ändrad.");
                    Console.ReadKey();
                    return outputFilePath;
                }
                else
                {
                    Console.WriteLine("Hittar inte mapp. Mappen kan inte hittas på den angivna sökvägen.");
                }
            }
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
        public static List<Person> CreateVaccinationOrder(List<Person> listOfPeople, int vaccineDoses, bool vaccinateChildren)
        {
            
            ReadCSVFile();
            
            List<Person> eligiblePeople = new List<Person>();

            if (!vaccinateChildren)
            {
                foreach (Person person in listOfPeople)
                {

                    if (IsPerson18(person.IdNumber) == true)
                    {
                        eligiblePeople.Add(person);
                    }

                }
            }
            else
            {
                eligiblePeople = listOfPeople;
            }

            var healthCareWorker = eligiblePeople
                .Where(person => person.HealthCarePro == 1)
                .OrderBy(person => person.IdNumber)
                .ToList();
            vaccinationOrder.AddRange(healthCareWorker);

            var over65 = eligiblePeople
                .Where(person => person.Age >= 65)
                .Except(healthCareWorker)
                .OrderBy(person => person.IdNumber)
                .ToList();
            vaccinationOrder.AddRange(over65);

            var riskGroup = eligiblePeople
                .Where(person => person.HighRisk == 1)
                .Except(healthCareWorker)
                .Except(over65)
                .OrderBy(person => person.IdNumber)
                .ToList();
            vaccinationOrder.AddRange(riskGroup);

            var restOfPopulation = eligiblePeople
                .Except(healthCareWorker)
                .Except(over65)
                .Except(riskGroup)
                .OrderBy(person => person.IdNumber)
                .ToList();
            vaccinationOrder.AddRange(restOfPopulation);

            foreach (var person in vaccinationOrder)
            {
                if (vaccineDoses <= 0)
                {
                    break;
                }

                int givenDoses = person.Infected ? 1 : 2;

                if (vaccineDoses == 1 && givenDoses == 2)
                {
                    break;
                }

                vaccineDoses -= givenDoses;
                person.GivenDoses = givenDoses;
            }

            return vaccinationOrder;

        }
        public static void SaveCSVFile(List<Person> vaccinationOrder, string OutputFilePath)
        {
            Console.Clear();
            if (string.IsNullOrWhiteSpace(OutputFilePath))
            {
                Console.WriteLine("Vänligen lägg till en outputfil.");
                Console.ReadKey();
                MainMenu();
            }
           
            var lines = new List<string>();

            foreach (var person in vaccinationOrder)
            {
                string line = $"{person.IdNumber:yyyyMMdd}-{person.LastFour},{person.LastName},{person.FirstName},{person.GivenDoses}";
                lines.Add(line);
            }

            if (File.Exists(OutputFilePath))
            {
                int option = ShowMenu("En fil existerar redan, vill du skriva över den?", new[]
                {
                    "Ja",
                    "Nej"
                });
                if (option == 0)
                {
                    File.WriteAllText(outputFilePath, string.Empty);
                    File.WriteAllLines(outputFilePath, lines);
                    Console.WriteLine($"Resultatet har sparats i {outputFilePath}.");
                    Console.ReadKey();
                }


            }
            else
            {
                File.Create(OutputFilePath).Close();
                Console.WriteLine("Fil skapad.");
                File.WriteAllLines(outputFilePath, lines);
                Console.WriteLine($"Resultatet har sparats i {outputFilePath}");
                Console.ReadKey();
            }

        }
        public static int ChangeVaccinDoses()
        {
            Console.Clear();
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

        public static bool ChangeAgeLimit()
        {
            Console.Clear();

            int option = ShowMenu("Ska personer under 18 vaccineras?", new[]
            {
                "Ja",
                "Nej"
            });
            if (option == 0)
            {
                vaccinateChildren = true;
            }
            else
            {
                vaccinateChildren = false;
            }
            return vaccinateChildren;
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


}