using UnityEngine;
using System.Collections;

public class RemoveStuffOutBox : MonoBehaviour {

    // Clever idea taken from Unity's spaceship shooter tutorial to remove objects that
    // are out of bounds.

    void OnTriggerExit(Collider other)
    {
        //Debug.Log("OnTriggerExit :" + other.tag);
        Destroy(other.gameObject);
    }
}
