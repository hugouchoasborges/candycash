/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using DG.Tweening;
using System;
using UnityEngine;

namespace core.ui
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIComponent : MonoBehaviour
    {
        private CanvasGroup _canvasGroup;
        private Vector3 _originalPosition;

        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _originalPosition = transform.localPosition;
        }

        public virtual void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }

        public virtual void Fade(float endValue, float duration = 1, Action callback = null)
        {
            _canvasGroup
                .DOFade(endValue, duration)
                .OnComplete(() => callback?.Invoke());
        }

        public virtual void Move(Vector3 to, float duration = 1, Action callback = null)
        {
            transform
                .DOLocalMove(to, duration)
                .OnComplete(() => callback?.Invoke());
        }

        public virtual void ResetPosition(float duration = 1, Action callback = null)
        {
            transform
                .DOLocalMove(_originalPosition, duration)
                .OnComplete(() => callback?.Invoke());
        }
    }
}