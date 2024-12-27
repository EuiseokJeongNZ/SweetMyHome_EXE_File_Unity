using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomItemForAmmo : MonoBehaviour
{
    public float currTime;
    public GameObject ammo;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // make time to go
        currTime += Time.deltaTime;


        // the enemy will automatically appear in 5 seconds
        if (currTime > 30)
        {
            // x,y,z random axis
            float newX = Random.Range(-10f, 40f), newY = Random.Range(4.5f, 5f), newZ = Random.Range(-30f, 50f);

            // move the object to the position
            ammo.transform.position = new Vector3(newX, newY, newZ);

            // bring object where positions
            Instantiate(ammo, new Vector3(newX, newY, newZ), Quaternion.identity);

            // when it reaches 10 seconds, currTime is initialised 0
            currTime = 0;
            Debug.Log("Ammo Created!");
        }
    }
}
