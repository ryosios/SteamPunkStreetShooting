using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;
using Spine;
using System.Collections.Generic;

public class CharacterPresenter : MonoBehaviour
{
    /// <summary>キャラクターの状態Model</summary>
    [SerializeField] private CharacterModel _model = new CharacterModel();

    /// <summary>攻撃力</summary>
    public float Power => _model.Power;

    /// <summary>現在HP</summary>
    public int Hp => _model.Hp;

    /// <summary>キャラのアタッチポイント</summary>
    [SerializeField] private Transform _characterAttachPoint;
    public Transform CharacterAttachPoint => _characterAttachPoint;

    /// <summary>ワールドのアタッチポイント</summary>
    private Transform _worldAttachPoint;
    public Transform WorldAttachPoint => _worldAttachPoint;

    /// <summary>キャラクターが使用できるアビリティ</summary>
    [SerializeField] private CharacterAbilityBase[] _characterAbilityBase;

    /// <summary>現在有効になっているアビリティのインスタンス</summary>
    private readonly List<Transform> _activeAbilityTransList = new();

    /// <summary>キャラ切り替えで非アクティブやアクティブにするオブジェクト</summary>
    [SerializeField] private GameObject _activateObject;

    private void Awake()
    {
        //ワールドアタッチポイント取得
        GameObject attachPointObject = GameObject.FindWithTag("AttachPoint");
        if (attachPointObject != null)
        {
            _worldAttachPoint = attachPointObject.transform;
        }
    }

    /// <summary>
    /// キャラ画像やコリジョンを含む表示用オブジェクトを切り替え
    /// </summary>
    public void SetActivateObjectActive(bool isActive)
    {
        if (_activateObject == null)
        {
            Debug.LogWarning("Activate object is not assigned.");
            return;
        }

        _activateObject.SetActive(isActive);
    }

    /// <summary>
    /// HPを加算
    /// </summary>
    public CharacterPresenter AddHp(int value)
    {
        _model.AddHp(value);
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

        CleanupDestroyedAbilities();

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

    /// <summary>
    /// Destroy済みのアビリティ参照をリストから削除
    /// </summary>
    private void CleanupDestroyedAbilities()
    {
        for (int i = _activeAbilityTransList.Count - 1; i >= 0; i--)
        {
            if (_activeAbilityTransList[i] == null)
            {
                _activeAbilityTransList.RemoveAt(i);
            }
        }
    }
}

