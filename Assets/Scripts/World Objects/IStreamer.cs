using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStreamer
{
    public void UpLoad();
    public void Load();
    public void BeforeSaving();
}
