using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public Transform fire;

    float scale = 1f;
    ParticleSystem fireParticleSystem;
    ParticleSystem smokeParticleSystem;
    ParticleSystem.EmissionModule smokeParticle;
    Transform[] particleEffects;
    bool setToDestroy = false;
    float initialSizeMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        particleEffects = GetComponentsInChildren<Transform>();
        Debug.Log("Length: " + particleEffects.Length);
        foreach (Transform particle in particleEffects) {
            Debug.Log("name of particle: " + particle.name);
            if (particle.name.Equals("FX_Fire")) {
                Debug.Log("Found fire: " + particle.name);
                fireParticleSystem = particle.GetComponent<ParticleSystem>();
            }

            if (particle.name.Equals("FX_Smoke")) {
                Debug.Log("Found smoke: " + particle.name);
                smokeParticleSystem = particle.GetComponent<ParticleSystem>();
            }
        }
        smokeParticle = smokeParticleSystem.emission;
        smokeParticleSystem.Stop();

        initialSizeMultiplier = fireParticleSystem.main.startSizeMultiplier;
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("on trigger entered by piss onto fire");
        if (other.CompareTag("Piss")) {
            //PissOnFire();
        }
    }

    void OnParticleTrigger() {
        Debug.Log("trigger by particle");
    }

    void OnParticleCollision(GameObject other) {
        Debug.Log("piss on me");
    }

    // Update is called once per frame
    void Update()
    {
        if (scale <= 0f) {
            // Particle Effect has been reduced to a size of 0
            // So destroy it
            DestroyFireParticleEffect();
        } else {
            // Decrease particle effect size
            scale -= 0.001f;
            var fpsm = fireParticleSystem.main;
            fpsm.startSizeMultiplier = initialSizeMultiplier * scale;
        }
    }

    // Stops the fire particle emission and destroys the object once it's duration is finished
    void DestroyFireParticleEffect() {
        if (!setToDestroy) {
            ParticleSystem.EmissionModule fireParticle = fireParticleSystem.emission;
            fireParticle.enabled = false;
            float timeToWaitBeforeDestroy = fireParticleSystem.main.duration + fireParticleSystem.main.startLifetimeMultiplier;
            Destroy(gameObject, timeToWaitBeforeDestroy);
            setToDestroy = true;
        }
    }
}
