using Celezt.Mathematics;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UsedContext
{
    public readonly IUse used;
    public readonly List<string> labels;
    public readonly UseBehaviour useBehaviour;
    public readonly Transform transform;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    internal UsedContext(
        IUse used,
        List<string> labels,
        Transform transform,
        UseBehaviour useBehaviour,
        string name,
        string id,
        int playerIndex,
        bool canceled,
        bool started,
        bool performed)
    {
        this.used = used;
        this.labels = labels;
        this.transform = transform;
        this.useBehaviour = useBehaviour;
        this.name = name;
        this.id = id;
        this.playerIndex = playerIndex;
        this.canceled = canceled;
        this.started = started;
        this.performed = performed;
    }

    /// <summary>
    /// Do damage to durability. Requires <see cref="IDestructor"/>.
    /// </summary>
    /// <param name="durability">Destructable's durability</param>
    /// <param name="damageMultiplier">Damage multiplier on the base damage.</param>
    /// <param name="star">Destructable's star</param>
    /// <returns>If containing <see cref="IDestructor"/> </returns>
    public bool Damage(ref float durability, MinMaxFloat damageMultiplier, Stars star = Stars.One)
    {
        if (!(used is IDestructor))
            return false;

        IDestructor destructor = used as IDestructor;
        int usedStar = (int)Stars.One;

        if (used is IStar)  // If destructor has implemented IStar, otherwise use standard.
            usedStar = (int)(used as IStar).Star;

        if (usedStar + 1 >= (int)star)   // Can damage if at least one star below.
            durability = Mathf.Clamp(durability - destructor.Damage * cmath.Map(usedStar / (int)star, new MinMaxFloat(1, 5), damageMultiplier), 0, float.MaxValue);

        return true;
    }

    public void Drop(IList<DropType> drops, int dropStack = 1)
    {
        for (int i = 0; i < drops.Count; i++)
        {
            int rate = drops[i].DropRate.RandomInRangeInclusive();

            for (int j = 0; j < rate; j++)
                UnityEngine.Object.Instantiate(ItemTypeSettings.Instance.ItemObject, transform.position, Quaternion.identity)
                    .Spawn(new ItemAsset { ID = drops[i].Item.Extract().GetRandom().ToString().ToSnakeCase(), Amount = dropStack });
        }
    }
}
