/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    public class RoundUIComponent : UIComponent
    {
        [Header("Game Round")]
        [Space]
        public CanvasGroup mRoundMessageSpr;
        public Text mRoundMessage;

        public GameObject mRoundValuesPanel;
        public Text mRoundScoreText;
        public Text mRoundCandyText;
    }
}