using UnityEngine;
using TMPro;
public class DeathScore : MonoBehaviour
{
    public TMP_Text score;

    void Start()
    {
        score.text = "Score: " + ScoreManager.score.ToString();
    }
}
