using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITaggable
{
    /// <summary>
    /// Called on awake.
    /// </summary>
    public void Initialize(Taggable taggable);
    /// <summary>
    /// Called every time activated.
    /// </summary>
    public void OnActive(Taggable taggable);
}
