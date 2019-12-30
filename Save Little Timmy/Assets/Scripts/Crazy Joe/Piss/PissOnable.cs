// This interface is used to indicate that an object should have a 
// specified reaction when Piss collides with its obi collider

using UnityEngine;

interface PissOnable {
    void HandlePiss();

    GameObject GetPissedOnParticleEffect();
}
