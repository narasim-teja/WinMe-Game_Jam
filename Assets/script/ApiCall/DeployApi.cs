using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
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
    //private readonly string deployServerUrl = "https://api.edgegap.com/v1/deploy";
    //private readonly string serverStatusUrl = "https://api.edgegap.com/v1/status";
    //private readonly string stopServerUrl = "https://api.edgegap.com/v1/stop";

    private readonly string requestNewServerUrl = "https://4a1042cbc9cf.pr.edgegap.net:31588/deploy";
    private readonly string APP_NAME = "winme_game_server";
    private readonly string VERSION_NAME = "v1";

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

    public async Task<ServerData> CreateNewServer()
    {
        userIpAddress = await GetPublicIp();
        List<string> ip_list = new List<string> { userIpAddress };

        DeployData data = new DeployData(APP_NAME, VERSION_NAME, ip_list);
        string jsonData = JsonUtility.ToJson(data);
        Debug.Log(jsonData);


        //try
        //{
        //    using (HttpClient httpClient = new HttpClient())
        //    {
        //        // Serialize the requestBody object to JSON string
        //        string jsonBody = JsonUtility.ToJson(data);
        //        Debug.Log(jsonBody);

        //        // Create HttpContent object to send as body
        //        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        //        // Add authorization header if necessary
        //        //httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "your-token");

        //        // Send the POST request
        //        HttpResponseMessage response = await httpClient.PostAsync(requestNewServerUrl, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            // Get the response content
        //            string result = await response.Content.ReadAsStringAsync();
        //            Debug.Log("Response: " + result);
        //            return new ServerData("","sda",true);
        //        }
        //        else
        //        {
        //            Debug.LogError("Error: " + response.StatusCode);
        //            return null;
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    Debug.LogError("Exception: " + ex.Message);
        //    Debug.LogError(ex.InnerException.ToString());
        //    return null;
        //}

        using (UnityWebRequest webRequest = new UnityWebRequest(requestNewServerUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            //webRequest.SetRequestHeader("Content-Type", "application/json");

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

                string serverIp = jsonDict["serverIp"].ToString();
                string serverPort = jsonDict["serverPort"].ToString();
                return new ServerData(serverIp, serverPort, true);
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

    //public async Task<ServerData> CreateNewServer()
    //{
    //    userIpAddress = await GetPublicIp();
    //    List<string> ip_list = new List<string> { userIpAddress };
    //    string requestId = await DeployServer(ip_list);
    //    Debug.Log(userIpAddress);
    //    if (requestId != null)
    //    {
    //        ServerData serverData;
    //        while (true)
    //        {
    //            //await Task.Delay(1000);
    //            //Debug.Log("hello2");
    //            serverData = await IsServerDeployed(requestId);
    //            if (serverData == null)
    //            {
    //                //Debug.Log("request failed");
    //                return null;
    //            }
    //            else if (serverData.isReady)
    //            {
    //                //Debug.Log("Deployed");
    //                //break;
    //                return serverData;
    //            }
    //        }
    //        //CreateLobby("first Lobby", false, 2, serverAddress[0], serverAddress[1]);
    //    }
    //    else
    //    {
    //        Debug.Log("request failed");
    //        return null;
    //    }
    //}

    public async Task StopServerGracefully()
    {
        string deleteUrl = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_URL");
        string deleteToken = Environment.GetEnvironmentVariable("ARBITRIUM_DELETE_TOKEN");


        if (string.IsNullOrEmpty(deleteUrl) || string.IsNullOrEmpty(deleteToken))
        {
            Debug.LogError("Could not retrieve Edgegap environment variables.");
            return;
        }

        try
        {
            // Create an HttpClient instance
            using (HttpClient httpClient = new HttpClient())
            {
                // Add authorization header with the token
                httpClient.DefaultRequestHeaders.Add("Authorization", deleteToken);

                // Send the DELETE request to the ARBITRIUM_DELETE_URL
                HttpResponseMessage response = await httpClient.DeleteAsync(deleteUrl);

                // Check if the response was successful
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    Debug.Log("Deployment stopped successfully: " + result);
                }
                else
                {
                    Debug.LogError("Failed to stop deployment: " + response.StatusCode);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error stopping deployment: " + ex.Message);
        }
    }
}
