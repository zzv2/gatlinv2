using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.rotation = GetRealWorldRotation();
		transform.position =  transform.rotation * new Vector3(0, 0, -1.5f) + new Vector3(0,0,1f);
	}

	private Quaternion GetRealWorldRotation() {
		return Quaternion.Euler (90, 0, 0) * Input.gyro.attitude * Quaternion.Euler(0, 0, 180);
	}
}
