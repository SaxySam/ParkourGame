using UnityEngine;

namespace SDK
{
    public class Respawn : MonoBehaviour
    {
        [SerializeField] private Transform player;
        [SerializeField] private Transform respawnPoint;

        private void OnTriggerEnter(Collider playerCol)
        {
            if (!playerCol.gameObject.CompareTag("Player")) return;
            
            Debug.Log("Respawn player");
            playerCol.gameObject.GetComponent<SamCharacterController>().enabled = false;
            player.transform.position = respawnPoint.transform.position;
            playerCol.gameObject.GetComponent<SamCharacterController>().enabled = true;

        }
    }
}