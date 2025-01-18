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

    public void SelectItem()
    {
        GameObject kartPrefabParent = GameObject.Find(Constants.currentKartGameObject);
        int bodyIndex = kartPrefabParent.transform.childCount - 1;

        StoreManager storeManager = GameObject.Find("Shop")
            .GetComponent<StoreManager>();

        if (type == StoreItemType.Kart)
        {
            DestroyImmediate(kartPrefabParent.transform.GetChild(bodyIndex).gameObject);
            storeManager.currentKartIndex = index;

            GameObject body = InstantiateBody(storeManager);
            InstantiateWheel(body.transform, storeManager);
            InstantiateTrail(body.transform, storeManager);
            InstantiateHat(body.transform, storeManager);
        }
        else if (type == StoreItemType.Wheel)
        {
            Transform body = kartPrefabParent.transform.GetChild(bodyIndex);
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
            Transform body = kartPrefabParent.transform.GetChild(bodyIndex);
            int trailIndex = body.Find("car/Wheel.RL").GetChild(0).childCount - 1;
            DestroyImmediate(body.Find("car/Wheel.RL").GetChild(0).GetChild(trailIndex).gameObject);
            DestroyImmediate(body.Find("car/Wheel.RR").GetChild(0).GetChild(trailIndex).gameObject);

            storeManager.currentTrailIndex = index;

            InstantiateTrail(body, storeManager);
        }
        else if(type == StoreItemType.Hat)
        {
            Transform body = kartPrefabParent.transform.GetChild(bodyIndex);
            DestroyImmediate(body.Find("hat_loc").GetChild(0).gameObject);

            storeManager.currentHatIndex = index;

            InstantiateHat(body, storeManager);
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

    void InstantiateHat(Transform body, StoreManager storeManager)
    {
        Instantiate(StoreData.Instance.hatList[storeManager.currentHatIndex].obj, body.Find("hat_loc"));
    }
}
