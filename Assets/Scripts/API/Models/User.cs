[System.Serializable]
public class User
{
    public string username;
    public string password;
    public string email;
}

[System.Serializable]
public struct LoginData
{
    public string username;
    public string password;
}

[System.Serializable]
public struct RecoverData
{
    public string username;
    public string email;
}

[System.Serializable]
public class PasswordData
{
    public string newPassword;
}

[System.Serializable]
public class TokenResponse
{
    public string token;
}