using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    private List<Vector3> spawnPoint = new List<Vector3>();
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bot;
    [SerializeField] private List<GameObject> weaponList;
    private int spawnTemp;
    private Collider2D[] nearObject;
    [SerializeField] private LineRenderer lr;
    [SerializeField] private PolygonCollider2D zoneCollider;
    private int circlePoints = 60;
    private float radius = 80;
    private Vector3 circlePointPos;
    private Vector3 zone;
    private float width = 0.25f;
    private Vector2[] colliderPoints;

    // Start is called before the first frame update
    void Start()
    {

        zone = new Vector3(UnityEngine.Random.Range(-40, 40), UnityEngine.Random.Range(-30, 30), 0); // choose random position

        // Set spawn posiitons for 20 characters in even grid
        for(int y = -2; y < 3; y++){

            for(int x = -2; x < 3; x++){

                spawnPoint.Add(new Vector3(x * 20, y * 20, 0));

            }

        }

        // Instantiate Player
        spawnCharacter(player);

        // Instantiate bots for remaining points
        while(spawnPoint.Count > 0){

            spawnCharacter(bot);

            int randWeapon = UnityEngine.Random.Range(0, 3);
            int j = 0;

            // spawn weapon
            foreach(GameObject i in weaponList){

                if(randWeapon == j) spawnWeapon(i); 

                j++;
            }

        }
        
        lr.positionCount = circlePoints;
        lr.startWidth = width;
        lr.endWidth = width;

        colliderPoints = new Vector2[circlePoints];

        transform.position = zone;

    }

    // Update is called once per frame
    void Update()
    {

        for(int i = 0; i < circlePoints; i++){

            circlePointPos.x = (float)(zone.x + Math.Sin((2 * Math.PI) / circlePoints * i + 1) * radius);
            circlePointPos.y = (float)(zone.y + Math.Cos((2 * Math.PI) / circlePoints * i + 1) * radius);

            lr.SetPosition(i, circlePointPos);

            colliderPoints[i] = new Vector2(circlePointPos.x, circlePointPos.y) - (Vector2) zone;

        }

        zoneCollider.SetPath(0, colliderPoints);

        if(radius >= 10) radius -= Time.deltaTime;

    }

    void spawnCharacter(GameObject prefab){

        spawnTemp = UnityEngine.Random.Range(0, spawnPoint.Count);
        int j = 0;

        foreach(Vector3 i in spawnPoint.ToList()){

            int offset = 0;

            if(j == spawnTemp) {

                bool spawnLoop = true;
                
                while(spawnLoop){

                    nearObject = Physics2D.OverlapCircleAll(i + new Vector3(offset, offset, 0), 2);
                    spawnLoop = false;

                    for(int k = 0; k < nearObject.Length; k++){

                        if(nearObject[k].tag.Equals("Object")) {
                            offset++;
                            spawnLoop = true;
                        }

                    }

                }

                Instantiate(prefab, i + new Vector3(offset, offset, 0), Quaternion.identity).SendMessage("setZone", zone);

                spawnPoint.Remove(i);

            }
            
            j++;
        }

    }

    void spawnWeapon(GameObject weapon){

        Instantiate(weapon, new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), 0), Quaternion.identity);

    }

}
