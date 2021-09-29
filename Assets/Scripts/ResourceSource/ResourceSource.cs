using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ResourceSource : MonoBehaviour, IInteractable
{
    [SerializeField] private ResourceSourceData _Data;
    [SerializeField] private GameObject _Player;
    [SerializeField] private LayerMask _LayerMasks;

    public UnityEvent OnQuantityChanged;

    private PlayerMovement _PlayerMovement;

    private float _CollectionTime;
    private int _CurrentAmount;
    private bool _IsBeingCollected;
    private Coroutine _CollectingCoroutine;
    private Bounds _Bounds;

    public static ResourceSource ResourceBeingCollected = null; // can only collect from one source at a time

    public ResourceSourceData Data => _Data;
    public Bounds Bounds => _Bounds;
    public int CurrentAmount => _CurrentAmount;

    public int Priority => 1;

    private void Awake()
    {
        _PlayerMovement = _Player.GetComponent<PlayerMovement>();
        _Bounds = GetComponent<MeshFilter>().mesh.bounds;
    }

    private void Start()
    {
        _CollectionTime = _Data.TotalCollectionTime / (_Data.Amount / _Data.AmountPerCollect);
    }

    private void Update()
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

    public bool StartCollecting()
    {
        if (_IsBeingCollected || _CollectingCoroutine != null)
            return false;

        Debug.Log("started collecting");

        _CollectingCoroutine = StartCoroutine(Collecting());

        _PlayerMovement.Velocity = Vector3.zero; // stops player

        ResourceBeingCollected = this;
        
        return (_IsBeingCollected = true);      
    }

    public bool StopCollecting()
    {
        if (!_IsBeingCollected || _CollectingCoroutine == null)
            return false;

        Debug.Log("stopped collecting");

        StopCoroutine(_CollectingCoroutine);
        _CollectingCoroutine = null;

        if (ResourceBeingCollected == this)
            ResourceBeingCollected = null;

        return !(_IsBeingCollected = false);
    }

    private IEnumerator Collecting()
    {
        while (true)
        {
            yield return new WaitForSeconds(_CollectionTime);

            int amountToAdd = ((_CurrentAmount + _Data.AmountPerCollect) > _Data.Amount) ? 
                (_Data.Amount - _CurrentAmount) : _Data.AmountPerCollect;

            _CurrentAmount += amountToAdd;

            Debug.Log("collected " + amountToAdd + " " + _Data.ItemID);

            // TODO: add items to player's inventory (check if available space, if not, then stop collecting and don't add amount)

            OnQuantityChanged.Invoke();
            ResourceSourceUI.Instance.UpdateText();

            if (_CurrentAmount >= _Data.Amount)
            {
                Debug.Log("finished collecting");

                StopCollecting();

                ResourceSourceUI.Instance.SetActive(null, false);

                Destroy(gameObject);  // simply destroy for now 

                break;
            }
        }
    }

    public void OnInteract(InteractContext context)
    {
        if (!context.performed)
            return;

        // TODO: check if the player is holding the right tool

        if (!_IsBeingCollected)
            StartCollecting();
        else
            StopCollecting();
    }
}
