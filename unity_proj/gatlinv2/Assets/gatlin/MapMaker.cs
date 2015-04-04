using UnityEngine;
using System.Collections;

public class MapMaker : MonoBehaviour {

	public IQTransform _iqtransform;
	public Renderer colorRenderer, depthRenderer;

	public ParticleSystem _particlesystem;

	private float samplingRate = .3f; // rate that pixels are used to map

	// Use this for initialization
	void Start () {
		StartCoroutine (delayGenerator ());
	}

	IEnumerator delayGenerator() {

		yield return new WaitForSeconds(.07f);

		GenerateMap ();

		StartCoroutine(delayGenerator ());
	}
	
	// Update is called once per frame
	void Update () {
		//check if picture has updated
		//_particlesystem.Emit (new Vector3(0, 0, 1), Vector3.zero, 1f, 5, new Color(1, 0, 0, 1));
		//if so GenerateMap()
	}

	//depth 16 bit 1 color, color 4 channel default
	public void GenerateMap() {
		Texture2D color = (Texture2D)colorRenderer.material.mainTexture;
		Texture2D depth = (Texture2D)depthRenderer.material.mainTexture;

		//Debug.Log ("Color: width: "+color.width + ", "+color.height );


		int scols = (int) (depth.width * samplingRate); //number of columns in sample window
		int swidth = depth.width / scols; //width of step

		int srows = (int) (depth.height * samplingRate); //number of rows
		int sheight = depth.height / srows; //height of step

		float cx = depth.width / 2f;
		float cy = depth.height / 2f;

		//57 degree horizontal, 43 degree vertical
		float fx = cx / Mathf.Tan((57f/2f) * Mathf.PI / 180f); //NEGATED BECAUSE THEY TOLD US IN CLASS
		float fy = cy / Mathf.Tan((43f/2f) * Mathf.PI / 180f);



		/* fx, 0, cx     * X  =  imageX   * z 
		 * 0, fy, cy     * Y  =  imageY   * z
		 * 0, 0, 1       * Z  =  depth    * z
		 */ 

		for (int x = 0; x < scols; x++) {
			for (int y = 0; y < srows; y++) {

				int imageX = x * swidth + Random.Range(0,swidth-1);
				int imageY = y * sheight + Random.Range(0,sheight-1);


				//Random displacement here

				float d = depth.GetPixel(imageX,imageY).grayscale;
				Color trying = depth.GetPixel(imageX,imageY);
				//Debug.Log ("At "+imageX +", " +imageY+": color "+ trying.r +" " +trying.b+ " " + trying.g+ " " + trying.a + " "+ trying.grayscale);

				if (d < 1 && d > .1f) {
					Color particolor = color.GetPixel(imageX,imageY);

					float depthMeters = d * 3.5f ; // color is from 0 to 1
					Vector3 local = new Vector3(0,0, depthMeters); //vector from the center

					local = new Vector3( depthMeters * ((imageX-cx) / fx), //vector pointing out of camera
					                     depthMeters * ((imageY-cy) / fy),
					                     depthMeters);

					//local = _iqtransform.transform.rotation * local + _iqtransform.transform.position; //robot position???

					//_particlesystem.Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color);
					_particlesystem.Emit (local, Vector3.zero, .04f, .3f, particolor); //an 
					//NOTE: SIMULATION SPACE IS LOCAL, GOOD FOR TESTING TOO
				}
			}
		}
	}
}
