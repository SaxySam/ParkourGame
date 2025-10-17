using UnityEngine;
using System.Collections;


namespace SDK
{
    public class PlayerFootsteps : MonoBehaviour
    {
        public void PlayFootstepParticle(ParticleSystem footstepParticle)
        {
            footstepParticle.Play();
            //Play Wwise Footstep
        }

        public void PlayLandParticle(ParticleSystem landParticle)
        {
            landParticle.Play();
        }

        public void PrintEvent(string s)
        {
            Debug.Log("PrintEvent called at " + Time.time + " with a value of " + s);
        }
    }
}