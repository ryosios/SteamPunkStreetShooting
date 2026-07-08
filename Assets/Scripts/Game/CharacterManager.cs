using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Spine;
using System.Collections.Generic;

public class CharacterManager : MonoBehaviour
{
    /// <summary>現在HP</summary>
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

    /// <summary>キャラクターが使用できるアビリティ</summary>
    [SerializeField] private CharacterAbilityBase[] _characterAbilityBase;

    /// <summary>現在有効になっているアビリティのインスタンス</summary>
    private readonly List<Transform> _activeAbilityTransList = new();

    /// <summary>
    /// HPを加算
    /// </summary>
    public CharacterManager AddHp(int value)
    {
        _hp = Mathf.Clamp(_hp + value, 0, MaxHp);
        return this;
    }

    /// <summary>
    /// UIのHP表示を更新
    /// </summary>
    public CharacterManager SetHpView(UiDataManager uiDataManager, int index)
    {
        uiDataManager.SetHp(_hp, index);
        return this;
    }

    /// <summary>
    /// 指定したアビリティを使用
    /// </summary>
    public Transform UseAbility(int abilityIndex = 0)
    {
        if (_characterAbilityBase == null || abilityIndex < 0 || abilityIndex >= _characterAbilityBase.Length)
        {
            Debug.LogWarning($"Character ability index is out of range. index: {abilityIndex}");
            return null;
        }

        return SetAbility(_characterAbilityBase[abilityIndex]);
    }

    /// <summary>
    /// 登録されている全てのアビリティを使用
    /// </summary>
    public void UseAllAbilities()
    {
        if (_characterAbilityBase == null || _characterAbilityBase.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < _characterAbilityBase.Length; i++)
        {
            SetAbility(_characterAbilityBase[i]);
        }
    }

    /// <summary>
    /// アビリティを使用
    /// </summary>
    public Transform SetAbility(CharacterAbilityBase ability)
    {
        if (ability == null)
        {
            Debug.LogWarning("Character ability is not assigned.");
            return null;
        }

        Transform abilityTrans = ability.ApplyAbility(this);
        if (abilityTrans != null)
        {
            _activeAbilityTransList.Add(abilityTrans);
        }

        return abilityTrans;
    }

    /// <summary>
    /// 現在有効なアビリティを全て停止
    /// </summary>
    public void StopAllAbilities()
    {
        for (int i = _activeAbilityTransList.Count - 1; i >= 0; i--)
        {
            Transform abilityTrans = _activeAbilityTransList[i];
            if (abilityTrans != null)
            {
                Destroy(abilityTrans.gameObject);
            }

            _activeAbilityTransList.RemoveAt(i);
        }
    }
}
