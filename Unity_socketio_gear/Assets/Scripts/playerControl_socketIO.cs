#region License
/*
 * playerControl_socketIO.cs
 * 
 * modified from TestSocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 Fabio Panettieri
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System.Collections;
using UnityEngine;
using SocketIO;

public class playerControl_socketIO : MonoBehaviour
{
	private SocketIOComponent socket;

	private bool moving = false;
	Camera head = null;
	private ThrowObj throwObj;
	private bool sockIOError = false;
	GameObject settingsScreen;

	[Tooltip ("Check this if building for Google Cardboard, otherwise uncheck it.")]
	public bool forCardboard = true;

	//This is the variable for the player speed  -- but controlling this programmatically now
//	[Tooltip("With this speed the player will move.")]
	private float speed;

	public void Start() 
	{

		Debug.Log ("Starting TestSocketIOobjcontrolSpeed");
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		settingsScreen = GameObject.Find ("Settings Canvas");
		// first thing: turn the screen off.
		settingsScreen.SetActive (false);

//		if (forCardboard) {
//			head = Camera.main.GetComponent<StereoController>().Head;
//		} else {
//			head = Camera.main;
//		}
		head = Camera.main;
		throwObj = GameObject.FindObjectOfType(typeof(ThrowObj)) as ThrowObj;

		socket.On("open", doOpen);
		socket.On("error", doError);
		socket.On("close", doClose);
		socket.On ("move", doMove);
		socket.On ("stop", doStop);
		socket.On ("throw", doThrow);
		socket.On ("restart", doRestart);
		socket.On("boop", TestBoop);
		

	}

	public void Update() {

		if (moving) 
		{
			Vector3 direction = new Vector3(head.transform.forward.x, 0, head.transform.forward.z).normalized * speed * Time.deltaTime;
			Quaternion rotation = Quaternion.Euler(new Vector3(0, -transform.rotation.eulerAngles.y, 0));
			transform.Translate(rotation * direction);
		}
		if (sockIOError) {
			Debug.Log ("Got socket IO error.  close socket and open settings screen.");
			//sockIOError = false;
			//Application.LoadLevel("Settings");
			sockIOError = false;
			if (socket.IsConnected) socket.Close(); // assume our error is because connection broke
			settingsScreen.SetActive(true);

		}

	}

	public void doOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}

	public void doMove(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Move received: " + e.name + " -> " + e.data);

		moving = true;
		JSONObject dat = e.data.GetField ("mag");

		speed = float.Parse (dat.str);
		Debug.Log ("[SocketIO] data: "+dat+" is type "+dat.type+" and yielded speed="+speed);

		
		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
			);
	}

	public void doThrow(SocketIOEvent e) 
	{
		Debug.Log("[SocketIO] Throw received: " + e.name + " -> " + e.data);

		throwObj.throwObj();

		if (e.data == null) { return; }
		
		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
			);
	}

	public void doRestart(SocketIOEvent e) {
		// NOTE: In order for this to work properly, Lightmaps have to be completely baked and not set
		//  to "Auto" build -- otherwise, level will restart with no environmental lighting.  This is 
		//  probably only when running in editor, not standalone
		Debug.Log("[SocketIO] Restart received: " + e.name + " -> " + e.data);

		Application.LoadLevel (Application.loadedLevelName);

		if (e.data == null) { return; }
		
		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
			);
	}
		
	public void doStop(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Stop received: " + e.name + " " + e.data);
		
		moving = false;
		
		if (e.data == null) { return; }
		
		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
			);
	}

	public void TestBoop(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Boop received: " + e.name + " " + e.data);

		if (e.data == null) { return; }

		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
		);
	}
	
	public void doError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error Event received: " + e.name + " " + e.data);
		sockIOError = true;

	}
	
	public void doClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] in TestSocketIO Close received: " + e.name + " " + e.data );
	}


}
