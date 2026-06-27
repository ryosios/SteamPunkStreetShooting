# セーブ機能サマリー

## 概要

`SaveManager` は、ゲームのセーブデータを JSON 形式で保存・読み込みするための共通クラスです。
デフォルトでは JSON を AES で暗号化し、`Application.persistentDataPath` 配下に `save.dat` として保存します。

## 主なファイル

- `SaveData.cs`: ハイスコア、所持コイン、選択キャラクター、音量などを入れるセーブデータ本体です。
- `SaveManager.cs`: JSON 変換、ファイル保存、読み込み、削除、暗号化・復号を担当します。

## 基本的な使い方

```csharp
var data = SaveManager.Load<SaveData>();
data.highScore = 1000;
data.coins += 10;
SaveManager.Save(data);
```

## 保存場所

保存先は次のコードで確認できます。

```csharp
Debug.Log(SaveManager.GetSavePath());
```

Windows の Unity Editor では、多くの場合次のような場所になります。

```text
C:\Users\<ユーザー名>\AppData\LocalLow\<CompanyName>\<ProductName>\save.dat
```

`CompanyName` と `ProductName` は Unity の `Project Settings > Player` の値で決まります。

## 暗号化について

通常は暗号化ありで保存します。

```csharp
SaveManager.Save(data);
```

中身を確認したい開発中だけ、平文 JSON として保存できます。

```csharp
SaveManager.Save(data, "save.json", encrypt: false);
var data = SaveManager.Load<SaveData>("save.json", encrypted: false);
```

平文 JSON は編集しやすい反面、ユーザーにも中身が見えるため、リリース用には暗号化保存を使います。
