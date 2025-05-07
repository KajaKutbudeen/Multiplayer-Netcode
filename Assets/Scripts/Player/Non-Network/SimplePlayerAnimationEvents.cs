using UnityEngine;
using UnityEngine.XR;

public class SimplePlayerAnimationEvents : MonoBehaviour
{
    private Animator _anim;
    public SimpleMovement _sm;
    public Rigidbody _rb;
    [SerializeField]
    float jumpamount = 2f;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }

    public void Jump()
    {
        _rb.AddForce(Vector3.up * jumpamount,ForceMode.Impulse);
    }

   
    public void OffAttack()
    {   
        _sm.PlayerAttackOff();
        _anim.ResetTrigger("Combat");
    }
}
