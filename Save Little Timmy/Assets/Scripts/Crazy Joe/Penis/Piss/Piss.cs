using Obi;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* 
    Handles individual piss particles.
     */
public class Piss : MonoBehaviour
{
    public ObiSolver solver;

    PissedOnParticleEffectManager pissedOnParticleEffectManager;
    float pissDamage;

    void Awake() {
    }

    // Start is called before the first frame update
    void Start() {
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
                    if (collider.transform.Find("NotPissOnable") != null) {
                        continue;
                    }
                    // handle if particle collides with an object that is PissOnable
                    if (collider.GetComponent(typeof(PissOnable)) is PissOnable) {
                        // destroy particle
                        ObiSolver.ParticleInActor pa = solver.particleToActor[e.contacts[i].particle];
                        ObiEmitter emitter = pa.actor as ObiEmitter;
                        
                        if (emitter != null) {
                            emitter.life[pa.indexInActor] = 0;
                        }

                        // Make these spawn much less frequently
                        // maybe a max of 10?
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
