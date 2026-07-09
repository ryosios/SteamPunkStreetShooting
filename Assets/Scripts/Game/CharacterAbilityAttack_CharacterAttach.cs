using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

[CreateAssetMenu(menuName = "Character Ability/Attack_CharacterAttach")]
public class CharacterAbilityAttack_CharacterAttach : CharacterAbilityBase
{
    /// <summary>攻撃用パーティクルPrefab</summary>
    [SerializeField] private ParticleSystem _attackParticle;

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="character"> キャラクター </param>
    public override Transform ApplyAbility(CharacterManager character) 
    {
        if (character == null || character.CharacterAttachPoint == null)
        {
            Debug.LogWarning("Ability target character or attach point is not assigned.");
            return null;
        }

        if (_attackParticle == null)
        {
            Debug.LogWarning("Attack ability particle is not assigned.");
            return null;
        }

        var abilityTrans = Instantiate(_attackParticle.transform, character.CharacterAttachPoint);
        abilityTrans.gameObject.SetActive(true);
        var abilityParticle = abilityTrans.GetComponent<ParticleSystem>();
        if (abilityParticle != null)
        {
            abilityParticle.Play();
        }

        DestroyAfterLifeTime(abilityTrans);
        return abilityTrans;
    }
}

