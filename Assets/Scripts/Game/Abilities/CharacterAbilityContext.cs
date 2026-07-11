using UnityEngine;

/// <summary>
/// アビリティ実行時に必要なキャラクター側の参照情報。
/// </summary>
public struct CharacterAbilityContext
{
    /// <summary>アビリティ所有者のTransform</summary>
    public readonly Transform OwnerTransform;

    /// <summary>キャラクター側に追従させる生成先</summary>
    public readonly Transform CharacterAttachPoint;

    /// <summary>ワールド側に生成するための生成先</summary>
    public readonly Transform WorldAttachPoint;

    public CharacterAbilityContext(
        Transform ownerTransform,
        Transform characterAttachPoint,
        Transform worldAttachPoint)
    {
        OwnerTransform = ownerTransform;
        CharacterAttachPoint = characterAttachPoint;
        WorldAttachPoint = worldAttachPoint;
    }
}
