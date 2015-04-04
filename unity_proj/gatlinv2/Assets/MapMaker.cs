using UnityEngine;
using System.Collections;

public class MapMaker : MonoBehaviour {

	public IQTransform _iqtransform;

	private Particle[] particles;
	private const int particlesLength = 4000;
	private int particleIndex = 0; 

	private float samplingRate = .0625f; // rate that pixels are used to map


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//depth 16 bit 1 color, color 4 channel default
	public void GenerateMap(Texture2D depth, Texture2D color) {
		int sampleSize = (int) (depth.width * samplingRate * depth.height * samplingRate);

		for (int i = 0; i < sampleSize; i++) {

			//delete



			int x = (int)(i % (depth.width / samplingRate)); //Scale into index of image. the image will be bigger than we use
			int y = (int)(i / (depth.width / samplingRate));
			particles[particleIndex].color = color.GetPixel(x,y);
			//particles[particleIndex].position = ;
			particleIndex++;
			particleIndex = particleIndex % particlesLength;
		}
	}

}
