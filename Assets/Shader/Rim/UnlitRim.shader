Shader "Unlit/UnlitRim"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_RimColor("RimColor",Color) = (1,0,0,1)
		_RimIntensity("RimIntensity",float) = 1
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

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				float rim : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _RimColor;
			float _RimIntensity;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
				o.rim = 1 - saturate(dot(viewDir,v.normal));
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				UNITY_APPLY_FOG(i.fogCoord, col);
				col.rgb = col.rgb + _RimColor.rgb * pow(i.rim,1/_RimIntensity);
				return col;
			}
			ENDCG
		}
	}
}
