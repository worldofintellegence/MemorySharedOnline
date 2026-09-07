using SurvivalEngine;
using UnityEngine;
using UnityEngine.UI;

public class AttackBtn : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerCharacter player;
    void Start()
    {
        player = FindAnyObjectByType<PlayerCharacter>();    
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(player != null)
             player.Attack();
             else
              player = FindAnyObjectByType<PlayerCharacter>();    
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
