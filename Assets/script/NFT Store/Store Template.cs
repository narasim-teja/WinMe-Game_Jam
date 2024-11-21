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

    public void SelectKart()
    {
        GameObject kartPrefabParent = GameObject.Find(Constants.currentKartGameObject);
        //Debug.Log(kartPrefabParent.name);
        
        foreach (Transform child in kartPrefabParent.transform)
        {
            Destroy(child.gameObject);
        }

        Instantiate(obj, kartPrefabParent.transform);

        StoreManager storeManager = GameObject.Find("Shop")
            .GetComponent<StoreManager>();
        storeManager.currentKartIndex = index;
    }
}
