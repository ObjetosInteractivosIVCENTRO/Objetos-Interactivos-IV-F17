// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Custom/CameraRenderShader" {
Properties {
    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    _Threshold ("Threshold", Float) = 15
    _NewColor ("New Color", Color) = (1.0,1.0,1.0)

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
            float4 _MainTex_ST;
            float _Threshold;
            fixed4 _NewColor;

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
                fixed4 col = tex2D(_MainTex, i.texcoord);
                UNITY_APPLY_FOG(i.fogCoord, col);

                float minValue = 0.0f;
                float maxValue = 1.0f;

                float resultedRed = 0.0f;
                float resultedGreen = 0.0f;
                float resultedBlue = 0.0f;
               

                fixed4 targetColor = fixed4(4.0f/255.0f,47.0f/255.0f,117.0f/255.0f,255.0f/1.0f);


                if(ColorTest(col,targetColor,_Threshold)){
                resultedRed = col.r -targetColor.r + _NewColor.r;
                resultedGreen = col.g -targetColor.g + _NewColor.g;
                resultedBlue = col.b -targetColor.b + _NewColor.b;

                if(resultedRed > maxValue){
                resultedRed = maxValue;
                }else if(resultedRed < minValue){
                resultedRed = minValue;
                }

                if(resultedGreen > maxValue){
                resultedGreen = maxValue;
                }else if(resultedGreen < minValue){
                resultedGreen = minValue;
                }

                if(resultedBlue > maxValue){
                resultedBlue = maxValue;
                }else if(resultedBlue < minValue){
                resultedBlue = minValue;
                }

                col.r = resultedRed;
                col.g = resultedGreen;
                col.b = resultedBlue;
                }

                return col;
            }
        ENDCG
    }
}

}