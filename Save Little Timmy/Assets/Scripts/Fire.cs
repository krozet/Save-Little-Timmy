using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    float scale = 1f;
    public Transform fire;
    private ParticleSystem fireParticleSystem;
    bool setToDestroy = false;
    float initialSizeMultiplier;
    // Start is called before the first frame update
    void Start()
    {
        fireParticleSystem = fire.GetComponent<ParticleSystem>();
        initialSizeMultiplier = fireParticleSystem.main.startSizeMultiplier;
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
