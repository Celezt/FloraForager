using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hotbar : MonoBehaviour
{
    // Start is called before the first frame update
    public int startXPos;
    public int startYPos;
    public int xSPace;
    public int nrOfSlots;
    public PlayerInventoryObject inventory;
    public InventorySlot[] hotbar;
    public GameObject[] slotUIs;
    
    void Start()
    {
        
    }
    private void Awake()
    {
        hotbar = new InventorySlot[nrOfSlots];
        for (int i = 0; i < nrOfSlots; i++)
        {
            hotbar[i] = inventory.inventoryArray[i];
            //Instantiate prefab hotslot
            //prefab hotslot[i].item = inventory.inventoryArray[i]

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Vector3 GetPos(int i) 
    {
        return new Vector3(startXPos + xSPace*i,startYPos,0f);
    }
}
