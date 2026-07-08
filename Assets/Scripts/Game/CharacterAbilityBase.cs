using UnityEngine;
using UniRx;
using DG.Tweening;
using System.Collections;

public abstract class CharacterAbilityBase : ScriptableObject
{

    /// <summary>
    /// アビリティを適用
    /// </summary>
    /// <param name="character"> キャラクター </param>
    public abstract Transform ApplyAbility(CharacterManager character);

}
