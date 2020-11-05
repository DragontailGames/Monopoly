using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;


public class Loader : MonoBehaviour
{
    #region singleton
    public static Loader instance = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    #endregion


    private System.Action<List<string>, int> process;

    private System.Action endCalls;

    void Initialize()
    {

    }

    public void StartLoader(string path, System.Action<List<string>, int> process, System.Action endCalls, string guidId)
    {
        this.process = process;
        this.endCalls = endCalls;
        StartCoroutine(CSVDownloader.DownloadData(AfterDownload, path, guidId));
    }

    public void AfterDownload(string data)
    {
        if (null == data)
        {
            Debug.LogError("Was not able to download data or retrieve stale data.");
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
            Debug.LogError("Was not able to process data: " + errorMessage);
            // TODO: 
        }
        else
        {
           endCalls?.Invoke();
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
        char lineEnding = Application.platform == RuntimePlatform.IPhonePlayer ?'\n': '\r';
        int lineEndingLength = Application.platform == RuntimePlatform.IPhonePlayer ? 1 :2;

        data += "\r\n";

        while (currCharIndex < data.Length)
        {
            if (!inQuote && (data[currCharIndex] == lineEnding))
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
                process(currLineEntries, currLineIndex);
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
        }

        onCompleted(null);
    }
}
