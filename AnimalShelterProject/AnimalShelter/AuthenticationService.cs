namespace AnimalShelter;

public class AuthenticationService{

    private string accountsFilePath = "accounts.txt";
    private Dictionary<string, string> accounts = new Dictionary<string, string>();
    public AuthenticationService(){
        if(File.Exists(this.accountsFilePath)){
            foreach(var line in File.ReadLines(accountsFilePath)){
                string[] usernameAndPassword = line.Split(",");
                this.accounts[usernameAndPassword[0]] = usernameAndPassword[1];
            }
        }
    }

    public bool Authenticate(string username, string password){
        return accounts[username] == password;
    }

    public void CreateAccount(string username, string password){
        accounts[username] = password;
        File.AppendAllText(this.accountsFilePath, $"{username},{password}{Environment.NewLine}");
    }

    public bool UsernameIsTaken(string username){
        return accounts.ContainsKey(username);
    }
}