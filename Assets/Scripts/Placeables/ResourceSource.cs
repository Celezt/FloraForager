using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public abstract class ResourceSource : MonoBehaviour, IUsable, IDestructableObject
{
    [SerializeField] private ResourceSourceData _Data;
    [SerializeField] private LayerMask _LayerMasks;

    public System.Action OnQuantityChanged = delegate { };
    public System.Action OnEmptied = delegate { };
    public System.Action OnStarted = delegate { };
    public System.Action OnStopped = delegate { };

    protected GameObject _Player;
    protected PlayerMovement _PlayerMovement;
    protected Inventory _Inventory;

    protected float _CollectionTime;
    protected int _CurrentAmount;
    protected bool _IsBeingCollected;
    protected Coroutine _CollectingCoroutine;

    public static ResourceSource ResourceBeingCollected = null; // can only collect from one source at a time

    private Bounds _Bounds;

    public ResourceSourceData Data => _Data;
    public Bounds Bounds => _Bounds;
    public int CurrentAmount => _CurrentAmount;

    public int Priority => 1;

    public abstract float Strength { get; set; }
    public abstract float Durability { get; set; }

    private void Awake()
    {
        _Player = PlayerInput.GetPlayerByIndex(0).gameObject;
        _PlayerMovement = _Player.GetComponent<PlayerMovement>();
        _Inventory = _Player.GetComponent<PlayerInfo>().Inventory;

        _Bounds = GetComponent<MeshFilter>().mesh.bounds;
    }

    protected virtual void Start()
    {
        _CollectionTime = (_Data.AmountPerCollect != 0) ? 
            _Data.TotalCollectionTime / (_Data.Amount / (float)_Data.AmountPerCollect) : 
            _Data.TotalCollectionTime;

        Strength = Data.Strength;
    }

    protected virtual void Update()
    {
        if (_PlayerMovement.Velocity.magnitude > float.Epsilon 
            || (ResourceBeingCollected != null && ResourceBeingCollected != this)) // stop collecting if player moved or this is not the resouce being collected
        {
            StopCollecting();
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        bool collision = Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity, _LayerMasks) && !EventSystem.current.IsPointerOverGameObject();

        if (collision && hitInfo.transform.gameObject == gameObject)
        {
            ResourceSourceUI.Instance.SetActive(this, true);
        }
        else if ((collision && !hitInfo.transform.CompareTag("Resource")) || !collision)
        {
            ResourceSourceUI.Instance.SetActive(null, false);
        }
    }

    protected virtual bool StartCollecting()
    {
        if (_IsBeingCollected || _CollectingCoroutine != null)
            return false;

        _CollectingCoroutine = StartCoroutine(Collect());

        _PlayerMovement.Velocity = Vector3.zero; // stops player

        ResourceBeingCollected = this;

        OnStarted.Invoke();
        
        return (_IsBeingCollected = true);      
    }

    protected virtual bool StopCollecting()
    {
        if (!_IsBeingCollected || _CollectingCoroutine == null)
            return false;

        StopCoroutine(_CollectingCoroutine);
        _CollectingCoroutine = null;

        if (ResourceBeingCollected == this)
            ResourceBeingCollected = null;

        OnStopped.Invoke();

        return !(_IsBeingCollected = false);
    }

    protected virtual IEnumerator Collect()
    {
        while (true)
        {
            yield return new WaitForSeconds(_CollectionTime);

            int amountToAdd = ((_CurrentAmount + _Data.AmountPerCollect) > _Data.Amount) ? 
                (_Data.Amount - _CurrentAmount) : _Data.AmountPerCollect;

            _CurrentAmount += amountToAdd;

            // TODO: add items to player's inventory (check if available space, if not, then stop collecting and don't add amount)
            _Inventory.Insert(new ItemAsset
            {
                ID = _Data.ItemID,
                Amount = amountToAdd
            });

            OnQuantityChanged.Invoke();

            if (_CurrentAmount >= _Data.Amount)
            {
                StopCollecting();

                OnEmptied.Invoke();

                ResourceSourceUI.Instance.SetActive(null, false);

                Destroy(gameObject);  // simply destroy for now 

                break;
            }
        }
    }

    public void OnUse(UsedContext context)
    {
        if (!context.performed)
            return;

        if (!_IsBeingCollected)
            StartCollecting();
        else
            StopCollecting();
    }

    public abstract ItemLabels Filter();

    void IDestructableObject.OnDamage(IDestructor destructor, IDestructable destructable, UsedContext context)
    {
        throw new System.NotImplementedException();
    }

    void IDestructableObject.OnDestruction(UsedContext context)
    {
        throw new System.NotImplementedException();
    }

}
