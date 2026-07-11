using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// DOTweenを使ったUIアニメーションViewの基底クラス。
/// 表示、非表示、即時反映、再生停止、UniTaskでの完了待ちを共通化する。
/// </summary>
public abstract class UiAnimationViewBase : MonoBehaviour
{
    /// <summary>アニメーション対象のRectTransform。</summary>
    [SerializeField] protected RectTransform _targetRect;

    /// <summary>フェードや入力制御に使うCanvasGroup。</summary>
    [SerializeField] protected CanvasGroup _canvasGroup;

    /// <summary>表示アニメーション時間。</summary>
    [SerializeField] protected float _showDuration = 0.2f;

    /// <summary>非表示アニメーション時間。</summary>
    [SerializeField] protected float _hideDuration = 0.15f;

    /// <summary>TimeScaleの影響を受けずに再生するか。</summary>
    [SerializeField] protected bool _ignoreTimeScale = true;

    /// <summary>表示完了後にレイキャストブロックを有効にするか。</summary>
    [SerializeField] private bool _blocksRaycastsOnShow = true;

#if UNITY_EDITOR
    /// <summary>Editor実行中にデバッグ用ショートカット入力を有効にするか。</summary>
    [SerializeField] private bool _enableEditorDebugShortcut;

    /// <summary>表示アニメーションを再生するデバッグキー。</summary>
    [SerializeField] private KeyCode _debugShowKey = KeyCode.F5;

    /// <summary>非表示アニメーションを再生するデバッグキー。</summary>
    [SerializeField] private KeyCode _debugHideKey = KeyCode.F6;

    /// <summary>表示状態へ即時反映するデバッグキー。</summary>
    [SerializeField] private KeyCode _debugShowImmediateKey = KeyCode.F7;

    /// <summary>非表示状態へ即時反映するデバッグキー。</summary>
    [SerializeField] private KeyCode _debugHideImmediateKey = KeyCode.F8;

    /// <summary>再生中アニメーションを停止するデバッグキー。</summary>
    [SerializeField] private KeyCode _debugStopKey = KeyCode.F9;

#endif

    /// <summary>現在再生中のTween。</summary>
    private Tween _currentTween;

    /// <summary>
    /// 参照が未設定の場合に、自身のRectTransformを自動設定する。
    /// </summary>
    protected virtual void Awake()
    {
        if (_targetRect == null)
        {
            _targetRect = transform as RectTransform;
        }
    }

    /// <summary>
    /// 破棄時に再生中のTweenを停止する。
    /// </summary>
    protected virtual void OnDestroy()
    {
        StopAnimation();
    }

#if UNITY_EDITOR
    /// <summary>
    /// Editor実行中だけ、デバッグ用ショートカット入力を監視する。
    /// </summary>
    protected virtual void Update()
    {
        if (!_enableEditorDebugShortcut)
        {
            return;
        }

        if (Input.GetKeyDown(_debugShowKey))
        {
            PlayShow();
        }

        if (Input.GetKeyDown(_debugHideKey))
        {
            PlayHide();
        }

        if (Input.GetKeyDown(_debugShowImmediateKey))
        {
            SetShownImmediate();
        }

        if (Input.GetKeyDown(_debugHideImmediateKey))
        {
            SetHiddenImmediate();
        }

        if (Input.GetKeyDown(_debugStopKey))
        {
            StopAnimation();
        }
    }
#endif

    /// <summary>
    /// 表示アニメーションを再生する。
    /// </summary>
    /// <returns>再生したTween。</returns>
    public Tween PlayShow()
    {
        StopAnimation();
        gameObject.SetActive(true);
        SetCanvasInteractable(false);

        return SetCurrentTween(CreateShowTween(), () => SetCanvasInteractable(true));
    }

    /// <summary>
    /// 表示アニメーションを再生し、完了まで待機する。
    /// </summary>
    /// <param name="cancellationToken">待機をキャンセルするためのトークン。</param>
    public UniTask PlayShowAsync(CancellationToken cancellationToken = default)
    {
        return WaitForTweenAsync(PlayShow(), cancellationToken);
    }

    /// <summary>
    /// 非表示アニメーションを再生する。
    /// </summary>
    /// <returns>再生したTween。</returns>
    public Tween PlayHide()
    {
        StopAnimation();
        SetCanvasInteractable(false);

        return SetCurrentTween(CreateHideTween(), null);
    }

    /// <summary>
    /// 非表示アニメーションを再生し、完了まで待機する。
    /// </summary>
    /// <param name="cancellationToken">待機をキャンセルするためのトークン。</param>
    public UniTask PlayHideAsync(CancellationToken cancellationToken = default)
    {
        return WaitForTweenAsync(PlayHide(), cancellationToken);
    }

    /// <summary>
    /// 再生中のアニメーションを停止する。
    /// </summary>
    public void StopAnimation()
    {
        if (_currentTween == null)
        {
            return;
        }

        _currentTween.Kill();
        _currentTween = null;
    }

    /// <summary>
    /// 表示状態へ即時反映する。
    /// </summary>
    public virtual void SetShownImmediate()
    {
        StopAnimation();
        gameObject.SetActive(true);
        SetCanvasInteractable(true);
    }

    /// <summary>
    /// 非表示状態へ即時反映する。
    /// </summary>
    public virtual void SetHiddenImmediate()
    {
        StopAnimation();
        SetCanvasInteractable(false);
    }

    /// <summary>
    /// GameObjectを非アクティブにする。
    /// 必要な場合は、非表示アニメーションの完了後に呼び出し側から実行する。
    /// </summary>
    public void SetInactive()
    {
        StopAnimation();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 表示アニメーションTweenを作成する。
    /// </summary>
    /// <returns>表示アニメーションTween。</returns>
    protected abstract Tween CreateShowTween();

    /// <summary>
    /// 非表示アニメーションTweenを作成する。
    /// </summary>
    /// <returns>非表示アニメーションTween。</returns>
    protected abstract Tween CreateHideTween();

    /// <summary>
    /// CanvasGroupの入力状態を設定する。
    /// </summary>
    /// <param name="isInteractable">入力可能にするか。</param>
    protected void SetCanvasInteractable(bool isInteractable)
    {
        if (_canvasGroup == null)
        {
            return;
        }

        _canvasGroup.interactable = isInteractable;
        _canvasGroup.blocksRaycasts = isInteractable && _blocksRaycastsOnShow;
    }

    /// <summary>
    /// 作成したTweenに共通設定を適用し、現在のTweenとして保持する。
    /// </summary>
    /// <param name="tween">設定対象のTween。</param>
    /// <param name="onComplete">完了時に実行する処理。</param>
    /// <returns>共通設定を適用したTween。</returns>
    private Tween SetCurrentTween(Tween tween, Action onComplete)
    {
        if (tween == null)
        {
            Debug.LogWarning("UI animation tween is not created.");
            return null;
        }

        _currentTween = tween
            .SetUpdate(_ignoreTimeScale)
            .SetLink(gameObject)
            .OnComplete(() => onComplete?.Invoke())
            .OnKill(() =>
            {
                if (_currentTween == tween)
                {
                    _currentTween = null;
                }
            });

        return _currentTween;
    }

    /// <summary>
    /// Tweenの完了までUniTaskで待機する。
    /// </summary>
    /// <param name="tween">待機対象のTween。</param>
    /// <param name="cancellationToken">待機をキャンセルするためのトークン。</param>
    private async UniTask WaitForTweenAsync(Tween tween, CancellationToken cancellationToken)
    {
        if (tween == null)
        {
            return;
        }

        try
        {
            await UniTask.WaitUntil(
                () => !tween.IsActive() || tween.IsComplete(),
                cancellationToken: cancellationToken
            );
        }
        catch (OperationCanceledException)
        {
            if (tween.IsActive())
            {
                tween.Kill();
            }

            throw;
        }
    }
}
