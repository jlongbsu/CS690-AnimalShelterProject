namespace AnimalShelter;

using System.IO;
using System;
using Spectre.Console;

class Program
{
    static int id = 0;
    static List<Animal> animals = new List<Animal>();
    static Dictionary<string, List<Appointment>> upcomingAppointments = new Dictionary<string, List<Appointment>>();
    static void Main(string[] args)
    {
        Dictionary<string, List<Appointment>> previousAppointments = new Dictionary<string, List<Appointment>>();
        string appointmentsFilePath = "appointments.txt";
        if(File.Exists(appointmentsFilePath)){
            foreach(var line in File.ReadLines(appointmentsFilePath)){
                string[] appointmentInfo = line.Split(",");
                string currentDate = DateTime.Today.ToString("d");
                if(DateTime.Parse(currentDate) <= DateTime.Parse(appointmentInfo[4])){
                    if(upcomingAppointments.ContainsKey(appointmentInfo[0])){
                        upcomingAppointments[appointmentInfo[0]].Add(new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3])));
                    }else{
                        upcomingAppointments[appointmentInfo[0]] = new List<Appointment>{new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3]))};
                    }
                }else{
                    if(previousAppointments.ContainsKey(appointmentInfo[0])){
                        previousAppointments[appointmentInfo[0]].Add(new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3])));
                    }else{
                        previousAppointments[appointmentInfo[0]] = new List<Appointment>{new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3]))};
                    }                    
                }
            }
        }
        Dictionary<string, List<Vaccine>> animalsVaccines = new Dictionary<string, List<Vaccine>>();
        string vaccinesFilePath = "vaccines.txt";
        if(File.Exists(vaccinesFilePath)){
            foreach(var line in File.ReadLines(vaccinesFilePath)){
                string[] vaccineInfo = line.Split(",");
                if(animalsVaccines.ContainsKey(vaccineInfo[0])){
                    animalsVaccines[vaccineInfo[0]].Add(new Vaccine(vaccineInfo[1], DateTime.Parse(vaccineInfo[2])));
                }
            }
        }

        string animalsFilePath = "animals.txt";
        if(File.Exists(animalsFilePath)){
            foreach(var line in File.ReadLines(animalsFilePath)){
                string[] animalInfo = line.Split(",");
                animals.Add(new Animal(id, animalInfo[0], animalInfo[1], int.Parse(animalInfo[2]), "", animalInfo[3], DateTime.Parse(animalInfo[4]), null, upcomingAppointments.ContainsKey(id.ToString()) ? upcomingAppointments[id.ToString()] : null));
                id++;
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
        AnsiConsole.Write("Enter the animal's name: ");
        string animalName = Console.ReadLine();
        AnsiConsole.Write("Enter the animal's type: ");
        string animalType = Console.ReadLine();
        AnsiConsole.Write("Enter the animal's age: ");
        int animalAge = int.Parse(Console.ReadLine());
        string animalAdoptionStatus = AnsiConsole.Prompt(
                                        new SelectionPrompt<string>()
                                            .Title("Select the animal's adoption status:")
                                            .AddChoices("Not Ready", "Ready"));
        AnsiConsole.WriteLine($"Select the animal's adoption status: {animalAdoptionStatus}");
        bool addVaccineInfo = AnsiConsole.Prompt(
                                new ConfirmationPrompt("Does this animal have any vaccine information to add?"));
        List<Vaccine> animalVaccines = new List<Vaccine>();
        while(addVaccineInfo){
            AnsiConsole.Write("Enter vaccine name: ");
            string vaccineName = Console.ReadLine();
            AnsiConsole.Write("Enter date given (mm/dd/yyyy format): ");
            string dateGiven = Console.ReadLine();
            animalVaccines.Add(new Vaccine(vaccineName, DateTime.Parse(dateGiven)));
            addVaccineInfo = AnsiConsole.Prompt(
                        new ConfirmationPrompt("Add another?"));
        }
        bool addPrevAppointments = AnsiConsole.Prompt(
                                new ConfirmationPrompt("Does this animal have any previous appointment visits to add?"));
        List<Appointment> animalPreviousAppointments = new List<Appointment>();
        while(addPrevAppointments){
            string appointmentType = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Select the appointment type:")
                                    .AddChoices("Vaccine", "Checkup"));
            AnsiConsole.Write("Enter date of appointment (mm/dd/yyyy format): ");
            string dateOfAppointment = Console.ReadLine();
            AnsiConsole.Write("Enter vet name: ");
            string vetName = Console.ReadLine();
            AnsiConsole.Write("Enter vet address: ");
            string vetAddress = Console.ReadLine();
            animalPreviousAppointments.Add(new Appointment(appointmentType, DateTime.Parse(dateOfAppointment), new Vet(vetName, vetAddress)));
            addPrevAppointments = AnsiConsole.Prompt(
                        new ConfirmationPrompt("Add another?"));
        }
        File.AppendAllText("animals.txt", $"{animalName},{animalType},{animalAge},{animalAdoptionStatus},{DateTime.Today.ToString("d")}{Environment.NewLine}");
        foreach(Vaccine vaccine in animalVaccines){
            File.AppendAllText("vaccines.txt", $"{id},{vaccine.Type},{vaccine.DateGiven.ToString("d")}{Environment.NewLine}");
        }
        id++;
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
        AnsiConsole.WriteLine("Upcoming appointments:");
        if(animals[id].UpcomingAppointments != null){
            Table appointments = new Table();
            appointments.AddColumn("Appointment Type");
            appointments.AddColumn("Vet Name");
            appointments.AddColumn("Vet Address");
            appointments.AddColumn("Date");
            foreach(Appointment appointment in animals[id].UpcomingAppointments){
                appointments.AddRow(appointment.Type, appointment.Vet.Name, appointment.Vet.Address, appointment.Date.ToString("d"));
            }
            AnsiConsole.Write(appointments);
        }
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
        string mode = AnsiConsole.Prompt(
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
        AnsiConsole.WriteLine($"View Upcoming Appointments");
        string mode = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .AddChoices(new[] {
                                "Add new appointment", "Return to menu"
                            }));
        if(mode == "Add new appointment"){
            AnsiConsole.Clear();
            AddAppointment();
        }
    }

    static void AddAppointment(){
        string[] animalNames = new string[animals.Count];
        for(int i = 0; i < animals.Count; i++){
            animalNames[i] = $"{animals[i].Name} (ID: {i})";
        }
        AnsiConsole.WriteLine($"Add New Appointment");
        string selectedAnimal = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Select an animal:")
                                    .AddChoices(animalNames));
        string selectedAnimalId = selectedAnimal.Split("ID: ")[1];
        selectedAnimalId = selectedAnimalId.Remove(selectedAnimalId.IndexOf(")"));
        AnsiConsole.WriteLine($"Animal: {selectedAnimal}");
        string appointmentType = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Select appointment type:")
                                    .AddChoices("Checkup", "Vaccination"));
        AnsiConsole.WriteLine($"Appointment Type: {appointmentType}");
        AnsiConsole.Write("Enter the vet's name: ");
        string vetName = Console.ReadLine();
        AnsiConsole.Write("Enter the vet's address: ");
        string vetAddress = Console.ReadLine();
        AnsiConsole.Write("Enter the date of the appointment (mm/dd/yyyy format): ");
        string appointmentDate = Console.ReadLine();
        File.AppendAllText("appointments.txt", $"{selectedAnimalId},{appointmentType},{vetName},{vetAddress},{appointmentDate}{Environment.NewLine}");
    }
}
