using UnityEngine;
using System.Collections;

public class DestroyLostBall : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < 0) {  // I've fallen off the edge of the world
			Destroy(gameObject);   // destroy me
		}
	
	}
}
