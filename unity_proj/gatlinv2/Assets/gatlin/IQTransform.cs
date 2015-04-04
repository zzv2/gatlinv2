using UnityEngine;
using System.Collections;

public class IQTransform : MonoBehaviour {
	
	public bool useQCAR = true, gyroActive = false;

	public bool toPositionCamera, snapToFloor = true; 
	
	//these will be set by posters, and added and set as position for ARCamera
	public Vector3 GamePosition, IQVector;
	private Vector3 IQPosition, bufferPosition;
	public Quaternion IQRotation, GameRotation;
	private Quaternion lastGyro; 

	//private CircularPastList<Vector3> pastIQPositions;
	
	//private float pastHeight = 0;
	
	public Joystick3D joystick;
	public float joystickVelocity = 1, freq = 0;
	
	//for compass and gyroscope
	private double lastCompassUpdateTime;
	private Quaternion correction, targetCorrection;
	
	//private GUIManager guiManager;
	
	//public ButtonManager redButtonManager; //used for balancing during shot!
	
	void Start () {
		Input.gyro.enabled = true; //on disable set these to false to save battery
		Input.compass.enabled = true;
		//Time.fixedDeltaTime = .05f;
		
		//Screen.autorotateToLandscapeLeft = false;
		Screen.autorotateToLandscapeRight = false;
		Screen.autorotateToPortraitUpsideDown = false;
		Screen.autorotateToPortrait = false;
		
		//QCARRenderer.Instance.DrawVideoBackground = false;

		GameRotation = Quaternion.identity;
		IQRotation = Quaternion.identity;
		lastGyro = Quaternion.identity;
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
		
		Vector3 newIQPosition = IQVector + bufferPosition;
		
		float deltaheight = 0;
		
		if (newIQPosition.y != IQPosition.y) 
			deltaheight += (newIQPosition.y - IQPosition.y);
		
		IQPosition = newIQPosition; //this is the only mandatory thing in this section


		Quaternion newIQRotation = ComplementaryFilter(); //YGyroRotation();
		if (gyroActive) {
			//AToB = Inverse(A) * B;
			Quaternion delta = Quaternion.Inverse(lastGyro) * newIQRotation;

			Vector3 direT = delta * Vector3.forward;
			direT = new Vector3(direT.x, 0, Mathf.Sqrt(1 - direT.x * direT.x)); //-direT.x
			
			delta = Quaternion.LookRotation(direT, Vector3.up);

			//IQRotation = IQRotation * delta;
			IQRotation = GameRotation * delta;
			// =  tempGameRotation;
		}
		
		// * IQRotation;
		updateJoystick ();

		//transform.rotation = IQRotation;
		//transform.position = IQPosition + GamePosition;

		lastGyro = newIQRotation;
	}

	void LateUpdate() {
		//sums all fingers and gyro rotation into new transform.rotation.
		//find delta between of rotation and rotate something to get a constant
		//AToB = Inverse(A) * B;
		Quaternion delta = Quaternion.Inverse ( GameRotation) * IQRotation; //change of rotation in degrees about y axis
		Vector3 dire = delta * Vector3.forward;

		// y= 0pp, x = adj
		float rads = Mathf.Atan2 (dire.x, dire.z);


		freq = (rads / Time.deltaTime); // divide by 2 pi

		GameRotation = IQRotation;
	}

	public void toggleGyroOn(bool ga) {
		gyroActive = ga;
	}
	
	public void foundPoster(Vector3 firstIQV, bool isFloor) {
		IQVector = firstIQV;
		bufferPosition = -IQVector;
		if (isFloor && snapToFloor) {
			GamePosition.y = IQVector.y;
		}
	}


	
	public void lostPoster() {
		//GamePosition += pastIQPositions.getOldest();
		IQPosition = Vector3.zero;
		IQVector = Vector3.zero;
		bufferPosition = Vector3.zero;
		//pastIQPositions.reset ();
	}
	
	private void updateJoystick() {
		if (joystick.position.x != 0 || joystick.position.y != 0) {
			Vector3 forvec = transform.rotation * Vector3.forward;
			Vector3 upvec;//this needs to be changed if using right hand glove
			
			Vector2 normjoy = joystick.position;
			/*if (!guiManager.left) {
				upvec = transform.rotation * Vector3.down; 
				normjoy = - normjoy;
			} else{*/
			upvec = transform.rotation * Vector3.up; 
			//}
			
			
			if (normjoy.magnitude > 1) normjoy.Normalize();
			
			
			Vector3 transvec = new Vector3(normjoy.x,0,normjoy.y);; // if left, if right negate both
			float angle = 0;
			
			if (Mathf.Abs(forvec.y) <= Mathf.Abs(upvec.y) || Mathf.Abs(forvec.y) < .6f) {
				//use forvec
				forvec.y = 0;
				angle = Vector3.Angle (Vector3.forward, forvec);
				if (forvec.x < 0) angle = -angle;
				
			} else {
				//use upvec because I want Y component to be small
				upvec.y = 0;
				//get rotation from forward, (0,0,1), to upvec, then 
				angle = Vector3.Angle (Vector3.forward, upvec);
				if (upvec.x < 0) angle = -angle;
			}
			GamePosition += (Quaternion.Euler (0, angle, 0) * transvec * Time.deltaTime * joystickVelocity);
		}
	}

	private Quaternion YGyroRotation() {
		Quaternion t = ComplementaryFilter (); //fromto
		Vector3 direT = t * Vector3.forward;
		direT = new Vector3(-direT.x, 0, Mathf.Sqrt(1 - direT.x * direT.x)); //-direT.x

		return Quaternion.LookRotation(direT, Vector3.up);
	}
	
	private Quaternion ComplementaryFilter() {
		
		return Quaternion.Euler (90, 0, 0) * Input.gyro.attitude * Quaternion.Euler(0, 0, 180);
		
	}
}