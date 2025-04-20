namespace AnimalShelter;

public class FamilyInformation{
    public string Name {get; private set;}
    public string Email {get; private set;}
    public string PhoneNumber {get; private set;}
    public string Address {get; private set;}
    public DateTime DateAdopted {get; private set;}
    public FamilyInformation(string name, string email, string phoneNumber, string address, DateTime dateAdopted){
        this.Name = name;
        this.Email = email;
        this.PhoneNumber = phoneNumber;
        this.Address = address;
        this.DateAdopted = dateAdopted;
    }
}