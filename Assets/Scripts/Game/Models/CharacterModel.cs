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

    public int Hp => _hp;
    public int MaxHp => _maxHp;
    public float Power => _power;

    /// <summary>
    /// HPを加算
    /// </summary>
    public int AddHp(int value)
    {
        _hp = Mathf.Clamp(_hp + value, 0, _maxHp);
        return _hp;
    }
}

