using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shop Item", menuName = "Scriptable objects/New Shop Item", order = 1)]
public class Storeitem : ScriptableObject
{
    public string title;
    public string desc;
    public int cost;
    public string contractAddress;
    public string contractAbi;
    public string imageUrl;
    public GameObject kartObject;
}
