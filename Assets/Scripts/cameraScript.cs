using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScript : MonoBehaviour
{

    [SerializeField] private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        // Follows player character
        if(player != null) transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);

    }

    void setPlayer(GameObject p){

        player = p;

    }

}
