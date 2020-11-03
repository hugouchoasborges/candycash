/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using leaderboard;
using UnityEngine;
using util.google;
using System;
using google;
using System.Collections.Generic;
using System.Linq;
using util;
using DG.Tweening;
using UnityEngine.UI;
using monster;

namespace core
{
    public class GameController : Singleton<GameController>
    {
        [Header("Network")]
        [SerializeField] private Loader mGoogleLoader;

        [Header("Menu Items")]
        [Space]
        [SerializeField] private ClickableComponent mDoorClickable;
        [SerializeField] private ClickableComponent mInfoFrameClickable;
        [SerializeField] private ClickableComponent mRankingClickable;

        [Header("Game Round")]
        [Space]
        [SerializeField] private CanvasGroup mRoundMessageSpr;
        [SerializeField] private Text mRoundMessage;

        [SerializeField] private GameObject mRoundValuesPanel;
        [SerializeField] private Text mRoundScoreText;
        [SerializeField] private Text mRoundCandyText;
        private int _roundScore;
        private int _roundCandy;

        [Header("Game Round Values")]
        [Space]
        [SerializeField] [Range(1, 50)] private int mBaseScorePrize = 5;
        [SerializeField] [Range(1, 50)] private int mBaseCandyPrize = 5;
        private int _currentMultiplier = 1;

        [Header("Game Controllers")]
        [Space]
        [SerializeField] private MonsterManager mMonsterManager;
        [SerializeField] private LeaderboardPoolController mLeaderboardPoolController;

        /// <summary>
        /// First call in the ENTIRE game
        /// </summary>
        private void Start()
        {
            LoadFromGoogle(() =>
            {
                mDoorClickable.onPointerDown.AddListener(Play);
                SetTouchActive(true);

                mMonsterManager.onGameOver = GameOver;
                mMonsterManager.onNextRound = NextRound;
                mMonsterManager.Init();
            });
        }


        // ----------------------------------------------------------------------------------
        // ========================== Game Flow ============================
        // ----------------------------------------------------------------------------------

        private void SetTouchActive(bool active)
        {
            mDoorClickable.touchable = active;
            mInfoFrameClickable.touchable = active;
            mRankingClickable.touchable = active;
        }

        private void Play()
        {
            GameDebug.Log("Starting Game...", util.LogType.Transition);

            // Remove click Listeners
            SetTouchActive(false);

            _roundScore = 0;
            _roundCandy = 0;

            // Remove LeaderBoards + GameInfoPanel
            mRankingClickable.transform.DOMoveX(mRankingClickable.transform.position.x - 400, 0.2f);
            mInfoFrameClickable.transform.DOMoveX(mInfoFrameClickable.transform.position.x + 400, 0.2f);

            // Show round values on screen
            mRoundValuesPanel.transform.DOLocalMoveY(0, 1);

            // Zoom in the Door
            mDoorClickable.transform.DOScale(2f, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
            {
                StartRound();
            });
        }

        private void GameOver()
        {
            GameDebug.Log("GameOver...", util.LogType.Transition);

            // Closing the door
            // TODO: Closing the door animation
            mDoorClickable.GetComponent<Image>()
                .DOFade(1, 0.5f)
                .OnComplete(() =>
                {
                    // Hide the monsters
                    mMonsterManager.SetMonstersAlpha(0);

                    // Hide the message
                    mRoundMessageSpr.alpha = 0;

                    // Zoom out the Door
                    mDoorClickable.transform.DOScale(1f, 1f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            // Remove RoundValuesPanel from screen
                            mRoundValuesPanel.transform.DOLocalMoveY(200, 0.5f);

                            // Add LeaderBoards + GameInfoPanel
                            mRankingClickable.transform.DOMoveX(mRankingClickable.transform.position.x + 400, 0.2f);
                            mInfoFrameClickable.transform.DOMoveX(mInfoFrameClickable.transform.position.x - 400, 0.2f);
                            SetTouchActive(true);
                        });
                });
        }



        // ----------------------------------------------------------------------------------
        // ========================== Round Specifics ============================
        // ----------------------------------------------------------------------------------

        private void UpdateRoundValues()
        {
            mRoundScoreText.text = _roundScore.ToString();
            mRoundCandyText.text = _roundCandy.ToString();
        }

        private void StartRound()
        {
            mMonsterManager.PrepareRound();

            Monster selectedMonster = mMonsterManager.GetSelectedMonster();
            mRoundMessage.text = $"Selecione {selectedMonster.name}";

            mRoundMessageSpr
            .DOFade(1f, 1f)
            .OnComplete(() =>
            {
                // TODO: OpenDoor Animation (then delay)
                mDoorClickable.GetComponent<Image>()
                .DOFade(0, 0.5f)
                .OnComplete(() =>
                {
                    GameDebug.Log("Starting Round...", util.LogType.Transition);
                    mMonsterManager.StartRound();
                });
            });
        }

        private void NextRound()
        {
            GameDebug.Log("Next Round...", util.LogType.Transition);

            // Increment score/candy then update screen values
            _roundCandy += mBaseCandyPrize;
            _roundScore += mBaseScorePrize * _currentMultiplier;

            UpdateRoundValues();

            // Closing the Door
            // TODO: Closing the door animation
            mDoorClickable.GetComponent<Image>()
                .DOFade(1, 0.5f)
                .OnComplete(() =>
                {
                    GameDebug.Log("Starting Next Round...", util.LogType.Transition);
                    StartRound();
                });
        }

        // ----------------------------------------------------------------------------------
        // ========================== Server Communication ============================
        // ----------------------------------------------------------------------------------


        public SheetEntry? GetEntryByName(string name)
        {
            return mGoogleLoader.GetEntryByName(name);
        }

        public void LoadFromGoogle(Action callback = null)
        {
            // Load form from Google Drive
            mGoogleLoader.Load((entries) =>
            {
                mLeaderboardPoolController.DestroyAll();

                // Remove duplicated entries (keep newer one)
                List<SheetEntry> singleEntriesList = new List<SheetEntry>();
                foreach (var entry in entries.Reverse())
                {
                    if (singleEntriesList.Where(e => e.name == entry.name).ToArray().Length == 0)
                        singleEntriesList.Add(entry);
                }
                SheetEntry[] singleEntries = singleEntriesList.ToArray();

                // Sort
                Array.Sort<SheetEntry>(singleEntries,
                    (x, y) =>
                    {
                        var score = x.score.CompareTo(y.score);
                        if (score == 0)
                        {
                            var coins = x.coins.CompareTo(y.coins);
                            if (coins == 0)
                                return x.name.CompareTo(y.name);
                            return coins;
                        }
                        return score;
                    });
                foreach (var entry in singleEntries)
                {
                    // Instantiate leaderboard items
                    LeaderboardItem leaderboardItem = mLeaderboardPoolController.Spawn();
                    leaderboardItem.UpdateInfo(entry.name, entry.password, entry.coins, entry.score);
                }

                callback?.Invoke();
            });
        }
    }
}