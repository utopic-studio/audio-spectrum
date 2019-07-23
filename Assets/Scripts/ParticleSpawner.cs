using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{

    public ParticleSystem particlePrefab;
    public Transform origin;

    public void SpawnPrefab()
    {
        if (!origin)
            origin = transform;
        GameObject.Instantiate(particlePrefab, origin);
    }
}
