namespace AnimalShelter;

using System.IO;
using System;
using Spectre.Console;
using Microsoft.VisualBasic;

public class DataHandler{
    private int id = 0;
    private List<Animal> animals = new List<Animal>();
    private Dictionary<string, List<Appointment>> upcomingAppointments = new Dictionary<string, List<Appointment>>();

    public DataHandler(){

    }

    public void SetupData(){

        Dictionary<string, List<Appointment>> previousAppointments = new Dictionary<string, List<Appointment>>();
        string appointmentsFilePath = "appointments.txt";

        //Handle previous and upcoming appointments
        if(File.Exists(appointmentsFilePath)){
            foreach(var line in File.ReadLines(appointmentsFilePath)){
                string[] appointmentInfo = line.Split(",");
                string currentDate = DateTime.Today.ToString("d");

                //If appointment date  is today or later, add it to upcoming appointments
                if(DateTime.Parse(currentDate) <= DateTime.Parse(appointmentInfo[4])){
                
                    //If an animal already has a list of upcoming appointments, add it to the list
                    if(upcomingAppointments.ContainsKey(appointmentInfo[0])){
                        upcomingAppointments[appointmentInfo[0]].Add(new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3])));

                    //If an animal doesn't have an upcoming appointment list, create one and add the appointment
                    }else{
                        upcomingAppointments[appointmentInfo[0]] = new List<Appointment>{new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3]))};
                    }

                //Otherwise, add it to previous appointments
                }else{

                    //If an animal already has a list of previous appointments, add it to the list
                    if(previousAppointments.ContainsKey(appointmentInfo[0])){
                        previousAppointments[appointmentInfo[0]].Add(new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3])));

                    //If an animal doesn't have a previous appointment list, create one and add the appointment 
                    }else{
                        previousAppointments[appointmentInfo[0]] = new List<Appointment>{new Appointment(appointmentInfo[1], DateTime.Parse(appointmentInfo[4]), new Vet(appointmentInfo[2], appointmentInfo[3]))};
                    }
                }
            }
        }

        Dictionary<string, List<Vaccine>> animalsVaccines = new Dictionary<string, List<Vaccine>>();
        string vaccinesFilePath = "vaccines.txt";

        //Handle vaccines
        if(File.Exists(vaccinesFilePath)){
            foreach(var line in File.ReadLines(vaccinesFilePath)){
                string[] vaccineInfo = line.Split(",");

                //If an animal already has a vaccine list, add it to the list
                if(animalsVaccines.ContainsKey(vaccineInfo[0])){
                    animalsVaccines[vaccineInfo[0]].Add(new Vaccine(vaccineInfo[1], DateTime.Parse(vaccineInfo[2])));

                //If an animal doesn't already have a vaccine list, create one and add the vaccine    
                }else{
                    animalsVaccines[vaccineInfo[0]] = new List<Vaccine>{new Vaccine(vaccineInfo[1], DateTime.Parse(vaccineInfo[2]))};
                }
            }
        }

        string animalsFilePath = "animals.txt";

        //Handle animals
        if(File.Exists(animalsFilePath)){
            foreach(var line in File.ReadLines(animalsFilePath)){
                string[] animalInfo = line.Split(",");

                //If an animal has no upcoming appointments, create an empty entry for the global tracker
                if(!upcomingAppointments.ContainsKey(id.ToString())){
                    upcomingAppointments[id.ToString()] = new List<Appointment>();
                }

                MedicalHistory medicalHistory = new MedicalHistory(new List<Appointment>(), new List<Vaccine>());

                //Add previous appointments to animal's medical history and sort them by date
                if(previousAppointments.ContainsKey(id.ToString())){
                    foreach(Appointment appointment in previousAppointments[id.ToString()]){
                        medicalHistory.PreviousAppointments.Add(appointment);
                    }
                    medicalHistory.SortPreviousAppointments();
                }

                //Add vaccines to animal's medical history and sort them by date
                if(animalsVaccines.ContainsKey(id.ToString())){
                    foreach(Vaccine vaccine in animalsVaccines[id.ToString()]){
                        medicalHistory.Vaccines.Add(vaccine);
                    }
                    medicalHistory.SortVaccines();
                }

                //Add new animal
                upcomingAppointments[id.ToString()] = upcomingAppointments[id.ToString()].OrderBy(appointment => appointment.Date).ThenBy(appointment => appointment.Type).ThenBy(appointment => appointment.Vet.Name).ToList();
                animals.Add(new Animal(id, animalInfo[0], animalInfo[1], int.Parse(animalInfo[2]), animalInfo[3], animalInfo[4], DateTime.Parse(animalInfo[5]), medicalHistory, new List<Appointment>(upcomingAppointments[id.ToString()])));
                
                id++;
            }
        }
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

    public void DisplayAnimals(string searchFilter, string adoptionStatusFilter){
        if(searchFilter == "" && adoptionStatusFilter == "Any"){
            
        }
    }
}