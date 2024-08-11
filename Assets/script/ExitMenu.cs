using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ExitMenu : MonoBehaviour
{
    bool onExitMenu = false;
    public GameObject exitMenu;
    MirrorNetworkManager networkManager;

    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager").GetComponent<MirrorNetworkManager>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (onExitMenu)
            {
                exitMenu.SetActive(false);
                onExitMenu = false;
            }
            else
            {
                exitMenu.SetActive(true);
                onExitMenu = true;
            }
        }
    }

    public void LeaveGame()
    {
        networkManager.StopClient();
    }

    public void BackToGame()
    {
        exitMenu.SetActive(false);
        onExitMenu = false;
    }
}
