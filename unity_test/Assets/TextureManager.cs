using System;
using System.IO;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Drawing;
using UnityEngine;

public class TextureManager : MonoBehaviour {
	internal class LoadTuple
	{
		public Texture2D texture;
		public byte[] pixels;
		public int width;
		public int height;

		public LoadTuple(Texture2D t, byte[] p, int w, int h)
		{
			texture = t;
			pixels = p;
			width = w;
			height = h;
		}
	}

	public static TextureManager instance { get; private set; }
	private String dataPath;
	private ConcurrentDictionary<String, Texture2D> textures;
	private ConcurrentQueue<String> toDownload;
	private ConcurrentQueue<LoadTuple> toLoad;

	private void Awake()
	{
		instance = this;
		dataPath = Application.dataPath;
		textures = new ConcurrentDictionary<String, Texture2D>();
		toDownload = new ConcurrentQueue<String>();
		toLoad = new ConcurrentQueue<LoadTuple>();
		new Thread(new ThreadStart(HTTPWorkerLoop)).Start();
	}

	private static String GetCachePath(String url)
	{
		return instance.dataPath + "/Resources/" + url.GetHashCode() + ".png";
	}

	private static void HTTPWorkerLoop()
	{
		ConcurrentDictionary<String, Texture2D> textures = instance.textures;
		ConcurrentQueue<String> toDownload = instance.toDownload;
		WebClient client = new WebClient();
		String url;
		String path;

		while (true)
		{
			while (toDownload.TryDequeue(out url))
			{
				path = GetCachePath(url);
				try
				{
					Texture2D tex;

					client.DownloadFile(url, path);
					textures.TryGetValue(url, out tex);
					ThreadPool.QueueUserWorkItem((_) => LoadPNGToTexture(path, tex));
				}
				catch (WebException e)
				{
					Debug.Log("Erreur lors du téléchargement d’une texture.\n"
							+ "URL : " + url + "\n"
							+ "Destination : " + path + "\n"
							+ e.Message);
				}
			}
			Thread.Sleep(15);
		}
	}

	private static void LoadPNGToTexture(String path, Texture2D tex)
	{
		Bitmap bmp = new Bitmap(path);
		byte[] pixels = new byte[bmp.Width * bmp.Height * 4];
		uint o = 0;

		for (int y = 0; y < bmp.Height; ++y)
			for (int x = 0; x < bmp.Width; ++x)
			{
				System.Drawing.Color c = bmp.GetPixel(x, y);

				pixels[o++] = c.A;
				pixels[o++] = c.R;
				pixels[o++] = c.G;
				pixels[o++] = c.B;
			}
		bmp.Dispose();
		instance.toLoad.Enqueue(new LoadTuple(tex, pixels, bmp.Width, bmp.Height));
	}

	public Texture2D Get(String url)
	{
		Texture2D tex;

		if (!textures.TryGetValue(url, out tex))
		{
			String path = GetCachePath(url);

			tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
			textures.TryAdd(url, tex);
			if (File.Exists(path))
				ThreadPool.QueueUserWorkItem((_) => LoadPNGToTexture(path, tex));
			else
				toDownload.Enqueue(url);
		}
		return tex;
	}

	private void Update()
	{
		LoadTuple t;

		while (toLoad.TryDequeue(out t))
		{
			t.texture.Resize(t.width, t.height);
			t.texture.LoadRawTextureData(t.pixels);
			t.texture.Apply();
		}
	}
}
