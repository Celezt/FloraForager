using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Flora : MonoBehaviour, IInteractable
{
    [SerializeField] private string _Name;
    [SerializeField, TextArea(3, 10)] private string _Description;
    [Space(10)]
    [SerializeField, Min(0)] private int _GrowTime = 0; // growth time in days
    [SerializeField] private Mesh[] _Stages;            // number of visual growth stages this flora has
                                                        // 0 = start, x = final
    private MeshFilter _MeshFilter;

    private float _StageUpdate = 0; // at what stages to update the mesh
    private int _Stage = 0;
    private int _Mesh = 0;

    public int Priority => 1;

    private void Awake()
    {
        _MeshFilter = GetComponent<MeshFilter>();
    }

    private void Start()
    {
        _MeshFilter.mesh = _Stages[_Mesh];
        _StageUpdate = _GrowTime / _Stages.Length;
    }

    private void Update()
    {
        
    }

    public void Grow()
    {
        if (++_Stage >= _StageUpdate)
            _MeshFilter.mesh = _Stages[++_Mesh];
    }

    public void OnInteract(InteractContext context)
    {

    }
}
