using System;
using UnityEditor.Localization.Plugins.XLIFF.V20;
using UnityEngine;

namespace Audio
{
    public class PlayerAudio : MonoBehaviour
    {
        public GameObject player;

        private void OnEnable()
        {
            GameManager.PlayerLandedEvent += OnLand;
        }

        void Start()
        {
            uint bankID;
            AkUnitySoundEngine.LoadBank("PlayerSound", out bankID);
            AkUnitySoundEngine.SetSwitch("Run", "Conc", player);
            AkUnitySoundEngine.SetSwitch("Land", "Conc", player);
        }

        void OnFootstep()
        {
            AkUnitySoundEngine.PostEvent("Play_Run", player);
        }

        void OnLand()
        {
            AkUnitySoundEngine.PostEvent("Play_Land", player);
        }

        void OnStartSlide()
        {
            AkUnitySoundEngine.PostEvent("Play_Slide", player);
        }

        void OnStopSlide()
        {
            AkUnitySoundEngine.PostEvent("Play_Slide_end", player);
        }
    }
}
