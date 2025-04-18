namespace AnimalShelter;

using Spectre.Console;

public class UI{

    private DataHandler data;

    public UI(){
        this.data = new DataHandler();
    }

    public void DisplayMenu(){

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


    public void Login(){

    }

    public void AddAnimal(){

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

        data.AddAnimal(animalName, animalType, animalAge, animalPersonality, animalAdoptionStatus, animalVaccines, animalPreviousAppointments);
    }

    public void ViewAnimals(){

        List<string> filters = AskForFilters();

        string searchFilter = filters[0];

        string adoptionStatusFilter = filters[1];

        string[] animalNames = data.ReturnFilteredAnimals(searchFilter, adoptionStatusFilter);

        AnsiConsole.Clear();

        string selectedAnimal = "";
        while(selectedAnimal != "Return to menu"){
            AnsiConsole.WriteLine("View Animals");
            AnsiConsole.WriteLine($"Search filter: {(searchFilter != "" ? searchFilter : "None")}");
            AnsiConsole.WriteLine($"Adoption Status: {adoptionStatusFilter}");
            selectedAnimal = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .AddChoices(animalNames));
            if(selectedAnimal != "Return to menu" && selectedAnimal != "Change search filters"){

                string selectedAnimalId = selectedAnimal.Split("ID: ")[1];
                selectedAnimalId = selectedAnimalId.Remove(selectedAnimalId.IndexOf(")"));
                AnsiConsole.Clear();
                //ViewAnimal(int.Parse(selectedAnimalId));

            }else if(selectedAnimal == "Change search filters"){

                AnsiConsole.Clear();
                filters = AskForFilters();

                searchFilter = filters[0];
                adoptionStatusFilter = filters[1];

                animalNames = data.ReturnFilteredAnimals(searchFilter, adoptionStatusFilter);
            }
            AnsiConsole.Clear();
        }
    }
    public List<string> AskForFilters(){
        AnsiConsole.Write("Enter search filter (leave blank for no filter): ");
        string searchFilter = Console.ReadLine();

        string adoptionStatusFilter = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select adoption status:")
                        .AddChoices("Any", "Not Ready", "Ready"));

        return new List<String>() {searchFilter, adoptionStatusFilter};
    }

    public void ViewAppointments(){

    }
}