namespace AnimalShelter.Tests;

public class DataHandlerTests : IDisposable
{
    public DataHandler data;
    public DataHandlerTests(){
        SetupFiles();
        this.data = new DataHandler();
    }

    public void Dispose(){
        File.Delete("animals.txt");
        File.Delete("appointments.txt");
        File.Delete("vaccines.txt");
    }

    public void SetupFiles(){

        string initialAnimals = $"Ace,Dog,3,Playful,Not Ready,04/06/2025{Environment.NewLine}" +
                                $"Beatrice,Cat,2,Shy,Not Ready,04/06/2025{Environment.NewLine}" +
                                $"Casper,Cat,3,Smart,Not Ready,04/07/2025{Environment.NewLine}";

        File.WriteAllText("animals.txt", initialAnimals);

        string initialAppointments = $"0,Vaccine,Aaron Beasley,123 Street Lane,04/07/2025{Environment.NewLine}" +
                                     $"1,Vaccine,Aaron Beasley,123 Street Lane,04/07/2025{Environment.NewLine}" + 
                                     $"2,Vaccine,Bailey Cruz,234 Place Drive,04/08/2025{Environment.NewLine}" + 
                                     $"0,Checkup,Aaron Beasley,123 Street Lane,05/25/2025{Environment.NewLine}" + 
                                     $"1,Checkup,Aaron Beasley,123 Street Lane,05/27/2025{Environment.NewLine}";
        
        File.WriteAllText("appointments.txt", initialAppointments);

        string initialVaccines = $"0,Vaccine 1,04/07/2025{Environment.NewLine}" +
                                 $"0,Vaccine 2,04/07/2025{Environment.NewLine}" +
                                 $"1,Vaccine 1,04/07/2025{Environment.NewLine}" +
                                 $"2,Vaccine 1,04/08/2025{Environment.NewLine}";

        File.WriteAllText("vaccines.txt", initialVaccines);

    }

    [Fact]
    public void LoadSavedAnimalsTest()
    {
        //Check the data handler has read the 3 animals from animals.txt
        Assert.Equal(3, data.ReturnAnimals().Count() - 1);      

    }    

    [Fact]
    public void AddAnimalTest()
    {
        //Data handler initially has 3 animals
        Assert.Equal(3, data.ReturnAnimals().Count() - 1);

        string animalName = "Dante"; 
        string animalType = "Dog";
        int animalAge = 3;
        string animalPersonality = "Emptyheaded";
        string animalAdoptionStatus = "Not Ready";
        List<Vaccine> animalVaccines = [new Vaccine("Vaccine 1", DateTime.Parse("04/10/2025")), new Vaccine("Vaccine 2", DateTime.Parse("04/11/2025"))];
        List<Appointment> animalPreviousAppointments = [new Appointment("Vaccine", DateTime.Parse("04/10/2025"), new Vet("Cody Daniels", "345 Road Way")), new Appointment("Vaccine", DateTime.Parse("04/11/2025"), new Vet("Cody Daniels", "345 Road Way"))];

        data.AddAnimal(animalName, animalType, animalAge, animalPersonality, animalAdoptionStatus, animalVaccines, animalPreviousAppointments);

        //Check data handler now has 4 animals
        Assert.Equal(4, data.ReturnAnimals().Count() - 1);

        //Check animal with id 3 is our new animal
        Assert.Equal("Dante", data.GetAnimal(3).Name);       

    }

    [Fact]
    public void AddAppointmentTest()
    {   
        //Animal with id 2 should initially have no upcoming appointments
        Assert.Empty(data.GetAnimal(2).UpcomingAppointments);

        int animalId = 2;
        string appointmentType = "Checkup";
        string vetName = "Bailey Cruz";
        string vetAddress = "234 Place Drive";
        string appointmentDate = "05/28/2025";

        data.AddAppointment(animalId, appointmentType, vetName, vetAddress, appointmentDate);

        //Animal with id 2 should now have 1 upcoming appointment
        Assert.Single(data.GetAnimal(2).UpcomingAppointments);      

    }    

    [Fact]
    public void AddVaccineTest()
    {
        //Animal with id 1 should have a single vaccine
        Assert.Single(data.GetAnimal(1).MedicalHistory.Vaccines);

        int animalId = 1; 
        string vaccineName = "Vaccine 2";
        string dateGiven = "04/15/2025";

        data.AddVaccine(animalId, vaccineName, dateGiven);

        //Animal with id 1 should now have 2 vaccines
        Assert.Equal(2, data.GetAnimal(1).MedicalHistory.Vaccines.Count());       

    }    

    [Fact]
    public void ChangeAnimalAdoptionStatusTest()
    {
        //Animal with id 0 should have an adoption status of Not Ready
        Assert.Equal("Not Ready", data.GetAnimal(0).AdoptionStatus);

        data.ChangeAdoptionStatus(0, "Ready");

        //Animal with id 0 should now have an adoption status of Ready
        Assert.Equal("Ready", data.GetAnimal(0).AdoptionStatus);

        data.ChangeAdoptionStatus(0, "Adopted");

        //Animal with id 0 should now have an adoption status of Adopted
        Assert.Equal("Adopted", data.GetAnimal(0).AdoptionStatus);     

    }

    [Fact]
    public void AddFamilyInfoTest()
    {
        //Animal with id 0 should have no family info (FamilyInfo object strings are all set to empty)
        Assert.Equal("", data.GetAnimal(0).FamilyInfo.Name);

        data.AddFamilyInfo(0, "Smith", "smith@email.com", "111-111-1111", "111 One Way");

        //Animal with id 0 should now have family info, with the family name being Smith
        Assert.Equal("Smith", data.GetAnimal(0).FamilyInfo.Name); 

    }        
}