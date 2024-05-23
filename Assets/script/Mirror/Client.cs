using Mirror.SimpleWeb;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // create client instance
        // call static SimpleWebClient.Create method so that the correct client for WebGL or standalone is created
        var tcpConfig = new TcpConfig(noDelay: false, sendTimeout: 5000, receiveTimeout: 20000);
        var client = SimpleWebClient.Create(ushort.MaxValue, 5000, tcpConfig);

        // listen for events
        client.onConnect += () => Debug.Log($"Connected to Server");
        client.onDisconnect += () => Debug.Log($"Disconnected from Server");
        client.onData += (data) => Debug.Log($"Data from Server, {BitConverter.ToString(data.Array, data.Offset, data.Count)})");
        client.onError += (exception) => Debug.Log($"Error because of Server, Error:{exception}");

        // create url and connect to server
        var builder = new UriBuilder
        {
            Scheme = "ws",
            Host = "www.example.com",
            Port = 7777
        };

        client.Connect(builder.Uri);

        // call Process to cause events to be invoked
        // Call this from inside Unity Update method so that it will process message each frame
        client.ProcessMessageQueue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
