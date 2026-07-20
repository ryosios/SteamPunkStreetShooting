using UnityEngine;

[CreateAssetMenu(menuName = "Character Ability/Power Buff")]
public class CharacterAbilityPowerBuff : CharacterAbilityBase
{
    /// <summary>生成するバフRuntime用Prefab。未設定なら空のGameObjectを生成する</summary>
    [SerializeField] private GameObject _buffRuntimePrefab;

    /// <summary>攻撃力への加算補正</summary>
    [SerializeField] private float _powerBonus;

    /// <summary>攻撃力への倍率補正</summary>
    [SerializeField] private float _powerMultiplier = 1f;

    /// <summary>
    /// 攻撃力バフを適用
    /// </summary>
    /// <param name="context">アビリティ実行に必要なキャラクター側の参照情報。</param>
    public override Transform ApplyAbility(CharacterAbilityContext context)
    {
        if (context.CharacterModel == null)
        {
            Debug.LogWarning("CharacterModel is not assigned.");
            return null;
        }

        context.CharacterModel.AddPowerBonus(_powerBonus);
        context.CharacterModel.AddPowerMultiplier(_powerMultiplier);

        Transform abilityTrans = CreateRuntimeTransform(context);
        RegisterAbilityRuntime(abilityTrans, context);
        return abilityTrans;
    }

    /// <summary>
    /// 攻撃力バフを解除
    /// </summary>
    /// <param name="context">アビリティ実行に必要なキャラクター側の参照情報。</param>
    public override void OnAbilityDestroyed(CharacterAbilityContext context)
    {
        if (context.CharacterModel == null)
        {
            return;
        }

        context.CharacterModel.RemovePowerMultiplier(_powerMultiplier);
        context.CharacterModel.RemovePowerBonus(_powerBonus);
    }

    /// <summary>
    /// バフ解除の基準になるRuntime Transformを生成
    /// </summary>
    /// <param name="context">アビリティ実行に必要なキャラクター側の参照情報。</param>
    private Transform CreateRuntimeTransform(CharacterAbilityContext context)
    {
        if (_buffRuntimePrefab != null)
        {
            GameObject runtimeObject = Instantiate(_buffRuntimePrefab, context.OwnerTransform);
            runtimeObject.SetActive(true);
            return runtimeObject.transform;
        }

        GameObject abilityObject = new GameObject(name);
        abilityObject.transform.SetParent(context.OwnerTransform);
        abilityObject.transform.localPosition = Vector3.zero;
        abilityObject.transform.localRotation = Quaternion.identity;
        abilityObject.transform.localScale = Vector3.one;
        return abilityObject.transform;
    }
}
