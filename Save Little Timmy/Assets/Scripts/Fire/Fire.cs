using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour, PissOnable
{
    public Transform fire;
    public GameObject smoke;

    float scale = 1f;
    ParticleSystem fireParticleSystem;
    Transform[] particleEffects;
    bool setToDestroy = false;
    float initialSizeMultiplier;
    float health;
    float maxHealth = 50f;
    Vector3 initialLocalScale;

    // Start is called before the first frame update
    void Start()
    {
        particleEffects = GetComponentsInChildren<Transform>();
        foreach (Transform particle in particleEffects) {
            if (particle.name.Equals("FX_Fire")) {
                fireParticleSystem = particle.GetComponent<ParticleSystem>();
            }
        }

        initialSizeMultiplier = fireParticleSystem.main.startSizeMultiplier;
        initialLocalScale = transform.localScale;

        health = maxHealth;
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("Fire: OnTriggerEnter");
    }

    void OnParticleTrigger() {
        Debug.Log("Fire: OnParticleTrigger");
    }

    void OnCollisionEnter(Collision collision) {
    }

    void OnParticleCollision(GameObject other) {
        if (other.CompareTag("Piss")) {
            PissOnFire(other.GetComponent<Piss>().GetPissDamage());
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public float GetFireDamage() {
        return 1f;
    }

    void PissOnFire(float pissDamage) {
        if (health <= 0f) {
            // Particle Effect has been reduced to a size of 0
            // So destroy it
            DestroyFireParticleEffect();
        } else {
            // Decrease particle effect size
            health -= pissDamage;
            scale = health/maxHealth;
            AdjustSizeOfFire();
        }
    }

    void AdjustSizeOfFire() {
        // Adjusts the size of the particle effect
        var fpsm = fireParticleSystem.main;
        fpsm.startSizeMultiplier = initialSizeMultiplier * scale;

        // Adjusts the size of the Fire object and the sphere collider
        Vector3 newSize = initialLocalScale * scale;
        // Don't let the collider shrink to less than 60% the original size
        if (newSize.magnitude/initialLocalScale.magnitude >= 0.6) {
            transform.localScale = initialLocalScale * scale;
        }
    }

    // Stops the fire particle emission and destroys the object once it's duration is finished
    void DestroyFireParticleEffect() {
        if (!setToDestroy) {
            ParticleSystem.EmissionModule fireParticle = fireParticleSystem.emission;
            fireParticle.enabled = false;
            float timeToWaitBeforeDestroy = fireParticleSystem.main.startLifetimeMultiplier;

            // Reduce the Fire object and sphere collider size to 0 while particle effect still lingers
            transform.localScale = Vector3.zero;

            Destroy(gameObject, timeToWaitBeforeDestroy);
            setToDestroy = true;
        }
    }

    public void HandlePiss()
    {
        
    }

    public GameObject GetPissedOnParticleEffect() {
        return smoke;
    }
}
