# Game Script Architecture

このドキュメントは、`Assets/Scripts/Game` 配下の主な依存関係を確認するためのメモです。
MVPの考え方そのものは `MVP_README.md` にまとめ、こちらでは実装上のクラス関係を中心に扱います。

## 基本方針

- `Model` は状態や計算を持つ。
- `View` は画面表示、UI部品、演出を扱う。
- `Presenter` は入力、イベント、ゲーム進行を受け取り、ModelとViewをつなぐ。
- `View` 同士の親子関係は許可する。
- `Model` から `View` や `Presenter` へ依存しない。
- `PlayerPresenter` や `BossPresenter` はHUDを直接更新せず、通知を発行する。
- `GamePresenter` がゲーム全体の通知を受け取り、`GameHudView` や他Presenterへ橋渡しする。

## 依存方針

```text
Presenter -> Model
Presenter -> View
Presenter -> Presenter
View -> Child View
View -> View Animation
View -> Model  ※ScoreViewのみ、現在はスコア保持のために使用
Model -> なし
Detector -> Presenter
Ability -> CharacterPresenter
```

`ScoreView -> ScoreModel` は、現状ではスコア表示とスコア保持が同じViewに入っています。
スコアをゲームルールで多く使うようになったら、`GamePresenter` か専用Presenter側に寄せる候補です。

## 図の凡例

```text
A --> B
```

`A` が `B` を直接参照している、または直接メソッドを呼ぶ関係です。

```text
A -. EventName .-> B
```

`A` が `EventName` という通知を発行し、`B` がそれを購読して受け取る関係です。
通知は、クラスに付属する発信機のようなものとして考えると分かりやすいです。

現在の実装では、この通知はUniRxの `Subject<T>` と `IObservable<T>` で表現しています。
購読側は `Subscribe(...).AddTo(this)` を使い、破棄時の購読解除をUniRxに任せます。

## 全体図

```mermaid
flowchart TD
    GamePresenter --> GameHudView
    GamePresenter --> PlayerPresenter
    GamePresenter --> BossPresenter
    GamePresenter --> BgPresenter

    PlayerPresenter --> BoardManager
    PlayerPresenter --> CharacterPresenter
    PlayerPresenter --> BoardPositionModel
    PlayerPresenter -. CharacterHpChanged .-> GamePresenter
    PlayerPresenter -. ScoreAdded .-> GamePresenter
    PlayerPresenter -. BgSpeedOffsetRequested .-> GamePresenter

    BossPresenter --> BoardManager
    BossPresenter --> BossModel
    BossPresenter --> BoardPositionModel
    BossPresenter -. BossHpAdded .-> GamePresenter
    BossPresenter -. BossHitBulletRequested .-> GamePresenter

    CharacterPresenter --> CharacterModel
    CharacterPresenter --> CharacterAbilityBase

    CharacterAbilityBase --> CharacterPresenter
    CharacterAbilityAttack_CharacterAttach --> CharacterAbilityBase
    CharacterAbilityAttack_WorldAttach --> CharacterAbilityBase
    CharacterAbilityShield --> CharacterAbilityBase

    GameHudView --> CharacterStatusView
    GameHudView --> ScoreView
    GameHudView --> BossStatusView

    ScoreView --> ScoreModel

    UiFadeAnimationView --> UiAnimationViewBase
    UiPopAnimationView --> UiAnimationViewBase
    UiSlideAnimationView --> UiAnimationViewBase

    BgPresenter --> BgBlockPresenter
    BgBlockPresenter --> Rigidbody

    BoardManager --> BoardSquare

    ParticleHitDetector --> PlayerPresenter
    ParticleHitDetectorPlayer --> BossPresenter
```

## Presenter

| Class | 主な責任 | 主な依存 |
| --- | --- | --- |
| `GamePresenter` | ゲーム全体の進行、HUD更新通知の集約、背景速度変更、ボスダメージの橋渡し | `GameHudView`, `PlayerPresenter`, `BossPresenter`, `BgPresenter` |
| `PlayerPresenter` | プレイヤー入力、ボード移動、キャラ切り替え、被弾、パリィ、グレイズ | `BoardManager`, `CharacterPresenter[]`, `BoardPositionModel` |
| `BossPresenter` | ボスのボード移動、被弾通知、受け取った攻撃力からのダメージ計算、HP変更通知 | `BossModel`, `BoardPositionModel`, `BoardManager` |
| `CharacterPresenter` | キャラクター状態、表示用オブジェクト切り替え、アビリティ実行 | `CharacterModel`, `CharacterAbilityBase[]` |
| `BgPresenter` | 背景ブロック生成、スクロール速度管理、ループ制御 | `BgBlockPresenter[]` |
| `BgBlockPresenter` | 背景ブロック単位のRigidbody移動 | `Rigidbody`, `Transform` |

## Model

| Class | 主な責任 | 使用元 |
| --- | --- | --- |
| `CharacterModel` | HP、最大HP、攻撃力 | `CharacterPresenter` |
| `BossModel` | 防御値、正規化ダメージ計算 | `BossPresenter` |
| `ScoreModel` | スコア保持と加算 | `ScoreView` |
| `BoardPositionModel` | ボード上の `x, y` インデックス | `PlayerPresenter`, `BossPresenter` |

## View

| Class | 主な責任 | 主な依存 |
| --- | --- | --- |
| `GameHudView` | HUD全体の親View。子Viewへ表示更新を振り分ける | `CharacterStatusView[]`, `ScoreView`, `BossStatusView` |
| `CharacterStatusView` | キャラクターHPアイコン、顔アイコン表示 | `Image[]`, `Image` |
| `BossStatusView` | ボスHPスライダー、名前表示 | `Slider`, `TextMeshProUGUI` |
| `ScoreView` | スコア表示 | `TextMeshProUGUI`, `ScoreModel` |

## UI Animation View

| Class | 主な責任 |
| --- | --- |
| `UiAnimationViewBase` | DOTweenアニメーションの共通処理、UniTask待機、Editor用ショートカット |
| `UiFadeAnimationView` | `CanvasGroup` の透明度を使ったフェード |
| `UiPopAnimationView` | `RectTransform.localScale` を使ったポップ |
| `UiSlideAnimationView` | `RectTransform.anchoredPosition` を使ったスライド |

`PlayHide()` は見た目を非表示にするだけで、`GameObject` は非アクティブにしません。
非表示後に止めたい場合は、呼び出し側で次のようにします。

```csharp
await uiAnimation.PlayHideAsync();
uiAnimation.SetInactive();
```

## Ability

| Class | 主な責任 |
| --- | --- |
| `CharacterAbilityBase` | キャラクターアビリティScriptableObjectの基底クラス |
| `CharacterAbilityAttack_CharacterAttach` | キャラクターのアタッチポイントに攻撃Particleを生成 |
| `CharacterAbilityAttack_WorldAttach` | ワールド側のアタッチポイントに攻撃Particleを生成 |
| `CharacterAbilityShield` | キャラクターのアタッチポイントにシールドColliderを生成 |

アビリティは `CharacterPresenter` から実行され、生成物は寿命が設定されている場合に自動Destroyされます。

## Detector

| Class | 主な責任 | 通知先 |
| --- | --- | --- |
| `ParticleHitDetector` | 敵弾Particleとプレイヤー側判定の検知 | `PlayerPresenter` |
| `ParticleHitDetectorPlayer` | プレイヤー弾Particleとボス側判定の検知 | `BossPresenter` |

## Board

| Class | 主な責任 |
| --- | --- |
| `BoardManager` | ボードインデックスから `BoardSquare` を取得 |
| `BoardSquare` | ボードマス単位の座標インデックスとTransform |

## イベント一覧

| 発行元 | イベント | 購読先 | 用途 |
| --- | --- | --- | --- |
| `PlayerPresenter` | `IObservable<CharacterHpChangedEvent> CharacterHpChanged` | `GamePresenter` | キャラクターHP表示更新 |
| `PlayerPresenter` | `IObservable<int> ScoreAdded` | `GamePresenter` | スコア表示加算 |
| `PlayerPresenter` | `IObservable<float> BgSpeedOffsetRequested` | `GamePresenter` | プレイヤー移動に伴う背景速度の一時変更 |
| `BossPresenter` | `IObservable<Unit> BossHitBulletRequested` | `GamePresenter` | ボス被弾時に、現在プレイヤー攻撃力でダメージを橋渡し |
| `BossPresenter` | `IObservable<float> BossHpAdded` | `GamePresenter` | ボスHP表示加算 |

## 更新時の目安

- 新しいUI表示を追加したら、`View` と `GameHudView` の関係を追記する。
- 新しいゲーム通知を追加したら、`イベント一覧` に追記する。
- Presenterが直接Viewを複数持ち始めたら、親Viewを作れないか確認する。
- ModelがViewやPresenterを参照し始めたら、依存方針を見直す。
- 依存関係が増えたらMermaid図も更新する。
