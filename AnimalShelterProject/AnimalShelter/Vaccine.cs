namespace AnimalShelter;

public class Vaccine{
    public string Type {get;}
    public DateTime DateGiven {get;}
    public Vaccine(string type, DateTime dateGiven){
        this.Type = type;
        this.DateGiven = dateGiven;
    }
}