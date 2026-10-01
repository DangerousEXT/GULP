using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[CreateAssetMenu(menuName = "Item")]
public class ItemData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private GameObject prefab;
    [SerializeField] private string nameId;
    [SerializeField] private Sprite icon;
    [SerializeField] private int sellPrice;
    [SerializeField] private int buyPrice;
    [SerializeField] private bool isEquipment;

    public string Id => id;
    public GameObject Prefab => prefab;
    public string NameId => nameId;
    public Sprite Icon => icon;
    public int SellPrice => sellPrice;
    public int BuyPrice => buyPrice;
    public bool IsEquipment => isEquipment;
}
