using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;

public interface IResource : IItem, IPlaceable
{
    [SerializeField]
    public struct DropType
    {
        public Items Items;
        public MinMaxInt DropAmount;
    }

    public List<DropType> Drops { get; set; }
}
