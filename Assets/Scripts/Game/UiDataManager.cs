using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public class UiDataManager : MonoBehaviour
{
    //ゲームデータの受け渡し等

    [SerializeField] private UiCharacterStatusManager[] _uiCharacterStatusManager;


    /// <summary>
    /// UIのHpを直接設定
    /// </summary>
    public void SetHp(int value ,int characterIndex)
    {
        _uiCharacterStatusManager[characterIndex].SetHpValue(value);

    }

    /// <summary>
    /// UIのスコアを設定
    /// </summary>
    public void SetScore(int value)
    {
       

    }

}
