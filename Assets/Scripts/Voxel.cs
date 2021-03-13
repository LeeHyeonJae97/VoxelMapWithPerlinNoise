using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Voxel : MonoBehaviour
{
    private UnityAction<GameObject> inActivate;

    // 'late' because inactivate voxels after place new voxels
    private void LateUpdate()
    {
        CalculateDistanceFromPlayer();
    }

    // initialize
    public void Init(UnityAction<GameObject> inActivate)
    {
        this.inActivate = inActivate;
    }

    // Calculate distance from player and determine whether inactivate voxel
    private void CalculateDistanceFromPlayer()
    {
        if (Mathf.Abs(transform.position.x - Player.Pos.x) > Player.SightRange * 10 || Mathf.Abs(transform.position.z - Player.Pos.z) > Player.SightRange * 10)
            inActivate(gameObject);
    }
}
