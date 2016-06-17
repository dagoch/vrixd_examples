using UnityEngine;
using System.Collections;

public class cameraMove : MonoBehaviour {

	public float speed = 6.0F;
	public float jumpSpeed = 8.0F;
	public float gravity = 20.0F;
	private Vector3 moveDirection = Vector3.zero;
	void Update() {
		Vector3 pos = transform.position;

		float moveHorizontal = -1*Input.GetAxis ("Vertical");
		float moveVertical = Input.GetAxis ("Horizontal");
		if ((moveHorizontal != 0) || (moveVertical != 0)) {
			print ("Got pos = " + pos);
			print ("got move direction: " + moveHorizontal + " + " + moveVertical);
			pos.x += moveHorizontal * speed;
			pos.z += moveVertical * speed;
			
			transform.position = pos;
		}

	}

	void OnTriggerEnter(Collider collider) {
		// how to avoid running through a bookcase?
	}
}
