using UnityEngine;
using Unity.Netcode;
public class EnemyHealthNetwork : NetworkBehaviour
{
    [Range(0, 5)]
    public float Health;
    public NetworkEnemy NetworkEnemy;

    public void GetHit()
    {
        Debug.Log("got hit");
        if (Health == 0)
        {
        //    enemy.Death();
            GetComponent<EnemyHealth>().enabled = false;
        }
        
        NetworkEnemy.GotHit();
        Health -= 1f;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                NetworkEnemy.Player = collision.gameObject;
            }
        }
    }
}
