#pragma strict

public static var COLORFEED = 1;
public static var DEPTHFEED = 2;

public var AddressChoice = 0;

//Depth     http://192.168.1.81:8080/snapshot?topic=/image_converter/output_video&width=256&height=192&quality=20

private var depth_address = "http://192.168.1.81:8080/snapshot?topic=/image_converter/output_video&width=256&height=192&quality=75";

private var color_address = "http://192.168.1.81:8080/snapshot?topic=/camera/rgb/image_rect_color&width=256&height=192&quality=25";

private var current_address = "";


function Start () {
	// Create a texture in DXT1 format
	
	if (AddressChoice == 1) {//color
		GetComponent.<Renderer>().material.mainTexture = new Texture2D(256, 192);
		current_address = color_address;
	} else if (AddressChoice == 2) { //depth
		GetComponent.<Renderer>().material.mainTexture = new Texture2D(256, 192, TextureFormat.Alpha8, true);
		current_address = depth_address;
	} else {
		Debug.Log("CHOOSE AN ADDRESS TO STREAM VIDEO FROM (SNAP SHOT [IN NetworkVideoTexture])");
		return;
	}
	
	//G
	
	while(true) {
		// Start a download of the given URL
		var www = new WWW(current_address);

		// wait until the download is done
		yield www;

		// assign the downloaded image to the main texture of the object
		www.LoadImageIntoTexture(GetComponent.<Renderer>().material.mainTexture);
	}
	
	//16uc1
}