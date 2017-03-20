Shader "Lighting/Extrusion"
{
	Properties
	{
		_Extrusion("Extrusion",Range(0,20)) = 5.0
	}



	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		ZWrite Off
		ColorMask A
		Offset 1,1

		Pass
		{
			Cull Back
			Blend DstColor One

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			uniform float4 _LightPosition;
			float _Extrusion;
			
			v2f vert (appdata v)
			{
				v2f o;
		
				float4 objectLightPos = mul(_LightPosition,UNITY_MATRIX_IT_MV);

				float3 toLight = normalize(objectLightPos.xyz - v.vertex.xyz * objectLightPos.w);

				float backFactor = dot(toLight,v.normal);

				float extrude = (backFactor < 0.0 ) ? 1.0 : 0.0;
				v.vertex.xyz -= toLight * (extrude * _Extrusion);
				v.vertex.xyz += v.normal * 0.001;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(1,1,1,1);
			}
			ENDCG
		}

		Pass
		{
			Cull Front
			Blend DstColor Zero

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			uniform float4 _LightPosition;
			float _Extrusion;
			
			v2f vert (appdata v)
			{
				v2f o;
		
				float4 objectLightPos = mul(_LightPosition,UNITY_MATRIX_IT_MV);

				float3 toLight = normalize(objectLightPos.xyz - v.vertex.xyz * objectLightPos.w);

				float backFactor = dot(toLight,v.normal);

				float extrude = (backFactor < 0.0 ) ? 1.0 : 0.0;
				v.vertex.xyz -= toLight * (extrude * _Extrusion);
				v.vertex.xyz += v.normal * 0.001;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return fixed4(0.5,0.5,0.5,0.5);
			}
			ENDCG
		}

	}
}
