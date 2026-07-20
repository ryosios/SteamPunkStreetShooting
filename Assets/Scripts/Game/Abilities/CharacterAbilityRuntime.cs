using UnityEngine;

/// <summary>
/// 生成されたアビリティ1回分の後始末を担当するRuntimeコンポーネント。
/// </summary>
public class CharacterAbilityRuntime : MonoBehaviour
{
    /// <summary>生成元のアビリティ設定</summary>
    private CharacterAbilityBase _ability;

    /// <summary>この1回分の適用先情報</summary>
    private CharacterAbilityContext _context;

    /// <summary>初期化済みかどうか</summary>
    private bool _isInitialized;

    /// <summary>
    /// 後始末に必要な情報を設定
    /// </summary>
    /// <param name="ability">生成元のアビリティ。</param>
    /// <param name="context">この1回分の適用先情報。</param>
    public void Initialize(CharacterAbilityBase ability, CharacterAbilityContext context)
    {
        _ability = ability;
        _context = context;
        _isInitialized = true;
    }

    private void OnDestroy()
    {
        if (!_isInitialized || _ability == null)
        {
            return;
        }

        _ability.OnAbilityDestroyed(_context);
    }
}
