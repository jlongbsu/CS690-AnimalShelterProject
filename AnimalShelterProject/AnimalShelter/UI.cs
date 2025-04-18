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
        data.AddAnimal();
    }

    public void ViewAnimals(){
        AnsiConsole.Write("Enter search filter (leave blank for no filter): ");
        string searchFilter = AnsiConsole.ReadLine();

        string adoptionStatusFilter = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select adoption status:")
                        .AddChoices("Any", "Not Ready", "Ready"));

        data.DisplayAnimals(searchFilter, adoptionStatusFilter);
    }

    public void ViewAppointments(){

    }
}