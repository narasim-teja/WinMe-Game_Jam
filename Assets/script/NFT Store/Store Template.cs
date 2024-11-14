using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoreTemplate : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text cost;
    public GameObject kartObject;
    public int index;

    public void SelectKart()
    {
        GameObject kartPrefabParent = GameObject.Find(Constants.currentKartGameObject);
        Debug.Log(kartPrefabParent.name);
        
        foreach (Transform child in kartPrefabParent.transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(kartObject, kartPrefabParent.transform);

        StoreManager storeManager = GameObject.Find("Shop")
            .GetComponent<StoreManager>();
        storeManager.currentKartIndex = index;
        //StoreManager storeManager = GameObject.Find("Shop")
        //    .GetComponent<StoreManager>();
        //storeManager.kartPrefab = kartObject;

        //ChooseKart(kartObject);
    }

    //void ChooseKart(GameObject kartPrefab)
    //{
    //    kartPrefab.AddComponent<NetworkIdentity>();
    //    NetworkClient.RegisterPrefab(kartPrefab);
    //    GameObject managerObj = GameObject.Find("NetworkManager");
    //    MirrorNetworkManager manager = managerObj.GetComponent<MirrorNetworkManager>();
    //    manager.playerPrefab = kartPrefab;

    //    // Enable network manager component
    //    managerObj.transform.GetChild(0).gameObject.SetActive(true);
    //    SceneManager.LoadScene(0);

    //}
}
