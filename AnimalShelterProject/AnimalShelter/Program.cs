namespace AnimalShelter;

using System.IO;
using System;
using Spectre.Console;

class Program
{
    static List<Animal> animals = new List<Animal>();
    static List<Appointment> upcomingAppointments = new List<Appointment>();
    static void Main(string[] args)
    {
        string animalsFilePath = "animals.txt";
        if(File.Exists(animalsFilePath)){
            int id = 0;
            foreach(var line in File.ReadLines(animalsFilePath)){
                string[] animalInfo = line.Split(",");
                animals.Add(new Animal(id, animalInfo[0], animalInfo[1], int.Parse(animalInfo[2]), "", animalInfo[3], DateTime.Parse(animalInfo[4]), null, null));
            }
        }
        DisplayMenu();
    }

    static void DisplayMenu(){
        AnsiConsole.Clear();
        AnsiConsole.WriteLine("Welcome to the Animal Shelter Management System!");
        string mode = "";
        while(mode != "Exit"){
            mode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to do?")
                    .AddChoices(new[] {
                        "Add new animal", "View/edit animals", "View/add appointments", "Exit"
                    }));

            if(mode == "Add new animal"){
                AnsiConsole.Clear();
                AddAnimal();
            }else if(mode == "View/edit animals"){
                AnsiConsole.Clear();
                ViewAnimals();
            }else if(mode == "View/add appointments"){
                AnsiConsole.Clear();
                ViewAppointments();
            }
        }  
    }

    static void AddAnimal(){
        AnsiConsole.WriteLine("Add New Animal");
        AnsiConsole.WriteLine("Enter the animal's name: ");
        string animalName = Console.ReadLine();
        AnsiConsole.WriteLine("Enter the animal's type: ");
        string animalType = Console.ReadLine();
        AnsiConsole.WriteLine("Enter the animal's age: ");
        int animalAge = int.Parse(Console.ReadLine());
        AnsiConsole.WriteLine("Enter the animal's adoption status: ");
        string animalAdoptionStatus = Console.ReadLine();

        File.AppendAllText("animals.txt", $"{animalName},{animalType},{animalAge},{animalAdoptionStatus},{DateTime.Today.ToString("d")}{Environment.NewLine}");
    }

    static void ViewAnimals(){
        string[] animalNames = new string[animals.Count + 1];
        for(int i = 0; i < animals.Count; i++){
            animalNames[i] = $"{animals[i].Name} (ID: {i})";
        }
        animalNames[animalNames.Length - 1] = "Return to menu";
        string selectedAnimal = "";
        while(selectedAnimal != "Return to menu"){
            AnsiConsole.WriteLine("View Animals");
            selectedAnimal = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .AddChoices(animalNames));
            if(selectedAnimal != "Return to menu"){
                string selectedAnimalId = selectedAnimal.Split("ID: ")[1];
                selectedAnimalId = selectedAnimalId.Remove(selectedAnimalId.IndexOf(")"));
                AnsiConsole.Clear();
                ViewAnimal(int.Parse(selectedAnimalId));
            }
            AnsiConsole.Clear();
        }
    }

    static void ViewAnimal(int id){
        AnsiConsole.WriteLine($"View Animal Information");
        AnsiConsole.WriteLine($"Name: {animals[id].Name}");
        AnsiConsole.WriteLine($"Type: {animals[id].Type}");
        AnsiConsole.WriteLine($"Age: {animals[id].Age}");
        AnsiConsole.WriteLine($"Adoption status: {animals[id].AdoptionStatus}");
        AnsiConsole.WriteLine($"Date added to shelter: {animals[id].DateAddedToShelter.ToString("d")}");
        string mode = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices(new[] {
                                "Edit", "Return to View Animals"
                            }));
        if(mode == "Edit"){
            AnsiConsole.Clear();
            EditAnimal(id);
        }
    }

    static void EditAnimal(int id){
        AnsiConsole.WriteLine($"Edit Animal Information");
        var mode = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices(new[] {
                                "Edit adoption status", "Update vaccine information", "Add appointment", "Return to View Animal"
                            }));
        if(mode == "Edit adoption status"){
            animals[id].AdoptionStatus = "ready";
        }else if(mode == "Update vaccine information"){

        }else if(mode == "Add appointment"){

        }
    }

    static void ViewAppointments(){
        
    }
}
