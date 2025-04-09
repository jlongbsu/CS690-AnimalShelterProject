namespace AnimalShelter;

public class MedicalHistory{
    public List<Appointment> PreviousAppointments {get; private set;}
    public List<Vaccine> Vaccines {get; private set;}
    public MedicalHistory(List<Appointment> previousAppointments, List<Vaccine> vaccines){
        this.PreviousAppointments = previousAppointments;
        this.Vaccines = vaccines;
    }

    public void SortPreviousAppointments(){
        this.PreviousAppointments = this.PreviousAppointments.OrderBy(appointment => appointment.Date).ThenBy(appointment => appointment.Type).ThenBy(appointment => appointment.Vet.Name).ToList();
    }

    public void SortVaccines(){
        this.Vaccines = this.Vaccines.OrderBy(vaccine => vaccine.DateGiven).ThenBy(vaccine => vaccine.Type).ToList();
    }
}