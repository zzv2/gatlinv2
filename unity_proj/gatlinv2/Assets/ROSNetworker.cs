using UnityEngine;
using System.Collections;
using ROSBridgeLib;
using System.Reflection;
using System;
using ROSBridgeLib.geometry_msgs;
using ROSBridgeLib.turtlesim;

/**
 * This is a toy example of the Unity-ROS interface talking to the TurtleSim 
 * tutorial (circa Groovy). Note that due to some changes since then this will have
 * to be slightly re-written, but as its a test ....
 * 
 * THis does all the ROS work.
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.0
 **/

public class ROSNetworker : MonoBehaviour  {


	public String RosBridgeAddress = "ws://192.168.1.81";
	public int RosBridgePort = 9090;

	public IQTransform iqtransform;
	public Joystick3D joystick;

	private ROSBridgeWebSocketConnection ros = null;	
	//private Boolean _useJoysticks;
	//private Boolean lineOn;


	
	// the critical thing here is to define our subscribers, publishers and service response handlers
	void Start () {
		ros = new ROSBridgeWebSocketConnection (RosBridgeAddress, RosBridgePort);

		ros.AddPublisher (typeof(TurtlebotTeleop));
		//ros.AddSubscriber (typeof(Turtle1Pose));

		ros.Connect ();
	}

	/*IEnumerator CheckWebServer() {
		//StartDownload
		WWW www = new WWW(RosWebServerAddress);

		//Wait to finish
		while(!www.isDone) 
			yield return;

		// assign the downloaded image to the main texture of the object
		www.LoadImageIntoTexture(videoFeed.material.mainTexture);
		StartCoroutine(CheckWebServer ());
	}*/
	
	// extremely important to disconnect from ROS. OTherwise packets continue to flow
	void OnApplicationQuit() {
		if(ros!=null)
			ros.Disconnect ();
	}
	
	// Update is called once per frame in Unity. The Unity camera follows the robot (which is driven by
	// the ROS environment. We also use the joystick or cursor keys to generate teleoperational commands
	// that are sent to the ROS world, which drives the robot which ...
	void Update () {
		
		float linear = 2f * joystick.position.y;
		float angular = iqtransform.GetYRotationVelocity();
		
		TwistMsg msg = new TwistMsg (new Vector3Msg(linear, 0.0, 0.0), new Vector3Msg(0.0, 0.0, angular));
		
		ros.Publish (TurtlebotTeleop.GetMessageTopic (), msg);
		
		/*if (Input.GetKeyDown (KeyCode.T)) {
			if (lineOn)
				ros.CallService ("/turtle1/set_pen", "{\"off\": 1}");
			else
				ros.CallService ("/turtle1/set_pen", "{\"off\": 0}");
			lineOn = !lineOn;
		}*/
		ros.Render ();
		
	}
}