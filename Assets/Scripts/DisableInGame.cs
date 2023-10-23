using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableInGame : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        this.gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
}
