using UnityEngine;

public class FootstepParticle : MonoBehaviour
{
    
    public ParticleSystem leftFootstepParticle;
    public ParticleSystem rightFootstepParticle;
    
    public void SpawnLeftFootstepParticleEffect()
    {
        leftFootstepParticle.Play();
    }

    public void SpawnRightFootstepParticleEffect()
    {
        rightFootstepParticle.Play();
    }

    public void SpawnDoubleParticleEffect()
    {
        leftFootstepParticle.Play();
        rightFootstepParticle.Play();
    }
}
