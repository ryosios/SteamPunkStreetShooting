using UnityEngine;
using TMPro;

public class ScoreView : MonoBehaviour
{
    /// <summary>スコアのテキスト</summary>
    [SerializeField] private TextMeshProUGUI _scoreText;

    /// <summary>
    /// テキストにスコアをセット
    /// </summary>
    public void SetScore(int score)
    {
        if (_scoreText != null)
        {
            _scoreText.text = score.ToString("D10");
        }
    }
}

