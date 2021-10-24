using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class ItemBehaviour : MonoBehaviour
{
    private static readonly ExposedProperty TEXTURE_ATTRIBUTE = "Texture";

    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private float _mass = 6;
    [SerializeField] private float _impulse = 20;
    [SerializeField] private LayerMask _groundMask;
    //[SerializeField] private float 

    public void Initialize(Texture2D texture)
    {
        _visualEffect.SetTexture(TEXTURE_ATTRIBUTE, texture);
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        StartCoroutine(Drop(new Vector3(randomDirection.x, 0.8f, randomDirection.y) * _impulse));;
    }

    private void FixedUpdate()
    {
        //Physics.OverlapSphere(transform.position, )
    }

    private IEnumerator Drop(Vector3 impulse)
    {
        float deltaTime;

        Vector3 velocity = impulse / _mass;
        Vector3 position = transform.position;
        Vector3 gravityForce = Physics.gravity * _mass;

        void DropPhysics()
        {
            Vector3 acceleration = (gravityForce) / _mass;
            velocity += acceleration * deltaTime;
            position += velocity * deltaTime + (acceleration * deltaTime * deltaTime) / 2;
        }

        while (true)
        {
            deltaTime = Time.deltaTime;

            DropPhysics();

            if (!Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, _groundMask))
                yield break;

            transform.position = position;

            yield return new WaitForFixedUpdate();
        }
    }
}
