using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossStatusView : MonoBehaviour
{
    /// <summary>ボスHP Slider</summary>
    [SerializeField] private Slider _hpSlider;

    /// <summary>ボス名テキスト</summary>
    [SerializeField] private TextMeshProUGUI _nameText;

    private float _currentHpValue = 1f;

    private void Awake()
    {
        SetHpValue(_currentHpValue);
    }

    /// <summary>
    /// スライダー値を取得
    /// </summary>
    public float GetHpValue()
    {
        return _currentHpValue;
    }

    /// <summary>
    /// HP表示値を設定
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
    /// ボス名を設定
    /// </summary>
    public void SetBossName(string name)
    {
        if (_nameText != null)
        {
            _nameText.text = name;
        }
    }
}

