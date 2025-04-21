namespace AnimalShelter.Tests;

public class AccountTests : IDisposable
{
    public AuthenticationService auth;
    public AccountTests(){
        File.WriteAllText("accounts.txt", $"user1,pass1{Environment.NewLine}");
        this.auth = new AuthenticationService();
    }

    public void Dispose(){
        File.Delete("accounts.txt");
    }

    [Fact]
    public void LoadSavedAccountTest()
    {

        Assert.True(this.auth.Authenticate("user1", "pass1"));       

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
        string username = "user1";

        Assert.True(this.auth.UsernameIsTaken(username));       

    }

    [Fact]
    public void WrongUsernameAndPasswordTest()
    {
        string username = "user2";
        string password = "pass1";

        Assert.False(this.auth.Authenticate(username, password));

        username = "user1";
        password = "pass2";

        Assert.False(this.auth.Authenticate(username, password)); 

        username = "user2";
        password = "pass2";

        Assert.False(this.auth.Authenticate(username, password));

    }    

    [Fact]
    public void RightUsernameAndPasswordTest()
    {
        string username = "user1";
        string password = "pass1";

        Assert.True(auth.Authenticate(username, password));       

    }
}