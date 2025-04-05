namespace AnimalShelter;

using System.IO;
using System;
using Spectre.Console;

class Program
{
    static void Main(string[] args)
    {
        // Console.WriteLine("What would you like to do?");
        // string mode = Console.ReadLine();

        string mode = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("What would you like to do?")
                            .AddChoices(new[] {
                                "Add new animal", "Option 2"
                            }));


        if(mode == "Add new animal"){
            Console.WriteLine("Enter the animal's name: ");
            string animalName = Console.ReadLine();
            Console.WriteLine("Enter the animal's type: ");
            string animalType = Console.ReadLine();
            Console.WriteLine("Enter the animal's age: ");
            int animalAge = int.Parse(Console.ReadLine());
            Console.WriteLine("Enter the animal's adoption status: ");
            string animalAdoptionStatus = Console.ReadLine();

            File.AppendAllText("animals.txt", $"{animalName},{animalType},{animalAge},{animalAdoptionStatus},{DateTime.Today.ToString("d")}{Environment.NewLine}");
        }
    }
}
