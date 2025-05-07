using UnityEngine;

public class HitEnemyDetect : MonoBehaviour
{
    public int id;
  //  public EnemyHealth EnemyHealth;
    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
           // other.gameObject.GetComponent<EnemyHealth>().GetHit();
            other.gameObject.GetComponent<EnemyHealthNetwork>().GetHit();
        }
    }
}
