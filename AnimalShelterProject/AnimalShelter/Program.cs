namespace AnimalShelter;

using System.IO;
using System;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("What would you like to do?");
        string mode = Console.ReadLine();

        if(mode == "add"){
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
