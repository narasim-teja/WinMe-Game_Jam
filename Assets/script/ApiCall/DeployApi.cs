using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class DeployData
{
    public string app_name;
    public string version;
    public List<string> ip_list;

    public DeployData(string app_name, string version, List<string> ip_list) { 
        this.app_name = app_name;
        this.version = version;
        this.ip_list = ip_list;
    }
}

internal class DeployApi 
{
    private string token = "token 1437094f-86a6-4d5c-9c08-fa35b289cb38";

    private string deployServerUrl = "https://api.edgegap.com/v1/deploy";
    private string serverStatusUrl = "https://api.edgegap.com/v1/status/";

    private static readonly Lazy<DeployApi> _instance = new Lazy<DeployApi>(() => new DeployApi());

    public static DeployApi Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    private DeployApi() { }

    public async Task<string> DeployServer(List<string> ip_list)
    {
        DeployData data = new DeployData("winme-ays1", "v4", ip_list);
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest webRequest = new UnityWebRequest(deployServerUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", token);

            // Send the request and wait for a response
            //yield return webRequest.SendWebRequest();
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                string request_id = jsonDict["request_id"].ToString();
                return request_id;
            }
        }
    }

    public async Task<List<string>> IsServerDeployed(string request_id)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(serverStatusUrl + request_id))
        {
            webRequest.SetRequestHeader("Authorization", token);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;
                Debug.Log("Response: " + jsonResponse);

                Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);

                string current_status = jsonDict["current_status"].ToString();
                Debug.Log($"current status: {current_status}");
                if (current_status == "Status.DEPLOYING")
                {
                    return new List<string>();
                }
                else if (current_status == "Status.READY")
                {
                    string ip = jsonDict["fqdn"].ToString();
                    var ports = jsonDict["ports"] as Newtonsoft.Json.Linq.JObject;
                    var gamePorts = ports["Game Port"] as Newtonsoft.Json.Linq.JObject;
                    string port = gamePorts["external"].ToString();
                    return new List<string> { ip, port };
                }
                else
                {
                    return null;
                }
            }
        }
    }

    public async Task<string> GetPublicIp()
    {
        using(UnityWebRequest webRequest = UnityWebRequest.Get("https://api.ipify.org?format=json"))
        {
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error: " + webRequest.error);
                return null;
            }
            else
            {
                // Parse the JSON response to get the IP address
                string jsonResponse = webRequest.downloadHandler.text;
                Dictionary<string, object> jsonDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonResponse);
                string current_ip = jsonDict["ip"].ToString();
                return current_ip;
            }
        }
    }
}
