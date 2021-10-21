using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StreamableData : IStreamable
{
    public TransformData Transform;

    object IStreamable.Stream()
    {
        return Transform;
    }
}
