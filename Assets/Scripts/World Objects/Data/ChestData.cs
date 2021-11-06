using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[System.Serializable]
public class ChestData : ObjectData
{
    [SerializeField, HideInInspector]
    private Data _Data;

    public Data ObjectData => _Data;

    public class Data
    {
        public Vector2Int CellPosition;
        public List<ItemAsset> Items;
    }

    public ChestData(string id, GameObject gameObject, string address, int sceneIndex, Cell cell, List<ItemAsset> items) 
        : base(id, gameObject, address, sceneIndex)
    {
        _Data = new Data();

        _Data.CellPosition = cell.Local;
        _Data.Items = items;
    }
}
