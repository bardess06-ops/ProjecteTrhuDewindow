using UnityEngine;

public class ActivateVaho : MonoBehaviour
{
    public Animator anim;
    public float temporizador = 0.0f;
    public bool isVaho = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        temporizador += Time.deltaTime;


        if (temporizador >= 60.0f)
        {
            isVaho = true;
        }
        return;
    }

}
