/*
 * Created by Mathew Ventures <http://www.mrventures.net/all-tutorials/collecting-player-feedback>
 * Edited by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace util
{
    public class GoogleCommunication : Singleton<GoogleCommunication>
    {
        [Header("Buttons")]
        [SerializeField] private InputField _nameInput;
        [SerializeField] private Button _sendButton;

        [Header("GForm data")]
        [Space]
        [SerializeField] private string kGFormBaseURL = "https://docs.google.com/forms/d/e/1FAIpQLSfRTCs6_jGFeu7Uiq4Oj318bybaygR-CwzEH19plbaH1dW2Fg/";
        [SerializeField] private string kGFormEntryID = "entry.1916679377";

        private void Awake()
        {
            _sendButton.onClick.AddListener(delegate { Send(); });
        }

        private IEnumerator Post<T>(T dataContainer)
        {
            bool isString = dataContainer is string;
            string jsonData = isString ? dataContainer.ToString() : JsonUtility.ToJson(dataContainer);

            WWWForm form = new WWWForm();
            form.AddField(kGFormEntryID, jsonData);
            string urlGFormResponse = kGFormBaseURL + "formResponse";
            using (UnityWebRequest www = UnityWebRequest.Post(urlGFormResponse, form))
            {
                yield return www.SendWebRequest();
                GameDebug.Log(www.isNetworkError
                    ? www.error
                    : $"Form upload complete!\nMessage: {jsonData}",
                    LogType.Web);
            }
        }

        public void Send()
        {
            StartCoroutine(Post(_nameInput.text));
        }
    }
}