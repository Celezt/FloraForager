using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
public class InventoryManager : MonoBehaviour
{
    private ItemSlot[] slots;
    public InventoryObject inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory.InventoryAction += (int i) =>
        {
            slots[i].item = inventory.Container[i];
            slots[i].TextMesh.text = inventory.Container[i].Amount.ToString();
            Debug.Log(inventory.Container[i].Amount.ToString());
        };

        Addressables.LoadAssetAsync<TextAsset>("inventory").Completed +=(handle)=>
        {
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            for (int i = 0; i < tmp.Items.Length; i++)
            {
                inventory.Container[i] = tmp.Items[i];
            }
            slots = GetComponentsInChildren<ItemSlot>();
            for (int i = 0; i < inventory.Container.Length; i++)
            {
                slots[i].item = inventory.Container[i];
            }
            
            for (int i = 0; i < 8; i++)
            {
                if (inventory.Container[i].ID != null)
                {
                    slots[i].item = inventory.Container[i];
                    slots[i].TextMesh.text = inventory.Container[i].Amount.ToString();
                    Debug.Log(inventory.Container[i].Amount);
                }
            }            
            //inventory.AddItem(new ItemAsset { ID = "Loka", Amount = 10});
            Addressables.Release(handle);
        };
    }
}
