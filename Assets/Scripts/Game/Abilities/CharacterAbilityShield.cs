using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu(menuName = "Character Ability/Shield")]
public class CharacterAbilityShield : CharacterAbilityBase
{
    /// <summary>シールド用コライダーPrefab</summary>
    [SerializeField] private SphereCollider _shieldCollider;

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="context">アビリティ実行に必要なキャラクター側の参照情報。</param>
    public override Transform ApplyAbility(CharacterAbilityContext context)
    {
        if (context.OwnerTransform == null)
        {
            Debug.LogWarning("Ability owner transform is not assigned.");
            return null;
        }

        if (_shieldCollider == null)
        {
            Debug.LogWarning("Shield ability collider is not assigned.");
            return null;
        }

        var abilityTrans = Instantiate(_shieldCollider.transform, context.OwnerTransform);
        abilityTrans.gameObject.SetActive(true);

        RegisterAbilityRuntime(abilityTrans, context);
        return abilityTrans;
    }
}


