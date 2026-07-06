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

    private float _currentHpValue = 1;

    private void Awake()
    {
        SetHpValue(_currentHpValue);
    }

    /// <summary>
    /// スライダーの値を取得
    /// </summary>
    public float GetHpValue()
    {
        return _currentHpValue;
    }

    /// <summary>
    /// UIのHpを設定
    /// </summary>
    public void SetHpValue(float hpValue)
    {
        _currentHpValue = Mathf.Clamp01(hpValue);

        if (_hpSlider != null)
        {
            _hpSlider.value = _currentHpValue;
        }
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
