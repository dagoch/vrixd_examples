using UnityEngine;
using System.Collections;

// on input, instantiate a book and throw it in the direction the camera is facing
public class ThrowBook : MonoBehaviour {

	public Rigidbody[] bookPrefabs; // will drag this in in inspector
		// alternate is to put book prefabs in Assets/Resources folder and instantiate with Load.Resources
		//  NOT THIS: = Object.Find"Pile of Book Set Texture Haphazard/Encyclopedia";

	public string throwButton = "Fire1";

//	public GameObject cam;
	CardboardHead head = null;
	// need to figure out object that the camera is facing.  Using CardBoard camera main doesn't work -- always throws to the left;
	// using cardboard head doesn't seem to work either -- always throws to the left side of wherever you're looking
	// but head is what rotates when you rotate the camera, so this should be right.
	// so -- how to get book to throw the actual direction

	// Use this for initialization
	void Start () {
		head = Camera.main.GetComponent<StereoController>().Head;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown (throwButton)) {
//			Debug.Log ("got throw button down");
			throwBook();           
		}
	}

	public void throwBook() {
		Rigidbody bookPrefab = bookPrefabs[Random.Range(0,bookPrefabs.Length)];
		int distance = 1; // how far in front of camera to instantiate book
		Rigidbody temp = (Rigidbody) Instantiate(bookPrefab, head.transform.position + head.transform.forward*distance, head.transform.rotation);
		
		print (temp);
		temp.isKinematic = false;
		temp.AddForce(new Vector3(head.transform.forward.x, 0.5f, head.transform.forward.z)*5,ForceMode.Impulse);
		
		// will get the torque from the position of the controller.
		// or could apply random spin around all three axes
		temp.AddTorque(.1F,.1F,3F,ForceMode.Impulse);    // give it a little english   
		
	}
}
