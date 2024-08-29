using Mirror.SimpleWeb;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Server : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // create server instance
        var tcpConfig = new TcpConfig(noDelay: false, sendTimeout: 5000, receiveTimeout: 20000);
        var server = new SimpleWebServer(5000, tcpConfig, ushort.MaxValue, 5000, new SslConfig());

        // listen for events
        server.onConnect += (id) => Debug.Log($"New Client connected, id:{id}");
        server.onDisconnect += (id) => Debug.Log($"Client disconnected, id:{id}");
        server.onData += (id, data) => Debug.Log($"Data from Client, id:{id}, {BitConverter.ToString(data.Array, data.Offset, data.Count)})");
        server.onError += (id, exception) => Debug.Log($"Error because of Client, id:{id}, Error:{exception}");

        // start server listening on port 7777
        server.Start(7777);

        // call Process to cause events to be invoked
        // Call this from inside Unity Update method so that it will process message each frame

        server.ProcessMessageQueue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
