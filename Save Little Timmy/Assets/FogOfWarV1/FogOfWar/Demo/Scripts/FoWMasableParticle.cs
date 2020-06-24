using System.Collections;
using System.Collections.Generic;
using ThirdPart;
using UnityEngine;

public class FoWMasableParticle : MonoBehaviour
{
    public FogArea fogArea;

    private ParticleSystem particleSys;
    public Color visableColor = new Color(1,0,0,0.5f);
    public Color invisableColor = new Color(0,0,0,0.5f);
    // Start is called before the first frame update
    void Start()
    {
        particleSys = GetComponent<ParticleSystem>();
    }


    // Update is called once per frame
    void Update()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSys.particleCount];
        particleSys.GetParticles(particles);
        List<Color> colors = new List<Color>();
        int errorCode;
        for(int i = 0; i < particles.Length; i++) {
            var temp = particles[i];
            var pPos = particles[i].position;
            if(particleSys.main.simulationSpace == ParticleSystemSimulationSpace.Local) {
                pPos = particleSys.transform.TransformPoint(pPos);
            }
            if(fogArea.TargetIsVisible(pPos,out errorCode)) {
                temp.startColor = visableColor;
                temp.position += temp.totalVelocity * Time.deltaTime * 10;
            } else {
                temp.startColor = invisableColor;
            }
            particles[i] = temp;
        }
        particleSys.SetParticles(particles);
    }
}
