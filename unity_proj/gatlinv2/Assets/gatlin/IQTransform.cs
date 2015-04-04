using UnityEngine;
using System.Collections;

public class IQTransform : MonoBehaviour {
	
	private bool gyroActive = false, tiltActive = false;

	private const float ROBOTACCELERATION = 1f, TOPVELOCITY = 1f; //absolute value limits
	private float acceleration = 0, velocity = 0, yAxisRotationVelocity = 0; //estimation of current acceleration and velocity of robot

	private float xAxisTranslationVelocity = 0;

	private Quaternion worldRotation, pilotBaseRotation, tiltBaseRotation; 
	private Quaternion robotRotation, userRotation, lastUserRotation; //robot will 
	//private Vector3 robotLocation, userLocation;

	private bool pilotMode = true;
	
	public Joystick3D joystick;
	//public float joystickVelocity = 1, freq = 0;
	
	void Start () {
		Input.gyro.enabled = true; //on disable set these to false to save battery
		Input.compass.enabled = true;
		//Time.fixedDeltaTime = .05f;
		
		//Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToPortrait = false;
		
		//QCARRenderer.Instance.DrawVideoBackground = false;

		worldRotation = Quaternion.identity;
		pilotBaseRotation = Quaternion.identity;
		tiltBaseRotation = Quaternion.identity;

		userRotation = Quaternion.identity;
		lastUserRotation = Quaternion.identity;

	}
	
	void OnEnable() {
		Input.gyro.enabled = true; //on disable set these to false to save battery
		Input.compass.enabled = true;
	}
	
	void OnDisable() {
		Input.gyro.enabled = false; //on disable set these to false to save battery
		Input.compass.enabled = false;
	}
	
	void OnAwake() {
		Input.gyro.enabled = true;
		Input.compass.enabled = true;
	}
	
	void OnApplicationFocus(bool status) {
		Input.gyro.enabled = status;
		Input.compass.enabled = status;
	}
	
	// Update is called once per frame
	void Update () {

		//Add rotation contributions to userRotation
		// several options for robot tracking
		//1. Estimate Robot Pose and continue to rotate towards that
		//2. SIMPLER add acceleration of last frame to current acceleration velocity understanding
		//3. incorporate data from robot to understand how it is turning

		Quaternion newWorldRotation = GetRealWorldRotation(); 

		//Gyro data is combined with rotation here
		if (gyroActive) {

			Quaternion delta;
			if (pilotMode) {
				delta = Quaternion.Inverse(pilotBaseRotation) * newWorldRotation; //AToB = Inverse(A) * B;
				delta = Quaternion.Slerp ( Quaternion.identity, delta, .08f);
			} else
				delta = Quaternion.Inverse(worldRotation) * newWorldRotation; //AToB = Inverse(A) * B;
			

			//find change in direction from vector pointing "forward" (Vector3(0, 0, 1))
			Vector3 direT = delta * Vector3.forward;
			direT = new Vector3(direT.x, 0, Mathf.Sqrt(1 - direT.x * direT.x)); //-direT.x
		
			delta = Quaternion.LookRotation(direT, Vector3.up);

			//update userRotation by adding delta rotation to it
			userRotation = userRotation * delta; 
		}

		if (tiltActive) {
			
			Quaternion delta;

			delta = Quaternion.Inverse(tiltBaseRotation) * newWorldRotation; //AToB = Inverse(A) * B;
			//delta = Quaternion.Slerp ( Quaternion.identity, delta, .08f);

			//find change in direction from vector pointing "forward" (Vector3(0, 0, 1))
			Vector3 direT = delta * Vector3.forward;
			//direT = new Vector3(0, direT.y, Mathf.Sqrt(1 - direT.y * direT.y)); //-direT.x
			
			//delta = Quaternion.LookRotation(direT, Vector3.up);

			float sign = 1;
			if (direT.y < 0)
				sign = -1;

			direT.y = sign * (direT.y * direT.y);

			xAxisTranslationVelocity = -10f * direT.y; //20 degree is 1
		}

		//All joystick rotation contributions
		if (joystick.position.x != 0) {
			//rotation rate fed to turtle bot, may be wrong
			float angularVelocityTemp = Time.deltaTime * joystick.position.x * 2 * 180 / Mathf.PI;
			userRotation = userRotation * Quaternion.Euler ( 0f, angularVelocityTemp, 0f);
		}

		//NOTE: finger screen drag rotation is called by input manager and it calls fingerRotation()

		worldRotation = newWorldRotation;
	}

	void LateUpdate() {
		//sums all fingers and gyro rotation into new transform.rotation.
		//find delta between of rotation and rotate something to get a constant
		//AToB = Inverse(A) * B;
		Quaternion delta = Quaternion.Inverse (lastUserRotation) * userRotation; //change of rotation in degrees about y axis
		Vector3 dire = delta * Vector3.forward;

		// y= 0pp, x = adj
		float rads = Mathf.Atan2 (dire.x, dire.z);
		yAxisRotationVelocity = -(rads / Time.deltaTime); // divide by 2 pi

		lastUserRotation = userRotation;
		//transform.rotation = userRotation;
		//transform.position += transform.rotation * Vector3.forward * Time.deltaTime * (joystick.position.y + xAxisTranslationVelocity);
	}

	//rotates left or right based on rotation
	public float GetYRotationVelocity() {
		return yAxisRotationVelocity;
	}

	//forward or backward based on tilt, x axis rotation
	public float GetXTranslationVelocity() {
		return xAxisTranslationVelocity;
	}

	public void fingerRotation (Vector2 drag) {
		userRotation = Quaternion.Euler (0, 0.25f*drag.x, 0) * userRotation;
	}

	public void toggleGyroOn(bool ga) {
		if (ga && pilotMode && ga != gyroActive) 
			pilotBaseRotation = worldRotation;

		gyroActive = ga;
	}

	public void toggleTiltOn(bool ga) {
		if (ga && ga != tiltActive) 
			tiltBaseRotation = worldRotation;

		if (!ga)
			xAxisTranslationVelocity = 0;

		tiltActive = ga;
	}

	//Returns a quaternion that can represent the phone's current rotation relative to ground. 
	//will rotate properly in all direction too, but will start facing a random direction possibly
	private Quaternion GetRealWorldRotation() {
		return Quaternion.Euler (90, 0, 0) * Input.gyro.attitude * Quaternion.Euler(0, 0, 180);
	}
}