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
    /// <param name="character"> キャラクター </param>
    public override Transform ApplyAbility(CharacterManager character) 
    {
        if (character == null)
        {
            Debug.LogWarning("Ability target character is not assigned.");
            return null;
        }

        if (_shieldCollider == null)
        {
            Debug.LogWarning("Shield ability collider is not assigned.");
            return null;
        }

        var abilityTrans = Instantiate(_shieldCollider.transform, character.transform);
        abilityTrans.gameObject.SetActive(true);

        DestroyAfterLifeTime(abilityTrans);
        return abilityTrans;
    }
}

