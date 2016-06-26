using UnityEngine;
using System.Collections;

// on input, instantiate a book and throw it in the direction the camera is facing
public class ThrowObj : MonoBehaviour {

	public Rigidbody[] objPrefabs; // will drag this in in inspector
		// alternate is to put the prefabs in Assets/Resources folder and instantiate with Load.Resources


	public string throwButton = "Fire1";  // this is for testing in Editor

//	public GameObject cam;
//	CardboardHead head = null;
	Camera head = null;

	// Use this for initialization
	void Start () {
		head = Camera.main; //.GetComponent<StereoController>().Head;
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetButtonDown (throwButton)) {
//			Debug.Log ("got throw button down");
			throwObj();           
		}
	}

	public void throwObj() {
		Rigidbody objPrefab = objPrefabs[Random.Range(0,objPrefabs.Length)];
		int distance = 1; // how far in front of camera to instantiate book
		Rigidbody temp = (Rigidbody) Instantiate(objPrefab, head.transform.position + head.transform.forward*distance, head.transform.rotation);
		
		print (temp);
		temp.isKinematic = false;
		temp.AddForce(new Vector3(head.transform.forward.x, 0.5f, head.transform.forward.z)*5,ForceMode.Impulse);

		temp.AddTorque(.1F,.1F,3F,ForceMode.Impulse);    // give it a little spin   
		
	}
}
