using System.Collections.Generic;

[System.Serializable]
public class ScoreData
{
    public int score;
}

[System.Serializable]
public class UserScore
{
    public string username;
    public int score;
}

[System.Serializable]
public class UserScoresResponse
{
    public List<UserScore> userScores;
}
