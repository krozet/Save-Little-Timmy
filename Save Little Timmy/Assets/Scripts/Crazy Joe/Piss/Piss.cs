﻿using Obi;
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

    // Start is called before the first frame update
    void Start() {
        pissParticleSystem = gameObject.GetComponent<ParticleSystem>();
        collisionEvents = new List<ParticleCollisionEvent>();
        pissedOnParticleEffectManager = new PissedOnParticleEffectManager();

        pissDamage = 1f;
    }

    // Detects when obi particle collides with obi collider
    void Solver_OnCollision(object sender, Obi.ObiSolver.ObiCollisionEventArgs e) {
        // if suffering from performance issues, try using a hashset?
        for (int i = 0; i < e.contacts.Count; ++i) {
            if (e.contacts.Data[i].distance < 0.06f) {
                Component collider;
                if (ObiCollider.idToCollider.TryGetValue(e.contacts.Data[i].other, out collider)) {
                    // handle if particle collides with an object that is PissOnable
                    if (collider.GetComponent(typeof(PissOnable)) is PissOnable) {
                        // destroy particle
                        ObiSolver.ParticleInActor pa = solver.particleToActor[e.contacts[i].particle];
                        ObiEmitter emitter = pa.actor as ObiEmitter;
                        
                        if (emitter != null) {
                            emitter.life[pa.indexInActor] = 0;
                        }

                        pissedOnParticleEffectManager.SpawnPissedOnParticleEffect(collider, pa.actor.GetParticlePosition(e.contacts[i].particle));
                    }
                }
            }
        }
    }

    void OnEnable() {
        solver.OnCollision += Solver_OnCollision;
    }

    void OnDisable() {
        solver.OnCollision -= Solver_OnCollision;
    }

    public void SetPissDamage(float _pissDamage) {
        pissDamage = _pissDamage;
    }

    public float GetPissDamage() {
        return pissDamage;
    }
}
