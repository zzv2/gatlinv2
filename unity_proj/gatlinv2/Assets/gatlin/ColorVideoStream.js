#pragma strict

//color picture
private var current_address = "http://192.168.1.81:8080/snapshot?topic=/camera/rgb/image_rect_color&width=256&height=192&quality=25";


function Start () {
	
	GetComponent.<Renderer>().material.mainTexture = new Texture2D(256, 192);
	
	while(true) {
		// Start a download of the given URL
		var www = new WWW(current_address);

		// wait until the download is done
		yield www;

		www.LoadImageIntoTexture(GetComponent.<Renderer>().material.mainTexture);
		
	}
}