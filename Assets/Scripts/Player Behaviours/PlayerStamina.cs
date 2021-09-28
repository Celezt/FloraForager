using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStamina : MonoBehaviour
{
    [SerializeField] private float _MaxStamina = 100.0f;
    [SerializeField] private float _StaminaDrainRate = 0.05f;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        _MaxStamina -= _StaminaDrainRate * Time.deltaTime;
    }
}
