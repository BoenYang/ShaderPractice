// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Lighting/Specular"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Specular("Specular",float) = 1
		_SpecularColor("Specular",Color) = (1,1,1,1)
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
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				UNITY_FOG_COORDS(2)
				float3 normal : Normal;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _Specular;
			float4 _SpecularColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld,v.vertex).xyz;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float3 N = normalize(i.normal);
				float3 L = normalize(-_WorldSpaceLightPos0.xyz);
				float3 V = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 H = normalize(L + V);

				fixed4 texCol =  tex2D(_MainTex,i.uv);
				fixed3 diffuse =  _LightColor0.rgb * saturate(dot(N,L)) * texCol.rgb;
				fixed3 specular = _LightColor0.rgb * _SpecularColor.rgb * pow(max(0,dot(N,H)),_Specular);
				
				
				fixed4 col = fixed4(specular + diffuse,texCol.a);
				
				
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
