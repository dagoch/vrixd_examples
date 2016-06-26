using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using SocketIO;

public class ConfigScreen : MonoBehaviour {

	public string ipAddr = "127.0.0.1";
	public string port = "4567";
	public InputField ipAddrInpField;
	public InputField portInpField; 
	public Text connectStatus;
	public SocketIOComponent sockio;

	// Use this for initialization
	void Start () {

		Debug.Log ("ConfigScreen start with ipAddr= "+ipAddr+":"+port);
		GameObject SocketIOobj = GameObject.Find ("SocketIO");
		sockio = (SocketIOComponent)SocketIOobj.GetComponent("SocketIOComponent");
 
		sockio.Close();

		if (PlayerPrefs.HasKey("IPAddress")) {

			ipAddr =  PlayerPrefs.GetString("IPAddress");
			Debug.Log ("PlayerPrefs has IPAddress = "+ipAddr);
		} else {
			ipAddr = sockio.ipAddr;
		}
		if (PlayerPrefs.HasKey("Port")) {
			
			port =  PlayerPrefs.GetString("Port");
			Debug.Log ("PlayerPrefs has Port = "+port);
		} else {
			port = sockio.port;
		}

		connectStatus.text = sockio.IsConnected ? "Connected" : "Not Connected";
		ipAddrInpField.text = ipAddr;
		portInpField.text = port;

		Debug.Log ("ConfigScreen start now ending with ipAddr= "+ipAddr+":"+port+" and connectStatus = "+connectStatus.text);

		//Text ipTextVal = ipAddrTextField.GetComponent<Text>(); 
		//GameObject ipAddrTextFieldPlaceholder = GameObject.Find("IpAddr-Placeholder");
		//Text placeholder = ipAddrTextFieldPlaceholder.GetComponent<Text>();
		//ipTextVal.text = ipAddr;
		//placeholder.text = ipAddr;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Called when the player clicks the Apply Button, this saves all of the values.
	/// </summary>
	public void OnApply()
	{
		ipAddr = ipAddrInpField.text;
		port = portInpField.text;
		// Set the VolumeLevel float.
		//PlayerPrefs.SetFloat("VolumeLevel", volumeSlider.normalizedValue);
		PlayerPrefs.SetString("IPAddress",ipAddr);
		PlayerPrefs.SetString("Port",port);

		PlayerPrefs.Save ();

		sockio.port = port;  
		sockio.ipAddr = ipAddr;

		Debug.Log ("Connect Button pressed, ipaddr = "+ipAddr+" and port = "+port);
		Application.LoadLevel("151210_1600_tryoptimize-ok");

	}
}
