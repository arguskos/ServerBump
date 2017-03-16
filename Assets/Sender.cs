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
       // t.RoomNumber = 0;
       // string url = "http://192.168.0.151:8000/";

       // WWWForm form = new WWWForm();
       // form.AddField("",t.RoomNumber);
       // byte[] rawData = form.data;
       // var headers = new Dictionary<string, string>();
       // //headers.Add("User-Agent", "ceilometerclient.openstack.common.apiclient");
       //// headers.Add("X-Auth-Token", "7da4596d42e24f9798d73ec40bbbbd81");

     
       // WWW www = new WWW(url,rawData,headers);
       // StartCoroutine(WaitForRequest(www));

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
