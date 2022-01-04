using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBehaviour : MonoBehaviour
{
    [SerializeField] private Gradient _deepColor;
    [SerializeField] private Gradient _shoreColor;
    [SerializeField] private Gradient _surfaceFoamColor1;
    [SerializeField] private Gradient _surfaceFoamColor2;
    [SerializeField] private Gradient _intersectionFoamColor;

    private Material _material;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
    }

    private void Update()
    {
        float time = GameTime.Instance.CurrentTime / 24.0f;

        _material.SetColor("_WaterColorDeep", _deepColor.Evaluate(time));
        _material.SetColor("_ShoreColor", _shoreColor.Evaluate(time));
        _material.SetColor("_SurfaceFoamColor1", _surfaceFoamColor1.Evaluate(time));
        _material.SetColor("_SurfaceFoamColor2", _surfaceFoamColor2.Evaluate(time));
        _material.SetColor("_IntersectionFoamColor", _intersectionFoamColor.Evaluate(time));
    }
}
