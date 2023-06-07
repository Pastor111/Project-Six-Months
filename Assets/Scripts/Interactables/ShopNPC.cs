using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaleItem
{
    public string Name;
    public ItemProbability item;
    //public int MinPrice;
    //public int MaxPrice;

    public int Price;

    public bool SoldOut = false;
}

public class ShopNPC : MonoBehaviour
{
    public SaleItem[] Possible_items;

    public int MinItemsToSale;
    public int MaxItemsToSale;

    public Transform AppearPoint;

    public SaleItem[] SoldItems;

    // Start is called before the first frame update
    void Start()
    {
        GetStore();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenStore()
    {
        Player.instance.SetShop(this);
    }

    public void CloseStore()
    {
        Player.instance.SetShop(null);
    }


    public void GetStore()
    {
        int itemsToSale = Random.Range(MinItemsToSale, MaxItemsToSale);

        SoldItems = new SaleItem[itemsToSale];

        for (int i = 0; i < SoldItems.Length; i++)
        {
            SoldItems[i] = Possible_items[Random.Range(0, Possible_items.Length)];
        }
    }

    public void Buy(SaleItem item)
    {
        for (int i = 0; i < SoldItems.Length; i++)
        {
            if(item == SoldItems[i])
            {
                if(Player.instance.GetGold() >= item.Price)
                {
                    Player.instance.SetGold(Player.instance.GetGold() - item.Price);
                    CloseStore();
                    Instantiate(item.item.obj, AppearPoint.position, item.item.obj.transform.rotation);
                    SoldItems[i].SoldOut = true;
                }
                else
                {
                    CloseStore();
                    Player.instance.ShowNotification(3f, "You dont have enough money to buy this item");
                }

            }
        }
    }
}
