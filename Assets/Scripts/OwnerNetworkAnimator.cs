using Unity.Netcode.Components;
using UnityEngine;

namespace HelloWorld
{
    public class OwnerNetworkAnimator : NetworkAnimator
    {
        protected override bool OnIsServerAuthoritative()
        {
            return false;
        }
    }
}
