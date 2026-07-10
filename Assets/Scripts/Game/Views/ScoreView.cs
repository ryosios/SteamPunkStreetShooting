using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    /// <summary>スコアのテキスト</summary>
    [SerializeField] private TextMeshProUGUI _scoreText;

    /// <summary>スコアModel</summary>
    [SerializeField] private ScoreModel _model = new ScoreModel();

    /// <summary>
    /// スコアを加算して表示を更新
    /// </summary>
    public void AddScore(int value)
    {
        SetScoreText(_model.AddScore(value));
    }

    /// <summary>
    /// 現在のスコアを取得
    /// </summary>
    public int GetScore()
    {
        return _model.Score;
    }

    /// <summary>
    /// テキストにスコアをセット
    /// </summary>
    public void SetScoreText(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = score.ToString("D10");
        }
    }
}

