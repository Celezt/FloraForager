using Celezt.Mathematics;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct UsedContext
{
    public readonly IUse used;
    public readonly List<string> labels;
    public readonly Transform transform;
    public readonly UseBehaviour useBehaviour;
    public readonly string name;
    public readonly string id;
    public readonly int playerIndex;
    public readonly bool canceled;
    public readonly bool started;
    public readonly bool performed;

    private readonly MonoBehaviour _usable;

    internal UsedContext(
        MonoBehaviour usable,
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
        this._usable = usable;
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
        else
            return false;

        return true;
    }

    public void Drop(Vector3 position, IList<DropType> drops, int dropStack = 1)
    {
        for (int i = 0; i < drops.Count; i++)
        {
            int rate = drops[i].DropRate.RandomInRangeInclusive();

            for (int j = 0; j < rate; j++)
                UnityEngine.Object.Instantiate(ItemTypeSettings.Instance.ItemObject, position, Quaternion.identity)
                    .Spawn(new ItemAsset { ID = drops[i].ID, Amount = dropStack });
        }
    }

    /// <summary>
    /// Shake an object.
    /// </summary>
    /// <param name="shakeTransform">Object to shake.</param>
    /// <param name="totalShakeDuration">Total duration of the shake.</param>
    /// <param name="decreasePoint">Start decreasing the shake at that point of the total duration. WARNING: Cannot be greater or equal to the total shake duration.</param>
    /// <param name="strength">Shake Speed.</param>
    /// <param name="angleRotation">Rotate whiles shaking (Optional).</param>
    public void Shake(Transform shakeTransform, float totalShakeDuration, float decreasePoint = 0.1f, float strength = 0.05f, float angleRotation = 1.0f)
    {
        IEnumerator ShakeCoroutine(Transform shakeTransform, float totalShakeDuration, float decreasePoint, float strength, float angleRotation)
        {
            if (decreasePoint >= totalShakeDuration)
            {
                Debug.LogError("ERROR: DecreasePoint must be less than totalShakeDuration...Exiting");
                yield break;
            }

            // Snapshot the original transform.
            Transform objTransform = shakeTransform;
            Vector3 defaultPos = objTransform.position;
            Quaternion defaultRot = objTransform.rotation;

            float counter = 0f;

            // Shake the object.
            while (counter < totalShakeDuration)
            {
                float deltaTime = Time.deltaTime;

                counter += deltaTime;
          
                float decreaseStrength = strength;
                float decreaseAngle = angleRotation;

                objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseStrength;
                objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-angleRotation, angleRotation), new Vector3(1f, 1f, 1f));
                yield return null;


                // Check if we have reached the decreasePoint then start decreasing decreaseSpeed value.
                if (counter >= decreasePoint)
                {
                    counter = 0f;
                    while (counter <= decreasePoint)
                    {
                        counter += deltaTime;
                        decreaseStrength = Mathf.Lerp(strength, 0, counter / decreasePoint);
                        decreaseAngle = Mathf.Lerp(angleRotation, 0, counter / decreasePoint);

                        objTransform.position = defaultPos + UnityEngine.Random.insideUnitSphere * decreaseStrength;
                        objTransform.rotation = defaultRot * Quaternion.AngleAxis(UnityEngine.Random.Range(-decreaseAngle, decreaseAngle), new Vector3(1f, 1f, 1f));

                        yield return null;
                    }

                    break;
                }
            }

            objTransform.position = defaultPos; // Reset to original position.
            objTransform.rotation = defaultRot; // Reset to original rotation.
        }

        _usable.StartCoroutine(ShakeCoroutine(shakeTransform, totalShakeDuration, decreasePoint, strength, angleRotation));
    }
}
