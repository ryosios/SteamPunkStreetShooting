#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 上下左右にループする白黒ノイズPNGを生成するEditor拡張。
/// Assets/Editorフォルダ内に配置してください。
/// </summary>
public sealed class SeamlessNoiseGeneratorWindow : EditorWindow
{
    private enum TextureSize
    {
        Size256 = 256,
        Size512 = 512,
        Size1024 = 1024,
        Size2048 = 2048
    }

    private const string DefaultOutputFolder = "Assets/Generated/Noise";

    [SerializeField] private TextureSize _textureSize = TextureSize.Size512;
    [SerializeField] private int _seed = 12345;
    [SerializeField] private float _scale = 6f;
    [SerializeField] private int _octaves = 5;
    [SerializeField] private float _persistence = 0.5f;
    [SerializeField] private float _lacunarity = 2f;
    [SerializeField] private float _contrast = 1f;
    [SerializeField] private bool _invert;
    [SerializeField] private string _fileName = "SeamlessNoise";
    [SerializeField] private string _outputFolder = DefaultOutputFolder;

    private Texture2D _previewTexture;
    private Vector2 _scrollPosition;

    [MenuItem("Tools/Seamless Noise Generator")]
    private static void OpenWindow()
    {
        SeamlessNoiseGeneratorWindow window =
            GetWindow<SeamlessNoiseGeneratorWindow>();

        window.titleContent = new GUIContent("Seamless Noise");
        window.minSize = new Vector2(380f, 600f);
        window.Show();
    }

    private void OnEnable()
    {
        GeneratePreview();
    }

    private void OnDisable()
    {
        DestroyPreviewTexture();
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        EditorGUILayout.Space(8f);
        EditorGUILayout.LabelField(
            "Seamless Noise Generator",
            EditorStyles.boldLabel);

        EditorGUILayout.HelpBox(
            "上下左右に繰り返しても継ぎ目が出ない、白黒ノイズPNGを生成します。",
            MessageType.Info);

        EditorGUILayout.Space(8f);

        EditorGUI.BeginChangeCheck();

        _textureSize = (TextureSize)EditorGUILayout.EnumPopup(
            "Texture Size",
            _textureSize);

        _seed = EditorGUILayout.IntField("Seed", _seed);

        _scale = EditorGUILayout.Slider(
            new GUIContent(
                "Scale",
                "値を大きくすると模様が細かくなります。"),
            _scale,
            1f,
            32f);

        _octaves = EditorGUILayout.IntSlider(
            new GUIContent(
                "Octaves",
                "ノイズを重ねる回数です。"),
            _octaves,
            1,
            8);

        _persistence = EditorGUILayout.Slider(
            new GUIContent(
                "Persistence",
                "細かいノイズの強さです。"),
            _persistence,
            0f,
            1f);

        _lacunarity = EditorGUILayout.Slider(
            new GUIContent(
                "Lacunarity",
                "オクターブごとの模様の細かさです。"),
            _lacunarity,
            1f,
            4f);

        _contrast = EditorGUILayout.Slider(
            "Contrast",
            _contrast,
            0.1f,
            3f);

        _invert = EditorGUILayout.Toggle("Invert", _invert);

        bool settingsChanged = EditorGUI.EndChangeCheck();

        EditorGUILayout.Space(12f);
        EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

        DrawPreview();

        EditorGUILayout.Space(8f);

        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generate Preview", GUILayout.Height(28f)))
            {
                GeneratePreview();
            }

            if (GUILayout.Button("Random Seed", GUILayout.Height(28f)))
            {
                _seed = UnityEngine.Random.Range(
                    int.MinValue,
                    int.MaxValue);

                GeneratePreview();
            }
        }

        if (settingsChanged)
        {
            GeneratePreview();
        }

        EditorGUILayout.Space(16f);
        EditorGUILayout.LabelField("Output", EditorStyles.boldLabel);

        _fileName = EditorGUILayout.TextField("File Name", _fileName);
        _outputFolder = EditorGUILayout.TextField(
            "Output Folder",
            _outputFolder);

        EditorGUILayout.HelpBox(
            $"出力先: {_outputFolder}",
            MessageType.None);

        EditorGUILayout.Space(4f);

        if (GUILayout.Button("Generate PNG", GUILayout.Height(36f)))
        {
            GenerateAndSavePng();
        }

        EditorGUILayout.Space(12f);

        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// EditorWindow内にプレビュー画像を表示する。
    /// </summary>
    private void DrawPreview()
    {
        Rect previewRect = GUILayoutUtility.GetAspectRect(1f);

        if (_previewTexture == null)
        {
            EditorGUI.DrawRect(previewRect, Color.black);
            return;
        }

        EditorGUI.DrawPreviewTexture(
            previewRect,
            _previewTexture,
            null,
            ScaleMode.ScaleToFit);
    }

    /// <summary>
    /// 軽量なプレビュー用テクスチャを生成する。
    /// </summary>
    private void GeneratePreview()
    {
        const int previewSize = 256;

        DestroyPreviewTexture();

        _previewTexture = GenerateNoiseTexture(previewSize);
        _previewTexture.name = "SeamlessNoisePreview";
        _previewTexture.hideFlags = HideFlags.HideAndDontSave;

        Repaint();
    }

    /// <summary>
    /// 指定サイズのシームレスノイズを生成する。
    /// </summary>
    private Texture2D GenerateNoiseTexture(int size)
    {
        Texture2D texture = new Texture2D(
            size,
            size,
            TextureFormat.RGBA32,
            false,
            true);

        texture.wrapMode = TextureWrapMode.Repeat;
        texture.filterMode = FilterMode.Bilinear;

        Color32[] pixels = new Color32[size * size];

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        // 一度float配列へ格納してから正規化する。
        float[] noiseValues = new float[size * size];

        for (int y = 0; y < size; y++)
        {
            // size - 1で割ることで、画像の両端を同一座標にする。
            float normalizedY = y / (float)(size - 1);

            for (int x = 0; x < size; x++)
            {
                float normalizedX = x / (float)(size - 1);

                float value = GenerateFractalNoise(
                    normalizedX,
                    normalizedY);

                int index = x + y * size;
                noiseValues[index] = value;

                minValue = Mathf.Min(minValue, value);
                maxValue = Mathf.Max(maxValue, value);
            }
        }

        float valueRange = maxValue - minValue;

        for (int i = 0; i < noiseValues.Length; i++)
        {
            float normalizedValue = valueRange > Mathf.Epsilon
                ? (noiseValues[i] - minValue) / valueRange
                : 0f;

            // 中央値0.5を基準にコントラストを調整する。
            normalizedValue = Mathf.Clamp01(
                (normalizedValue - 0.5f) * _contrast + 0.5f);

            if (_invert)
            {
                normalizedValue = 1f - normalizedValue;
            }

            byte gray = (byte)Mathf.RoundToInt(
                normalizedValue * 255f);

            pixels[i] = new Color32(gray, gray, gray, 255);
        }

        texture.SetPixels32(pixels);
        texture.Apply(false, false);

        return texture;
    }

    /// <summary>
    /// 複数オクターブの周期的なグラディエントノイズを合成する。
    /// </summary>
    private float GenerateFractalNoise(float x, float y)
    {
        float total = 0f;
        float amplitude = 1f;
        float amplitudeTotal = 0f;
        float frequency = 1f;

        for (int octave = 0; octave < _octaves; octave++)
        {
            // 周期を整数にすることで端のグラディエントを一致させる。
            int period = Mathf.Max(
                1,
                Mathf.RoundToInt(_scale * frequency));

            float sampleX = x * period;
            float sampleY = y * period;

            float octaveValue = PeriodicGradientNoise(
                sampleX,
                sampleY,
                period,
                period,
                _seed + octave * 1013);

            total += octaveValue * amplitude;
            amplitudeTotal += amplitude;

            amplitude *= _persistence;
            frequency *= _lacunarity;
        }

        return amplitudeTotal > Mathf.Epsilon
            ? total / amplitudeTotal
            : 0f;
    }

    /// <summary>
    /// 指定周期で繰り返す2Dグラディエントノイズ。
    /// 左右および上下の境界で、同じ格子勾配を参照する。
    /// </summary>
    private static float PeriodicGradientNoise(
        float x,
        float y,
        int periodX,
        int periodY,
        int seed)
    {
        int x0 = Mathf.FloorToInt(x);
        int y0 = Mathf.FloorToInt(y);

        int x1 = x0 + 1;
        int y1 = y0 + 1;

        float localX = x - x0;
        float localY = y - y0;

        int wrappedX0 = PositiveModulo(x0, periodX);
        int wrappedX1 = PositiveModulo(x1, periodX);
        int wrappedY0 = PositiveModulo(y0, periodY);
        int wrappedY1 = PositiveModulo(y1, periodY);

        Vector2 gradient00 = GetGradient(
            wrappedX0,
            wrappedY0,
            seed);

        Vector2 gradient10 = GetGradient(
            wrappedX1,
            wrappedY0,
            seed);

        Vector2 gradient01 = GetGradient(
            wrappedX0,
            wrappedY1,
            seed);

        Vector2 gradient11 = GetGradient(
            wrappedX1,
            wrappedY1,
            seed);

        float dot00 = Vector2.Dot(
            gradient00,
            new Vector2(localX, localY));

        float dot10 = Vector2.Dot(
            gradient10,
            new Vector2(localX - 1f, localY));

        float dot01 = Vector2.Dot(
            gradient01,
            new Vector2(localX, localY - 1f));

        float dot11 = Vector2.Dot(
            gradient11,
            new Vector2(localX - 1f, localY - 1f));

        float fadeX = Fade(localX);
        float fadeY = Fade(localY);

        float bottom = Mathf.Lerp(dot00, dot10, fadeX);
        float top = Mathf.Lerp(dot01, dot11, fadeX);

        return Mathf.Lerp(bottom, top, fadeY);
    }

    /// <summary>
    /// 格子座標とSeedから再現可能なランダム勾配を生成する。
    /// </summary>
    private static Vector2 GetGradient(int x, int y, int seed)
    {
        uint hash = Hash(
            unchecked((uint)x),
            unchecked((uint)y),
            unchecked((uint)seed));

        float angle = (hash / (float)uint.MaxValue) *
                      Mathf.PI *
                      2f;

        return new Vector2(
            Mathf.Cos(angle),
            Mathf.Sin(angle));
    }

    /// <summary>
    /// 整数座標用の簡易ハッシュ。
    /// </summary>
    private static uint Hash(uint x, uint y, uint seed)
    {
        uint hash = seed;

        hash ^= x * 374761393u;
        hash = RotateLeft(hash, 17);
        hash *= 668265263u;

        hash ^= y * 2246822519u;
        hash = RotateLeft(hash, 15);
        hash *= 3266489917u;

        hash ^= hash >> 16;
        hash *= 2246822519u;
        hash ^= hash >> 13;
        hash *= 3266489917u;
        hash ^= hash >> 16;

        return hash;
    }

    private static uint RotateLeft(uint value, int amount)
    {
        return (value << amount) |
               (value >> (32 - amount));
    }

    /// <summary>
    /// Perlin系ノイズで使われる補間曲線。
    /// </summary>
    private static float Fade(float value)
    {
        return value * value * value *
               (value * (value * 6f - 15f) + 10f);
    }

    private static int PositiveModulo(int value, int modulus)
    {
        int result = value % modulus;
        return result < 0 ? result + modulus : result;
    }

    /// <summary>
    /// 指定設定でPNGを書き出す。
    /// </summary>
    private void GenerateAndSavePng()
    {
        if (string.IsNullOrWhiteSpace(_fileName))
        {
            EditorUtility.DisplayDialog(
                "Invalid File Name",
                "ファイル名を入力してください。",
                "OK");

            return;
        }

        if (string.IsNullOrWhiteSpace(_outputFolder) ||
            !_outputFolder.StartsWith(
                "Assets",
                StringComparison.Ordinal))
        {
            EditorUtility.DisplayDialog(
                "Invalid Output Folder",
                "出力先はAssetsから始まるパスにしてください。",
                "OK");

            return;
        }

        int size = (int)_textureSize;

        try
        {
            EditorUtility.DisplayProgressBar(
                "Seamless Noise Generator",
                "ノイズテクスチャを生成しています。",
                0.25f);

            Texture2D texture = GenerateNoiseTexture(size);

            EditorUtility.DisplayProgressBar(
                "Seamless Noise Generator",
                "PNGへ変換しています。",
                0.75f);

            byte[] pngData = texture.EncodeToPNG();
            DestroyImmediate(texture);

            string absoluteFolderPath =
                GetAbsolutePathFromAssetPath(_outputFolder);

            Directory.CreateDirectory(absoluteFolderPath);

            string sanitizedFileName =
                SanitizeFileName(_fileName);

            string assetPath =
                $"{_outputFolder}/{sanitizedFileName}.png";

            assetPath = AssetDatabase.GenerateUniqueAssetPath(
                assetPath);

            string absoluteFilePath =
                GetAbsolutePathFromAssetPath(assetPath);

            File.WriteAllBytes(absoluteFilePath, pngData);

            AssetDatabase.ImportAsset(
                assetPath,
                ImportAssetOptions.ForceUpdate);

            ConfigureTextureImporter(assetPath);

            UnityEngine.Object generatedTexture =
                AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

            Selection.activeObject = generatedTexture;
            EditorGUIUtility.PingObject(generatedTexture);

            Debug.Log(
                $"シームレスノイズを生成しました: {assetPath}");

            EditorUtility.DisplayDialog(
                "Generation Complete",
                $"PNGを生成しました。\n\n{assetPath}",
                "OK");
        }
        catch (Exception exception)
        {
            Debug.LogException(exception);

            EditorUtility.DisplayDialog(
                "Generation Failed",
                $"PNGの生成に失敗しました。\n\n{exception.Message}",
                "OK");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
    }

    /// <summary>
    /// 生成した画像のWrap ModeをRepeatへ設定する。
    /// </summary>
    private static void ConfigureTextureImporter(string assetPath)
    {
        TextureImporter importer =
            AssetImporter.GetAtPath(assetPath) as TextureImporter;

        if (importer == null)
        {
            return;
        }

        importer.textureType = TextureImporterType.Default;
        importer.wrapMode = TextureWrapMode.Repeat;
        importer.filterMode = FilterMode.Bilinear;
        importer.mipmapEnabled = true;
        importer.alphaSource = TextureImporterAlphaSource.None;
        importer.sRGBTexture = false;
        importer.textureCompression =
            TextureImporterCompression.Uncompressed;

        importer.SaveAndReimport();
    }

    private static string GetAbsolutePathFromAssetPath(
        string assetPath)
    {
        string projectRoot = Directory.GetParent(
            Application.dataPath)?.FullName;

        if (string.IsNullOrEmpty(projectRoot))
        {
            throw new DirectoryNotFoundException(
                "Unityプロジェクトのルートパスを取得できませんでした。");
        }

        return Path.GetFullPath(
            Path.Combine(projectRoot, assetPath));
    }

    private static string SanitizeFileName(string fileName)
    {
        foreach (char invalidCharacter in
                 Path.GetInvalidFileNameChars())
        {
            fileName = fileName.Replace(
                invalidCharacter,
                '_');
        }

        return fileName.Trim();
    }

    private void DestroyPreviewTexture()
    {
        if (_previewTexture == null)
        {
            return;
        }

        DestroyImmediate(_previewTexture);
        _previewTexture = null;
    }
}

#endif