using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Sender : MonoBehaviour
{
    private Team t=new Team();
    public Text tesd;
    void Start()
    {
        t.teamId = 439243;
        t.RoomNumber = 0.ToString();
        string url = "http://127.168.0.151:8000/";

        WWWForm form = new WWWForm();
        form.AddField("",t.RoomNumber);
        form.AddField("",t.teamId );
        byte[] rawData = form.data;
        var headers = new Dictionary<string, string>();
        //headers.Add("User-Agent", "ceilometerclient.openstack.common.apiclient");
       // headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

     
        WWW www = new WWW(url,rawData,headers);
        StartCoroutine(WaitForRequest(www));

    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.data);
            tesd.text = www.data;

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
            tesd.text = www.error;

        }

    }
}
