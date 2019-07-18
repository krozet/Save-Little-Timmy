using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piss : MonoBehaviour
{
    private ParticleSystem pissParticleSystem;
    private List<ParticleCollisionEvent> collisionEvents;
    bool setToDestroy = false;

    public GameObject smoke;

    // Start is called before the first frame update
    void Start() {
        pissParticleSystem = gameObject.GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        DestroyAfterEffectHasEnded();
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("on trigger entered by piss");
        if (other.CompareTag("Fire")) {
            PissOnFire();
        }
    }

    void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Fire")) {
            int numCollisionEvents = pissParticleSystem.GetCollisionEvents(other, collisionEvents);
            Debug.Log("numCollisionEvents = " + numCollisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                Vector3 collisionHitLoc = collisionEvents[i].intersection;
                CreateSmokeParticleEffect(collisionHitLoc);
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateSmokeParticleEffect(Vector3 collisionLocation) {
        GameObject instanciatedSmoke = Instantiate(smoke, collisionLocation, Quaternion.identity);
        float timeToWaitBeforeDestroy = smoke.GetComponent<ParticleSystem>().main.startLifetimeMultiplier;
        Destroy(instanciatedSmoke, timeToWaitBeforeDestroy);
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
