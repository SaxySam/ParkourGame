using UnityEngine;

namespace Audio
{
    public enum MaterialTypes
    {
        Concrete,
        Metal,
        Wood
    }

    public class AudioMaterialSwitcher : MonoBehaviour
    {
        public MaterialTypes materialToSwitchTo =  MaterialTypes.Concrete;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        void OnTriggerEnter(Collider other)
        {
            GameObject audioPoint = GameObject.Find("CameraFollowPoint");
            if (other.CompareTag("Player"))
            {
                switch (materialToSwitchTo)
                {
                case MaterialTypes.Concrete:
                    AkUnitySoundEngine.SetSwitch("Run", "Conc", audioPoint);
                    AkUnitySoundEngine.SetSwitch("Land", "Conc", audioPoint);
                    break;
                case MaterialTypes.Metal:
                    AkUnitySoundEngine.SetSwitch("Run", "Metal", audioPoint);
                    AkUnitySoundEngine.SetSwitch("Land", "Metal", audioPoint);
                    break;
                case MaterialTypes.Wood:
                    AkUnitySoundEngine.SetSwitch("Run", "Wood", audioPoint);
                    AkUnitySoundEngine.SetSwitch("Land", "Wood", audioPoint);
                    break;
                }
            }
        }
    }

}
