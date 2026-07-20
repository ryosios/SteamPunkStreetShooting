using UnityEngine;

/// <summary>
/// キャラクターの状態と数値を持つModel。
/// Unityの見た目や入力には触れない。
/// </summary>
[System.Serializable]
public class CharacterModel
{
    /// <summary>現在HP</summary>
    [SerializeField] private int _hp = 3;

    /// <summary>最大HP</summary>
    [SerializeField] private int _maxHp = 3;

    /// <summary>攻撃力</summary>
    [SerializeField] private float _power = 0.1f;

    /// <summary>攻撃力への加算補正</summary>
    private float _powerAdd;

    /// <summary>攻撃力への倍率補正</summary>
    private float _powerMultiplier = 1f;

    public int Hp => _hp;
    public int MaxHp => _maxHp;
    public float Power => Mathf.Max(0f, (_power + _powerAdd) * _powerMultiplier);

    /// <summary>
    /// HPを加算
    /// </summary>
    public int AddHp(int value)
    {
        _hp = Mathf.Clamp(_hp + value, 0, _maxHp);
        return _hp;
    }

    /// <summary>
    /// 攻撃力の加算補正を追加
    /// </summary>
    /// <param name="value">追加する攻撃力。</param>
    public void AddPowerBonus(float value)
    {
        _powerAdd += value;
    }

    /// <summary>
    /// 攻撃力の加算補正を解除
    /// </summary>
    /// <param name="value">解除する攻撃力。</param>
    public void RemovePowerBonus(float value)
    {
        _powerAdd -= value;
    }

    /// <summary>
    /// 攻撃力の倍率補正を追加
    /// </summary>
    /// <param name="multiplier">追加する倍率。</param>
    public void AddPowerMultiplier(float multiplier)
    {
        _powerMultiplier *= GetSafePowerMultiplier(multiplier);
    }

    /// <summary>
    /// 攻撃力の倍率補正を解除
    /// </summary>
    /// <param name="multiplier">解除する倍率。</param>
    public void RemovePowerMultiplier(float multiplier)
    {
        _powerMultiplier /= GetSafePowerMultiplier(multiplier);
    }

    /// <summary>
    /// 攻撃力倍率補正に使える安全な値を取得
    /// </summary>
    private float GetSafePowerMultiplier(float multiplier)
    {
        return Mathf.Max(0.0001f, multiplier);
    }
}

