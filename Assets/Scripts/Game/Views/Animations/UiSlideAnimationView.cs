using DG.Tweening;
using UnityEngine;

/// <summary>
/// RectTransformのanchoredPositionを使ったUIスライドアニメーションView。
/// </summary>
public class UiSlideAnimationView : UiAnimationViewBase
{
    /// <summary>表示時のanchoredPosition。</summary>
    [SerializeField] private Vector2 _shownAnchoredPosition;

    /// <summary>非表示時のanchoredPosition。</summary>
    [SerializeField] private Vector2 _hiddenAnchoredPosition;

    /// <summary>スライドアニメーションと同時にフェードするか。</summary>
    [SerializeField] private bool _useFade = true;

    /// <summary>
    /// 表示位置が未設定の場合に現在位置を表示位置として使う。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (_targetRect != null && _shownAnchoredPosition == Vector2.zero)
        {
            _shownAnchoredPosition = _targetRect.anchoredPosition;
        }
    }

    /// <summary>
    /// 表示状態へ即時反映する。
    /// </summary>
    public override void SetShownImmediate()
    {
        base.SetShownImmediate();
        SetAnchoredPosition(_shownAnchoredPosition);
        SetAlpha(1f);
    }

    /// <summary>
    /// 非表示状態へ即時反映する。
    /// </summary>
    public override void SetHiddenImmediate()
    {
        SetAnchoredPosition(_hiddenAnchoredPosition);
        SetAlpha(0f);
        base.SetHiddenImmediate();
    }

    /// <summary>
    /// スライド表示Tweenを作成する。
    /// </summary>
    /// <returns>スライド表示Tween。</returns>
    protected override Tween CreateShowTween()
    {
        SetAnchoredPosition(_hiddenAnchoredPosition);
        SetAlpha(0f);

        Sequence sequence = DOTween.Sequence();
        sequence.Join(_targetRect.DOAnchorPos(_shownAnchoredPosition, _showDuration).SetEase(Ease.OutCubic));

        if (_useFade && _canvasGroup != null)
        {
            sequence.Join(_canvasGroup.DOFade(1f, _showDuration).SetEase(Ease.OutQuad));
        }

        return sequence;
    }

    /// <summary>
    /// スライド非表示Tweenを作成する。
    /// </summary>
    /// <returns>スライド非表示Tween。</returns>
    protected override Tween CreateHideTween()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(_targetRect.DOAnchorPos(_hiddenAnchoredPosition, _hideDuration).SetEase(Ease.InCubic));

        if (_useFade && _canvasGroup != null)
        {
            sequence.Join(_canvasGroup.DOFade(0f, _hideDuration).SetEase(Ease.InQuad));
        }

        return sequence;
    }

    /// <summary>
    /// RectTransformのanchoredPositionを設定する。
    /// </summary>
    /// <param name="anchoredPosition">設定するanchoredPosition。</param>
    private void SetAnchoredPosition(Vector2 anchoredPosition)
    {
        if (_targetRect == null)
        {
            Debug.LogWarning("Target RectTransform is not assigned.");
            return;
        }

        _targetRect.anchoredPosition = anchoredPosition;
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
