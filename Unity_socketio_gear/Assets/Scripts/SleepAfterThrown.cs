using UnityEngine;
using System.Collections;

public class SleepAfterThrown : MonoBehaviour {
	// this is for books that are thrown (and therefore not "woken when bumped"): set timer to make sure they eventually become kinematic

	private Rigidbody rb;
	private float startTime;
	
	public float stopPhysicsAfter = 20.0F;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();	
		// i am instantiated only when used, so should set up timer here
		startTime = Time.time;
//		Debug.Log ("SleepAfterThrown:Start "+gameObject.name + " startTime was "+startTime+ " and sleeping = "+rb.IsSleeping());
		StartCoroutine("enoughAlready");  // set timer for when to stop physics
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator enoughAlready() {
//		Debug.Log ("SleepAfterThrown:enoughAlready() "+gameObject.name + " enoughAlready time is: "+Time.time+" and startTime was "+startTime+ " and sleeping = "+rb.IsSleeping());

		if ((Time.time - startTime) < stopPhysicsAfter) {  // if haven't exceeded timeout
			if (((Time.time - startTime) < 1.0) ||  // if i've just been hit or....
			    !rb.IsSleeping()) {  // let it run 5 more seconds
				yield return new WaitForSeconds(5.0f);
			}
		}
		stopPhysics();
	}

	void stopPhysics() {
//				Debug.Log ("stopPhysics on "+gameObject.name);
		if (!rb.isKinematic) {
			
			//Debug.Log (gameObject.name + " was Collided by " + col.gameObject.name);
			rb.isKinematic = true;
			//Debug.Log ("isKinematic now = " + rb.isKinematic);
			
		}
	}

	// can't use staystilltilbumped on thrown books, so include its functionality here, so it is awaken if collided
	void OnCollisionEnter (Collision col) {
		if (rb.isKinematic) {
			startTime = Time.time;
			
//			Debug.Log (gameObject.name + " was Collided by " + col.gameObject.name + " at "+startTime+" seconds");
			rb.isKinematic = false;
			//Debug.Log ("isKinematic now = " + rb.isKinematic);
			

			StartCoroutine("enoughAlready");  // set timer for when to stop physics
			
		}
	}

}
