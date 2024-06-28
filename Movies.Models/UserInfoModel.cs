namespace Movies.Models;

public class UserInfoModel
{
    public Dictionary<string, string> UserInfoDict { get; private set; } = null;
    public UserInfoModel(Dictionary<string, string> userInfoDict)
    {
        UserInfoDict = userInfoDict;
    }
}