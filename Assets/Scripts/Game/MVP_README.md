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

## Presenter

入力、当たり判定、ゲーム進行などを受け取り、Modelを更新してViewへ反映します。Unityの`MonoBehaviour`としてシーン上のオブジェクトに付ける中心役です。

- `GamePresenter`: ゲーム全体の進行管理とHUD更新の集約役
- `PlayerPresenter`: プレイヤー操作、キャラ切り替え、被弾、パリィ、グレイズ
- `CharacterPresenter`: キャラクターの状態とアビリティの橋渡し
- `BossPresenter`: ボス移動、被弾、ダメージ計算イベント
- `BgPresenter`: 背景ブロックの生成とスクロール制御
- `BgBlockPresenter`: 背景ブロック単体の移動

## 今回の参照方針

`PlayerPresenter` や `BossPresenter` は `GameHudView` を直接持たず、HP変更やスコア加算などのイベントだけを発行します。

`GamePresenter` がそれらのイベントを購読し、`GameHudView` に表示更新を依頼します。

これにより、プレイヤーやボスの処理はHUDの具体的な作りを知らなくてよくなります。

## 判断の目安

- 数値やルールを覚えるだけなら `Model`
- Text、Slider、Particle、Spriteなど見た目を触るなら `View`
- 入力やイベントを受けて、ModelとViewをつなぐなら `Presenter`
- 複数のPresenterをまたいでUI更新やゲーム進行をまとめるなら上位の `GamePresenter`
