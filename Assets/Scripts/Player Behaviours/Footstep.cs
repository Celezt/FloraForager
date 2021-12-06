using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    private PlayerMovement _playerMovement;

    private void Awake()
    {
        _playerMovement = GetComponentInParent<PlayerMovement>();
    }

    public void PlayWalk()
    {
        if (_playerMovement == null)
            return;

        if (_playerMovement.RawVelocity.magnitude > float.Epsilon && !_playerMovement.IsRunning)
            SoundPlayer.Instance.Play($"footstep_{Random.Range(1, 5)}");
    }

    public void PlayRun()
    {
        if (_playerMovement == null)
            return;

        if (_playerMovement.RawVelocity.magnitude > float.Epsilon && _playerMovement.IsRunning)
            SoundPlayer.Instance.Play($"footstep_{Random.Range(1, 5)}");
    }
}
