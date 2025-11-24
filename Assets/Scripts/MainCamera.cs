using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainCamera : MonoBehaviour
{
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

    public void Nivel1()
    {
        SceneManager.LoadScene(1);
    }

    public void Nivel2()
    {
        SceneManager.LoadScene(2);
    }

    public void Nivel3()
    {
        SceneManager.LoadScene(3);
    }

    public void Controls()
    {
        SceneManager.LoadScene(4);
    }

    public void Musica()
    {
        SceneManager.LoadScene(5);
    }

    public void Creditos()
    {
        SceneManager.LoadScene(6);
    }

    



}
