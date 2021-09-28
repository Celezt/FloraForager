using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;
public class InventoryManager : MonoBehaviour
{
    private Item[] slots;
    public InventoryObject inventory;

    // Start is called before the first frame update
    void Start()
    {
        
        Addressables.LoadAssetAsync<TextAsset>("inventory").Completed +=(handle)=> 
        {
            InventoryAsset tmp = JsonConvert.DeserializeObject<InventoryAsset>(handle.Result.text);
            for (int i = 0; i < tmp.Items.Length; i++)
            {
                inventory.Container[i] = new InventorySlot(tmp.Items[i]);
            }

            slots = GetComponentsInChildren<Item>();
            Debug.Log(inventory.Container.Length +":" + slots.Length);
            for (int i = 0; i < inventory.Container.Length; i++)
            {
                slots[i].item = inventory.Container[i];
            }
            
            for (int i = 0; i < 8; i++)
            {
                if (inventory.Container[i] != null)
                {
                    slots[i].item = inventory.Container[i];
                    slots[i].TextMesh.text = inventory.Container[i].item.Amount.ToString();
                    Debug.Log(inventory.Container[i].item.Amount);
                }
                
            }
            inventory.AddItem(new InventorySlot(new ItemAsset { ID = "Loka", Amount = 10}));
            inventory.InventoryAction += (int i) =>
            {
                slots[i].item = inventory.Container[i];
                slots[i].TextMesh.text = inventory.Container[i].item.Amount.ToString();

            };
            Addressables.Release(handle);
        };
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
