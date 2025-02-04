using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StoreItemType
{
    Kart,
    Wheel,
    Trail,
    Hat
}

[CreateAssetMenu(fileName = "Shop Item", menuName = "Scriptable objects/New Shop Item", order = 1)]
public class StoreItem : ScriptableObject
{
    public string title;
    public string desc;
    public int cost;
    public string contractAddress;
    public string contractAbi;
    public string imageUrl;
    public GameObject obj;
    public StoreItemType type;
}
