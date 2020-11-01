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
        public new string name
        {
            get => _nameText.text;
            set => _nameText.text = value;
        }

        [Header("Player Info")]
        [Space]
        [SerializeField] private string username;
        [SerializeField] private string userpass;
        [SerializeField] private int coins;
        [SerializeField] private int maxScore;


    }
}