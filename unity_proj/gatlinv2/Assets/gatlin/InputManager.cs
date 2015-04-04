using System;
using UnityEngine;
using System.Collections;

//this is a mess, but used to get the input working. I didn't want to iterate throuch touches a bunch or do gui in separate places

public class InputManager : MonoBehaviour {
	public IQTransform iqtransform;
	public Joystick3D joystick;
	private int joystickID = -7777;
	Camera thiscamera;
	public Camera thirdCamera, firstCamera;

	private Vector2 finalDelta;

	// Use this for initialization
	void Start () {
		thiscamera = GetComponent<Camera>();
		finalDelta = Vector2.zero;
	}
	
	// Update is called once per frame
	void Update () {
		bool joystickActive = false;
		bool gyroActive = false;
		bool tiltActive = false;
		bool thirdActive = false;

		Vector2 deltaAcc = Vector2.zero;
		
		foreach (Touch t in Input.touches) {
			Vector3 v  = thiscamera.ScreenToWorldPoint(new Vector3(t.position.x, t.position.y, 1f));
			if (t.fingerId == joystickID) {
				joystickActive = true;
				joystick.Hit(v);
				break;
			}

			RaycastHit hit = new RaycastHit();
			//Debug.Log(hit.ToString());
			int layerMask = 1 << 8;
			if(Physics.Raycast(new Vector3( v.x, v.y, 0), new Vector3(0, 0, 1), out hit, 2f, layerMask)) { 
				if (hit.collider.name == "joystick") {
					joystickID = t.fingerId;
					joystickActive = true;

					joystick.Hit(v);
				} else if (hit.collider.name == "gyro") {
					gyroActive = true;
				} else if (hit.collider.name == "tilt") {
					tiltActive = true;
				} else if (hit.collider.name == "third") {
					thirdActive = true;
				}
			} else {
				deltaAcc+= t.deltaPosition;
			}
		}

		if (finalDelta.x != 0 || finalDelta.y != 0 || deltaAcc.x != 0 || deltaAcc.y != 0) {

			//add all finger movements to final delta
			finalDelta = finalDelta + deltaAcc;

			//take set amount from final delta and rotate that much
			Vector2 thisTouch = Vector2.MoveTowards (Vector2.zero, finalDelta, 20); // in pixels??
			finalDelta = finalDelta - thisTouch;

			iqtransform.fingerRotation (thisTouch);
		} else {
			finalDelta = Vector2.zero;
		}

		//if the third person camera should be the value of the first person value, you know you need to change it
		if (thirdActive == firstCamera.enabled) {
			toggleThirdPersonCameraOn(thirdActive);
		}

		if (!joystickActive) {
			joystickID = -7777;
			joystick.LerpHome();
		}

		iqtransform.toggleGyroOn(gyroActive);
		iqtransform.toggleTiltOn(tiltActive);
		
	}

	private void toggleThirdPersonCameraOn(bool t) {

			firstCamera.enabled = !t;
			thirdCamera.gameObject.SetActive(t);
	
	}

	/*Vector3 projectRay(Vector3 p) {
		float vertExtent = camera.orthographicSize;    
		float horzExtent = vertExtent * Screen.width / Screen.height;

		tx = horzExtent * p.x / Screen.width - horzExtent/2;
		ty = vertExtent * p.y / Screen.height - vertExtent/2;

		return new Vector3(tx, ty, 1f);
		/*Plane hPlane = new Plane(transform.forward, transform.position+(transform.forward));
		// Plane.Raycast stores the distance from ray.origin to the hit point in this variable:
		float distance = 0; 
		// if the ray hits the plane...
		hPlane.Raycast(ray, out distance);
		
		return transform.InverseTransformPoint(ray.GetPoint(distance));
	}*/
}
