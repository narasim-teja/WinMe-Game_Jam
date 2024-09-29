using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UnityAuth : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
