// StayStillTilBumped: use on the enclosing one of a set of objects that have rigidbodies but are kinematic
// when the object is collided, will turn kinematic off so that physics can operate on all objects in the group
// and after n seconds, it will turn physics off again so the game doesn't slow down for too long

using UnityEngine;
using System.Collections;

public class StayStillTilBumpedAllSibs : MonoBehaviour {
	private Rigidbody rb;
	private float startTime;

	public float stopPhysicsAfter = 20.0F;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	

	void OnCollisionEnter (Collision col) {
	  if (rb.isKinematic) {
			startTime = Time.time;

			Debug.Log (gameObject.name + " was Collided by " + col.gameObject.name + " at "+startTime+" seconds");
			rb.isKinematic = false;
			//Debug.Log ("isKinematic now = " + rb.isKinematic);

			// get this object's parent
			Transform pt = transform.parent;
			if (pt != null) {
				foreach (Transform child in pt) {
				  if (child.gameObject.activeInHierarchy == true) {  // how do i check this?
					// Debug.Log ("got child "+child.name);
					Rigidbody childRb = child.GetComponent<Rigidbody> ();
					if (childRb != null) {
						//Debug.Log (" and rb iskinematic = "+childRb.isKinematic);
						childRb.isKinematic = false;
					}
				  }
				}
			}
			StartCoroutine("enoughAlready");  // set timer for when to stop physics

		}
	}


	IEnumerator enoughAlready() {

		while ((Time.time - startTime) < stopPhysicsAfter) {
			Debug.Log (gameObject.name + " enoughAlready time is: "+Time.time+" and startTime was "+startTime+ " and sleeping = "+rb.IsSleeping());
			yield return new WaitForSeconds(5.0f);
		}
		stopPhysics();
	}

	void stopPhysics() {
//		Debug.Log ("stopPhysics on "+gameObject.name);
		if (!rb.isKinematic) {

			//Debug.Log (gameObject.name + " was Collided by " + col.gameObject.name);
			rb.isKinematic = true;
			//Debug.Log ("isKinematic now = " + rb.isKinematic);
			
			// get this object's parent
			Transform pt = transform.parent;
			if (pt != null) {
				foreach (Transform child in pt) {
					if (child.gameObject.activeInHierarchy == true) {  // how do i check this?
						//Debug.Log ("got child "+child.name);
						Rigidbody childRb = child.GetComponent<Rigidbody> ();
						if (childRb != null) {
							//Debug.Log (" and rb iskinematic = "+childRb.isKinematic);
							childRb.isKinematic = true;
						}
					}
				}
			}
		}
	}

//	void OnCollisionStay(Collision col) {
//		Debug.Log("Collision stay "+col.gameObject.name);
//		print("Collision stay "+col.gameObject.name);
//		foreach (ContactPoint contact in col.contacts) {
//			Debug.DrawRay(contact.point, contact.normal * 10, Color.white);
//		}
//	}
}
