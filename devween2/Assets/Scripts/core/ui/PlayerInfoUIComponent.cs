/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    public class PlayerInfoUIComponent : UIComponent
    {
        [Header("Player Info Components")]
        [Space]
        public Text playerName;
        public Text playerScore;
        public Text playerCoins;
    }
}