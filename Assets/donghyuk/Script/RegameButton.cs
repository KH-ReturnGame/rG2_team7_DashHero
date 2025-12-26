using UnityEngine;
using UnityEngine.SceneManagement;

public class RegameButton : MonoBehaviour
{

    public void StartGame()
    {
        SceneManager.LoadScene("Start Scene");
    }
}
