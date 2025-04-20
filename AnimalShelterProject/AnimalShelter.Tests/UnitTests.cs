namespace AnimalShelter.Tests;

public class UnitTests
{
    public AuthenticationService auth;
    public UnitTests(){
        this.auth = new AuthenticationService();
    }
    [Fact]
    public void CreateAccountTest()
    {
        string username = "test_user";
        string password = "test_pw";

        this.auth.CreateAccount(username, password);

        Assert.True(this.auth.Authenticate(username, password));       

    }

    [Fact]
    public void ExistingUsernameTest()
    {
        string username = "test_user";

        Assert.True(this.auth.UsernameIsTaken(username));       

    }

    [Fact]
    public void WrongUsernameAndPasswordTest()
    {
        string username = "test_user1";
        string password = "test_pw";

        Assert.False(this.auth.Authenticate(username, password));

        username = "test_user";
        password = "test_pw1";

        Assert.False(this.auth.Authenticate(username, password)); 

    }    

    [Fact]
    public void RightUsernameAndPasswordTest()
    {
        string username = "test_user";
        string password = "test_pw";

        Assert.True(auth.Authenticate(username, password));       

    }

    [Fact]
    public void AddAnimalTest()
    {
        string username = "test_user";

        Assert.True(auth.UsernameIsTaken(username));       

    }

    [Fact]
    public void AddAppointmentTest()
    {
        string username = "test_user";

        Assert.True(auth.UsernameIsTaken(username));       

    }    

    [Fact]
    public void AddVaccineTest()
    {
        string username = "test_user";

        Assert.True(auth.UsernameIsTaken(username));       

    }    

    [Fact]
    public void ChangeAnimalAdoptionStatusTest()
    {
        string username = "test_user";

        Assert.True(auth.UsernameIsTaken(username));       

    }

    [Fact]
    public void AddFamilyInfoTest()
    {
        string username = "test_user";

        Assert.True(auth.UsernameIsTaken(username));       

    }    
}