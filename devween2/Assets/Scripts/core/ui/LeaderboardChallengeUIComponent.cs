/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.UI;
using util;

namespace core.ui
{
    public class LeaderboardChallengeUIComponent : UIComponent
    {
        [Header("Leaderboard Challenge")]
        [Space]
        public Text targetText;
        public Text betText;
        public Slider betSlider;
        [HideInInspector] public int BetScore;

        [Header("Leaderboard Challenge Buttons")]
        [Space]
        public Button cancelButton;
        public Button betButton;

        private void Start()
        {
            betSlider.onValueChanged.AddListener(OnValueChanged);
            cancelButton.onClick.AddListener(Cancel);
            betButton.onClick.AddListener(Bet);
            SetActive(false);
        }

        public override void SetActive(bool value)
        {
            base.SetActive(value);

            if (!value)
            {
                betButton.onClick.RemoveAllListeners();
                betButton.onClick.AddListener(Bet);
                SetTarget("", 0, 0);
            }
        }

        private void Cancel()
        {
            GameDebug.Log("Challenge CANCELED", util.LogType.Leaderboard);
            SetActive(false);
        }

        private void Bet()
        {
            GameDebug.Log("Challenge STARTED", util.LogType.Leaderboard);
            gameObject.SetActive(false);
        }

        public void SetTarget(string target, int maxCoins, int score)
        {
            targetText.text = target;
            betSlider.maxValue = maxCoins;
            BetScore = score;
        }

        public string GetTargetName()
        {
            return targetText.text;
        }

        public int GetBetScore()
        {
            return BetScore;
        }

        public int GetBetCoins()
        {
            return (int)betSlider.value;
        }

        private void OnValueChanged(float value)
        {
            betText.text = $"{(int)value} doce(s)";
            betButton.interactable = value > 0;
        }
    }
}