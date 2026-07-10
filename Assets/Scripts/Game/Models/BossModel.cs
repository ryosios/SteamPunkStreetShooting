using UnityEngine;

/// <summary>
/// ボスの戦闘数値を持つModel。
/// </summary>
[System.Serializable]
public class BossModel
{
    /// <summary>防御力。キャラクター攻撃力を割ってHP減少量を決める</summary>
    [SerializeField] private float _guard = 10f;

    /// <summary>
    /// スライダー値に対する正規化ダメージを計算
    /// </summary>
    public float CalculateNormalizedDamage(float attackerPower)
    {
        float safeGuard = Mathf.Max(0.0001f, _guard);
        return attackerPower / safeGuard;
    }
}

