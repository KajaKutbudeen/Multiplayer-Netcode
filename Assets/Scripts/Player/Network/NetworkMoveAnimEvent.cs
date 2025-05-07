using HelloWorld;
using UnityEngine;
using Unity.Netcode;
public class NetworkMovementEvent : NetworkBehaviour
{
    private Animator _anim;
    public CharacterPlayer _player;
    [SerializeField]
    float jumpamount = 2f;
    public Rigidbody rb;

    private void Start()
    {
        _anim = GetComponent<Animator>();
    }
    public void Jump()
    {
        rb.AddForce(Vector3.up * jumpamount, ForceMode.Impulse);
    }
    public void OffAttack()
    {
        _player.PlayerAttackOff();
        _anim.ResetTrigger("Combat");
    }
}
