using UnityEngine;

public class EndButton : MonoBehaviour
{
    public void ToMenu(){
        GameManager.instance.ExitToMenu();
    }
}
