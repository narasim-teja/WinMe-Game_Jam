using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

internal class ServerData
{
    public string ip;
    public string port;
    public bool isReady;

    public string request_id;
    public ServerData(string ip, string port, bool isReady, string request_id) { 
        this.ip = ip;
        this.port = port;
        this.isReady = isReady;
        this.request_id = request_id;
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
    private readonly string token = "token c7ed741e-582b-42d4-9099-51c260baaf86";

    private string request_id;

    private readonly string deployServerUrl = "https://api.edgegap.com/v1/deploy";
    private readonly string serverStatusUrl = "https://api.edgegap.com/v1/status";
    private readonly string stopServerUrl = "https://api.edgegap.com/v1/stop";

    private string userIpAddress;

    private readonly HttpClient client = new();

    private static readonly Lazy<DeployApi> _instance = new Lazy<DeployApi>(() => new DeployApi());

    public static DeployApi Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    private DeployApi() { }

    #region Getter Setter

    public string GetRequestId()
    {
        return request_id;
    }
    public void SetRequestID(string request_id)
    {
        if(this.request_id == null){
            this.request_id = request_id;
        }
        Debug.Log($"deploy api class: {this.request_id}, is null: {this.request_id == null}");
    }
    #endregion

    public async Task<string> DeployServer(List<string> ip_list)
    {
        DeployData data = new DeployData("winmeash", "v1", ip_list);
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

                    this.request_id = request_id;
                    return new ServerData(ip,port, true, request_id);
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
        Debug.Log("dsad");
        using(UnityWebRequest webRequest = UnityWebRequest.Get("https://api.ipify.org?format=json"))
        {
            Debug.Log("dasd1");
            var operation = webRequest.SendWebRequest();
            Debug.Log("dsa2");
            while (!operation.isDone)
                await Task.Yield();
            Debug.Log("dsa3");

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log("dsa4");
                Debug.LogError("Error: " + webRequest.error);
                return null;
            }
            else
            {
                Debug.Log("dsa5");
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

    public async Task<bool> StopServer()
    {
        Debug.Log($"stop server: {request_id}");
        using (UnityWebRequest webRequest = UnityWebRequest.Delete($"{stopServerUrl}/{request_id}"))
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

    public async Task DeployServerHttp(List<string> ip_list)
    {
        try
        {

            DeployData data = new DeployData("winmeash", "v1", ip_list);
            string jsonData = JsonUtility.ToJson(data);
            Debug.Log(jsonData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            Debug.Log("abc 1");

            client.DefaultRequestHeaders.Add("Authorization", token);
            Debug.Log("abc 2");
            // Send the POST request asynchronously
            HttpResponseMessage response = await client.PostAsync(deployServerUrl, content);
            Debug.Log("abc 3");

            // Check the response status and log it
            if (response.IsSuccessStatusCode)
            {
                Debug.Log("abc 4");
                string responseBody = await response.Content.ReadAsStringAsync();
                Debug.Log("Request successful! Response: " + responseBody);
            }
            else
            {
                Debug.Log("abc 5");
                Debug.LogError("Error in API request: " + response.StatusCode);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("abc 6");
            Debug.LogError("Exception occurred: " + ex.Message);
        }
    }
}
