# MVP整理メモ

このプロジェクトでは、ゲームのスクリプトを大きく `Model` / `View` / `Presenter` に分けています。

## Model

状態や計算だけを持つクラスです。できるだけ `MonoBehaviour` にせず、UIや入力やPrefabを直接触りません。

- `CharacterModel`: キャラクターのHP、最大HP、攻撃力
- `BossModel`: ボスの防御値とダメージ計算
- `ScoreModel`: スコア
- `BoardPositionModel`: ボード上の位置

## View

画面表示やUI部品を操作するクラスです。状態の計算はなるべく持たず、Presenterから渡された値を表示します。

- `GameHudView`: HUD全体の表示窓口
- `CharacterStatusView`: キャラクターHP表示
- `BossStatusView`: ボスHP表示
- `ScoreView`: スコア表示
- `UiAnimationViewBase`: UIアニメーションViewの共通処理
- `UiFadeAnimationView` / `UiPopAnimationView` / `UiSlideAnimationView`: DOTweenを使ったUIアニメーション部品

## Presenter

入力、当たり判定、ゲーム進行などを受け取り、Modelを更新してViewへ反映します。Unityの`MonoBehaviour`としてシーン上のオブジェクトに付ける中心役です。

- `GamePresenter`: ゲーム全体の進行管理とHUD更新の集約役
- `ScorePresenter`: スコアModelとScoreViewの橋渡し
- `PlayerPresenter`: プレイヤー操作、キャラ切り替え、被弾、パリィ、グレイズ
- `CharacterPresenter`: キャラクターの状態とアビリティの橋渡し
- `BossPresenter`: ボス移動、被弾、ダメージ計算イベント
- `BgPresenter`: 背景ブロックの生成とスクロール制御
- `BgBlockPresenter`: 背景ブロック単体の移動

## 今回の参照方針

`PlayerPresenter` や `BossPresenter` は `GameHudView` を直接持たず、HP変更やスコア加算などのイベントだけを発行します。

`GamePresenter` がそれらのイベントを購読し、`GameHudView` に表示更新を依頼します。

これにより、プレイヤーやボスの処理はHUDの具体的な作りを知らなくてよくなります。

スコアは `ScorePresenter` が `ScoreModel` を更新し、`ScoreView` に表示値を渡します。
`ScoreView` はスコアを保持せず、受け取った値をテキストに表示するだけにします。

UIアニメーションは表示演出なのでView側に置きます。Presenterは `PlayShow()` や `PlayHide()` を呼んで、どのタイミングで表示演出するかだけを決めます。`PlayHide()` は見た目を非表示にするだけで、GameObjectは非アクティブにしません。

アビリティ生成物は、寿命が設定されていれば時間経過でDestroyされます。
寿命0以下で継続させる場合でも、通常弾ループのようにキャラクター切り替えで止めたいものは、アビリティの `Stop On Character Change` を有効にします。

アニメーション完了を待ちたい場合は `PlayShowAsync()` や `PlayHideAsync()` を使います。これらはUniTaskを返すので、Presenter側から `await` できます。非表示完了後にGameObjectも止めたい場合は、`await PlayHideAsync()` のあとに `SetInactive()` を呼びます。

アニメーション内容を実行中に確認したい場合は、`UiFadeAnimationView` など各アニメーションViewの `_enableEditorDebugShortcut` にチェックを入れます。これは `#if UNITY_EDITOR` 内の機能なので、Editor実行中だけ有効です。初期設定では `F5` が表示、`F6` が非表示、`F7` が即時表示、`F8` が即時非表示、`F9` が停止です。

## 判断の目安

- 数値やルールを覚えるだけなら `Model`
- Text、Slider、Particle、Spriteなど見た目を触るなら `View`
- 入力やイベントを受けて、ModelとViewをつなぐなら `Presenter`
- 複数のPresenterをまたいでUI更新やゲーム進行をまとめるなら上位の `GamePresenter`
