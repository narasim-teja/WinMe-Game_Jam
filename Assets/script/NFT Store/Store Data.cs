using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreData : MonoBehaviour
{
    public static StoreData Instance { get; private set; }

    public Storeitem[] kartList;
    public Storeitem[] wheelList;
    public Storeitem[] trailList;

    public void Awake()
    {
        Instance = this;
    }
}
