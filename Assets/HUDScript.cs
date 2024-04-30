using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
        transform.position = new Vector3(Screen.width - 100, Screen.height - 100, 0);
        Debug.Log(new Vector3(Screen.width - 100, Screen.height - 100, 0));

    }

    // Update is called once per frame
    void Update()
    {
        


    }
}
