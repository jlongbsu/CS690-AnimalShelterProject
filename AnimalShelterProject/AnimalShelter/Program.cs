namespace AnimalShelter;

using System.IO;
using System;
using Spectre.Console;
using Microsoft.VisualBasic;

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
                }else{
                    animalsVaccines[vaccineInfo[0]] = new List<Vaccine>{new Vaccine(vaccineInfo[1], DateTime.Parse(vaccineInfo[2]))};
                }
            }
        }

        string animalsFilePath = "animals.txt";
        if(File.Exists(animalsFilePath)){
            foreach(var line in File.ReadLines(animalsFilePath)){
                string[] animalInfo = line.Split(",");
                if(!upcomingAppointments.ContainsKey(id.ToString())){
                    upcomingAppointments[id.ToString()] = new List<Appointment>();
                }
                MedicalHistory medicalHistory = new MedicalHistory(new List<Appointment>(), new List<Vaccine>());
                if(previousAppointments.ContainsKey(id.ToString())){
                    foreach(Appointment appointment in previousAppointments[id.ToString()]){
                        medicalHistory.PreviousAppointments.Add(appointment);
                    }
                    medicalHistory.SortPreviousAppointments();
                }
                if(animalsVaccines.ContainsKey(id.ToString())){
                    animalsVaccines[id.ToString()] = animalsVaccines[id.ToString()].OrderBy(vaccine => vaccine.DateGiven).ThenBy(vaccine => vaccine.Type).ToList();
                    foreach(Vaccine vaccine in animalsVaccines[id.ToString()]){
                        medicalHistory.Vaccines.Add(vaccine);
                    }
                    medicalHistory.SortVaccines();
                }

                upcomingAppointments[id.ToString()] = upcomingAppointments[id.ToString()].OrderBy(appointment => appointment.Date).ThenBy(appointment => appointment.Type).ThenBy(appointment => appointment.Vet.Name).ToList();

                animals.Add(new Animal(id, animalInfo[0], animalInfo[1], int.Parse(animalInfo[2]), animalInfo[3], animalInfo[4], DateTime.Parse(animalInfo[5]), medicalHistory, new List<Appointment>(upcomingAppointments[id.ToString()])));
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
        AnsiConsole.Write("Enter the animal's personality: ");
        string animalPersonality = Console.ReadLine();
        string animalAdoptionStatus = AnsiConsole.Prompt(
                                        new SelectionPrompt<string>()
                                            .Title("Select the animal's adoption status:")
                                            .AddChoices("Not Ready", "Ready"));
        AnsiConsole.WriteLine($"Select the animal's adoption status: {animalAdoptionStatus}");
        bool addVaccineInfo = AnsiConsole.Prompt(
                                new ConfirmationPrompt("Does this animal have any vaccine information to add?"));
        MedicalHistory animalMedicalHistory = new MedicalHistory(new List<Appointment>(), new List<Vaccine>());
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
        File.AppendAllText("animals.txt", $"{animalName},{animalType},{animalAge},{animalPersonality},{animalAdoptionStatus},{DateTime.Today.ToString("d")}{Environment.NewLine}");

        foreach(Vaccine vaccine in animalVaccines){
            File.AppendAllText("vaccines.txt", $"{id},{vaccine.Type},{vaccine.DateGiven.ToString("d")}{Environment.NewLine}");
            animalMedicalHistory.Vaccines.Add(new Vaccine(vaccine.Type, vaccine.DateGiven));
        }
        animalMedicalHistory.SortVaccines();

        foreach(Appointment appointment in animalPreviousAppointments){
            File.AppendAllText("appointments.txt", $"{id},{appointment.Type},{appointment.Vet.Name},{appointment.Vet.Address},{appointment.Date.ToString("d")}{Environment.NewLine}");
            animalMedicalHistory.PreviousAppointments.Add(new Appointment(appointment.Type, appointment.Date, appointment.Vet));
        }
        animalMedicalHistory.SortPreviousAppointments();
        upcomingAppointments[id.ToString()] = new List<Appointment>();
        animals.Add(new Animal(id, animalName, animalType, animalAge, animalPersonality, animalAdoptionStatus, DateTime.Today, animalMedicalHistory, new List<Appointment>()));

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
        string mode = "";
        while(mode != "Return to View Animals"){
            AnsiConsole.WriteLine("View Animal Information");
            AnsiConsole.WriteLine($"Name: {animals[id].Name}");
            AnsiConsole.WriteLine($"Type: {animals[id].Type}");
            AnsiConsole.WriteLine($"Age: {animals[id].Age}");
            AnsiConsole.WriteLine($"Personality: {animals[id].Personality}");
            AnsiConsole.WriteLine($"Adoption status: {animals[id].AdoptionStatus}");
            AnsiConsole.WriteLine($"Date added to shelter: {animals[id].DateAddedToShelter.ToString("d")}");

            AnsiConsole.WriteLine($"Vaccine information:{(animals[id].MedicalHistory.Vaccines.Count > 0 ? "" : " None")}");
            if(animals[id].MedicalHistory.Vaccines.Count > 0){
                Table vaccines = new Table();
                vaccines.AddColumn("Vaccine");
                vaccines.AddColumn("Date Given");
                foreach(Vaccine vaccine in animals[id].MedicalHistory.Vaccines){
                    vaccines.AddRow(vaccine.Type, vaccine.DateGiven.ToString("d"));
                }
                AnsiConsole.Write(vaccines);            
            }

            AnsiConsole.WriteLine($"Previous Appointments:{(animals[id].MedicalHistory.PreviousAppointments.Count > 0 ? "" : " None")}");
            if(animals[id].MedicalHistory.PreviousAppointments.Count > 0){
                Table appointments = new Table();
                appointments.AddColumn("Appointment Type");
                appointments.AddColumn("Vet Name");
                appointments.AddColumn("Vet Address");
                appointments.AddColumn("Date");
                foreach(Appointment appointment in animals[id].MedicalHistory.PreviousAppointments){
                    appointments.AddRow(appointment.Type, appointment.Vet.Name, appointment.Vet.Address, appointment.Date.ToString("d"));
                }
                AnsiConsole.Write(appointments);
            }

            AnsiConsole.WriteLine($"Upcoming appointments:{(animals[id].UpcomingAppointments.Count > 0 ? "" : " None")}");
            if(animals[id].UpcomingAppointments.Count > 0){
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
                mode = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .AddChoices(new[] {
                                    "Edit", "Return to View Animals"
                                }));
                if(mode == "Edit"){
                    AnsiConsole.Clear();
                    EditAnimal(id);
                }
        }
    }

    static void EditAnimal(int id){
        string mode = "";
        while(mode != "Return to Animal Overview"){
            AnsiConsole.WriteLine("Edit Animal Information");
            mode = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .AddChoices(new[] {
                                    "Edit adoption status (Not implemented)", "Update vaccine information", "Add appointment (Not implemented)", "Return to Animal Overview"
                                }));
            if(mode == "Edit adoption status"){
                //animals[id].AdoptionStatus = "ready";
            }else if(mode == "Update vaccine information"){
                AnsiConsole.Clear();
                UpdateVaccineInfo(id);
            }else if(mode == "Add appointment (Not implemented)"){

            }
        }
        AnsiConsole.Clear();
    }

    static void UpdateVaccineInfo(int id){
        AnsiConsole.WriteLine("Update Animal's Vaccine Information");
        bool addVaccine = true;
        while(addVaccine){
            AnsiConsole.Write("Enter vaccine name: ");
            string vaccineName = Console.ReadLine();
            AnsiConsole.Write("Enter date given (mm/dd/yyyy format): ");
            string dateGiven = Console.ReadLine();
            animals[id].MedicalHistory.Vaccines.Add(new Vaccine(vaccineName, DateTime.Parse(dateGiven)));
            File.AppendAllText("vaccines.txt", $"{id},{vaccineName},{dateGiven}{Environment.NewLine}");
            addVaccine = AnsiConsole.Prompt(
                        new ConfirmationPrompt("Add another?"));
        }
        animals[id].MedicalHistory.SortVaccines();
        AnsiConsole.Clear();
    }

    static void ViewAppointments(){
        AnsiConsole.WriteLine("View Upcoming Appointments");
        string mode = "";
        while(mode != "Return to menu"){
            Table appointments = new Table();
            appointments.AddColumn("Animal");
            appointments.AddColumn("Appointment Type");
            appointments.AddColumn("Vet Name");
            appointments.AddColumn("Vet Address");
            appointments.AddColumn("Date");
            var sortedUpcomingAppointments = upcomingAppointments.SelectMany(kv => kv.Value.Select(appointment => new {AnimalId = kv.Key, Appointment = appointment})).OrderBy(entry => entry.Appointment.Date).ThenBy(entry => int.Parse(entry.AnimalId)).ThenBy(entry => entry.Appointment.Type).ThenBy(entry => entry.Appointment.Vet.Name).ToList();
            foreach(var animal in sortedUpcomingAppointments){
                appointments.AddRow($"{animals[int.Parse(animal.AnimalId)].Name} (ID: {animal.AnimalId})", animal.Appointment.Type, animal.Appointment.Vet.Name, animal.Appointment.Vet.Address, animal.Appointment.Date.ToString("d"));
            }
            AnsiConsole.Write(appointments);
            mode = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .AddChoices(new[] {
                                        "Add new appointment", "Return to menu"
                                    }));
                if(mode == "Add new appointment"){
                    AnsiConsole.Clear();
                    AddAppointment();
                }
            AnsiConsole.Clear();
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
        upcomingAppointments[selectedAnimalId].Add(new Appointment(appointmentType, DateTime.Parse(appointmentDate), new Vet(vetName, vetAddress)));
        upcomingAppointments[selectedAnimalId] = upcomingAppointments[selectedAnimalId].OrderBy(appointment => appointment.Date).ThenBy(appointment => appointment.Type).ThenBy(appointment => appointment.Vet.Name).ToList();
        animals[int.Parse(selectedAnimalId)].UpcomingAppointments.Add(new Appointment(appointmentType, DateTime.Parse(appointmentDate), new Vet(vetName, vetAddress)));
        animals[int.Parse(selectedAnimalId)].SortUpcomingAppointments();
    }
}
