using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {
	public IQTransform iqtransform;
	public Joystick3D joystick;
	protected int joystickID = -7777;
	Camera thiscamera;

	// Use this for initialization
	void Start () {
		thiscamera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		bool joystickActive = false;
		bool gyroActive = false;
		
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
				}
			} else {
				iqtransform.transform.rotation = Quaternion.Euler (0, 0.25f*t.deltaPosition.x, 0) * iqtransform.transform.rotation;
			}
		}
		
		if (!joystickActive) {
			joystickID = -7777;
			joystick.LerpHome();
		}

		iqtransform.toggleGyroOn(gyroActive);
		
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
