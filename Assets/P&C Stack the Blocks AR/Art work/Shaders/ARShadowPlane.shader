// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//Shader "Unlit/queuefrag"
//{
//	//Properties
//	//{
//	//	//_MainTex ("Texture", 2D) = "white" {}
// //           _Color ("Color", Color) = (1,1,1,1)
// //        _Alpha("Alpha", Range(0,1)) = 0.5
//	//}
//	SubShader
//	{
//		Tags { "Queue"="Geometry+10"}
//     //   Tags { "Queue"="Geometry" "RenderType"="Opaque" }
//		LOD 100
//        ColorMask 0
//        ZWrite On
//      //  Blend SrcAlpha OneMinusSrcAlpha
        
//        Pass {}
		
//		}
	
//}



Shader  "Poly&Code/ARShadowPlane"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,.5)
        _ShadowIntensity ("Shadow Intensity", Range (0, 1)) = 0.6
    }
 
 
    SubShader
    {
 
        Tags {"Queue"="Geometry+10" }
        ZWrite On
        Pass
        {
            Tags {"LightMode" = "ForwardBase" }
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
 
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            uniform fixed4  _Color;
            uniform float _ShadowIntensity;
 
            struct v2f
            {
                float4 pos : SV_POSITION;
                LIGHTING_COORDS(0,1)
            };
            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos (v.vertex);
                TRANSFER_VERTEX_TO_FRAGMENT(o);
             
                return o;
            }
            fixed4 frag(v2f i) : COLOR
            {
                float attenuation = LIGHT_ATTENUATION(i);
                return fixed4(0,0,0,(1-attenuation)*_ShadowIntensity) * _Color;
            }
            ENDCG
        }
 
    }
    Fallback "VertexLit"
}
