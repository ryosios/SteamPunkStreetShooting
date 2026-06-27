using System;

/// <summary>
/// ゲーム全体で保存するデータ。
/// 新しい保存項目を増やす場合は、このクラスにフィールドを追加する。
/// </summary>
[Serializable]
public class SaveData
{
    /// <summary>セーブデータのバージョン。将来のデータ移行判定に使う。</summary>
    public int version = 1;

    /// <summary>プレイヤーの最高スコア。</summary>
    public int highScore;

    /// <summary>所持コイン数。</summary>
    public int coins;

    /// <summary>選択中のキャラクターID。</summary>
    public string selectedCharacterId = "default";

    /// <summary>BGM音量。0から1の範囲で扱う想定。</summary>
    public float bgmVolume = 1f;

    /// <summary>効果音音量。0から1の範囲で扱う想定。</summary>
    public float seVolume = 1f;
}
