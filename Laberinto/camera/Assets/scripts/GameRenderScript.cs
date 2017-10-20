using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRenderScript : MonoBehaviour {

	/*
	 *****************************************
	 *****************************************
	 * VARIABLES
	 *****************************************
	 *****************************************
	 */

	private Material material;
	private Texture2D OutputTex;
	private RenderTexture buffer;
	public float _W;
	public float _H;
	public Color[] colors; 

	/*
	*****************************************
	*****************************************
	* LIFE CYCLE METHODS
	*****************************************
	*****************************************
	*/



	void Awake(){

		//print (":-:-: EdgeRendererScript :: Awake :-:-:");
		material = new Material(Shader.Find("Custom/GameRenderShader"));




	}
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}


	public void setupScript(float _w, float _h){

		OutputTex = new Texture2D((int)_w, (int)_h);
		OutputTex.filterMode = FilterMode.Point;
		OutputTex.wrapMode = TextureWrapMode.Clamp;




		buffer = new RenderTexture(
			(int)_w, 
			(int)_h, 
			0,                            // No depth/stencil buffer
			RenderTextureFormat.ARGB32,   // Standard colour format
			RenderTextureReadWrite.Linear // No sRGB conversions
		);

	}

	void OnRenderImage (RenderTexture source, RenderTexture destination)
	{
		Graphics.Blit (source, destination, material); // Blit for visual testing

		// Aquí tomo un screenshot del resultado del shader
		Graphics.Blit(source, buffer, material);

		RenderTexture.active = buffer;  


		OutputTex.ReadPixels(
			new Rect(0, 0, (int)_W, (int)_H), 
			0, 0,                          
			false                          
		);



		Graphics.Blit (source, destination, material); // Blit for visual testing

	}


	/*
	*****************************************
	*****************************************
	* IMAGE METHODS
	*****************************************
	*****************************************
	*/




	public Texture2D GetRendererTexture(){



		Texture2D texture = new Texture2D(2, 2);
		byte[] pngBytes = OutputTex.EncodeToPNG ();

		texture.LoadImage(pngBytes);
		return texture; 
	}

	public Texture2D GetRawRendererTexture(){

	
		//StartCoroutine (getColorsTexture());
		Texture2D _TextureFromCamera = new Texture2D(OutputTex.width,OutputTex.height);

		_TextureFromCamera.SetPixels ( OutputTex.GetPixels ());
		_TextureFromCamera.Apply();

		return _TextureFromCamera; 
	}

	public Color[] GetColors(){

		//print (OutputTex.width + ", " + OutputTex.height);
		colors = OutputTex.GetPixels ();
		return colors;
	}

	public IEnumerator getColorsTexture(){

		yield return new WaitForEndOfFrame();
		colors = OutputTex.GetPixels ();
		//_renderActive = false;
		yield return null; 
	}


	/*
	*****************************************
	*****************************************
	* SHADER METHODS
	*****************************************
	*****************************************
	*/


	public void setTargetColor(Color _targetColor){
		material.SetColor ("_TargetColor", _targetColor);

	}

	public void setTexture2D(WebCamTexture _tex){
		//_renderActive = true;
		material.SetTexture ("_MainTex",_tex);

	}

	public void setTexture2D(Texture2D _tex){
		//_renderActive = true;
		material.SetTexture ("_MainTex",_tex);

	}

	public void setFilter(float _filter){
		material.SetFloat ("_Filter",_filter);
	}

	public void setPosition(Vector2 _pos){
		material.SetFloat ("_Px",_pos.x);
		material.SetFloat ("_Py",_pos.y);
	}

	public void setWidth(float _w){
		_W = _w;
		material.SetFloat ("_W", _w);

	}

	public void setHeight(float _h){
		_H = _h;
		material.SetFloat ("_H", _h);

	}

	public void setEdge(float _edge){
		material.SetFloat ("_Edge",_edge);
	}

	public void setContrast(float _contrast){
		material.SetFloat ("_Contrast",_contrast);
	}


	public void setIsFromCamera(float _is){
		material.SetFloat ("_isFromCamera", _is);
	}


}
