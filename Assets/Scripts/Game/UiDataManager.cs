using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class UiDataManager : MonoBehaviour
{
    //ゲームデータの受け渡し等

    /// <summary>キャラクターのステータスUI</summary>
    [SerializeField] private UiCharacterStatusManager[] _uiCharacterStatusManager;

    /// <summary>スコアUI</summary>
    [SerializeField] private UiScoreManager _uiScoreManager;

    /// <summary>
    /// UIのHpを直接設定
    /// </summary>
    public void SetHp(int value ,int characterIndex)
    {
        _uiCharacterStatusManager[characterIndex].SetHpValue(value);

    }

    /// <summary>
    /// UIのスコアを加算
    /// </summary>
    public void AddScore(int value)
    {
        var currentScore = _uiScoreManager.GetScore();
        currentScore += value;
        _uiScoreManager.SetScoreText(currentScore);       

    }

}
