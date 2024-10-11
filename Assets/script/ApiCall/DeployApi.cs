using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

internal class ServerData
{
    public string ip;
    public string port;
    public bool isReady;
    public ServerData(string ip, string port, bool isReady) { 
        this.ip = ip;
        this.port = port;
        this.isReady = isReady;
    }

    public ServerData(bool isReady)
    {
        this.isReady = isReady;
        ip = null;
        port = null;
    }
}

public class DeployData
{
    public string app_name;
    public string version_name;
    public List<string> ip_list;

    public DeployData(string app_name, string version_name, List<string> ip_list) { 
        this.app_name = app_name;
        this.version_name = version_name;
        this.ip_list = ip_list;
    }
}

internal class DeployApi 
{
    private readonly string token = "token c22c8a31-8b3c-4f81-b778-b27e0d5f8027";

    private readonly string deployServerUrl = "https://api.edgegap.com/v1/deploy";
    private readonly string serverStatusUrl = "https://api.edgegap.com/v1/status";
    private readonly string stopServerUrl = "https://api.edgegap.com/v1/stop";

    private string userIpAddress;

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
        DeployData data = new DeployData("winmetest", "latest", ip_list);
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData);

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

    public async Task<ServerData> IsServerDeployed(string request_id)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{serverStatusUrl}/{request_id}"))
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
                if (current_status == "Status.TERMINATED" || current_status == "Status.ERROR")
                {
                    return null;
                }
                else if (current_status == "Status.READY")
                {
                    string ip = jsonDict["fqdn"].ToString();
                    var ports = jsonDict["ports"] as Newtonsoft.Json.Linq.JObject;
                    var gamePorts = ports["Game Port"] as Newtonsoft.Json.Linq.JObject;
                    string port = gamePorts["external"].ToString();
                    return new ServerData(ip,port, true);
                }
                else
                {
                    return new ServerData(false);
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

    public async Task<ServerData> CreateNewServer()
    {
        userIpAddress = await GetPublicIp();
        List<string> ip_list = new List<string> { userIpAddress };
        string requestId = await DeployServer(ip_list);
        Debug.Log(userIpAddress);
        if (requestId != null)
        {
            ServerData serverData;
            while (true)
            {
                //await Task.Delay(1000);
                //Debug.Log("hello2");
                serverData = await IsServerDeployed(requestId);
                if (serverData == null)
                {
                    //Debug.Log("request failed");
                    return null;
                }
                else if (serverData.isReady)
                {
                    //Debug.Log("Deployed");
                    //break;
                    return serverData;
                }
            }
            //CreateLobby("first Lobby", false, 2, serverAddress[0], serverAddress[1]);
        }
        else
        {
            Debug.Log("request failed");
            return null;
        }
    }

    public async Task<bool> StopServer(string request_id)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get($"{stopServerUrl}/{request_id}"))
        {
            webRequest.SetRequestHeader("Authorization", token);
            var operation = webRequest.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Delete request successful!");
                return true;
            }
            else
            {
                Debug.LogError("Error in DELETE request: " + webRequest.error);
                return false;
            }
        }
    }
}
