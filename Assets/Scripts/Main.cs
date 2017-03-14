using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team
{
    public string RoomNumber { get; set; }
    public int teamId { get; set; }
}
public class Main : MonoBehaviour
{

    string qrString = "";
    bool background = true;
    private Team t = new Team();

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
        t.RoomNumber ="0";
        string url = "http://127.168.0.151:8080/";
        
        WWWForm form = new WWWForm();
        form.AddField("", t.RoomNumber);
        form.AddField("", t.teamId);
        byte[] rawData = form.data;
        //headers.Add("User-Agent", "ceilometerclient.openstack.common.apiclient");
        // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

        Debug.Log("opj");
        WWW www = new WWW(url, rawData);

        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);
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
        Debug.Log(qrString);
        GUI.Label(new Rect(0, 0, Screen.width, 50), "Result: " + qrString+"new Version");
        if (qrString.Length < 3)
        {
            string url = "http://127.168.0.151:8080/";
            t.RoomNumber = qrString;
            WWWForm form = new WWWForm();
            form.AddField("", t.RoomNumber);
            form.AddField("", t.teamId);
            byte[] rawData = form.data;
            var headers = new Dictionary<string, string>();

            WWW www = new WWW(url, rawData, headers);

            StartCoroutine(WaitForRequest(www));
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
