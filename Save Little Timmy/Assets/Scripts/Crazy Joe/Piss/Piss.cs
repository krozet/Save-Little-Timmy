using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piss : MonoBehaviour
{
    private ParticleSystem pissParticleSystem;
    private List<ParticleCollisionEvent> collisionEvents;
    private PissedOnParticleEffectManager pissedOnParticleEffectManager;
    bool setToDestroy = false;

    public ObiEmitter obiEmitter;
    public GameObject smoke;
    public GameObject blood;

    float pissDamage;

    ObiSolver solver;

    void Awake() {
        solver = GetComponent<Obi.ObiSolver>();
    }

    void OnEnable() {
        solver.OnCollision += Solver_OnCollision;
    }

    void OnDisable() {
        solver.OnCollision -= Solver_OnCollision;
    }

    // Detects when obi particle collides with obi collider
    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e) {
        // if suffering from performance issues, try using a hashset?
        for (int i = 0; i < e.contacts.Count; ++i)
        {
            if (e.contacts.Data[i].distance < 0.06f)
            {
                Component collider;
                if (ObiCollider.idToCollider.TryGetValue(e.contacts.Data[i].other, out collider))
                {
                    // handle if particle collides with an object that is PissOnable
                    if (collider.GetComponent(typeof(PissOnable)) is PissOnable)
                    {
                        // destroy particle
                        ObiSolver.ParticleInActor pa = solver.particleToActor[e.contacts[i].particle];
                        ObiEmitter emitter = pa.actor as ObiEmitter;

                        if (emitter != null) {
                            emitter.life[pa.indexInActor] = 0;
                        }
                        /*e.contacts[i].point*/
                    }
                }
            }
        }

    }

    // Start is called before the first frame update
    void Start() {
        pissParticleSystem = gameObject.GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        DestroyAfterEffectHasEnded();
        pissDamage = 1f;
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
            int i = 0;
            while (i < numCollisionEvents) {
                Vector3 collisionHitLoc = collisionEvents[i].intersection;
                CreateSmokeParticleEffect(collisionHitLoc);
                i++;
            }
        }

        if (other.CompareTag("Little Timmy")) {
            int numCollisionEvents = pissParticleSystem.GetCollisionEvents(other, collisionEvents);
            int i = 0;
            while (i < numCollisionEvents) {
                Vector3 collisionHitLoc = collisionEvents[i].intersection;
                CreateBloodParticleEffect(collisionHitLoc);
                i++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPissDamage(float _pissDamage) {
        pissDamage = _pissDamage;
    }

    public float GetPissDamage() {
        return pissDamage;
    }

    // Creates the smoke effect when piss collides with Fire
    void CreateSmokeParticleEffect(Vector3 collisionLocation) {
        GameObject instanciatedSmoke = Instantiate(smoke, collisionLocation, Quaternion.identity);
        ParticleSystem smokeParticleSystem = instanciatedSmoke.GetComponent<ParticleSystem>();
        float timeToWaitBeforeDestroy = smokeParticleSystem.main.startLifetimeMultiplier;
        AdjustSizeOfSmoke(smokeParticleSystem);
        Destroy(instanciatedSmoke, timeToWaitBeforeDestroy);
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

    // Creates the blood effect when piss collides with Little Timmy
    void CreateBloodParticleEffect(Vector3 collisionLocation) {
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

    void PissOnFire() {
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
