namespace AnimalShelter;

public class MedicalHistory{
    public List<Appointment> PreviousAppointments {get;}
    public List<Vaccine> Vaccines {get;}
    public MedicalHistory(List<Appointment> previousAppointments, List<Vaccine> vaccines){
        this.PreviousAppointments = previousAppointments;
        this.Vaccines = vaccines;
    }
}