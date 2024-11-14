using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class StoreManager : MonoBehaviour
{
    public StoreTemplate[] storePanels;
    //public Storeitem[] storeItem;
    public Button[] purchaseBtn;
    public int currentKartIndex;

    void Start()
    {
        LoadPanels();
        //CheckPurchaseble();
    }

    public void LoadPanels()
    {
        for (int i = 0; i < StoreData.Instance.storeItemList.Length; i++)
        {
            storePanels[i].title.text = StoreData.Instance.storeItemList[i].title;
            storePanels[i].desc.text = StoreData.Instance.storeItemList[i].desc;
            storePanels[i].cost.text = StoreData.Instance.storeItemList[i].cost.ToString();
            storePanels[i].kartObject = StoreData.Instance.storeItemList[i].kartObject;
            storePanels[i].index = i;
        }
    }

    public void CheckPurchaseble()
    {
        for(int i=0;i< StoreData.Instance.storeItemList.Length; i++)
        {
            //condition
            if (i % 2 == 0)
            {
                purchaseBtn[i].interactable = true;
            }
            else
            {
                purchaseBtn[i].interactable= false;
            }
        }
    }

    public void ChooseKart()
    {
        Constants.currentKartIndex = currentKartIndex;
        Debug.Log($"store kart index: {Constants.currentKartIndex}");
        GoToMainMenu();
        //if (StoreData.Instance.currentKart != null)
        //{
        //    //NetworkClient.RegisterPrefab(StoreData.Instance.currentKart);
        //    GameObject managerObj = GameObject.Find("NetworkManager");
        //    MirrorNetworkManager manager = managerObj.GetComponent<MirrorNetworkManager>();
        //    manager.playerPrefab = StoreData.Instance.currentKart;

        //    GoToMainMenu(managerObj);
        //}
    }

    public void GoToMainMenu()
    {
        GameObject managerObj = GameObject.Find("NetworkManager");
        managerObj.transform.GetChild(0).gameObject.SetActive(true);
        SceneManager.LoadScene(0);
    }
}
