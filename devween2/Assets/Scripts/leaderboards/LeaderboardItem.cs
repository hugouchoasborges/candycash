/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.UI;

namespace leaderboard
{
    public class LeaderboardItem : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _coinsText;
        [SerializeField] private Text _scoreText;

        [Header("Player Info")]
        [Space]
        [SerializeField] private string username;
        [SerializeField] private string userpass;
        [SerializeField] private int coins;
        [SerializeField] private int maxScore;

        public void UpdateInfo(string username, string userpass, int coins, int maxScore)
        {
            this.username = username;
            this.userpass = userpass;
            this.coins = coins;
            this.maxScore = maxScore;

            _nameText.text = username;
            _coinsText.text = coins.ToString();
            _scoreText.text = maxScore.ToString();
        }
    }
}