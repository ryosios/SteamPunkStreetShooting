using DG.Tweening;
using UnityEngine;

/// <summary>
/// RectTransformのスケールを使ったUIポップアニメーションView。
/// </summary>
public class UiPopAnimationView : UiAnimationViewBase
{
    /// <summary>表示時のスケール。</summary>
    [SerializeField] private Vector3 _shownScale = Vector3.one;

    /// <summary>非表示時のスケール。</summary>
    [SerializeField] private Vector3 _hiddenScale = Vector3.zero;

    /// <summary>スケールアニメーションと同時にフェードするか。</summary>
    [SerializeField] private bool _useFade = true;

    /// <summary>
    /// 表示状態へ即時反映する。
    /// </summary>
    public override void SetShownImmediate()
    {
        base.SetShownImmediate();
        SetScale(_shownScale);
        SetAlpha(1f);
    }

    /// <summary>
    /// 非表示状態へ即時反映する。
    /// </summary>
    public override void SetHiddenImmediate()
    {
        SetScale(_hiddenScale);
        SetAlpha(0f);
        base.SetHiddenImmediate();
    }

    /// <summary>
    /// ポップ表示Tweenを作成する。
    /// </summary>
    /// <returns>ポップ表示Tween。</returns>
    protected override Tween CreateShowTween()
    {
        SetScale(_hiddenScale);
        SetAlpha(0f);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(_targetRect.DOScale(_shownScale, _showDuration).SetEase(Ease.OutBack));

        if (_useFade && _canvasGroup != null)
        {
            sequence.Join(_canvasGroup.DOFade(1f, _showDuration).SetEase(Ease.OutQuad));
        }

        return sequence;
    }

    /// <summary>
    /// ポップ非表示Tweenを作成する。
    /// </summary>
    /// <returns>ポップ非表示Tween。</returns>
    protected override Tween CreateHideTween()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(_targetRect.DOScale(_hiddenScale, _hideDuration).SetEase(Ease.InBack));

        if (_useFade && _canvasGroup != null)
        {
            sequence.Join(_canvasGroup.DOFade(0f, _hideDuration).SetEase(Ease.InQuad));
        }

        return sequence;
    }

    /// <summary>
    /// RectTransformのスケールを設定する。
    /// </summary>
    /// <param name="scale">設定するスケール。</param>
    private void SetScale(Vector3 scale)
    {
        if (_targetRect == null)
        {
            Debug.LogWarning("Target RectTransform is not assigned.");
            return;
        }

        _targetRect.localScale = scale;
    }

    /// <summary>
    /// CanvasGroupの透明度を設定する。
    /// </summary>
    /// <param name="alpha">設定する透明度。</param>
    private void SetAlpha(float alpha)
    {
        if (!_useFade || _canvasGroup == null)
        {
            return;
        }

        _canvasGroup.alpha = alpha;
    }
}
