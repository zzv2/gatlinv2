#pragma strict
private var RosWebServerAddress = "http://192.168.1.81:8080/snapshot?topic=/camera/rgb/image_rect_color&width=256&height=192&quality=50";

//http://192.168.1.81:8080/stream?topic=/camera/depth_registered/image_raw

//http://192.168.1.81:8080/stream?topic=/camera/rgb/image_rect_color&width=256&height=192&quality=50


function Start () {
	// Create a texture in DXT1 format
	GetComponent.<Renderer>().material.mainTexture = new Texture2D(256, 192);
	while(true) {
		// Start a download of the given URL
		var www = new WWW(RosWebServerAddress);

		// wait until the download is done
		yield www;

		// assign the downloaded image to the main texture of the object
		www.LoadImageIntoTexture(GetComponent.<Renderer>().material.mainTexture);
	}
}