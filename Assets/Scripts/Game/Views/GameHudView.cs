using UnityEngine;
using UnityEngine.Serialization;

public class GameHudView : MonoBehaviour
{
    /// <summary>キャラクターのステータスView</summary>
    [FormerlySerializedAs("_uiCharacterStatusManager")]
    [SerializeField] private CharacterStatusView[] _characterStatusViews;

    /// <summary>スコアView</summary>
    [FormerlySerializedAs("_uiScoreManager")]
    [SerializeField] private ScoreView _scoreView;

    /// <summary>ボスのステータスView</summary>
    [FormerlySerializedAs("_uiBossStatusManager")]
    [SerializeField] private BossStatusView _bossStatusView;

    /// <summary>
    /// キャラクターHP表示を更新
    /// </summary>
    public void SetHp(int value, int characterIndex)
    {
        if (characterIndex < 0 || characterIndex >= _characterStatusViews.Length)
        {
            Debug.LogWarning($"Character status UI index is out of range. index: {characterIndex}");
            return;
        }

        if (_characterStatusViews[characterIndex] == null)
        {
            Debug.LogWarning($"Character status UI is not assigned. index: {characterIndex}");
            return;
        }

        _characterStatusViews[characterIndex].SetHpValue(value);
    }

    /// <summary>
    /// スコア表示を加算更新
    /// </summary>
    public void AddScore(int value)
    {
        if (_scoreView == null)
        {
            Debug.LogWarning("ScoreView is not assigned.");
            return;
        }

        _scoreView.AddScore(value);
    }

    /// <summary>
    /// ボスHP表示を変更
    /// </summary>
    public void AddBossHp(float value)
    {
        if (_bossStatusView == null)
        {
            Debug.LogWarning("BossStatusView is not assigned.");
            return;
        }

        var currentBossHp = _bossStatusView.GetHpValue();
        currentBossHp += value;
        _bossStatusView.SetHpValue(currentBossHp);
    }
}



