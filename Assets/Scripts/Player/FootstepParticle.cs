using UnityEngine;

public class FootstepParticle : MonoBehaviour
{
    
    public ParticleSystem leftFootstepParticle;
    public ParticleSystem rightFootstepParticle;
    
    void SpawnLeftFootstepParticleEffect()
    {
        Debug.Log("*Left Particle Effect*");
        leftFootstepParticle.Play();
    }

    void SpawnRightFootstepParticleEffect()
    {
        Debug.Log("*Right Particle Effect*");
        rightFootstepParticle.Play();
    }

    void SpawnSlideParticleEffect()
    {
        leftFootstepParticle.Play();
        rightFootstepParticle.Play();
    }
    
}
