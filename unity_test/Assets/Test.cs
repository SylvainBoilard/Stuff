using System;
using UnityEngine;

public class Test : MonoBehaviour {
	private static String[] urls = {
		"http://www.clker.com/cliparts/9/a/1/e/13397010011226451784pomme_lrg-md.png",
		"http://www.pngpix.com/wp-content/uploads/2016/03/Pear-PNG-image-500x772.png",
		"http://www.pngpix.com/wp-content/uploads/2016/03/Orange-Fruit-PNG-image-2-500x507.png",
		"http://www.pngpix.com/wp-content/uploads/2016/08/PNGPIX-COM-Yellow-Banana-PNG-Transparent-Image-500x406.png",
		"http://miam-images.m.i.pic.centerblog.net/o/26536748.png",
		"http://cdn.pixabay.com/photo/2016/02/23/17/36/mango-1218147_960_720.png"
	};
	private long urlId;

	private void UpdateTexture()
	{
		GetComponent<MeshRenderer>().material.mainTexture
			= TextureManager.instance.Get(urls[urlId]);
	}

	private void Start ()
	{
		urlId = 0;
		UpdateTexture();
	}

	private void OnMouseDown ()
	{
		urlId = (urlId + 1) % urls.Length;
		UpdateTexture();
	}
}
