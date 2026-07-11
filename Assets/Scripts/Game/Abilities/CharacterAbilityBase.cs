using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public abstract class CharacterAbilityBase : ScriptableObject
{
    /// <summary>生成したアビリティをDestroyするまでの時間。0以下なら自動Destroyしない</summary>
    [SerializeField] private float _destroyTime = 0f;

    /// <summary>キャラクター切り替え時に生成したアビリティを停止するか</summary>
    [Header("通常弾ループの場合は設定")]
    [SerializeField] private bool _stopOnCharacterChange = false;

    /// <summary>キャラクター切り替え時に停止するか</summary>
    public bool StopOnCharacterChange => _stopOnCharacterChange;

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="context">アビリティ実行に必要なキャラクター側の参照情報。</param>
    public abstract Transform ApplyAbility(CharacterAbilityContext context);

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

