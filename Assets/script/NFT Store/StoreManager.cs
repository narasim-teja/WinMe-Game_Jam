using Mirror;
using System.Collections;
using System.Collections.Generic;
using Thirdweb;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class StoreManager : MonoBehaviour
{
    public StoreTemplate[] storePanels;
    public Button[] purchaseBtn;
    public int currentKartIndex;

    void Start()
    {
        LoadImages();
        LoadPanels();
        ThirdWebTesting();
        //CheckPurchaseble();
    }

    public async void ThirdWebTesting()
    {
        var sdk = ThirdwebManager.Instance.SDK;
        bool isConnected = await sdk.Wallet.IsConnected();

        if (isConnected)
        {
            string address = await sdk.Wallet.GetAddress();
            Contract contract = sdk.GetContract(Constants.raceCarAddress, Constants.raceCarAbi);
            var data = await contract.Read<int>(Constants.functionName, address, Constants.tokenId);
            var img = await contract.Read<string>("uri", 1);

            Contract sport = sdk.GetContract(Constants.sportCarAddress, Constants.sportCarAbi);
            var data1 = await sport.Read<int>(Constants.functionName, address, Constants.tokenId);
            var img1 = await sport.Read<string>("uri", 1);

            Contract trackCar = sdk.GetContract(Constants.trackCarAddress, Constants.trackCarAbi);
            var data2 = await trackCar.Read<int>(Constants.functionName, address, Constants.tokenId);
            var img2 = await trackCar.Read<string>("uri", 1);

            Debug.Log($"img: {img} img1: {img1} img2: {img2}");
        }
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

    async void LoadImages()
    {
        for(int i=0;i< StoreData.Instance.storeItemList.Length; i++)
        {
            storePanels[i].image.texture = await ShopApi.Instance
                .GetImage(StoreData.Instance.storeItemList[i].imageUrl);
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
        //Debug.Log($"store kart index: {Constants.currentKartIndex}");
        GoToMainMenu();
    }

    public void GoToMainMenu()
    {
        GameObject managerObj = GameObject.Find("NetworkManager");
        managerObj.transform.GetChild(0).gameObject.SetActive(true);
        SceneManager.LoadScene(0);
    }
}
