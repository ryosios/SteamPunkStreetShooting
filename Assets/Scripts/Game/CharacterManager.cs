using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Spine;

public class CharacterManager : MonoBehaviour
{

    /// <summary>hp</summary>
    private int _hp = 3;

    /// <summary>最大HP</summary>
    private const int MaxHp = 3;

    /// <summary>攻撃力</summary>
    [SerializeField] private float _power = 0.1f;

    /// <summary>攻撃力</summary>
    public float Power => _power;

    /// <summary>キャラのアタッチポイント</summary>
    [SerializeField] private Transform _characterAttachPoint;
    public Transform CharacterAttachPoint => _characterAttachPoint;

    /// <summary>ワールドのアタッチポイント</summary>
    [SerializeField] private Transform _worldAttachPoint;
    public Transform WorldAttachPoint => _worldAttachPoint;

    /// <summary>アビリティ</summary>
    [SerializeField] private CharacterAbilityBase[] _characterAbilityBase;

    /// <summary>
    /// hpを加算する
    /// </summary>
    public CharacterManager AddHp(int value)
    {
        _hp = Mathf.Clamp(_hp + value, 0, MaxHp);
        return this;
    }

    /// <summary>
    /// UIのHpを設定する
    /// </summary>
    public CharacterManager SetHpView(UiDataManager uiDataManager,int index)
    {
        uiDataManager.SetHp(_hp,index);
        return this;
    }

    /// <summary>
    /// アビリティを使用する
    /// </summary>
    public void SetAbility(CharacterAbilityBase ability)
    {
        ability.ApplyAbility(this);
    }
}
