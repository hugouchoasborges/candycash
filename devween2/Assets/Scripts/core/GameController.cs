/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using monster;
using leaderboard;
using UnityEngine;
using util.google;
using System;
using google;

namespace core
{
    public class GameController : Singleton<GameController>
    {
        [Header("Network")]
        [SerializeField] private Loader mGoogleLoader;

        [Header("Game Controllers")]
        [Space]
        [SerializeField] private MonsterPoolController mMonsterPoolController;
        [SerializeField] private LeaderboardPoolController mLeaderboardPoolController;

        /// <summary>
        /// First call in the ENTIRE game
        /// </summary>
        private void Start()
        {
            LoadFromGoogle();
        }

        public void LoadFromGoogle()
        {
            // Load form from Google Drive
            mGoogleLoader.Load((entries) =>
            {
                mLeaderboardPoolController.DestroyAll();
                Array.Sort<SheetEntry>(entries,
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
                // TODO: Remove duplicated entries (keep newer ones)
                foreach (var entry in entries)
                {
                    // Instantiate leaderboard items
                    LeaderboardItem leaderboardItem = mLeaderboardPoolController.Spawn();
                    leaderboardItem.UpdateInfo(entry.name, entry.password, entry.coins, entry.score);
                }
            });
        }
    }
}