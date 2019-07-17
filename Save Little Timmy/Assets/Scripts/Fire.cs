using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    float scale = 1f;
    public Transform fire;
    bool setToDestroy = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (scale <= 0f) {
            DestroyFireParticleEffect();
        } else {
            scale -= 0.01f;
            
            fire.localScale = Vector3.one * scale;
            Debug.Log("scale: " + scale);
        }
    }

    void DestroyFireParticleEffect() {
        if (!setToDestroy) {
            ParticleSystem.EmissionModule fireParticle = fire.GetComponent<ParticleSystem>().emission;
            fireParticle.enabled = false;
            Destroy(gameObject, fire.GetComponent<ParticleSystem>().main.duration + fire.GetComponent<ParticleSystem>().main.startLifetimeMultiplier);
            setToDestroy = true;
        }
    }
}
