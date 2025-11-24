using UnityEngine;
using UnityEngine.SceneManagement;

public class CambioEscena : MonoBehaviour
{
    public void IrAEscenaGame()
    {
        SceneManager.LoadScene("Game");
    }
}