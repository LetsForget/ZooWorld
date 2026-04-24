using System;
using Common.Pool;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace ZooWorld.UI
{
    public class FloatingTextView : MonoBehaviour, IPoolable
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private RectTransform rectTransform;

        private Sequence activeSequence;
        private Action<FloatingTextView> onComplete;

        public TMP_Text Label
        {
            get
            {
                EnsureReferences();
                return label;
            }
        }

        public RectTransform RectTransform
        {
            get
            {
                EnsureReferences();
                return rectTransform;
            }
        }

        public void Play(
            string text,
            Vector2 anchoredPosition,
            FloatingTextAnimationSettings settings,
            Action<FloatingTextView> onComplete)
        {
            EnsureReferences();

            this.onComplete = onComplete;
            gameObject.SetActive(true);
            label.text = text;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.localScale = Vector3.one * settings.startScale;
            canvasGroup.alpha = 1f;

            activeSequence?.Kill();
            activeSequence = DOTween.Sequence();
            activeSequence.Join(rectTransform.DOAnchorPosY(anchoredPosition.y + settings.moveOffsetY, settings.duration).SetEase(settings.moveEase));
            activeSequence.Join(rectTransform.DOScale(settings.endScale, settings.duration).SetEase(settings.scaleEase));
            activeSequence.Join(canvasGroup.DOFade(0f, settings.duration).SetDelay(settings.fadeDelay).SetEase(settings.fadeEase));
            activeSequence.OnComplete(HandleCompleted);
        }

        public void OnTakenFromPool()
        {
            EnsureReferences();
            gameObject.SetActive(true);
            canvasGroup.alpha = 1f;
            rectTransform.localScale = Vector3.one;
        }

        public void OnReturnedBackToPool()
        {
            EnsureReferences();

            activeSequence?.Kill();
            activeSequence = null;
            onComplete = null;

            label.text = string.Empty;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.localScale = Vector3.one;
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
        }

        private void OnDisable()
        {
            activeSequence?.Kill();
            activeSequence = null;
        }

        private void HandleCompleted()
        {
            var callback = onComplete;
            onComplete = null;
            callback?.Invoke(this);
        }

        private void EnsureReferences()
        {
            if (rectTransform == null)
            {
                rectTransform = transform as RectTransform;
            }

            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            if (label == null)
            {
                label = GetComponent<TMP_Text>();
            }
        }
    }
}
