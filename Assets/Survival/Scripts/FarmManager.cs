using UnityEngine;

public class FarmManager : MonoBehaviour
{
        public GameObject healthBarCanvas;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        this.Invoke(() =>
        {
        Transform playerTransform = GameObject.FindWithTag("Player").transform;

             if (healthBarCanvas != null && playerTransform != null)
                {
                    Instantiate(healthBarCanvas, playerTransform);
                }
        },2.5f);
                
               
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
