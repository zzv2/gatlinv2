using UnityEngine;
using System.Collections;

public class Joystick3D : MonoBehaviour {
	
	//localtransform is position joystick on screen
	float maxDistance = .2f;
	Vector3 BasePoint, plantPoint;
	public Vector2 position;//(currentDisp.X|Y)/.005
	
	// Use this for initialization
	void Start () {
		Debug.Log("initial Position: "+ transform.position.x+", "+transform.position.y+", "+transform.position.z);
		BasePoint = transform.position;
		position = Vector2.zero;
	}
	
	//3d intersection of plane
	public void Hit(Vector3 touch) {
		
		
		//update position
		
		if (plantPoint == Vector3.zero) {
			plantPoint = touch;
		} 
		
		
		float fox = (transform.position.x - plantPoint.x)/maxDistance; 
		float foy = (transform.position.y - plantPoint.y)/maxDistance; 
		
		if (fox > 1)
			fox = 1;
		
		if (foy > 1)
			foy = 1;
		
		fox = fox * Mathf.Abs(fox);
		foy = foy * Mathf.Abs(foy);
		
		position = new Vector2(fox, foy);
		
		
		if (Vector3.Distance( touch, plantPoint) > maxDistance) {
			touch = Vector3.MoveTowards( plantPoint, touch, maxDistance);
		} 
		
		//Debug.Log("Joystick local position: "+touch.x+", "+touch.y+", "+touch.z);
		
		transform.position = touch;
		
		//Debug.Log("joystick relative position: " + position.x + ", " + position.y);
		//Debug.Log("joystick world position: " + transform.position.x + ", " + transform.position.y + ", "+transform.position.z);
	}
	
	public void LerpHome() {
		
		//Debug.Log("LERPHOME CALLED!!!!!!!!");
		position = Vector2.zero;
		plantPoint = Vector3.zero;
		transform.position = Vector3.Lerp(transform.position, BasePoint, .8f);
		
	}
	
	// Update is called once per frame
	void Update () {

	}
}
