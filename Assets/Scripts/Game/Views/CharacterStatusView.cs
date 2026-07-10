using UnityEngine;
using UnityEngine.UI;

public class CharacterStatusView : MonoBehaviour
{
    [SerializeField] private Image[] _hpIconArray;

    [SerializeField] private Image _faceIcon;

    private int _currentHp = 0;

    /// <summary>
    /// HP表示値を設定
    /// </summary>
    public void SetHpValue(int hpValue)
    {
        _currentHp = hpValue;
        SetHpView();
    }

    /// <summary>
    /// HPアイコン表示を更新
    /// </summary>
    private void SetHpView()
    {
        for (int i = 0; i < _hpIconArray.Length; i++)
        {
            _hpIconArray[i].gameObject.SetActive(i < _currentHp);
        }
    }

    /// <summary>
    /// キャラクターアイコンを設定
    /// </summary>
    private void SetFaceIcon()
    {
    }
}

