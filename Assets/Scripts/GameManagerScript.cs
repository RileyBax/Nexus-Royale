using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{

    private List<Vector3> spawnPoint = new List<Vector3>();
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject bot;
    [SerializeField] private List<GameObject> weaponList;
    private int spawnTemp;
    private Collider2D[] nearObject;
    // draw circle for zone

    // Start is called before the first frame update
    void Start()
    {

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
        
    }

    // Update is called once per frame
    void Update()
    {
        
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

                Instantiate(prefab, i + new Vector3(offset, offset, 0), Quaternion.identity);

                spawnPoint.Remove(i);

            }
            
            j++;
        }

    }

    void spawnWeapon(GameObject weapon){

        Instantiate(weapon, new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), 0), Quaternion.identity);

    }

}
