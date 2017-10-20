using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PhoneCamera : MonoBehaviour {

	/*
	 *****************************************
	 *****************************************
	 * VARIABLES
	 *****************************************
	 *****************************************
	 */

	private bool camAvailable;
	private WebCamTexture DeviceCamera;
	public GameObject ContainerCube;
	public GameObject MazeCube;
	public Camera VirtualCamera;

	private float _cameraResW = 0.0f;
	private float _cameraResH = 0.0f;
	private float _screenResW = 640.0f;
	private float _screenResH = 1136.0f;
	//private float _screenResW = 540.0f;// Android
	//private float _screenResH = 960.0f;// Android

	public Canvas WelcomeCanvas;

	public Material CollideRenderMaterialMaterial;
	private GameRenderScript gameRenderScript;
	public Camera CollideCamera;


	public RawImage ContainerTestRawImage;
	private int Counter;

	Color[] imageColors = {};

	public Color WinerColor;
	public Color LoserColor;




	/*
	 *****************************************
	 *****************************************
	 * LIFE CYCLE METHODS
	 *****************************************
	 *****************************************
	 */

	// Use this for initialization
	void Start () {
		InitializeCamera ();
		InitializeGameRender ();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCamera ();
		UpdateUserInteractions ();
		UpdateGameStatus ();
	}

	/*
	 *****************************************
	 *****************************************
	 * CAMERA METHODS
	 *****************************************
	 *****************************************
	 */

	private void InitializeCamera (){
		WebCamDevice[] devices = WebCamTexture.devices;

		if (devices.Length == 0) {
			Debug.Log ("No camera found");
			camAvailable = false;
			return;
		}
		DeviceCamera = new WebCamTexture (devices[0].name, (int)_screenResH, (int)_screenResH);
		//DeviceCamera = new WebCamTexture (devices[0].name);

		if (DeviceCamera == null) {
			Debug.Log ("Unable to find camera");
			return;
		}

		DeviceCamera.Play ();

		camAvailable = true;
	}

	private void UpdateCamera(){
		if (!camAvailable) {
			return;
		}

		if(DeviceCamera.width<100.0f){
			Debug.Log ("Still waiting another frame for correct info...");
		}

		_cameraResW = DeviceCamera.width;
		_cameraResH = DeviceCamera.height;
		SetCorrectPositions ();

		ContainerCube.GetComponent<Renderer> ().material.SetTexture ("_MainTex",DeviceCamera);
	}

	private void SetCorrectPositions(){
		SetVirtualCameraPosition ();
		SetContainerCubePosition ();
	}

	private void SetVirtualCameraPosition (){
		VirtualCamera.transform.position = new Vector3 (0.0f,0.0f,-10.0f);
		VirtualCamera.orthographicSize = _screenResW;
		VirtualCamera.aspect = _screenResW / _screenResH;
		VirtualCamera.rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f); // Línea del diablo

		CollideCamera.transform.position = new Vector3 (0.0f,0.0f,-3.0f);
		CollideCamera.orthographicSize = _screenResW;
		CollideCamera.aspect = _screenResW / _screenResH;
		CollideCamera.rect = new Rect (0.0f, 0.0f, 1.0f, 1.0f); // Línea del diablo


	}

	private void SetContainerCubePosition (){

		float deviceCameraAspectRatio = _cameraResW / _cameraResH;
		//ContainerCube.transform.localScale = new Vector3 (_screenResH * deviceCameraAspectRatio, _screenResH, 1.0f);//Android
		//MazeCube.transform.localScale = new Vector3 (_screenResH * deviceCameraAspectRatio, _screenResH, 1.0f);//Android
		ContainerCube.transform.localScale = new Vector3 (_screenResH * deviceCameraAspectRatio, -_screenResH, 1.0f);
		MazeCube.transform.localScale = new Vector3 (_screenResH * deviceCameraAspectRatio, -_screenResH, 1.0f);

	}

	/*
	 *****************************************
	 *****************************************
	 * GAME RENDER METHODS
	 *****************************************
	 *****************************************
	 */

	private void InitializeGameRender(){

		if(!gameRenderScript){
			gameRenderScript = CollideCamera.GetComponent<GameRenderScript>();
			gameRenderScript.setupScript (_screenResW,_screenResH);
		}

		setWidth (_screenResW);
		setHeight (_screenResH);

		UpdateGameRender ();
		Counter = -1;

	}

	private void UpdateGameRender(){
		//EdgeRendererCubeMaterial.SetTexture ("_MainTex",DeviceCamera);
		gameRenderScript.setTexture2D (DeviceCamera);
	}


	private void UpdateGameStatus(){
		if (Counter > 50) {
			StartCoroutine (TakeScreen ());
			Counter = 0;
		}


		Counter++;
	}

	/*
	 *****************************************
	 *****************************************
	 * SHADER METHODS
	 *****************************************
	 *****************************************
	 */

	public void UpdateThresholdValue(float _threshold){
		ContainerCube.GetComponent<Renderer> ().material.SetFloat ("_Threshold",_threshold);
	}


	private void setWidth(float _w){

		gameRenderScript.setWidth (_w);
	

	}

	private void setHeight(float _h){

		gameRenderScript.setHeight (_h);
	

	}

	/*
	 *****************************************
	 *****************************************
	 * UI METHODS
	 *****************************************
	 *****************************************
	 */


	private void UpdateUserInteractions(){
		if(Input.GetMouseButtonUp(0)){
			OnClickUp();
		}

		if(Input.GetMouseButtonDown(0)){
			OnClickDown();
		}
	}


	private void OnClickUp(){

	}

	private void OnClickDown(){
		
		//StartCoroutine (TakeScreen ());
		SetNewTargetColor();
	}



	/*
	 *****************************************
	 *****************************************
	 * GAME METHODS
	 *****************************************
	 *****************************************
	 */


	public void StartGame(){

		Vector3 hidePosition = new Vector3 (1000.0f,0.0f,0.0f);
		WelcomeCanvas.transform.position = hidePosition;
		Counter = 0;

	}


	public void RestartGame(){

	}

	public void SetNewTargetColor(){

		Color newTargetColor = GetColorMousePositionRelativeToImage ();
		ContainerCube.GetComponent<Renderer> ().material.SetColor ("_TargetColor",newTargetColor);
		gameRenderScript.setTargetColor (newTargetColor);
	}


	public void SaveScreen(){
		/*
		Texture2D screenTexture = new Texture2D (640,1136);
		Texture2D cubeTexture = ContainerCube.GetComponent<Renderer> ().material.GetTexture ("_MainTex") as Texture2D;

		screenTexture.SetPixels (DeviceCamera.GetPixels());
		screenTexture.Apply ();

		byte[] bytes = screenTexture.EncodeToPNG ();
		string imageName = "ScreenSave.png";
		File.WriteAllBytes (imageName,bytes);
        */
	}


	public IEnumerator TakeScreen(){


		imageColors = gameRenderScript.GetColors ();

		for(int x = 0; x<(int)_screenResW; x= x+10){

			for(int y = 0; y<(int)_screenResH; y= y+10){
				//Color C = colors[x + y * (int)_screenResW];
				Color C = imageColors[x + y * (int)_screenResW];

				if(ColorTest(C,new Color(220.0f/255.0f, 80.0f/255.0f,170.0f/255.0f,1.000f),5.0f)){
					print (":-:-: GAME OVER :-:-:");
				}else if(ColorTest(C,new Color(66.0f/255.0f, 123.0f/255.0f,208.0f/255.0f,1.000f),5.0f)){
					print (":-:-: YOU WIN !!! :-:-:");
				}

			}
		}



		yield return null; 
	}


	public void TakeScreenForTest(){
		/*
		Texture2D screenTexture = new Texture2D ((int)_screenResW,(int)_screenResH);
		screenTexture.SetPixels (gameRenderScript.GetEdgeColors());
		screenTexture.Apply ();
		//ContainerTestRawImage.texture = gameRenderScript.GetRendererTexture ();
		ContainerTestRawImage.texture = screenTexture;
		*/

		/*
		Texture2D screenTexture = new Texture2D ((int)_screenResW,(int)_screenResH);
		screenTexture.SetPixels (imageColors);
		screenTexture.Apply ();
		ContainerTestRawImage.texture = screenTexture;
        */

	}


	private  bool ColorTest(Color c1, Color c2, float tol) {
		float diffRed   = Mathf.Abs(c1.r - c2.r);
		float diffGreen = Mathf.Abs(c1.g - c2.g);
		float diffBlue  = Mathf.Abs(c1.b - c2.b);
	
		float pctDiffRed   = (float)diffRed;
		float pctDiffGreen = (float)diffGreen;
		float pctDiffBlue  = (float)diffBlue;

		float diffPercentage = (pctDiffRed + pctDiffGreen + pctDiffBlue) / 3 * 100;

		if(diffPercentage >= tol) {
			return false;
		} else { 
			return true;
		}
	}


	private Color GetColorMousePositionRelativeToImage()
	{
		int x = (int)(Input.mousePosition.x );

		int y = (int)(Input.mousePosition.y);

		return imageColors[x + y * (int)_screenResW];
	}


}
