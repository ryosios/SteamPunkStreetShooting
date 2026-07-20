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
    /// <param name="context">アビリティ実行に必要なキャラクター側の参照情報。</param>
    public override Transform ApplyAbility(CharacterAbilityContext context)
    {
        if (context.CharacterAttachPoint == null)
        {
            Debug.LogWarning("Character attach point is not assigned.");
            return null;
        }

        if (_attackParticle == null)
        {
            Debug.LogWarning("Attack ability particle is not assigned.");
            return null;
        }

        var abilityTrans = Instantiate(_attackParticle.transform, context.CharacterAttachPoint);
        abilityTrans.gameObject.SetActive(true);
        var abilityParticle = abilityTrans.GetComponent<ParticleSystem>();
        if (abilityParticle != null)
        {
            abilityParticle.Play();
        }

        RegisterAbilityRuntime(abilityTrans, context);
        return abilityTrans;
    }
}


