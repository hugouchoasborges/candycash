/*
 * Created by Mathew Ventures <http://www.mrventures.net/all-tutorials/downloading-google-sheets>
 * Edited by Hugo Uchoas Borges <hugouchoas@outlook.com>
 */

using google;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace util.google
{
    public class Loader : MonoBehaviour
    {
        private int _progress = 0;
        private List<string> _headers = new List<string>();

        private List<SheetEntry> _entries = new List<SheetEntry>();

        private Action<SheetEntry[]> _afterProcessDataCallback;

        public void Load(Action<SheetEntry[]> callback = null)
        {
            _afterProcessDataCallback = callback;
            StartCoroutine(CSVDownloader.DownloadData(AfterDownload));
        }

        public void AfterDownload(string data)
        {
            if (null == data)
            {
                GameDebug.LogError("Was not able to download data or retrieve stale data.", LogType.Web);
                // TODO: Display a notification that this is likely due to poor internet connectivity
                //       Maybe ask them about if they want to report a bug over this, though if there's no internet I guess they can't
            }
            else
            {
                StartCoroutine(ProcessData(data, AfterProcessData));
            }
        }

        private void AfterProcessData(string errorMessage)
        {
            if (null != errorMessage)
            {
                GameDebug.LogError("Was not able to process data: " + errorMessage, LogType.Web);
                // TODO: 
            }
            else
            {
                _afterProcessDataCallback?.Invoke(_entries.ToArray());
            }
        }

        public IEnumerator ProcessData(string data, System.Action<string> onCompleted)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Line level
            int currLineIndex = 0;
            bool inQuote = false;
            int linesSinceUpdate = 0;
            int kLinesBetweenUpdate = 15;

            // Entry level
            string currEntry = "";
            int currCharIndex = 0;
            bool currEntryContainedQuote = false;
            List<string> currLineEntries = new List<string>();

            // "\r\n" means end of line and should be only occurence of '\r' (unless on macOS/iOS in which case lines ends with just \n)
            char lineEnding = IsIOS() ? '\n' : '\r';
            int lineEndingLength = IsIOS() ? 1 : 2;

            while (currCharIndex <= data.Length)
            {
                if (!inQuote && (currCharIndex == data.Length || data[currCharIndex] == lineEnding))
                {
                    // Skip the line ending
                    currCharIndex += lineEndingLength;

                    // Wrap up the last entry
                    // If we were in a quote, trim bordering quotation marks
                    if (currEntryContainedQuote)
                    {
                        currEntry = currEntry.Substring(1, currEntry.Length - 2);
                    }

                    currLineEntries.Add(currEntry);
                    currEntry = "";
                    currEntryContainedQuote = false;

                    // Line ended
                    ProcessLineFromCSV(currLineEntries, currLineIndex);
                    currLineIndex++;
                    currLineEntries = new List<string>();

                    linesSinceUpdate++;
                    if (linesSinceUpdate > kLinesBetweenUpdate)
                    {
                        linesSinceUpdate = 0;
                        yield return new WaitForEndOfFrame();
                    }
                }
                else
                {
                    if (data[currCharIndex] == '"')
                    {
                        inQuote = !inQuote;
                        currEntryContainedQuote = true;
                    }

                    // Entry level stuff
                    {
                        if (data[currCharIndex] == ',')
                        {
                            if (inQuote)
                            {
                                currEntry += data[currCharIndex];
                            }
                            else
                            {
                                // If we were in a quote, trim bordering quotation marks
                                if (currEntryContainedQuote)
                                {
                                    currEntry = currEntry.Substring(1, currEntry.Length - 2);
                                }

                                currLineEntries.Add(currEntry);
                                currEntry = "";
                                currEntryContainedQuote = false;
                            }
                        }
                        else
                        {
                            currEntry += data[currCharIndex];
                        }
                    }
                    currCharIndex++;
                }

                _progress = (int)((float)currCharIndex / data.Length * 100.0f);
            }

            onCompleted(null);
        }

        private void ProcessLineFromCSV(List<string> currLineElements, int currLineIndex)
        {

            // This line contains the column headers
            if (currLineIndex == 0)
            {
                _headers = new List<string>();
                for (int columnIndex = 0; columnIndex < currLineElements.Count; columnIndex++)
                {
                    string currHeader = currLineElements[columnIndex];
                    _headers.Add(currHeader);
                }
                UnityEngine.Assertions.Assert.IsFalse(_headers.Count == 0);
            }
            // This is a normal node
            else if (currLineElements.Count > 1)
            {
                CreateSheetEntry(currLineElements);
            }
            else
            {
                GameDebug.LogError("Database line did not fall into one of the expected categories.", LogType.Web);
            }
        }

        private void CreateSheetEntry(List<string> currLine)
        {
            _entries.Add(new SheetEntry()
            {
                timeStamp = currLine[0],
                name = currLine[1],
            });
        }

        private bool IsAndroid()
        {
#if UNITY_IOS
            return false;
#endif
            return true;
        }

        private bool IsIOS()
        {
            return !IsAndroid();
        }
    }
}