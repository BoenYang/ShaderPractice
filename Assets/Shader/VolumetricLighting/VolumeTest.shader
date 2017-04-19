Shader "VolumetricLighting/VolumeTest"
{
	Properties
	{
	}

	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor;
			uniform float4 _LightPos;
			uniform float _LightIndensity;

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 wpos : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.wpos = mul(unity_ObjectToWorld,v.vertex).xyz;
				return o;
			}
			

			float GetLightAttenuation(float3 wpos){
				float3 toLight = _LightPos.xyz - wpos;
				float att = _LightIndensity / dot(toLight , toLight);
				return att;
			}

			float4 raymatch(float3 rayStart,float3 rayDir,float rayLength){
				
				int stepCount = 20;
				float stepSize = rayLength / stepCount;
				float3 step = rayDir * stepSize;

				float3 currentPos = rayStart + step;

				float4 lightColor = float4(0,0,0,0);
				for(int i = 0; i < stepCount; ++i){
					float lightAtten = GetLightAttenuation(currentPos);
					lightColor += _LightColor * lightAtten;
					currentPos += step;
				}
				return lightColor;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float3 rayStart = _WorldSpaceCameraPos;

				float3 rayEnd = i.wpos;
				float3 rayDir = rayEnd - rayStart;

				return raymatch(rayStart,rayDir,3);
			}
			ENDCG
		}
	}
}
