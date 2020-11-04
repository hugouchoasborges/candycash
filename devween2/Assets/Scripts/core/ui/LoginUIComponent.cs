/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using UnityEngine;
using UnityEngine.UI;

namespace core.ui
{
    public class LoginUIComponent : UIComponent
    {
        [Header("Login Menus")]
        [Space]
        public InputField mNameInput;
        public InputField mPasswordInput;

        public Button mLoginButton;
        public Text mLoginFeedback;
    }
}