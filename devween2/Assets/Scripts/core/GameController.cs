/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using monster;
using leaderboard;
using UnityEngine;
using util.google;

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