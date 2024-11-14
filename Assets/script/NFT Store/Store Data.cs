using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreData : MonoBehaviour
{
    public static StoreData Instance { get; private set; }

    public Storeitem[] storeItemList;

    public void Awake()
    {
        Instance = this;
    }
}
