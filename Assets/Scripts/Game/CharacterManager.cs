using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Spine;

public class CharacterManager : MonoBehaviour
{

    /// <summary>Id</summary>
    [SerializeField]private int _id;
    public int ID => _id;

    /// <summary>hp</summary>
    private int _hp = 3;
    
    /// <summary>
    /// hp‚ğ‰ÁZ‚·‚é
    /// </summary>
    public CharacterManager AddHp(int value)
    {
        if(_hp > 0 && _hp <= 3)
        {
            _hp += value;
        }        
        return this;
    }

    /// <summary>
    /// UI‚ÌHp‚ğİ’è‚·‚é
    /// </summary>
    public CharacterManager SetHpView(UiDataManager uiDataManager)
    {
        uiDataManager.SetHp(_hp,_id);
        return this;
    }
}
