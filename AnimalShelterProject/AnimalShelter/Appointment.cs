namespace AnimalShelter;

public class Appointment{
    public string Type {get;}
    public DateTime Date {get;}
    public Vet Vet {get;}
    public Appointment(string type, DateTime date, Vet vet){
        this.Type = type;
        this.Date = date;
        this.Vet = vet;
    }
}