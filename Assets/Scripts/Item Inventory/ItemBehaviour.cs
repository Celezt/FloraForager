using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

public class ItemBehaviour : MonoBehaviour
{
    private static readonly ExposedProperty TEXTURE_ATTRIBUTE = "Texture";

    [SerializeField] private VisualEffect _visualEffect;
    [SerializeField] private ItemAsset _item;
    [SerializeField] private float _mass = 6;
    [SerializeField] private float _impulse = 20;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private float _radius = 2;
    [SerializeField] private float _startTime = 1;
    [SerializeField] private float _checkFrequency = 0.5f;
    [SerializeField] private LayerMask _pickMask;
    [SerializeField] private float _destroyTimer = 0.5f;
    [SerializeField] private string _pickupSound = "pickup_04";

    private Transform _checkTransform;
    private Inventory _inventory;

    private bool _isdropped = true;

    public void Spawn(ItemAsset item) => Spawn(item, Random.insideUnitCircle.normalized);
    public void Spawn(ItemAsset item, Vector2 direction)
    {
        _item = item;
        _visualEffect.SetTexture(TEXTURE_ATTRIBUTE, ItemTypeSettings.Instance.ItemIconChunk[item.ID].texture);

        StartCoroutine(Drop(new Vector3(direction.x, 0.8f, direction.y) * _impulse));
    }

    private void Start()
    {
        InvokeRepeating(nameof(InsideCheck), _startTime, _checkFrequency);
    }

    private void InsideCheck()
    {
        if (!_isdropped)
            return;

        if (string.IsNullOrEmpty(_item.ID))
            return;

        Vector3 position = transform.position;
        Collider[] colliders = Physics.OverlapSphere(position, _radius, _pickMask);

        if (colliders.Length > 0)
        {
            _checkTransform = colliders[0].transform;

            if (_inventory == null)
                if (_checkTransform.TryGetComponent(out PlayerInfo playerInfo))
                        _inventory = playerInfo.Inventory;

            if (_inventory != null && _inventory.FindEmptySpace(_item.ID) > 0)  // If their is still space left.
                StartCoroutine(Absorb());
        }
        else
            _inventory = null;
    }

    private IEnumerator Absorb()
    {
        Vector3 position = transform.position;

        float offset = 0.0f;
        if (_checkTransform.TryGetComponent(out Collider collider))
            offset = collider.bounds.size.y;

        while (true)
        {
            float deltaTime = Time.fixedDeltaTime;
            Vector3 checkPosition = _checkTransform.position + Vector3.up * offset;

            position = Vector3.MoveTowards(position, checkPosition, 10 * deltaTime);

            if (Vector3.Distance(position, checkPosition) <= float.Epsilon)
            {
                Destroy(gameObject, _destroyTimer);
            }

            transform.position = position;

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Drop(Vector3 impulse)
    {
        float deltaTime;

        _isdropped = false;
        Vector3 position = transform.position;
        Vector3 velocity = impulse / _mass;
        Vector3 gravityForce = Physics.gravity * _mass;
        Vector3 point = Vector3.zero;

        void DropPhysics()
        {
            Vector3 acceleration = (gravityForce) / _mass;
            velocity += acceleration * deltaTime;
            position += velocity * deltaTime + (acceleration * deltaTime * deltaTime) / 2;
        }

        while (true)
        {
            deltaTime = Time.fixedDeltaTime;

            if (Physics.Raycast(position, Vector3.down, out RaycastHit hit, float.MaxValue, _groundMask))
                point = hit.point;

            DropPhysics();

            if (position.y <= point.y)  // End if at or below the hit point.
            {
                position.y = point.y;
                transform.position = position;
                _isdropped = true;
                yield break;
            }

            transform.position = position;

            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDestroy()
    {
        _visualEffect.Stop();

        if (gameObject.scene.isLoaded)
        {
            _inventory?.Insert(_item.ID, _item.Amount);
            SoundPlayer.Instance?.Play(_pickupSound, 0, 0, 0, 0.02f);
        }
    }
}
