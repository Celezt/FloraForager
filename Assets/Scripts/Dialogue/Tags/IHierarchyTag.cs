using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHierarchyTag : ITag
{
    /// <summary>
    /// Called when entering a child.
    /// </summary>
    public void EnterChild(Taggable thisTaggable, Taggable childTaggable, string parameter);
    /// <summary>
    /// Called when exiting a child.
    /// </summary>
    public void ExitChild(Taggable thisTaggable, Taggable childTaggable, string parameter);
    /// <summary>
    /// Process any children that does not contain this tag.
    /// </summary>
    public IEnumerator ProcessChild(Taggable thisTaggable, Taggable childTaggable, int currentIndex, int length, string parameter);
}
