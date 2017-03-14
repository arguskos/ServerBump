using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;


public class Team
{
    public int  RoomNumber { get; set; }
    public int teamId { get; set; }
    public int ScoreRoom { get; set; }
    public int ScoreAll { get; set; }
}

public class Room
{
    public int RoomTime;
    
}
public class Main : MonoBehaviour
{
    private Room _currentRoom;
    public Text RoomTimer;
    public Text Score;
    string qrString = "";
    bool background = true;
    private Team t = new Team();
    private int lastTime;

    public void IncScore()
    {
        Score.text = (Int32.Parse(Score.text) + 1).ToString();
    }
    void Start()
    {
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC(gameObject.name, "OnFinishScan");

       

        t.teamId = PlayerPrefs.GetInt("teamId",0); ;
        if (t.teamId == 0)
        {
            t.teamId = Random.Range(0, 333);
        }
        PlayerPrefs.SetInt("teamId",t.teamId);
        t.RoomNumber =0;
        string url = "http://192.168.0.151:8000/";
        
        WWWForm form = new WWWForm();
        form.AddField("", t.RoomNumber);
        form.AddField("", t.teamId);
        byte[] rawData = form.data;
        var headers = new Dictionary<string, string>();

        headers.Add("Stage", "TeamCreation");
        // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

        Debug.Log("opj");
        WWW www = new WWW(url, rawData,headers);

        StartCoroutine(WaitForRequest(www));
    }

    void Update()
    {
        if (_currentRoom!=null)
        {
            if (_currentRoom.RoomTime > 0)
            {
                _currentRoom.RoomTime -= ((int)Time.time - lastTime)* 1000;
                lastTime = (int) Time.time;
                RoomTimer.text = (_currentRoom.RoomTime / 1000).ToString();
                if (_currentRoom.RoomTime <= 0)
                {
                    string url = "http://192.168.0.151:8000/";
                    Debug.Log("RoomFinished localy");
                    WWWForm form = new WWWForm();
                    t.ScoreRoom = int.Parse(Score.text);
                    form.AddField("", t.teamId);

                    byte[] rawData = form.data;
                    var headers = new Dictionary<string, string>();
                    
                    headers.Add("Stage", "RoomFinished");
                    headers.Add("RoomScore",t.ScoreRoom.ToString());
                    headers.Add("TeamId",t.teamId.ToString());
                    headers.Add("RoomNumber", t.RoomNumber.ToString());

                    // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

                    WWW www = new WWW(url, rawData, headers);
                    StartCoroutine(WaitForRequest(www));

                }
            }
        }
    }
    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);
            
            var temp =JsonUtility.FromJson<Room>(www.data);
            if (temp != null)
            {
                _currentRoom = temp;
                RoomTimer.text = (_currentRoom.RoomTime/1000).ToString();
            }

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    void OnGUI()
    {
        if (!background)
        {
            // Scan NFC button
            if (GUI.Button(new Rect(0, Screen.height - 50, Screen.width, 50), "Scan NFC"))
            {
                AndroidNFCReader.ScanNFC(gameObject.name, "OnFinishScan");
            }
            if (GUI.Button(new Rect(0, Screen.height - 100, Screen.width, 50), "Enable Backgraound Mode"))
            {
                AndroidNFCReader.enableBackgroundScan();
                background = true;
            }
        }
        else
        {
            if (GUI.Button(new Rect(0, Screen.height - 50, Screen.width, 50), "Disable Backgraound Mode"))
            {
                AndroidNFCReader.disableBackgroundScan();
                background = false;
            }
        }
        if (qrString != "NO_ALLOWED_OS")
        {
            Debug.Log(qrString);
            GUI.Label(new Rect(0, 0, Screen.width, 50), "Result: " + qrString + "new Version");
            if (qrString.Length < 3)
            {
                string url = "http://192.168.0.151:8000/";
                t.RoomNumber = Int32.Parse(qrString);
                WWWForm form = new WWWForm();
                form.AddField("", t.RoomNumber);
                form.AddField("", t.teamId);
                byte[] rawData = form.data;
                var headers = new Dictionary<string, string>();
                headers.Add("Stage", "RoomEntery");

                WWW www = new WWW(url, rawData, headers);

                StartCoroutine(WaitForRequest(www));
            }
        }
    }

    // NFC callback
    void OnFinishScan(string result)
    {

        // Cancelled
        if (result == AndroidNFCReader.CANCELLED)
        {

            // Error
        }
        else if (result == AndroidNFCReader.ERROR)
        {


            // No hardware
        }
        else if (result == AndroidNFCReader.NO_HARDWARE)
        {
        }


        qrString = getToyxFromUrl(result);
    }

    // Extract toyxId from url
    string getToyxFromUrl(string url)
    {
        int index = url.LastIndexOf('/') + 1;

        if (url.Length > index)
        {
            return url.Substring(index);
        }

        return url;
    }

}
