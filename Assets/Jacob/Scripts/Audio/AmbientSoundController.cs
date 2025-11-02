using UnityEngine;

namespace Audio
{
    public enum SoundType
    {
        Bird,
        Crow,
        Pipe
    }
    
    public class AmbientSoundController : MonoBehaviour
    {
        public SoundType soundType =  SoundType.Bird;
        
        private uint _soundId;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            if (_soundId == 0)
            {
                switch (soundType)
                { 
                    case SoundType.Bird:
                        _soundId = AkUnitySoundEngine.PostEvent("Play_Birds", gameObject);
                        break;
                    case SoundType.Crow:
                        _soundId = AkUnitySoundEngine.PostEvent("Play_Crows", gameObject);
                        break;
                    case SoundType.Pipe:
                        _soundId = AkUnitySoundEngine.PostEvent("Play_Pipe", gameObject);
                        break;
                }
            }
        }
    }
}

