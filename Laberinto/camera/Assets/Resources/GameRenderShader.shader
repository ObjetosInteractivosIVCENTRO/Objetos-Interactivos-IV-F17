// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/GameRenderShader" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _MazeTex ("Maze Texture", 2D) = "white" {}
    _Threshold ("Threshold", Float) = 15
    _NewColor ("New Color", Color) = (1.0,1.0,1.0)
    _TargetColor ("Target Color", Color) = (1.0,1.0,1.0)


}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            	bool ColorTest(fixed4 _colorA, fixed4 _colorB, float tol){
		float difRed = abs(_colorA.r - _colorB.r);
		float difGreen = abs(_colorA.g - _colorB.g);
		float difBlue = abs(_colorA.b - _colorB.b);

		float diffPercentage = 100 * (difRed + difGreen + difBlue) / 3;

			if(diffPercentage >= tol){
				return false;
			}else{
				return true;
				 }
				};

            sampler2D _MainTex;
            sampler2D _MazeTex;
            float4 _MainTex_ST;
            float _Threshold;
            fixed4 _NewColor;
            fixed4 _TargetColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {

                fixed4 col = tex2D(_MainTex, i.texcoord); // Color de la WebCam

                fixed4 colMaze = tex2D(_MazeTex, i.texcoord ); // Color de Laberinto

                UNITY_APPLY_FOG(i.fogCoord, col);


               
                // Color de mi Player (AZUL)
                fixed4 targetColor = fixed4(4.0f/255.0f,47.0f/255.0f,117.0f/255.0f,1.0f);
                //fixed4 targetColor = _TargetColor;
                // Color del laberinto
                fixed4 mazeColor = fixed4(255.0f/255.0f,255.0f/255.0f,0.0f/255.0f,1.0f);
                // Color meta
                fixed4 finishColor = fixed4(0.0f/255.0f,255.0f/255.0f,0.0f/255.0f,1.0f);

                //Win Color
                fixed4 winerColor = fixed4(66.0f/255.0f,123.0f/255.0f,208.0f/255.0f,1.0f);

                //Lose Color
                fixed4 loserColor = fixed4(220.0f/255.0f,80.0f/255.0f,170.0f/255.0f,1.0f);


                if(ColorTest(col,targetColor,_Threshold)){// ¿Estoy detectando a mi JUGADOR?
                   col.r = 1.0f;

                  if(ColorTest(colMaze,mazeColor,5.0f)){// Ya sé que detecto a mi jugador, ahora ¿Esta chocando con la muralla?
                  col.r = loserColor.r;
                  col.g = loserColor.g;
                  col.b = loserColor.b;
                  }

                  if(ColorTest(colMaze,finishColor,5.0f)){// Ya sé que detecto a mi jugador, ahora ¿Ha llegado a la meta?
                  col.r = winerColor.r;
                  col.g = winerColor.g;
                  col.b = winerColor.b;
                  }

                }else if(ColorTest(colMaze,mazeColor,5.0f)){// ¿O Estoy detectando una MURALLA?
                  col.r = 77.0f/255.0f;
                  col.g = 29.0f/255.0f;
                  col.b = 213.0f/255.0f;
                }else if(ColorTest(colMaze,finishColor,5.0f)){// ¿O Estoy detectando a mi META?
                  col.r = colMaze.r;
                  col.g = colMaze.g;
                  col.b = colMaze.b;
                } 

                return col;
            }
        ENDCG
    }
}

}