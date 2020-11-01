/*
 * Created by Mathew Ventures <http://www.mrventures.net/all-tutorials/downloading-google-sheets>
 */

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace util.google
{
    public static class CSVDownloader
    {
        private const string k_googleSheetDocID = "1CvhFP6J1Hn44uIWvt4ikkUKn4HH3KxB1iLEwst0Bnvk";
        private const string url = "https://docs.google.com/spreadsheets/d/" + k_googleSheetDocID + "/export?format=csv";

        internal static IEnumerator DownloadData(System.Action<string> onCompleted)
        {
            yield return new WaitForEndOfFrame();

            string downloadData = null;
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {

                yield return webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    GameDebug.Log("Download Error: " + webRequest.error, LogType.Web);
                    downloadData = PlayerPrefs.GetString("LastDataDownloaded", null);
                    string versionText = PlayerPrefs.GetString("LastDataDownloaded", null);
                    GameDebug.Log("Using stale data version: " + versionText, LogType.Web);
                }
                else
                {
                    GameDebug.Log("Download success", LogType.Web);
                    GameDebug.Log("Data: " + webRequest.downloadHandler.text, LogType.Web);

                    // First term will be preceeded by version number, e.g. "100=English"
                    //string versionSection = webRequest.downloadHandler.text.Substring(0, 5);
                    //int equalsIndex = versionSection.IndexOf('=');
                    //UnityEngine.Assertions.Assert.IsFalse(equalsIndex == -1, "Could not find a '=' at the start of the CVS");

                    //string versionText = webRequest.downloadHandler.text.Substring(0, equalsIndex);
                    //GameDebug.Log("Downloaded data version: " + versionText, LogType.Web);

                    PlayerPrefs.SetString("LastDataDownloaded", webRequest.downloadHandler.text);
                    //PlayerPrefs.SetString("LastDataDownloadedVersion", versionText);

                    downloadData = webRequest.downloadHandler.text;
                    //downloadData = webRequest.downloadHandler.text.Substring(/*equalsIndex + */1);
                }
            }

            onCompleted(downloadData);
        }
    }
}