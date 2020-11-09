using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Particle effects will self-destruct after 5 seconds for memory efficiency
[RequireComponent(typeof(ParticleSystem))]
public class ParticleBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
