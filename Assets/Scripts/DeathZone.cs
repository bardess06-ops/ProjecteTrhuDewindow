using UnityEngine;

public class DeathZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //comprueba si el objeto que ha entrado es el Player isTrigger!!
        if (collision.CompareTag("Player"))
        {
            //llama al metodo de muerte del Player
            collision.GetComponent<Player>().Death();
      
        }
    }
}
