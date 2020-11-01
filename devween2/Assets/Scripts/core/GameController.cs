/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using monster;
using leaderboard;
using UnityEngine;
using util.google;

namespace core
{
    public class GameController : MonoBehaviour
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
            // Load form from Google Drive
            mGoogleLoader.Load((entries) =>
            {
                foreach (var entry in entries)
                {
                    // Instantiate leaderboard items
                    LeaderboardItem leaderboardItem = mLeaderboardPoolController.Spawn();
                    leaderboardItem.name = entry.name;
                }
            });
        }
    }
}