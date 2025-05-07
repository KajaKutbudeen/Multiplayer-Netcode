using UnityEngine;

public class DetectPlayertrigger : MonoBehaviour
{

    public bool isplayerdetected = false;
    public bool isplayernear = false;
    public NetworkEnemy _ne;
    public bool playerdetect
    {
        get
        {
            return isplayerdetected;
        }

        set
        {
            isplayerdetected = value;
        }
    }

    public bool playernear
    {
        get
        {
            return isplayernear;
        }
        set
        {
            isplayernear = value;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                _ne.Player = other.gameObject;
                playerdetect = true;
                
            }
        }    

    }


    private void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isplayernear = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other!=null)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                playerdetect = false;
                _ne.Player = null;
            }
        }
    }


}
