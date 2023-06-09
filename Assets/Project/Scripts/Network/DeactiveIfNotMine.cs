using Photon.Pun;
using UnityEngine;

public class DeactivateIfNotMine : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }
}