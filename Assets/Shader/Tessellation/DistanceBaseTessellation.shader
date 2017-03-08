Shader "Custom/DistanceBaseTessellation" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_DispTex ("DispTex", 2D) = "white" {}
		_NormalTex("Normal",2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Displacement("Displacement",float) = 0
		_Tess("Tessellation",Range(1,32)) = 4
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 300
		
		CGPROGRAM

		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard fullforwardshadows vertex:disp tessellate:tessFixed

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0
		#include "Tessellation.cginc"

		sampler2D _MainTex;
		sampler2D _DispTex;
		sampler2D _NormalTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float _Displacement;
		float _Tess;

		struct appdata{
			float4 vertex: POSITION;
			float4 tangent : TANGENT;
			float3 normal : NORMAL;
			float2 texcoord : TEXCOORD0;
		};

		float4 tessFixed(appdata v1,appdata v2, appdata v3){
			return UnityDistanceBasedTess(v1.vertex,v2.vertex,v3.vertex,1.0,5.0,_Tess);
		}

		void disp(inout appdata v){
			float d = tex2Dlod(_DispTex,float4(v.texcoord,0,0)).r *_Displacement;
			v.vertex.xyz += v.normal * d;
		}

		void surf (Input IN, inout SurfaceOutputStandard o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
			o.Normal = UnpackNormal(tex2D(_NormalTex,IN.uv_MainTex));
		}
		ENDCG
	}
	FallBack "Diffuse"
}
