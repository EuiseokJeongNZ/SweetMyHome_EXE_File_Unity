using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class RandomEnemyA : MonoBehaviour
{
    public float currTime;
    public GameObject monster;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        currTime += Time.deltaTime;

        // the enemy will automatically appear in 5 seconds
        if (currTime > 5)
        {
            // x,y,z random axis
            float newX = Random.Range(-10f, 50f), newY = Random.Range(1f, 5f), newZ = Random.Range(-35f, 60f);

            // move the object to the position
            monster.transform.position = new Vector3(newX, newY, newZ);

            // bring object where positions
            Instantiate(monster, new Vector3(newX, newY, newZ), Quaternion.identity);

            // when it reaches 10 seconds, currTime is initialised 0
            currTime = 0;
        }
    }
}
