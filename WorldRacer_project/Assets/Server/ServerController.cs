﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class Playfield
{
    public List<Vector2> points = new List<Vector2>();
}

public class ServerController : MonoBehaviour
{
    public string serverAddress;
    private string uri;

    [System.NonSerialized]
    public string roomPin;
    [System.NonSerialized]
    public string playerName;

    public UIManager uiManager;


    void Start()
    {
        string prefix = "http://";

        if (serverAddress.StartsWith(prefix, System.StringComparison.Ordinal))
        {
            uri = serverAddress;
        }
        else
        {
            uri = string.Format("http://{0}", serverAddress);
        }


    }

    public void CreateGame(int time, Playfield playfield)
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "create_game");
        sendObject.AddField("time", time);

        JSONObject playfieldObject = new JSONObject();

        List<Vector2> points = playfield.points;
        foreach (Vector2 point in points)
        {
            JSONObject pointObject = new JSONObject();
            pointObject.AddField("longitude", point.x);
            pointObject.AddField("latitude", point.y);

            playfieldObject.Add(pointObject);
        }
        sendObject.AddField("playfield", playfieldObject);

        sendObject.AddField("ip", "123.456.12.34");
        sendObject.AddField("name", playerName);

        StartCoroutine(SendRequest(sendObject, CreateGameCallback));
    }

    private void CreateGameCallback(JSONObject incomingJson)
    {
        roomPin = incomingJson.GetField("room_pin").ToString();
        Debug.Log(roomPin);
    }


    public void JoinGame(string roomPin)
    {
        JSONObject sendObject = new JSONObject();
        sendObject.AddField("action", "join_game");
        sendObject.AddField("room_pin", roomPin);

        sendObject.AddField("ip", "123.456.33.22");
        sendObject.AddField("name", playerName);

        StartCoroutine(SendRequest(sendObject, JoinGameCallback));
    }

    private void JoinGameCallback(JSONObject incomingJson)
    {
        string status = incomingJson.GetField("status").str;

        if (status == "success")
        {
            Debug.Log("Room found");
        }
        else if (status == "failed")
        {
            Debug.Log("Room not found");
            uiManager.ShowPopup("Room not found", uiManager.popupDuration);
        }
    }


    IEnumerator SendRequest(JSONObject outgoingJson, Action<JSONObject> callback = null)
    {
        Debug.Log(outgoingJson);

        string jsonString = outgoingJson.ToString();
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonString);

        using (UnityWebRequest webRequest = UnityWebRequest.Put(uri, bytes))
        {
            webRequest.method = "POST";
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.isNetworkError || webRequest.isHttpError)
            {
                Debug.Log(webRequest.error);
            }
            else
            {
                byte[] answer = webRequest.downloadHandler.data;
                string answerString = System.Text.Encoding.UTF8.GetString(answer);

                JSONObject incomingJson = new JSONObject(answerString);
                Debug.Log(incomingJson);
                callback?.Invoke(incomingJson);

            }
        }
    }
}
