namespace AnimalShelter;

public class Animal{
    public int ID {get;}
    public string Name {get;}
    public string Type {get;}
    public int Age {get;}
    public string Personality {get;}
    public string AdoptionStatus {get; set;}
    public DateTime DateAddedToShelter {get;}
    public MedicalHistory MedicalHistory {get;}
    public List<Appointment> UpcomingAppointments {get; private set;}
    public Animal(int id, string name, string type, int age, string personality, string adoptionStatus, DateTime dateAddedToShelter, MedicalHistory medicalHistory, List<Appointment> upcomingAppointments){
        this.ID = id;
        this.Name = name;
        this.Type = type;
        this.Age = age;
        this.Personality = personality;
        this.AdoptionStatus = adoptionStatus;
        this.DateAddedToShelter = dateAddedToShelter;
        this.MedicalHistory = medicalHistory;
        this.UpcomingAppointments = upcomingAppointments;
    }

    public void SortUpcomingAppointments(){
        this.UpcomingAppointments = this.UpcomingAppointments.OrderBy(appointment => appointment.Date).ThenBy(appointment => appointment.Type).ThenBy(appointment => appointment.Vet.Name).ToList();
    }
}