using HelloWorld;
using UnityEngine;
using Unity.Netcode;
using Photon.Pun;

namespace Photon {
    public class PhotonAnimationEvent : MonoBehaviourPunCallbacks
    {
        public Animator _anim;
        public PhotonCharacterPlayer _player;
        [SerializeField]
        float jumpamount = 2f;
        public Rigidbody rb;

        private void Start()
        {
            // _anim = GetComponent<Animator>();
        }
        public void Jump()
        {
            rb.AddForce(Vector3.up * jumpamount, ForceMode.Impulse);
        }
        [PunRPC]
        public void OffAttack()
        {
            _player.PlayerAttackOff();
            _anim.ResetTrigger("Combat");
        }
    }
}
