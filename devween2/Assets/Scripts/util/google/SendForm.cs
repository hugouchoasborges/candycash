/*
 * Created by Mathew Ventures <http://www.mrventures.net/all-tutorials/collecting-player-feedback>
 * Edited by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

#define DEVELOPER

using core;
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
            Send(null, formEntries.ToArray());
        }

        private void UpdateTestMode()
        {
            GameController.Instance.LoadFromGoogle();
        }

        private IEnumerator Post(Action callback = null, params FormEntry[] entries)
        {
            string allData = "";

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

        public void Send(Action callback = null, params FormEntry[] entries)
        {
            StartCoroutine(Post(callback, entries));
        }
    }

    [System.Serializable]
    public struct FormEntry
    {
        public string formEntryId;
        [HideInInspector] public string entryStr;
    }

    [System.Serializable]
    public struct FormInput
    {
        public InputField input;
        public FormEntry entry;
    }
}