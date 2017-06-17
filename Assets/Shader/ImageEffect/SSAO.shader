// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/SSAO"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass // 0
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			

			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(1,1,1,1);
			}
			ENDCG
		}

		Pass // 1
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			sampler2D_float _CameraDepthTexture;
			sampler2D_float _CameraDepthNormalsTexture;

			half4x4 _CameraToWorldMatrix;
			half4x4 _InverseViewProject;
			half _Distance;
			half _Intensity;
			half _Radius;
			half _DistanceCutoff;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			half getDepth(half2 uv){
				return SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture,uv);
			}
			
			half3 getViewSpaceNormal(half2 uv){
				half4 normal = tex2D(_CameraDepthNormalsTexture,uv);
				return DecodeViewNormalStereo(normal);
			}

			half3 getWorldSpaceNormal(half2 uv){
				half3 vsNormal = getViewSpaceNormal(uv);
				half3 wsNormal = mul((half3x3)_CameraToWorldMatrix , vsNormal);
				return wsNormal;
			}

			half3 getWorldSpacePos(half2 uv,half depth){
				half4 pos = half4(uv * 2.0 - 1.0, depth, 1.0);
				half4 ray = mul(_InverseViewProject,pos);
				return ray.xyz/ray.w;
			}

			half caulateAo(half2 uv,half2 offset,half3 p,half3 normal){
				half2 sampleUv = uv + offset;
				half depth = getDepth(sampleUv);
				half3 diff = getWorldSpacePos(sampleUv,depth) - p;
				half3 v = normalize(diff);
				half  d = length(diff) * _Distance;
				return max(0.0,dot(normal,v))*(1.0/(1.0 + d)) * _Intensity;
			}

			half ssao(half2 uv){

				half2 CROSS[4] = { half2(1.0, 0.0), half2(-1.0, 0.0), half2(0.0, 1.0), half2(0.0, -1.0) };
				half depth = getDepth(uv);
				half eyeDepth = LinearEyeDepth(depth);

				half3 position = getWorldSpacePos(uv,depth);
				half3 normal = getWorldSpaceNormal(uv);


				half radius = max(_Radius / eyeDepth, 0.005);
				clip(_DistanceCutoff - eyeDepth); // Skip out of range pixels

				half ao = 0.0;
				for(int i = 0; i < 4; i++){
					half2 sampleOffset = CROSS[i] * radius;
					ao += caulateAo(uv,sampleOffset * 0.50,position,normal);
				}

				ao /= 4.0f;
				return 1.0 - ao;
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			fixed4 frag (v2f i) : SV_Target
			{
				half ao = ssao(i.uv);
				return half4(ao.xxx,1.0);
			}
			ENDCG
		}

		Pass //2
		{ 
		}

		pass //3
		{
		}
	}
}
