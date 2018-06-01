using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using summerschoolai;
using System;
using SimpleJSONGameAI;

public class GameManager : MonoBehaviour
{

    public string API_key;
    public string session_id = Guid.NewGuid().ToString("D"); // need it at the start of the game

    public GameObject minotaur;
    public GameObject player;

    // Use this for initialization
    void Start()
    {
        var jsonDict = new JSONObject();
        jsonDict.Add("session", this.session_id); // generated at the start of a game
        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        jsonDict.Add("ts", unixTimestamp);
        jsonDict.Add("event", "LAUNCH"); // e.g move, stop, spawned, raycast
        summerschoolai.detail.SummerSchoolAIHttpClient.instance.sendPost(this.API_key, jsonDict);

        InvokeRepeating("SendAnalytics", 0.0f, 5.0f);
	}

    private void SendAnalytics()
    {
        var jsonDict = new JSONObject();
        jsonDict.Add("session", this.session_id); // generated at the start of a game
        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
        jsonDict.Add("ts", unixTimestamp);
        jsonDict.Add("event", "move"); // e.g move, stop, spawned, raycast
        jsonDict.Add ("type","mino"); // player or mino
        jsonDict.Add("x", minotaur.transform.position.x); // x_coordinate
        jsonDict.Add("y", minotaur.transform.position.y); // y_coordinate

        summerschoolai.detail.SummerSchoolAIHttpClient.instance.sendPost(this.API_key, jsonDict);
        jsonDict = new JSONObject();
        jsonDict.Add("ts", unixTimestamp);
        jsonDict.Add("session", this.session_id); // generated at the start of a game
        jsonDict.Add("event", "move"); // e.g move, stop, spawned, raycast
        jsonDict.Add("type", "player"); // player or mino
        jsonDict.Add("x", player.transform.position.x); // x_coordinate
        jsonDict.Add("y", player.transform.position.y); // y_coordinate
        summerschoolai.detail.SummerSchoolAIHttpClient.instance.sendPost(this.API_key, jsonDict);

    }
}
