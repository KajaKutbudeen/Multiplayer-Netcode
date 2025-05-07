using Unity.Netcode;
using UnityEngine;

namespace HelloWorld
{

    public class Player : NetworkBehaviour
    {
        float hori;
        float verti;
        Vector3 move = Vector3.zero;
        [SerializeField]
        float speed;

        private void Update()
        {
            hori = Input.GetAxisRaw("Horizontal");
            verti = Input.GetAxisRaw("Vertical");
            move = new Vector3(verti, 0, hori);

        }

        private void FixedUpdate()
        {
          //  if (!IsOwner || !Application.isFocused) return;
            Move();
        }
        public void Move()
        {
            transform.Translate(move * speed * Time.deltaTime);
        }
    }
}
