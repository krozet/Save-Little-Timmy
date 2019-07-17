using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piss : MonoBehaviour
{
    private ParticleSystem pissParticleSystem;
    bool setToDestroy = false;
    // Start is called before the first frame update
    void Start() {
        pissParticleSystem = gameObject.GetComponent<ParticleSystem>();
        DestroyAfterEffectHasEnded();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("on trigger entered by piss");
        if (other.CompareTag("Fire")) {
            PissOnFire();
        }
    }

    void OnParticleCollision(GameObject other) {
        if (other.name != "Some Shit") {
            Debug.Log("Collided with: " + other.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PissOnFire() {
        Debug.Log("Pissed on the fire");
        Destroy(gameObject);
    }

    void DestroyAfterEffectHasEnded() {
        if (!setToDestroy) {
            ParticleSystem.EmissionModule pissParticle = pissParticleSystem.emission;
            float timeToWaitBeforeDestroy = pissParticleSystem.main.duration + pissParticleSystem.main.startLifetimeMultiplier;
            Destroy(gameObject, timeToWaitBeforeDestroy);
            setToDestroy = true;
        }
    }
}
