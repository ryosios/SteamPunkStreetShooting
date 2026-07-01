using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class UiBossStatusManager : MonoBehaviour
{
    /// <summary>ボスのHpSlider</summary>
    [SerializeField] private Slider _hpSlider;

    /// <summary>ボスの名前のテキスト</summary>
    [SerializeField] private TextMeshProUGUI _nameText;

    private int _currentHpValue = 0;

    /// <summary>
    /// ボスのHpを取得
    /// </summary>
    public int GetHpValue()
    {
        return _currentHpValue;
    }

    /// <summary>
    /// UIのHpを設定
    /// </summary>
    public void SetHpValue(int hpValue)
    {
        _currentHpValue = hpValue;
        _hpSlider.value = _currentHpValue;
    }

    /// <summary>
    /// ボスの名前を設定
    /// </summary>
    public void SetBossName(string name)
    {

    }



    /// <summary>
    /// キャラクターのアイコンを設定
    /// </summary>
    private void SetFaceIcon()
    {
       
    }


}
