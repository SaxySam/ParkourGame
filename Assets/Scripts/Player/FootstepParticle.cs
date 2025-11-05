using UnityEngine;

public class FootstepParticle : MonoBehaviour
{
    
    public ParticleSystem leftFootstepParticle;
    public ParticleSystem rightFootstepParticle;
    
    void SpawnLeftFootstepParticleEffect()
    {
        leftFootstepParticle.Play();
    }

    void SpawnRightFootstepParticleEffect()
    {
        rightFootstepParticle.Play();
    }

    void SpawnSlideParticleEffect()
    {
        leftFootstepParticle.Play();
        rightFootstepParticle.Play();
    }
}
