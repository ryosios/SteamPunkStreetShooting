/// <summary>
/// スコアの状態を持つModel。
/// </summary>
[System.Serializable]
public class ScoreModel
{
    public int Score { get; private set; }

    /// <summary>
    /// スコアを加算
    /// </summary>
    public int AddScore(int value)
    {
        Score += value;
        return Score;
    }
}

