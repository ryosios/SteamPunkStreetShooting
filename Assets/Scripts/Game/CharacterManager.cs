using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Spine;

public class CharacterManager : MonoBehaviour
{
    /// <summary>hp</summary>
    private int _hp = 3;
    
    /// <summary>
    /// hp‚ð‰ÁŽZ‚·‚é
    /// </summary>
    public void AddHp(int value)
    {
        _hp += value;
    }
}
