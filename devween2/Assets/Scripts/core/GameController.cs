/*
 * Created by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

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
using monster;
using System.Collections;
using core.ui;

namespace core
{
    public class GameController : Singleton<GameController>
    {
        [Header("Network")]
        [SerializeField] private Loader mGoogleLoader;
        [SerializeField] private float mFetchServerEverySeconds;

        [Header("Menu Items")]
        [Space]
        [SerializeField] private ClickableComponent mDoorClickable;
        [SerializeField] private ClickableComponent mInfoFrameClickable;
        [SerializeField] private ClickableComponent mRankingClickable;

        [Header("Game Round")]
        [Space]
        [SerializeField] private RoundUIComponent roundUI;
        private int _roundScore;
        private int _roundCandy;

        [Header("Game Round Values")]
        [Space]
        [SerializeField] [Range(1, 50)] private int mBaseScorePrize = 5;
        [SerializeField] [Range(1, 50)] private int mBaseCandyPrize = 5;
        private int _currentMultiplier = 1;

        [Header("Game Controllers")]
        [Space]
        [SerializeField] private MonsterManager mMonsterManager;
        [SerializeField] private LeaderboardPoolController mLeaderboardPoolController;

        [Header("Login-out Menus")]
        [Space]
        [SerializeField] private LoginUIComponent loginUI;
        [SerializeField] private LogoutUIComponent logoutUI;

        [Header("User Info")]
        [Space]
        [SerializeField] private PlayerInfoUIComponent mLoggedUserUI;
        [SerializeField] private Player mLoggedUser;

        /// <summary>
        /// First call in the ENTIRE game
        /// </summary>
        private void Start()
        {
            LoadFromGoogle(() =>
            {
                mDoorClickable.onPointerDown.AddListener(Play);
                SetTouchActive(true);

                mMonsterManager.onGameOver = GameOver;
                mMonsterManager.onNextRound = NextRound;
                mMonsterManager.Init();

                loginUI.SetActive(true);
                loginUI.mLoginButton.onClick.AddListener(Login);
            });
        }


        // ----------------------------------------------------------------------------------
        // ========================== Login ============================
        // ----------------------------------------------------------------------------------

        private void Login()
        {
            // Remove login button listener
            loginUI.mLoginButton.onClick.RemoveAllListeners();

            List<FormEntry> formEntries = new List<FormEntry>()
            {
                new FormEntry(SendForm.Instance.kNameEntry, loginUI.mNameInput.text),     // Name
                new FormEntry(SendForm.Instance.kPasswordEntry, loginUI.mPasswordInput.text), // Password
            };

            if (SendForm.Instance.CheckValidEntry(formEntries.ToArray()))
            {
                mLoggedUser = new Player(loginUI.mNameInput.text, loginUI.mPasswordInput.text, 0, 0);

                var currentUserEntry = GetEntryByName(loginUI.mNameInput.text);
                if (currentUserEntry.HasValue)
                {
                    mLoggedUser.coins = currentUserEntry.Value.coins;
                    mLoggedUser.score = currentUserEntry.Value.score;
                }

                UpdateUIPlayerInfo();

                loginUI.mLoginFeedback.text = "<color=\"green\">SUCESSO</color>";

                // DELAY then hide login
                StartCoroutine(DelayCall(() => HideLoginPanel(), 2));
            }
            else
            {
                loginUI.mLoginFeedback.text = "<color=\"red\">ERRO</color>";

                // Add Login Listener back
                loginUI.mLoginButton.onClick.AddListener(Login);
            }
        }

        private void Logout()
        {
            logoutUI.mLogoutButton.onClick.RemoveAllListeners();
            loginUI.SetActive(true);
            logoutUI.SetActive(false);
            mLoggedUserUI.SetActive(false);

            mLoggedUser = null;

            loginUI.mLoginButton.onClick.AddListener(Login);
        }

        private void HideLoginPanel()
        {
            GameDebug.Log("Hiding Login Panel", util.LogType.Transition);
            loginUI.mLoginFeedback.text = "";
            loginUI.SetActive(false);
            logoutUI.SetActive(true);
            logoutUI.mLogoutButton.onClick.AddListener(Logout);

            // PlayerInfoUI
            mLoggedUserUI.SetActive(true);
        }


        // ----------------------------------------------------------------------------------
        // ========================== Game Flow ============================
        // ----------------------------------------------------------------------------------

        private void SetTouchActive(bool active)
        {
            mDoorClickable.touchable = active;
            mInfoFrameClickable.touchable = active;
            mRankingClickable.touchable = active;
        }

        private void Play()
        {
            GameDebug.Log("Starting Game...", util.LogType.Transition);

            // Remove click Listeners
            SetTouchActive(false);

            _roundScore = 0;
            _roundCandy = 0;

            UpdateRoundValues();

            // Remove LeaderBoards + GameInfoPanel
            mRankingClickable.transform.DOMoveX(mRankingClickable.transform.position.x - 400, 0.2f);
            mInfoFrameClickable.transform.DOMoveX(mInfoFrameClickable.transform.position.x + 400, 0.2f);

            // Show round values on screen
            roundUI.mRoundValuesPanel.transform.DOLocalMoveY(0, 1);

            // Zoom in the Door
            mDoorClickable.transform.DOScale(2f, 1f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
            {
                StartRound();
            });
        }

        private void GameOver()
        {
            GameDebug.Log("GameOver...", util.LogType.Transition);

            // Closing the door
            // TODO: Closing the door animation
            mDoorClickable.GetComponent<Image>()
                .DOFade(1, 0.5f)
                .OnComplete(() =>
                {
                    // Hide the monsters
                    mMonsterManager.SetMonstersAlpha(0);

                    // Hide the message
                    roundUI.mRoundMessageSpr.alpha = 0;

                    // Zoom out the Door
                    mDoorClickable.transform.DOScale(1f, 1f)
                        .SetEase(Ease.OutQuad)
                        .OnComplete(() =>
                        {
                            // Remove RoundValuesPanel from screen
                            roundUI.mRoundValuesPanel.transform.DOLocalMoveY(200, 0.5f);

                            // Add LeaderBoards + GameInfoPanel
                            mRankingClickable.transform.DOMoveX(mRankingClickable.transform.position.x + 400, 0.2f);
                            mInfoFrameClickable.transform.DOMoveX(mInfoFrameClickable.transform.position.x - 400, 0.2f);
                            SetTouchActive(true);

                            UpdatePlayerInfo();
                            SendValuesToServer();
                        });
                });
        }

        private void UpdateUIPlayerInfo()
        {
            // Update LoggedUser UI info
            mLoggedUserUI.playerName.text = mLoggedUser.name;
            mLoggedUserUI.playerCoins.text = mLoggedUser.coins.ToString();
            mLoggedUserUI.playerScore.text = mLoggedUser.score.ToString();
        }

        private void UpdatePlayerInfo()
        {
            mLoggedUser.coins += _roundCandy;
            mLoggedUser.score += _roundScore;

            UpdateUIPlayerInfo();
        }

        private void SendValuesToServer()
        {
            if (mLoggedUser == null)
            {
                GameDebug.LogError("User not logged... Not sending any data to server", util.LogType.Round);
                LoadFromGoogle();
                return;
            }

            List<FormEntry> formEntries = new List<FormEntry>()
            {
                new FormEntry(SendForm.Instance.kNameEntry, mLoggedUser.name),     // Name
                new FormEntry(SendForm.Instance.kPasswordEntry, mLoggedUser.password), // Password
                
                new FormEntry(SendForm.Instance.kScoreEntry, mLoggedUser.score.ToString()),    // Score
                new FormEntry(SendForm.Instance.kCoinsEntry, mLoggedUser.coins.ToString()),    // Coins
            };

            // Send the new Score then load all data from GoogleDocs again
            SendForm.Instance.Send(() => LoadFromGoogle(), formEntries.ToArray());
        }

        // ----------------------------------------------------------------------------------
        // ========================== Round Specifics ============================
        // ----------------------------------------------------------------------------------

        private void UpdateRoundValues()
        {
            roundUI.mRoundScoreText.text = _roundScore.ToString();
            roundUI.mRoundCandyText.text = _roundCandy.ToString();
        }

        private void StartRound()
        {
            mMonsterManager.PrepareRound();

            Monster selectedMonster = mMonsterManager.GetSelectedMonster();
            roundUI.mRoundMessage.text = $"Selecione {selectedMonster.name}";

            roundUI.mRoundMessageSpr
            .DOFade(1f, 1f)
            .OnComplete(() =>
            {
                // TODO: OpenDoor Animation (then delay)
                mDoorClickable.GetComponent<Image>()
                .DOFade(0, 0.5f)
                .OnComplete(() =>
                {
                    GameDebug.Log("Starting Round...", util.LogType.Transition);
                    mMonsterManager.StartRound();
                });
            });
        }

        private void NextRound()
        {
            GameDebug.Log("Next Round...", util.LogType.Transition);

            // Increment score/candy then update screen values
            _roundCandy += mBaseCandyPrize;
            _roundScore += mBaseScorePrize * _currentMultiplier;

            UpdateRoundValues();

            // Closing the Door
            // TODO: Closing the door animation
            mDoorClickable.GetComponent<Image>()
                .DOFade(1, 0.5f)
                .OnComplete(() =>
                {
                    GameDebug.Log("Starting Next Round...", util.LogType.Transition);
                    StartRound();
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
                // Remove also zero score\candy entries
                singleEntriesList = singleEntriesList.Where(e => e.coins > 0 && e.score > 0).ToList();

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
                for (int i = 0; i < singleEntries.Length; i++)
                {
                    // Instantiate leaderboard items
                    var entry = singleEntries[i];
                    LeaderboardItem leaderboardItem = mLeaderboardPoolController.Spawn();
                    leaderboardItem.UpdateInfo(entry.name, entry.password, entry.coins, entry.score, singleEntries.Length - i);
                }

                callback?.Invoke();

                // Remove all delay calls
                StopAllCoroutines();

                // Adds a Server fetch delayed call
                StartCoroutine(DelayCall(() => LoadFromGoogle(null), mFetchServerEverySeconds));
            });
        }

        private IEnumerator DelayCall(Action call, float delay)
        {
            GameDebug.Log($"DelayCall ==> {call.Method.Name}", util.LogType.Thread);
            yield return new WaitForSeconds(delay);
            call.Invoke();
        }
    }

    [Serializable]
    public class Player
    {
        public string name;
        public string password;

        public int score;
        public int coins;

        public Player(string name, string password, int score, int coins)
        {
            this.name = name;
            this.password = password;
            this.score = score;
            this.coins = coins;
        }
    }
}