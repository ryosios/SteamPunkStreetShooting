using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public abstract class CharacterAbilityBase : ScriptableObject
{
    /// <summary>生成したアビリティをDestroyするまでの時間。0以下なら自動Destroyしない</summary>
    [SerializeField] private float _destroyTime = 0f;

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="character"> キャラクター </param>
    public abstract Transform ApplyAbility(CharacterManager character);

    /// <summary>
    /// 寿命が設定されている場合、生成したアビリティを自動Destroy
    /// </summary>
    protected void DestroyAfterLifeTime(Transform abilityTrans)
    {
        if (abilityTrans == null || _destroyTime <= 0f)
        {
            return;
        }

        Destroy(abilityTrans.gameObject, _destroyTime);
    }
}
