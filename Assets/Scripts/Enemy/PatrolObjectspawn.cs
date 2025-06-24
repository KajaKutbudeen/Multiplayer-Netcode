using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PatrolObjectspawn : MonoBehaviour
{
    public PatrolLocations _PL;

    public List<Transform> ReciveList(List<Transform> objlist,int id)
    {
        PatrolObjects patrol = _PL.PatrolObjects[id];
        for (int i = 0; i < patrol.gameObjects.Length; i++)
        {
            objlist.Add(patrol.gameObjects[i]);
        }
        return objlist;
    }
}
