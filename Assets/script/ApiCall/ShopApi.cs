using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

internal class ShopApi
{


    private static readonly Lazy<ShopApi> _instance = new Lazy<ShopApi>(() => new ShopApi());

    public static ShopApi Instance
    {
        get
        {
            return _instance.Value;
        }
    }

    private ShopApi() { }

    public async Task<Texture2D> GetImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield(); // Await the completion of the request
            }

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error fetching image: {request.error}");
                return null;
            }

            return DownloadHandlerTexture.GetContent(request);
        }
    }
}
