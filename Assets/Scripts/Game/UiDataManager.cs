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

    /// <summary>ボスのステータスUI</summary>
    [SerializeField] private UiBossStatusManager _uiBossStatusManager;

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

    /// <summary>
    /// ボスのHpを変更
    /// </summary>
    public void AddBossHp(int value)
    {
        var currentBossHp = _uiBossStatusManager.GetHpValue();
        currentBossHp -= value;
        _uiBossStatusManager.SetHpValue(currentBossHp);

    }

}
