/*
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

namespace core
{
    public class GameController : Singleton<GameController>
    {
        [Header("Network")]
        [SerializeField] private Loader mGoogleLoader;

        [Header("Menu Items")]
        [Space]
        [SerializeField] private ClickableComponent mDoorClickable;

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
                mDoorClickable.onPointerDown.AddListener(Play);
            });
        }

        private void Play()
        {
            GameDebug.Log("Starting Game...", util.LogType.Transition);

            // TODO: Remove all click Listeners
            mDoorClickable.onPointerDown.RemoveAllListeners();

            // TODO: Remove LeaderBoards + GameInfo
            // TODO: Zoom in the Door
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