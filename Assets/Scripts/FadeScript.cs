using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{

    private RawImage image;
    private float timer = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
        
        image = GetComponent<RawImage>();

    }

    // Update is called once per frame
    void Update()
    {

        image.color = new Color(0,0,0, 1.0f * (timer / 3.0f));
        timer -= Time.deltaTime;

        if(timer <= 0.0f) Destroy(this.gameObject);

    }
}
