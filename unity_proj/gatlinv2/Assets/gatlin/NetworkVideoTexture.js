#pragma strict

private var depth_address = "http://192.168.1.81:8080/snapshot?topic=/image_converter/output_video&width=256&height=192&quality=95";

function Start () {

	//GetComponent.<Renderer>().material.mainTexture = new Texture2D(256, 192, TextureFormat.Alpha8, true);
	GetComponent.<Renderer>().material.mainTexture = new Texture2D(256, 192, TextureFormat.Alpha8, true);
	
	while(true) {
		// Start a download of the given URL
		var www = new WWW(depth_address);

		// wait until the download is done
		yield www;
		
		www.LoadImageIntoTexture(GetComponent.<Renderer>().material.mainTexture);
	
	}
};
