using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeData : IStreamable
{
    public int Hej = 10;

    object IStreamable.Stream()
    {
        return Hej;
    }
}
