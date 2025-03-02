using UnityEngine;

public class NextLevelTrigger : MonoBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            GameSceneManager.instance.LoadNextLevel();
        }
    }
}
