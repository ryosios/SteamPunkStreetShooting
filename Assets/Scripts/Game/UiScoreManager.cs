using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UiScoreManager : MonoBehaviour
{
    /// <summary>スコアのテキスト</summary>
    [SerializeField] private TextMeshProUGUI _scoreText;

    /// <summary>現在のスコア</summary>
    private int _currentScoreValue = 0;


    //public Subject<Unit> Default = new Subject<Unit>();

    private void Awake()
    {

    }

    /// <summary>
    /// 現在のスコアを取得
    /// </summary>
    public int GetScore()
    {
        return _currentScoreValue;

    }

    /// <summary>
    /// テキストにスコアをセット
    /// </summary>
    public void SetScoreText(int score)
    {
        _currentScoreValue = score;
        _scoreText.text = _currentScoreValue.ToString("D10");

    }


}
