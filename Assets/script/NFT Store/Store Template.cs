using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreTemplate : MonoBehaviour
{
    public TMP_Text title;
    public TMP_Text desc;
    public TMP_Text cost;
    public GameObject obj;
    public int index;
    public RawImage image;
    public StoreItemType type;

    public void SelectKart()
    {
        GameObject kartPrefabParent = GameObject.Find(Constants.currentKartGameObject);

        StoreManager storeManager = GameObject.Find("Shop")
            .GetComponent<StoreManager>();

        if (type == StoreItemType.Kart)
        {
            DestroyImmediate(kartPrefabParent.transform.GetChild(0).gameObject);
            storeManager.currentKartIndex = index;

            GameObject body = InstantiateBody(storeManager);
            InstantiateWheel(body.transform, storeManager);
            InstantiateTrail(body.transform, storeManager);
            //GameObject body = Instantiate(StoreData.Instance.kartList[index].obj, storeManager.kartModelParent.transform);

            //Instantiate(StoreData.Instance.wheelList[index].obj, body.transform.Find("car/Wheel.FR"));
            //Instantiate(StoreData.Instance.wheelList[index].obj, body.transform.Find("car/Wheel.FL"));

            //GameObject rearRight = Instantiate(StoreData.Instance.wheelList[index].obj, body.transform.Find("car/Wheel.RR"));
            //GameObject rearLeft = Instantiate(StoreData.Instance.wheelList[index].obj, body.transform.Find("car/Wheel.RL"));

            //Instantiate(StoreData.Instance.trailList[index].obj, rearRight.transform);
            //Instantiate(StoreData.Instance.trailList[index].obj, rearLeft.transform);
        }
        else if (type == StoreItemType.Wheel)
        {
            Transform body = kartPrefabParent.transform.GetChild(0);
            foreach (Transform child in body.Find("car/Wheel.FR"))
                DestroyImmediate(child.gameObject);
            foreach (Transform child in body.Find("car/Wheel.FL"))
                DestroyImmediate(child.gameObject);
            foreach (Transform child in body.Find("car/Wheel.RR"))
                DestroyImmediate(child.gameObject);
            foreach (Transform child in body.Find("car/Wheel.RL"))
                DestroyImmediate(child.gameObject);

            storeManager.currentWheelIndex = index;

            InstantiateWheel(body.transform, storeManager);
            InstantiateTrail(body.transform, storeManager);
        }
        else if (type == StoreItemType.Trail)
        {
            Transform body = kartPrefabParent.transform.GetChild(0);
            DestroyImmediate(body.Find("car/Wheel.RL").GetChild(0).GetChild(1).gameObject);
            DestroyImmediate(body.Find("car/Wheel.RR").GetChild(0).GetChild(1).gameObject);

            storeManager.currentTrailIndex = index;

            InstantiateTrail(body, storeManager);
        }
    }

    GameObject InstantiateBody(StoreManager storeManager)
        =>Instantiate(StoreData.Instance.kartList[storeManager.currentKartIndex].obj, 
            storeManager.kartModelParent.transform);

    void InstantiateWheel(Transform body, StoreManager storeManager)
    {
        Instantiate(StoreData.Instance.wheelList[storeManager.currentWheelIndex].obj, body.Find("car/Wheel.FR"));
        Instantiate(StoreData.Instance.wheelList[storeManager.currentWheelIndex].obj, body.Find("car/Wheel.FL"));
        Instantiate(StoreData.Instance.wheelList[storeManager.currentWheelIndex].obj, body.Find("car/Wheel.RR"));
        Instantiate(StoreData.Instance.wheelList[storeManager.currentWheelIndex].obj, body.Find("car/Wheel.RL"));
    }

    void InstantiateTrail(Transform body, StoreManager storeManager)
    {
        Instantiate(StoreData.Instance.trailList[storeManager.currentTrailIndex].obj, body.Find("car/Wheel.RR").GetChild(0));
        Instantiate(StoreData.Instance.trailList[storeManager.currentTrailIndex].obj, body.Find("car/Wheel.RL").GetChild(0));
    }
}
