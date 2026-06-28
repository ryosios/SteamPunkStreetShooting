using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class UiCharacterStatusManager : MonoBehaviour
{
    [SerializeField] private Image[] _hpIconArray;

    [SerializeField] private Image _faceIcon;

    private int _currentHp = 0;

    /// <summary>
    /// UIのHpを設定
    /// </summary>
    public void SetHpValue(int hpValue)
    {
        _currentHp = hpValue;
        SetHpView();
    }

    /// <summary>
    /// Hpのアイコン表示を設定
    /// </summary>
    private void SetHpView()
    {
        for (int i = 0; i < _hpIconArray.Length; i++)
        {
            _hpIconArray[i].gameObject.SetActive(false);
            if(i < _currentHp)
            {
                _hpIconArray[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// キャラクターのアイコンを設定
    /// </summary>
    private void SetFaceIcon()
    {
       
    }


}
