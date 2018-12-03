using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class bulletBehavior : MonoBehaviour {

    public GameObject bulletPrefab;
    public Transform bulletSpawn;
 

    void Fire()
    {
        var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn.position,
            bulletSpawn.rotation);

            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 6;

            Destroy(bullet, 6.0f);
    }

    void Start () {
		
	}

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Fire.
        { 
            Fire();
        }
    }
}
