namespace AnimalShelter;

using Spectre.Console;

public class UI{

    private DataHandler data;
    private AuthenticationService auth;

    public UI(){
        this.data = new DataHandler();
        this.auth = new AuthenticationService();
    }

    public void DisplayMenu(){

        string mode = "";

        while(mode != "Exit"){

            AnsiConsole.Clear();
            AnsiConsole.WriteLine("Welcome to the Animal Shelter Management System!");

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

        AnsiConsole.Clear();
    }


    public void LoginScreen(){

        string mode = "";

        while(mode != "Exit"){

            AnsiConsole.Clear();
            
            mode = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[] {
                        "Create new account", "Login", "Exit"
                    }));

            if(mode == "Create new account"){

                AnsiConsole.Clear();
                CreateAccount();

            }else if(mode == "Login"){

                AnsiConsole.Clear();
                Login();

            }
            AnsiConsole.Clear();
        }
    }

    public void CreateAccount(){

        bool usernameTaken = true;
        string username = "";
        AnsiConsole.WriteLine("Create Account");

        while(usernameTaken){

            AnsiConsole.Write("Enter a username: ");
            username = Console.ReadLine();

            usernameTaken = auth.UsernameIsTaken(username);
            
            if(usernameTaken){
                AnsiConsole.WriteLine("That username is already taken.");
            }
        }

        bool passwordsDontMatch = true;
        string password = "";
        
        while(passwordsDontMatch){

            AnsiConsole.Write("Enter a password: ");
            password = Console.ReadLine();

            AnsiConsole.Write("Re-enter the password: ");
            string password2 = Console.ReadLine();

            passwordsDontMatch = !(password == password2);

            if(passwordsDontMatch){
                AnsiConsole.WriteLine("The passwords don't match.");
            }
        }

        auth.CreateAccount(username, password);

        AnsiConsole.Clear();
        DisplayMenu();
    }

    public void Login(){

        bool notAuthenticated = true;
        AnsiConsole.WriteLine("Login");

        while(notAuthenticated){

            AnsiConsole.Write("Enter username: ");
            string username = Console.ReadLine();

            AnsiConsole.Write("Enter password: ");
            string password = Console.ReadLine();

            notAuthenticated = !(auth.Authenticate(username, password));

            if(notAuthenticated){
                AnsiConsole.WriteLine("The entered username and password do not match.");
            }

        }

        AnsiConsole.Clear();
        DisplayMenu();
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

        AnsiConsole.WriteLine("New Animal Successfully Added");
        Thread.Sleep(1000);
    }

    public void ViewAnimals(){

        List<string> filters = AskForFilters();

        string searchFilter = filters[0];

        string adoptionStatusFilter = filters[1];

        string[] animalNames;

        string selectedAnimal = "";

        while(selectedAnimal != "Return to menu"){
            
            animalNames = data.ReturnFilteredAnimals(searchFilter, adoptionStatusFilter);

            AnsiConsole.Clear();

            AnsiConsole.WriteLine("View Animals");
            AnsiConsole.WriteLine($"Search filter: {(searchFilter != "" ? searchFilter : "None")}");
            AnsiConsole.WriteLine($"Adoption Status: {adoptionStatusFilter}");
            AnsiConsole.WriteLine("");

            selectedAnimal = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .AddChoices(animalNames));

            if(selectedAnimal != "Return to menu" && selectedAnimal != "Change search filters"){

                string selectedAnimalId = selectedAnimal.Split("ID: ")[1];
                selectedAnimalId = selectedAnimalId.Remove(selectedAnimalId.IndexOf(")"));
                AnsiConsole.Clear();
                ViewAnimal(int.Parse(selectedAnimalId));

            }else if(selectedAnimal == "Change search filters"){

                AnsiConsole.Clear();
                filters = AskForFilters();

                searchFilter = filters[0];
                adoptionStatusFilter = filters[1];

                //animalNames = data.ReturnFilteredAnimals(searchFilter, adoptionStatusFilter);
            }

            AnsiConsole.Clear();
        }
    }

    public void ViewAnimal(int animalId){

        Animal animal = data.GetAnimal(animalId);
        string mode = "";

        while(mode != "Return to View Animals"){

            AnsiConsole.WriteLine("View Animal Information");
            AnsiConsole.WriteLine($"Name: {animal.Name}");
            AnsiConsole.WriteLine($"Type: {animal.Type}");
            AnsiConsole.WriteLine($"Age: {animal.Age}");
            AnsiConsole.WriteLine($"Personality: {animal.Personality}");
            AnsiConsole.WriteLine($"Adoption status: {animal.AdoptionStatus}");
            AnsiConsole.WriteLine($"Date added to shelter: {animal.DateAddedToShelter.ToString("d")}");

            if(animal.FamilyInfo.Name != ""){

                AnsiConsole.WriteLine("Adopted family information:");

                Table familyInfo = new Table();
                familyInfo.AddColumn("Family Name");
                familyInfo.AddColumn("Email");
                familyInfo.AddColumn("Phone");
                familyInfo.AddColumn("Address");
                familyInfo.AddColumn("Date Adopted");

                familyInfo.AddRow(animal.FamilyInfo.Name, animal.FamilyInfo.Email, animal.FamilyInfo.PhoneNumber, animal.FamilyInfo.Address, animal.FamilyInfo.DateAdopted.ToString("d"));

                AnsiConsole.Write(familyInfo);

            }

            AnsiConsole.WriteLine($"Vaccine information:{(animal.MedicalHistory.Vaccines.Count > 0 ? "" : " None")}");
            if(animal.MedicalHistory.Vaccines.Count > 0){

                Table vaccines = new Table();
                vaccines.AddColumn("Vaccine");
                vaccines.AddColumn("Date Given");

                foreach(Vaccine vaccine in animal.MedicalHistory.Vaccines){

                    vaccines.AddRow(vaccine.Type, vaccine.DateGiven.ToString("d"));

                }

                AnsiConsole.Write(vaccines);

            }

            AnsiConsole.WriteLine($"Previous Appointments:{(animal.MedicalHistory.PreviousAppointments.Count > 0 ? "" : " None")}");

            if(animal.MedicalHistory.PreviousAppointments.Count > 0){

                Table appointments = new Table();
                appointments.AddColumn("Appointment Type");
                appointments.AddColumn("Vet Name");
                appointments.AddColumn("Vet Address");
                appointments.AddColumn("Date");

                foreach(Appointment appointment in animal.MedicalHistory.PreviousAppointments){

                    appointments.AddRow(appointment.Type, appointment.Vet.Name, appointment.Vet.Address, appointment.Date.ToString("d"));

                }

                AnsiConsole.Write(appointments);

            }

            AnsiConsole.WriteLine($"Upcoming appointments:{(animal.UpcomingAppointments.Count > 0 ? "" : " None")}");

            if(animal.UpcomingAppointments.Count > 0){

                Table appointments = new Table();
                appointments.AddColumn("Appointment Type");
                appointments.AddColumn("Vet Name");
                appointments.AddColumn("Vet Address");
                appointments.AddColumn("Date");

                foreach(Appointment appointment in animal.UpcomingAppointments){

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
                EditAnimal(animalId);

            }
        }
    }

    public void EditAnimal(int animalId){

        string mode = "";

        while(mode != "Return to Animal Overview"){

            AnsiConsole.Clear();

            mode = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title($"Edit Animal Information for {data.GetAnimal(animalId).Name} (ID: {data.GetAnimal(animalId).ID})")
                                .AddChoices(new[] {
                                    "Edit adoption status", "Update vaccine information", "Add appointment", "Return to Animal Overview"
                                }));

            if(mode == "Edit adoption status"){

                AnsiConsole.Clear();
                EditAdoptionStatus(animalId);

            }else if(mode == "Update vaccine information"){

                AnsiConsole.Clear();
                UpdateVaccineInfo(animalId);

            }else if(mode == "Add appointment"){

                AnsiConsole.Clear();
                AddAppointment(animalId);

            }
        }
    }

    public void EditAdoptionStatus(int animalId){

        List<string> adoptionStatuses = ["Ready", "Not Ready", "Adopted"];

        adoptionStatuses.RemoveAll(item => item == data.GetAnimal(animalId).AdoptionStatus);
        adoptionStatuses.Add("Return to Edit Animal Menu");

        AnsiConsole.WriteLine($"Edit Adoption Status for {data.GetAnimal(animalId).Name} (ID: {data.GetAnimal(animalId).ID})");

        string newAdoptionStatus = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"Select new adoption status (Currently: {data.GetAnimal(animalId).AdoptionStatus})")
                    .AddChoices(adoptionStatuses.ToArray()));

        if(newAdoptionStatus != "Return to Edit Animal Menu"){

            data.ChangeAdoptionStatus(animalId, newAdoptionStatus);

            if(newAdoptionStatus == "Adopted"){
                AnsiConsole.Clear();
                AddFamilyInfo(animalId);
            }

            AnsiConsole.WriteLine("Adoption Status Successfully Changed");
            Thread.Sleep(1000);

        }
    }

    public void AddFamilyInfo(int animalId){

        AnsiConsole.WriteLine("Add Adopted Family Information");

        AnsiConsole.Write("Enter family name: ");
        string familyName = Console.ReadLine();

        AnsiConsole.Write("Enter family email contact: ");
        string email = Console.ReadLine();

        AnsiConsole.Write("Enter family phone number contact: ");
        string phoneNumber = Console.ReadLine();

        AnsiConsole.Write("Enter family address: ");
        string address = Console.ReadLine();

        data.AddFamilyInfo(animalId, familyName, email, phoneNumber, address);

        AnsiConsole.WriteLine("Family Contact Info Successfully Added");
    }

    public void UpdateVaccineInfo(int animalId){
        
        AnsiConsole.WriteLine("Update Animal's Vaccine Information");
        bool addVaccine = true;

        while(addVaccine){

            AnsiConsole.Write("Enter vaccine name: ");
            string vaccineName = Console.ReadLine();

            AnsiConsole.Write("Enter date given (mm/dd/yyyy format): ");
            string dateGiven = Console.ReadLine();

            data.AddVaccine(animalId, vaccineName, dateGiven);

            addVaccine = AnsiConsole.Prompt(
                        new ConfirmationPrompt("Vaccine added! Add another?"));
            
        }
    }

    public List<string> AskForFilters(){

        AnsiConsole.Write("Enter search filter (leave blank for no filter): ");
        string searchFilter = Console.ReadLine();

        string adoptionStatusFilter = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Select adoption status:")
                        .AddChoices("Any", "Not Ready", "Ready", "Adopted"));

        return new List<String>() {searchFilter, adoptionStatusFilter};
    }

    public void ViewAppointments(){

        string mode = "";

        while(mode != "Return to menu"){

            AnsiConsole.WriteLine("View Upcoming Appointments");

            Table appointments = new Table();
            appointments.AddColumn("Animal");
            appointments.AddColumn("Appointment Type");
            appointments.AddColumn("Vet Name");
            appointments.AddColumn("Vet Address");
            appointments.AddColumn("Date");

            data.AddSortedUpcomingAppointmentsToTable(appointments);

            AnsiConsole.Write(appointments);

            mode = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .AddChoices(new[] {
                                        "Add new appointment", "Return to menu"
                                    }));

            if(mode == "Add new appointment"){

                AnsiConsole.Clear();
                ChooseAnimal();

            }

            AnsiConsole.Clear();
        }
    }

    public void ChooseAnimal(){

        AnsiConsole.WriteLine($"Add New Appointment");

        string selectedAnimal = AnsiConsole.Prompt(
                                new SelectionPrompt<string>()
                                    .Title("Select an animal:")
                                    .AddChoices(data.ReturnAnimals()));

        if(selectedAnimal != "Return to upcoming appointments"){

            string selectedAnimalId = selectedAnimal.Split("ID: ")[1];
            selectedAnimalId = selectedAnimalId.Remove(selectedAnimalId.IndexOf(")"));

            AnsiConsole.Clear();
            AddAppointment(int.Parse(selectedAnimalId));

        }
    }

    public void AddAppointment(int animalId){

        AnsiConsole.WriteLine($"Add New Appointment");
        AnsiConsole.WriteLine($"Animal: {data.GetAnimal(animalId).Name} (ID: {data.GetAnimal(animalId).ID})");

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

        data.AddAppointment(animalId, appointmentType, vetName, vetAddress, appointmentDate);

        AnsiConsole.WriteLine("Appointment Successfully Added");
        Thread.Sleep(1000);
    }
}