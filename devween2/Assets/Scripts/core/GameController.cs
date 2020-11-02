﻿/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using monster;
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

        [Header("Game Controllers")]
        [Space]
        [SerializeField] private MonsterPoolController mMonsterPoolController;
        [SerializeField] private LeaderboardPoolController mLeaderboardPoolController;

        /// <summary>
        /// First call in the ENTIRE game
        /// </summary>
        private void Start()
        {
            LoadFromGoogle(() =>
            {
                SetDefaultListeners();
            });
        }


        // ----------------------------------------------------------------------------------
        // ========================== Game Flow ============================
        // ----------------------------------------------------------------------------------

        private void RemoveAllListeners()
        {
            mDoorClickable.onPointerDown.RemoveAllListeners();
            mInfoFrameClickable.onPointerDown.RemoveAllListeners();
            mRankingClickable.onPointerDown.RemoveAllListeners();
        }

        private void SetDefaultListeners()
        {
            RemoveAllListeners();
            mDoorClickable.onPointerDown.AddListener(Play);
            //mInfoFrameClickable.onPointerDown.AddListener(ShowPlayerInfo);
        }

        private void Play()
        {
            GameDebug.Log("Starting Game...", util.LogType.Transition);

            // Remove click Listeners
            RemoveAllListeners();

            // Remove LeaderBoards + GameInfoPanel
            mRankingClickable.transform.DOMoveX(mRankingClickable.transform.position.x - 400, 0.2f);
            mInfoFrameClickable.transform.DOMoveX(mInfoFrameClickable.transform.position.x + 400, 0.2f);

            // Zoom in the Door
            mDoorClickable.transform.DOScale(2f, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
            {
                // TODO: Show Message (delay)
                mRoundMessageSpr
                .DOFade(1f, 1f)
                .OnComplete(() =>
                {
                    // TODO: OpenDoor Animation (delay)
                    mDoorClickable.GetComponent<Image>()
                    .DOFade(0, 0.5f)
                    .OnComplete(() =>
                    {
                        GameDebug.Log("Starting Round...", util.LogType.Transition);
                    });
                });
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