using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Range(0,5)]
    public float Health;
    public Enemy enemy;
    
    
    public void GetHit()
    {
        if (Health == 0)
        {
            enemy.Death();
            GetComponent<EnemyHealth>().enabled = false;
        }
        enemy.GotHit();
      
        Health -= 1f;
    }
}
