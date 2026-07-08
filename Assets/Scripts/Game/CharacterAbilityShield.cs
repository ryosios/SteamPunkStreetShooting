using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu]
public class CharacterAbilityShield : CharacterAbilityBase
{
    [SerializeField] private SphereCollider _shieldCollider;

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="character"> キャラクター </param>
    public override Transform ApplyAbility(CharacterManager character) 
    {
        var abilityTrans =  Instantiate(_shieldCollider.transform,character.transform) as Transform;
        abilityTrans.gameObject.SetActive(true);

        return abilityTrans;
    
    }

}
