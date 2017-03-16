using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Random = UnityEngine.Random;

public enum RoomsID
{
    Debref=9999,
    Bref=9998,
    Between=9997,
    Game=9996
}
public class Team
{ 
    public string TeamName;
    public int RoomNumber;
    public int TeamId;
    public int ScoreRoom;
    public float RoomTime;
    public int ScoreAll;
}

public class Room
{
    public int RoomTime;
    
}

public class Main : MonoBehaviour
{
    private Room _currentRoom;
    public Text RoomTimer;
    public Text Info;
    public InputField InputField;
    public Text Score;
    private float _localTimer;
    private string qrString = "";
    private bool background = true;
    private Team t;
    private bool _enteredRoom = false;
    private float _serverEllapsed = 0;





    public void IncScore()
    {
        Score.text = (Int32.Parse(Score.text) + 1).ToString();
    }
    void Start()
    {
        AndroidNFCReader.enableBackgroundScan();
        AndroidNFCReader.ScanNFC(gameObject.name, "OnFinishScan");

       

      
    }

    public void CreateTeam()
    {
        t=new Team();

        //t.teamId = PlayerPrefs.GetInt("teamId", 0); ;
        //if (t.teamId == 0)
        //{
        //    t.teamId = Random.Range(0, 333);
        //}
        //PlayerPrefs.SetInt("teamId", t.teamId);
        string url = "http://192.168.0.151:8000/";

        WWWForm form = new WWWForm();
        form.AddField("", "bla");
        byte[] rawData = form.data;
        var headers = new Dictionary<string, string>();

        headers.Add("Stage", "TeamCreation");
        headers.Add("Name", InputField.text);
        // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

        Debug.Log("opj");
        WWW www = new WWW(url, rawData, headers);

        StartCoroutine(RequestTeamCreation(www));
    }

   
    public void EnterRoom()
    {
        string url = "http://192.168.0.151:8000/";

        WWWForm form = new WWWForm();
        form.AddField("", "bla");
        byte[] rawData = form.data;
        var headers = new Dictionary<string, string>();

        headers.Add("Stage", "RoomEntering");
        headers.Add("TeamId", t.TeamId.ToString());
        // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

        WWW www = new WWW(url, rawData, headers);

        StartCoroutine(RoomEnterRequest(www));



    }

    public void LeaveRoom()
    {
        string url = "http://192.168.0.151:8000/";

        WWWForm form = new WWWForm();
        form.AddField("", "bla");
        byte[] rawData = form.data;
        var headers = new Dictionary<string, string>();
        t.ScoreRoom = int.Parse(Score.text);
        headers.Add("Stage", "RoomFinished");
        headers.Add("TeamId", t.TeamId.ToString());
        headers.Add("RoomNumber", t.RoomNumber.ToString());
        Debug.Log("Team:" +t.TeamId.ToString()+" leaving room with score:" +t.ScoreRoom.ToString() );
        headers.Add("RoomScore",t.ScoreRoom.ToString());
        
        // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

        WWW www = new WWW(url, rawData, headers);

        StartCoroutine(RoomEndRequest(www));


    }
    IEnumerator GetTimeRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            //   Debug.Log("WWW Ok!: " + www.data);

            //t = JsonUtility.FromJson<Team>(www.data);
            //Debug.Log(t.TeamName);
            //Info.text = t.TeamName + " is in room:" + (t.RoomNumber) + " \nID:" + t.TeamId + " CurrScore" + t.ScoreRoom +
            //            " All scores" + t.ScoreAll;
            //Debug.Log(t.TeamId+"----"+t.RoomNumber.ToString());
            // print(float.Parse( www.data));
            //print(www.responseHeaders+"SAASDO{{SAD{SA");
            foreach (var resp in www.responseHeaders)
            {
                print(resp.Key+""+resp.Value);
            }
           
            // Debug.Log(r.RoomTime);
            // t.RoomTime = r.RoomTime;
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    void Update()
    {

        _localTimer +=Time.deltaTime;
        if (_localTimer >= 2 && t!=null)
        {
            _localTimer = 0;
            string url = "http://192.168.0.151:8000/";
            var headers = new Dictionary<string, string>();
            headers.Add("GetTime", "True");

            WWW www = new WWW(url, null, headers);

            StartCoroutine(GetTimeRequest(www));
            Debug.Log("ROOM TIME AFTER COURUTINE "+t.RoomTime);
            if (t != null)
            {
                if (t.RoomTime > 0)
                {
                    //t.RoomTime -= (int)_serverEllapsed;
                    Debug.Log(_serverEllapsed);
                    RoomTimer.text = t.RoomTime.ToString();
                    if (t.RoomTime <= 0)
                    {
                        RoomTimer.text = "0";
                        LeaveRoom();
                    }
                    //_currentRoom.RoomTime -= ((int)Time.time - lastTime)* 1000;
                    //lastTime = (int) Time.time;
                    //RoomTimer.text = (_currentRoom.RoomTime / 1000).ToString();
                    //if (_currentRoom.RoomTime <= 0)
                    //{
                    //    string url = "http://192.168.0.151:8000/";
                    //    Debug.Log("RoomFinished localy");
                    //    WWWForm form = new WWWForm();
                    //    t.ScoreRoom = int.Parse(Score.text);
                    //    form.AddField("", "bla");

                    //    byte[] rawData = form.data;
                    //    var headers = new Dictionary<string, string>();

                    //    headers.Add("Stage", "RoomFinished");
                    //    headers.Add("RoomScore",t.ScoreRoom.ToString());
                    //    headers.Add("TeamId",t.TeamId.ToString());
                    //    headers.Add("RoomNumber", t.RoomNumber.ToString());

                    //    // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");
                    //    _enteredRoom = false;
                    //    WWW www = new WWW(url, rawData, headers);
                    //    StartCoroutine(WaitForRequest(www));

                    //}
                }
            }
        }

        
    }
    IEnumerator RoomEnterRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);

            //t = JsonUtility.FromJson<Team>(www.data);
            //Debug.Log(t.TeamName);
            //Info.text = t.TeamName + " is in room:" + (t.RoomNumber) + " \nID:" + t.TeamId + " CurrScore" + t.ScoreRoom +
            //            " All scores" + t.ScoreAll;
            //Debug.Log(t.TeamId+"----"+t.RoomNumber.ToString());
            var temp = JsonUtility.FromJson<Team>(www.data);
            
            Debug.Log("REQUEST team name:"+temp.TeamName+" team id"+temp.TeamId+ "\n " +temp.RoomTime);
            if (temp != null)
            {
                t = temp;
            }

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }


    IEnumerator GetTeamInfoRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);

            //t = JsonUtility.FromJson<Team>(www.data);
            //Debug.Log(t.TeamName);
            //Info.text = t.TeamName + " is in room:" + (t.RoomNumber) + " \nID:" + t.TeamId + " CurrScore" + t.ScoreRoom +
            //            " All scores" + t.ScoreAll;
            //Debug.Log(t.TeamId+"----"+t.RoomNumber.ToString());
            t = JsonUtility.FromJson<Team>(www.data);
            Debug.Log(t.TeamName);
            Info.text = t.TeamName + " is in room:" + (t.RoomNumber) + " \nID:" + t.TeamId + " CurrScore" + t.ScoreRoom +
                        " All scores" + t.ScoreAll;

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    IEnumerator RoomEndRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);

            t = JsonUtility.FromJson<Team>(www.data);
            Debug.Log("End room Request team Id:" +t.TeamId);
            Info.text = t.TeamName + " is in room:" + (t.RoomNumber) + " \nID:" + t.TeamId + " CurrScore" + t.ScoreRoom +
                        " All scores" + t.ScoreAll;
            //Debug.Log(t.TeamId+"----"+t.RoomNumber.ToString());

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    IEnumerator RequestTeamCreation(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);

            t = JsonUtility.FromJson<Team>(www.data);
 
            Info.text = t.TeamName + " is in room:" + (t.RoomNumber) + " \nID:" + t.TeamId + " CurrScore" + t.ScoreRoom +
                        " All scores" + t.ScoreAll;
            //Debug.Log(t.TeamId+"----"+t.RoomNumber.ToString());

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
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
                form.AddField("", t.TeamId);
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
