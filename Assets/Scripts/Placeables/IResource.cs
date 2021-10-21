using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public interface IResource : IItem, IPlace
{
    [SerializeField]
    public struct DropType
    {
        public Items Items;
        public MinMaxInt DropRate;
    }

    public List<DropType> Drops { get; set; }
}
