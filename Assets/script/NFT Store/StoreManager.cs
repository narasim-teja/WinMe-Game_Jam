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
    private GameObject kartModelParent;
    [SerializeField]
    private GameObject contents, storeItemPrefab;

    //public StoreTemplate[] storePanels;
    public List<StoreTemplate> storePanels = new List<StoreTemplate>();
    public Button[] purchaseBtn;

    public int currentKartIndex;
    public int currentWheelIndex;
    public int currentTrailIndex;

    void Start()
    {
        Intialize();
        AssembleKart();
        LoadKarts();
        LoadKartImages();
        ThirdWebTesting();
        //CheckPurchaseble();
    }

    void Intialize()
    {
        kartModelParent = GameObject.Find(Constants.currentKartGameObject);
        currentKartIndex = 0;
        currentWheelIndex = 0;
        currentTrailIndex = 0;
    }

    void AssembleKart()
    {
        GameObject body = Instantiate(StoreData.Instance.kartList[currentKartIndex].obj, kartModelParent.transform);
        
        Instantiate(StoreData.Instance.wheelList[currentWheelIndex].obj, body.transform.Find("car/Wheel.FR"));
        Instantiate(StoreData.Instance.wheelList[currentWheelIndex].obj, body.transform.Find("car/Wheel.FL"));
        
        GameObject rearRight = Instantiate(StoreData.Instance.wheelList[currentWheelIndex].obj, body.transform.Find("car/Wheel.RR"));
        GameObject rearLeft = Instantiate(StoreData.Instance.wheelList[currentWheelIndex].obj, body.transform.Find("car/Wheel.RL"));

        Instantiate(StoreData.Instance.trailList[currentTrailIndex].obj, rearRight.transform);
        Instantiate(StoreData.Instance.trailList[currentTrailIndex].obj, rearLeft.transform);
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

    public void LoadKarts()
    {
        for (int i = 0; i < StoreData.Instance.kartList.Length; i++)
        {
            GameObject item = Instantiate(storeItemPrefab, contents.transform);
            storePanels.Add(item.GetComponent<StoreTemplate>());
            storePanels[i].title.text = StoreData.Instance.kartList[i].title;
            storePanels[i].desc.text = StoreData.Instance.kartList[i].desc;
            storePanels[i].cost.text = StoreData.Instance.kartList[i].cost.ToString();
            storePanels[i].obj = StoreData.Instance.kartList[i].obj;
            storePanels[i].index = i;
        }
    }

    async void LoadKartImages()
    {
        for(int i=0;i< StoreData.Instance.kartList.Length; i++)
        {
            storePanels[i].image.texture = await ShopApi.Instance
                .GetImage(StoreData.Instance.kartList[i].imageUrl);
        }
    }

    public void CheckPurchaseble()
    {
        for(int i=0;i< StoreData.Instance.kartList.Length; i++)
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
