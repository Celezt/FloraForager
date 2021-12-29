using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodrayBehaviour: MonoBehaviour
{
    [SerializeField]
    private AnimationCurve _fadeCurve;

    private Material _material;
    private float _alpha;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;
        _alpha = _material.color.a;
    }

    private void Update()
    {
        Color color = _material.color;
        color.a = _alpha * _fadeCurve.Evaluate(GameTime.Instance.CurrentTime / 24.0f);
        _material.color = color;
    }
}
