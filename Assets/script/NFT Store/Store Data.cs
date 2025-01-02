using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreData : MonoBehaviour
{
    public static StoreData Instance { get; private set; }

    public StoreItem[] kartList;
    public StoreItem[] wheelList;
    public StoreItem[] trailList;
    public StoreItem[] hatList;

    public void Awake()
    {
        Instance = this;
    }
}
