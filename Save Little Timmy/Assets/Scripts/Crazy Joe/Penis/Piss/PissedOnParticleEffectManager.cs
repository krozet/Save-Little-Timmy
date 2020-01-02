using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PissedOnParticleEffectManager : MonoBehaviour
{
    // TODO: Accept object from piss, check if it has collided with fire, then send smoke effect to createsmokeparticleefftect
    Piss piss;

    public void init(Piss _piss) {
        piss = _piss;
    }
    public void SpawnPissedOnParticleEffect(Component collider, Vector3 point) {
        if (collider.gameObject.CompareTag("Fire")) {
            Fire fire = collider.gameObject.GetComponent<Fire>();
            if (fire != null) {
                CreateSmokeParticleEffect(point, fire.GetPissedOnParticleEffect());
            }
        }

        if (collider.gameObject.CompareTag("Little Timmy")) {
            Vector3 collisionHitLoc = new Vector3(point.x, point.y, point.z);
            //LittleTimmy littleTimmy = collider.gameObject.GetComponent<Fire>();
            //if (fire != null) {
            //    CreateBloodParticleEffect(collisionHitLoc, fire.GetPissedOnParticleEffect());
            //}
        }
    }

    // Creates the smoke effect when piss collides with Fire
    void CreateSmokeParticleEffect(Vector3 collisionLocation, GameObject smoke) {
        if (piss.GetParticleCount(Piss.SMOKE_PARTICLE_INDEX) <= 10) {
            GameObject instanciatedSmoke = Instantiate(smoke, collisionLocation, Quaternion.identity);
            piss.AddParticleToCounter(Piss.SMOKE_PARTICLE_INDEX);

            ParticleSystem smokeParticleSystem = instanciatedSmoke.GetComponent<ParticleSystem>();
            float timeToWaitBeforeDestroy = smokeParticleSystem.main.startLifetimeMultiplier;
            AdjustSizeOfSmoke(smokeParticleSystem);
            Destroy(instanciatedSmoke, timeToWaitBeforeDestroy);
            piss.StartCoroutineRemoveParticleFromCounter(timeToWaitBeforeDestroy, Piss.SMOKE_PARTICLE_INDEX);
        }
    }

    // Adjusts the size of the smoke particle effect
    void AdjustSizeOfSmoke(ParticleSystem smokeParticleSystem) {
        smokeParticleSystem.Stop();
        float sizeScale = 0.3f;
        float durationScale = 0.2f;
        var spsm = smokeParticleSystem.main;
        spsm.startSizeMultiplier *= sizeScale;
        spsm.duration *= durationScale;

        smokeParticleSystem.Play();
    }

    //TODO: Do this for little timmy, add blood effect to little timmy prefab
    // Creates the blood effect when piss collides with Little Timmy
    void CreateBloodParticleEffect(Vector3 collisionLocation, GameObject blood) {
        GameObject instanciatedBlood = Instantiate(blood, collisionLocation, Quaternion.identity);
        ParticleSystem bloodParticleSystem = instanciatedBlood.GetComponent<ParticleSystem>();
        float timeToWaitBeforeDestroy = bloodParticleSystem.main.startLifetimeMultiplier;
        AdjustSizeOfBlood(bloodParticleSystem);
        Destroy(instanciatedBlood, timeToWaitBeforeDestroy);
    }

    // Adjusts the size of the explosion particle effect
    void AdjustSizeOfBlood(ParticleSystem bloodParticleSystem) {
        bloodParticleSystem.Stop();
        float sizeScale = 1f;
        float durationScale = 0.2f;
        var bpsm = bloodParticleSystem.main;
        bpsm.startSizeMultiplier *= sizeScale;
        bpsm.duration *= durationScale;
        bpsm.startDelay = 0;

        bloodParticleSystem.Play();
    }
}
