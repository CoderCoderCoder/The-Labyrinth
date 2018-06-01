using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollision : MonoBehaviour
{

    public GameObject[] walls;

    // Use this for initialization
    void Start()
    {
        walls = GameObject.FindGameObjectsWithTag("wall");
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = new Vector3(1, 0, 1);
        checkIfPosEmpty(transform.position + v);
    }

    public bool checkIfPosEmpty(Vector3 targetPos)
    {
        for (int i = 0; i < walls.Length; i++)
        {
            if ( Vector3.Distance(walls[i].transform.position,transform.position) >= 1
                && Vector3.Distance(walls[i].transform.position, transform.position) <= 2)
            {
                return true;
            }
        }
        return false;
    }
}
