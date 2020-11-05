/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    public class RoundUIComponent : UIComponent
    {
        [Header("Game Round")]
        [Space]
        public CanvasGroup mRoundMessageSpr;
        public Text mRoundMessage;

        public GameObject mRoundValuesPanel;
        public Text mRoundScoreText;
        public Text mRoundCandyText;

        [Header("Round Feedback Images")]
        [Space]
        public Image correctImage;
        public Image wrongImage;

        private void Start()
        {
            correctImage.color = new Color(1, 1, 1, 0.2f);
            wrongImage.color = new Color(1, 1, 1, 0.2f);

            SetFeedbackActive(false);
        }

        public void SetFeedbackActive(bool value)
        {
            correctImage.gameObject.SetActive(value);
            wrongImage.gameObject.SetActive(value);
        }

        public void SetCorrect(bool correct, Action callback = null)
        {
            Image toAnimate;
            if (correct)
            {
                correctImage.color = new Color(1, 1, 1, 1f);
                wrongImage.color = new Color(1, 1, 1, 0.2f);

                toAnimate = correctImage;
            }
            else
            {
                correctImage.color = new Color(1, 1, 1, 0.2f);
                wrongImage.color = new Color(1, 1, 1, 1f);

                toAnimate = wrongImage;
            }

            toAnimate.transform.localScale = Vector2.zero;
            toAnimate.transform.DOShakeRotation(1);
            toAnimate.transform
                .DOScale(1, 1f)
                .SetEase(Ease.OutBounce)
                .OnComplete(() => callback?.Invoke());
        }
    }
}