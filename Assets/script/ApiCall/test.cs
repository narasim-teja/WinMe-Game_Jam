using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Edgegap;


public class test : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        //Uri telemetryAgentUri = new Uri("https://console-agent.edgegap.com/");
        //string config = "./config";
        //var logger = new TelemetryAgentLogger();

        //var telemetryAgent = Edgegap.TelemetryAgent.TelemetryAgentFactory.GetAgent(logger, config, telemetryAgentUri);
        //telemetryAgent.Start();
        //logger.Debug(telemetryAgent.PlayerUUID);
    }


    void OnDestroy()
    {
        //Uri telemetryAgentUri = new Uri("http://127.0.0.1:5013");
        //string config = "./config";
        //var logger = new TelemetryAgentLogger();
        //var telemetryAgent = Edgegap.TelemetryAgent.TelemetryAgentFactory.GetAgent(logger, config, telemetryAgentUri);
        //telemetryAgent.Stop();
    }
}
