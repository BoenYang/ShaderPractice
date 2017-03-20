Shader "Lighting/ShadowMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_slopeScaleDepthBias("Bias Sloop Scale", Range(0, 1)) = 0
        _depthBias("depth bias", float) = 0.001
		
        _pixelWidth("pixel width", float) = 0.001
        _pixelHeight("pixel height", float) = 0.001
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
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : Normal;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : Normal;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			sampler2D _ShadowDepthMap;
			float4x4  _LightProjection;

			// For ShadowBias
			float _slopeScaleDepthBias;
			float _depthBias;


			        // For PCF
			float _pixelWidth;
			float _pixelHeight;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}

			float GetDepth(float3 worldPos,float4x4 lightTrans,sampler2D depthMap,out float3 posInLight){
				posInLight = mul(lightTrans,float4(worldPos,1)).xyz;
				return DecodeFloatRGBA(tex2D(depthMap,posInLight.xy));
			}

			float GetShadowBias(float3 worldNormal,float4x4 lightTrans,float slopeFactor,float depthBias){
				float3 normalInLight = mul((float3x3)lightTrans,worldNormal);
				return (1 - normalInLight.z) * slopeFactor + depthBias;
			}

				float GetNearDepth(float3 pos,float bias,sampler2D depthMap,float offsetX,float offsetY,float factor){
				return (pos.z - bias) > DecodeFloatRGBA(tex2D(depthMap,float2(pos.x + offsetX,pos.y + offsetY))) ? factor : 0;
			}

			float GetShadowAttenPCF(float3 pos,float bias,sampler2D depthMap,float pixelWidth,float pixelHeight){
			
				float atten = 0;
				int j = 0;
				int i = 0;

				for(i = -2; i <=2 ; i++)
					for(j = -2; j <=2; j++)
						atten += GetNearDepth(pos,bias,depthMap,i * pixelWidth,j * pixelHeight,1.0f);
				atten = atten / 25;
				return atten;
			}



			float GetShadowAtten(float3 worldPos,float3 worldNormal){
				float3 posInLight;
				float shadowDepth = GetDepth(worldPos,_LightProjection,_ShadowDepthMap,posInLight);
				float shadowBias = GetShadowBias(worldNormal,_LightProjection,_slopeScaleDepthBias,_depthBias);
				float strength = GetShadowAttenPCF(posInLight,shadowBias,_ShadowDepthMap,_pixelWidth,_pixelHeight);
				return 1 - strength;
			}

		
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * GetShadowAtten(i.worldPos,i.worldNormal);

				return col;
			}
			ENDCG
		}
	}

}
