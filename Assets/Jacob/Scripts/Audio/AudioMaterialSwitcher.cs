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
            if (other.CompareTag("Player"))
            {
                switch (materialToSwitchTo)
                {
                case MaterialTypes.Concrete:
                    AkUnitySoundEngine.SetSwitch("Run", "Conc", other.gameObject);

                    break;
                case MaterialTypes.Metal:
                    AkUnitySoundEngine.SetSwitch("Run", "Metal", other.gameObject);

                    break;
                case MaterialTypes.Wood:
                    AkUnitySoundEngine.SetSwitch("Run", "Wood", other.gameObject);

                    break;
                }
            }
        }
    }

}
