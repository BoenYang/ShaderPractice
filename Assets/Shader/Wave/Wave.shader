Shader "Unlit/Wave"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_SpecularColor("SpecularColor",Color) = (1,1,1,1)
		_Specular("Specular",float) = 2
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
			#include "Lighting.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
			};

			fixed4 _Color;
			fixed3 _SpecularColor;
			float _Specular;
			uniform float4 _Dirs[100];
			uniform float _Amps[100];
			uniform float _Phases[100];
			uniform float _Frequency[100];
			uniform float _Num;


			
			v2f vert (appdata v)
			{
				v2f o;
				float3 normal = float3(0,1,0);
				for(int i = 0; i < _Num; i++){
					v.vertex.y += _Amps[i] * sin(dot(_Dirs[i].xz,v.vertex.xz) * _Frequency[i] + _Phases[i] * _Time.y);
					normal.xz += -_Frequency[i] * _Dirs[i].xz * _Amps[i] * cos(dot(_Dirs[i].xz,v.vertex.xz) * _Frequency[i] + + _Phases[i] * _Time.y);
				}
				o.normal = UnityObjectToWorldNormal(normal);
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 N = normalize(i.normal);
				fixed3 L = normalize(_WorldSpaceLightPos0.xyz);
				float3 V = normalize(UnityWorldSpaceViewDir(i.vertex));
				float3 H = normalize(L + V);

				fixed3 diffuse =  saturate(dot(N,L)) * _Color.rgb;
				fixed3 specular = _SpecularColor.rgb * pow(max(0,dot(N,H)),_Specular);

				return fixed4(diffuse + specular,_Color.a);
			}
			ENDCG
		}
	}

	FallBack "Mobile/Diffuse"
}
