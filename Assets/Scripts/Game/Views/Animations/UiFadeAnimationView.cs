using DG.Tweening;
using UnityEngine;

/// <summary>
/// CanvasGroupの透明度を変化させるUIフェードアニメーションView。
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UiFadeAnimationView : UiAnimationViewBase
{
    /// <summary>表示時の透明度。</summary>
    [SerializeField] private float _shownAlpha = 1f;

    /// <summary>非表示時の透明度。</summary>
    [SerializeField] private float _hiddenAlpha = 0f;

    /// <summary>
    /// CanvasGroup参照を自動設定する。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (_canvasGroup == null)
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// 表示状態へ即時反映する。
    /// </summary>
    public override void SetShownImmediate()
    {
        base.SetShownImmediate();
        SetAlpha(_shownAlpha);
    }

    /// <summary>
    /// 非表示状態へ即時反映する。
    /// </summary>
    public override void SetHiddenImmediate()
    {
        SetAlpha(_hiddenAlpha);
        base.SetHiddenImmediate();
    }

    /// <summary>
    /// フェードインTweenを作成する。
    /// </summary>
    /// <returns>フェードインTween。</returns>
    protected override Tween CreateShowTween()
    {
        SetAlpha(_hiddenAlpha);
        return _canvasGroup.DOFade(_shownAlpha, _showDuration).SetEase(Ease.OutQuad);
    }

    /// <summary>
    /// フェードアウトTweenを作成する。
    /// </summary>
    /// <returns>フェードアウトTween。</returns>
    protected override Tween CreateHideTween()
    {
        return _canvasGroup.DOFade(_hiddenAlpha, _hideDuration).SetEase(Ease.InQuad);
    }

    /// <summary>
    /// CanvasGroupの透明度を設定する。
    /// </summary>
    /// <param name="alpha">設定する透明度。</param>
    private void SetAlpha(float alpha)
    {
        if (_canvasGroup == null)
        {
            Debug.LogWarning("CanvasGroup is not assigned.");
            return;
        }

        _canvasGroup.alpha = alpha;
    }
}
