/*
 * Created by Mathew Ventures <http://www.mrventures.net/all-tutorials/collecting-player-feedback>
 * Edited by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

//#define DEVELOPER

using core;
using google;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace util.google
{
    public class SendForm : Singleton<SendForm>
    {
        [Header("GForm data")]
        [Space]
        [SerializeField] private string kGFormBaseURL = "";
        [SerializeField] private string kPrimaryKey = "";
        [SerializeField] private string kPasswordKey = "";

        [Header("GForm Keys")]
        [Space]
        public string kNameEntry = "";
        public string kPasswordEntry = "";
        public string kScoreEntry = "";
        public string kCoinsEntry = "";

        [Header("Test components")]
        [SerializeField] private bool _testMode;
        [SerializeField] private GameObject _testPanel;
        [SerializeField] private Button _sendButton;
        [SerializeField] private Button _updateButton;
        [SerializeField] private FormInput[] _formInputs;

        private void Awake()
        {
#if UNITY_EDITOR || DEVELOPER
            if (_testMode)
            {
                _testPanel.SetActive(true);
                _sendButton.onClick.AddListener(delegate { SendTestMode(); });
                _updateButton.onClick.AddListener(delegate { UpdateTestMode(); });
            }
            else
                _testPanel.SetActive(false);
#else
            _testPanel.SetActive(false);
#endif
        }

        private void SendTestMode()
        {
            List<FormEntry> formEntries = new List<FormEntry>();

            foreach (var input in _formInputs)
            {
                formEntries.Add(new FormEntry()
                {
                    formEntryId = input.entry.formEntryId,
                    entryStr = input.input.text
                });
            }

            // Send the new Score then load all data from GoogleDocs again
            Send(UpdateTestMode, formEntries.ToArray());
        }

        private void UpdateTestMode()
        {
            GameController.Instance.LoadFromGoogle();
        }

        public bool CheckValidEntry(params FormEntry[] entries)
        {
            SheetEntry? currEntry = null;

            foreach (var entry in entries)
            {
                if (entry.formEntryId != kPrimaryKey)
                    continue;

                // Check if the entry already exists
                currEntry = GameController.Instance.GetEntryByName(entry.entryStr);
                if (currEntry.HasValue)
                    break;
                else
                {
                    // Return true if the entry is new
                    if (string.IsNullOrEmpty(entry.entryStr))
                    {
                        GameDebug.LogError("Primary Key not filled", LogType.Web);
                        return false;
                    }
                    return true;
                }
            }

            // Check For Password
            if (currEntry.HasValue)
            {
                foreach (var entry in entries)
                {
                    if (entry.formEntryId == kPasswordKey)
                    {
                        if (entry.entryStr == currEntry.Value.password)
                            return true;

                        if (string.IsNullOrEmpty(entry.entryStr))
                            GameDebug.LogError("Password not filled", LogType.Web);
                        else
                            GameDebug.LogError("Wrong Password", LogType.Web);
                        return false;
                    }
                }

                GameDebug.LogError("Password not filled", LogType.Web);
                return false;
            }

            // PrimaryKey not found
            GameDebug.LogError("Primary Key not filled", LogType.Web);
            return false;
        }

        private IEnumerator Post(Action callback = null, params FormEntry[] entries)
        {
            string allData = "";

            if (!CheckValidEntry(entries))
                yield break;

            WWWForm form = new WWWForm();
            foreach (var entry in entries)
            {
                allData += entry.entryStr + ", ";
                form.AddField(entry.formEntryId, entry.entryStr);
            }

            string urlGFormResponse = kGFormBaseURL + "formResponse";
            using (UnityWebRequest www = UnityWebRequest.Post(urlGFormResponse, form))
            {
                yield return www.SendWebRequest();
                GameDebug.Log(www.isNetworkError
                    ? www.error
                    : $"Form upload complete!\nMessage: {allData}",
                    LogType.Web);

                callback?.Invoke();
            }
        }

        private IEnumerator DelayCallback(Action callback)
        {
            yield return new WaitForSeconds(1f);
            callback.Invoke();
        }

        public void Send(Action callback = null, params FormEntry[] entries)
        {
            StartCoroutine(Post(() => StartCoroutine(DelayCallback(callback)), entries));
        }
    }

    [System.Serializable]
    public struct FormEntry
    {
        public string formEntryId;
        [HideInInspector] public string entryStr;

        public FormEntry(string formEntryId, string entryStr)
        {
            this.formEntryId = formEntryId;
            this.entryStr = entryStr;
        }
    }

    [System.Serializable]
    public struct FormInput
    {
        public InputField input;
        public FormEntry entry;
    }
}