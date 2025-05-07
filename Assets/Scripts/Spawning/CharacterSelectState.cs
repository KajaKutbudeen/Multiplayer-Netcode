using System;
using Unity.Netcode;
using UnityEngine;

    public struct CharacterSelectState : INetworkSerializable, IEquatable<CharacterSelectState>
    {
        public ulong Clientid;

        public int Characterid;

        public CharacterSelectState(ulong clientid, int characterid = -1)
        {
            Clientid = clientid;
            Characterid = characterid;
        }
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Clientid);
            serializer.SerializeValue(ref Characterid);

        }
        public bool Equals(CharacterSelectState other)
        {
            return Clientid == other.Clientid && 
                Characterid == other.Characterid;
        }
    }

