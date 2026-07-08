using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu]
public class CharacterAbilityAttack : CharacterAbilityBase
{
    [SerializeField] private ParticleSystem _attackParticle;

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="character"> キャラクター </param>
    public override Transform ApplyAbility(CharacterManager character) 
    {
        var abilityTrans =  Instantiate(_attackParticle.transform,character.CharacterAttachPoint) as Transform;
        abilityTrans.gameObject.SetActive(true);
        var abilityParticle = abilityTrans.GetComponent<ParticleSystem>();
        abilityParticle.Play();

        return abilityTrans;
    
    }

}
