using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

/// <summary>
/// JSON形式のセーブデータをファイルへ保存・読み込みする共通クラス。
/// デフォルトではAES暗号化した内容をApplication.persistentDataPathに保存する。
/// </summary>
public static class SaveManager
{
    /// <summary>ファイル名を指定しない場合に使うセーブファイル名。</summary>
    private const string DefaultFileName = "save.dat";

    /// <summary>暗号化キーを指定しない場合に使うデフォルトキー。</summary>
    private const string DefaultEncryptionKey = "SteamPunkStreetShooting_SaveKey";

    /// <summary>暗号鍵の生成に使うソルトのバイト数。</summary>
    private const int SaltSize = 16;

    /// <summary>AES暗号化に使う初期化ベクトルのバイト数。</summary>
    private const int IvSize = 16;

    /// <summary>AES-256用の暗号鍵サイズ。</summary>
    private const int KeySize = 32;

    /// <summary>パスワードから暗号鍵を作るときの反復回数。</summary>
    private const int DeriveIterations = 10000;

    /// <summary>セーブファイルを配置するディレクトリ。</summary>
    public static string SaveDirectory => Application.persistentDataPath;

    /// <summary>
    /// セーブファイルのフルパスを取得する。
    /// </summary>
    /// <param name="fileName">取得したいセーブファイル名。</param>
    /// <returns>Application.persistentDataPath配下のフルパス。</returns>
    public static string GetSavePath(string fileName = DefaultFileName)
    {
        return Path.Combine(SaveDirectory, fileName);
    }

    /// <summary>
    /// 指定したデータをJSONへ変換し、ファイルへ保存する。
    /// encryptがtrueの場合は、JSONを暗号化してから保存する。
    /// </summary>
    /// <typeparam name="T">保存するデータ型。</typeparam>
    /// <param name="data">保存するデータ本体。</param>
    /// <param name="fileName">保存先ファイル名。</param>
    /// <param name="encrypt">暗号化して保存するかどうか。</param>
    /// <param name="encryptionKey">暗号化に使うキー。</param>
    public static void Save<T>(T data, string fileName = DefaultFileName, bool encrypt = true, string encryptionKey = DefaultEncryptionKey) where T : class
    {
        if (data == null)
        {
            throw new ArgumentNullException(nameof(data));
        }

        Directory.CreateDirectory(SaveDirectory);

        string json = JsonUtility.ToJson(data, true);
        string content = encrypt ? Encrypt(json, encryptionKey) : json;
        File.WriteAllText(GetSavePath(fileName), content, Encoding.UTF8);
    }

    /// <summary>
    /// セーブファイルを読み込み、JSONから指定型のデータへ復元する。
    /// ファイルが存在しない場合は、新しいデータを作って返す。
    /// </summary>
    /// <typeparam name="T">読み込むデータ型。</typeparam>
    /// <param name="fileName">読み込み元ファイル名。</param>
    /// <param name="encrypted">保存ファイルが暗号化されているかどうか。</param>
    /// <param name="encryptionKey">復号に使うキー。</param>
    /// <returns>読み込んだセーブデータ。存在しない場合は新規データ。</returns>
    public static T Load<T>(string fileName = DefaultFileName, bool encrypted = true, string encryptionKey = DefaultEncryptionKey) where T : class, new()
    {
        string path = GetSavePath(fileName);
        if (!File.Exists(path))
        {
            return new T();
        }

        string content = File.ReadAllText(path, Encoding.UTF8);
        string json = encrypted ? Decrypt(content, encryptionKey) : content;
        return JsonUtility.FromJson<T>(json) ?? new T();
    }

    /// <summary>
    /// 指定したセーブファイルが存在するか調べる。
    /// </summary>
    /// <param name="fileName">確認するセーブファイル名。</param>
    /// <returns>存在する場合はtrue。</returns>
    public static bool Exists(string fileName = DefaultFileName)
    {
        return File.Exists(GetSavePath(fileName));
    }

    /// <summary>
    /// 指定したセーブファイルを削除する。
    /// ファイルが存在しない場合は何もしない。
    /// </summary>
    /// <param name="fileName">削除するセーブファイル名。</param>
    public static void Delete(string fileName = DefaultFileName)
    {
        string path = GetSavePath(fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// JSON文字列をAESで暗号化し、Base64文字列として返す。
    /// </summary>
    /// <param name="plainText">暗号化するJSON文字列。</param>
    /// <param name="encryptionKey">暗号化に使うキー。</param>
    /// <returns>ソルト、IV、暗号本文をまとめたBase64文字列。</returns>
    private static string Encrypt(string plainText, string encryptionKey)
    {
        byte[] salt = RandomBytes(SaltSize);
        byte[] iv = RandomBytes(IvSize);
        byte[] key = DeriveKey(encryptionKey, salt);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        byte[] payload = new byte[salt.Length + iv.Length + cipherBytes.Length];
        Buffer.BlockCopy(salt, 0, payload, 0, salt.Length);
        Buffer.BlockCopy(iv, 0, payload, salt.Length, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, payload, salt.Length + iv.Length, cipherBytes.Length);

        return Convert.ToBase64String(payload);
    }

    /// <summary>
    /// Base64化された暗号データを復号し、元のJSON文字列へ戻す。
    /// </summary>
    /// <param name="encryptedText">保存ファイルから読み込んだ暗号文字列。</param>
    /// <param name="encryptionKey">復号に使うキー。</param>
    /// <returns>復号されたJSON文字列。</returns>
    private static string Decrypt(string encryptedText, string encryptionKey)
    {
        byte[] payload = Convert.FromBase64String(encryptedText);
        if (payload.Length <= SaltSize + IvSize)
        {
            throw new InvalidDataException("Save data is invalid.");
        }

        byte[] salt = new byte[SaltSize];
        byte[] iv = new byte[IvSize];
        byte[] cipherBytes = new byte[payload.Length - SaltSize - IvSize];

        Buffer.BlockCopy(payload, 0, salt, 0, SaltSize);
        Buffer.BlockCopy(payload, SaltSize, iv, 0, IvSize);
        Buffer.BlockCopy(payload, SaltSize + IvSize, cipherBytes, 0, cipherBytes.Length);

        byte[] key = DeriveKey(encryptionKey, salt);

        using Aes aes = Aes.Create();
        aes.Key = key;
        aes.IV = iv;

        using ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }

    /// <summary>
    /// 暗号化キーとソルトからAES用の固定長キーを生成する。
    /// </summary>
    /// <param name="encryptionKey">元になる暗号化キー。</param>
    /// <param name="salt">キー生成に使うソルト。</param>
    /// <returns>AESで使う32バイトのキー。</returns>
    private static byte[] DeriveKey(string encryptionKey, byte[] salt)
    {
        if (string.IsNullOrEmpty(encryptionKey))
        {
            throw new ArgumentException("Encryption key is empty.", nameof(encryptionKey));
        }

        using Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(encryptionKey, salt, DeriveIterations);
        return deriveBytes.GetBytes(KeySize);
    }

    /// <summary>
    /// 暗号化用のランダムなバイト列を生成する。
    /// </summary>
    /// <param name="length">生成するバイト数。</param>
    /// <returns>指定した長さのランダムバイト列。</returns>
    private static byte[] RandomBytes(int length)
    {
        byte[] bytes = new byte[length];
        using RandomNumberGenerator generator = RandomNumberGenerator.Create();
        generator.GetBytes(bytes);
        return bytes;
    }
}
