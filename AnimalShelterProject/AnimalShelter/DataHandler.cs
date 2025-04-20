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
        SetupData();
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

        Dictionary<string, FamilyInformation> families = new Dictionary<string, FamilyInformation>();
        string familyFilePath = "adopted.txt";

        if(File.Exists(familyFilePath)){
            foreach(var line in File.ReadLines(familyFilePath)){
                string[] familyInfo = line.Split(",");
                families[familyInfo[0]] = new FamilyInformation(familyInfo[1], familyInfo[2], familyInfo[3], familyInfo[4], DateTime.Parse(familyInfo[5]));
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
                
                if(families.ContainsKey(id.ToString())){

                    FamilyInformation fi = families[id.ToString()];
                    animals[id].AddFamilyInfo(fi.Name, fi.Email, fi.PhoneNumber, fi.Address, fi.DateAdopted);

                }

                id++;
            }
        }
    }

    public void AddAnimal(string animalName, string animalType, int animalAge, string animalPersonality, string animalAdoptionStatus, List<Vaccine> animalVaccines, List<Appointment> animalPreviousAppointments){

        File.AppendAllText("animals.txt", $"{animalName},{animalType},{animalAge},{animalPersonality},{animalAdoptionStatus},{DateTime.Today.ToString("d")}{Environment.NewLine}");
        
        MedicalHistory animalMedicalHistory = new MedicalHistory(new List<Appointment>(), new List<Vaccine>());
        
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

    public string[] ReturnAnimals(){
        
        List<string> animalNames = new List<string>();

        for(int i = 0; i < animals.Count; i++){

            animalNames.Add($"{animals[i].Name} (ID: {i})");
            
        }

        animalNames.Add("Return to upcoming appointments");

        return animalNames.ToArray();
    }

    public string[] ReturnFilteredAnimals(string searchFilter, string adoptionStatusFilter){

        List<string> animalNames = new List<string>();

        for(int i = 0; i < animals.Count; i++){

            if(animals[i].Name.Contains(searchFilter) && (adoptionStatusFilter == "Any" || adoptionStatusFilter == animals[i].AdoptionStatus)){
                animalNames.Add($"{animals[i].Name} (ID: {i})");
            }
        }

        animalNames.Add("Change search filters");
        animalNames.Add("Return to menu");

        return animalNames.ToArray();
    }

    public Animal GetAnimal(int animalId){

        return animals[animalId];
    }

    public void ChangeAdoptionStatus(int animalId, string newAdoptionStatus){
        animals[animalId].AdoptionStatus = newAdoptionStatus;
        
        string[] arrLine = File.ReadAllLines("animals.txt");
        arrLine[animalId] = $"{animals[animalId].Name},{animals[animalId].Type},{animals[animalId].Age},{animals[animalId].Personality},{animals[animalId].AdoptionStatus},{animals[animalId].DateAddedToShelter.ToString("d")}";
        File.WriteAllLines("animals.txt", arrLine);
    }

    public void AddVaccine(int animalId, string vaccineName, string dateGiven){

        animals[animalId].MedicalHistory.Vaccines.Add(new Vaccine(vaccineName, DateTime.Parse(dateGiven)));
        animals[animalId].MedicalHistory.SortVaccines();
        File.AppendAllText("vaccines.txt", $"{animalId},{vaccineName},{dateGiven}{Environment.NewLine}");

    }

    public void AddAppointment(int animalId, string appointmentType, string vetName, string vetAddress, string appointmentDate){

        File.AppendAllText("appointments.txt", $"{animalId},{appointmentType},{vetName},{vetAddress},{appointmentDate}{Environment.NewLine}");

        animals[animalId].UpcomingAppointments.Add(new Appointment(appointmentType, DateTime.Parse(appointmentDate), new Vet(vetName, vetAddress)));
        animals[animalId].SortUpcomingAppointments();

        upcomingAppointments[animalId.ToString()] = new List<Appointment>(animals[animalId].UpcomingAppointments);
    }

    public void AddSortedUpcomingAppointmentsToTable(Table table){

        List<string> results = new List<string>();

        var sortedUpcomingAppointments = upcomingAppointments.SelectMany(kv => kv.Value.Select(appointment => new {AnimalId = kv.Key, Appointment = appointment})).OrderBy(entry => entry.Appointment.Date).ThenBy(entry => int.Parse(entry.AnimalId)).ThenBy(entry => entry.Appointment.Type).ThenBy(entry => entry.Appointment.Vet.Name).ToList();
        
        foreach(var animal in sortedUpcomingAppointments){

            table.AddRow($"{animals[int.Parse(animal.AnimalId)].Name} (ID: {animal.AnimalId})", animal.Appointment.Type, animal.Appointment.Vet.Name, animal.Appointment.Vet.Address, animal.Appointment.Date.ToString("d"));
        
        }
    }

    public void AddFamilyInfo(int animalId, string name, string email, string phoneNumber, string address){

        animals[animalId].AddFamilyInfo(name, email, phoneNumber, address, DateTime.Today);
        File.AppendAllText("adopted.txt", $"{animalId},{name},{email},{phoneNumber},{address},{animals[animalId].FamilyInfo.DateAdopted.ToString("d")}{Environment.NewLine}");

    }
}