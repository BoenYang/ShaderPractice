Shader "Unlit/Shield"
{
	Properties
	{
		_MainColor("MainColor",Color) = (1,1,1,1)
		_IntersectThreshold("IntersectThreshold",Range(0,1)) = 0.1
		_Freshel("Freshel",float) = 1
	}
	SubShader
	{
		Tags {"Queue" = "Overlay" "RenderType"="Transparent" }
		LOD 100

		Pass
		{
			
			Lighting Off
			ZWrite On
			Cull Off
			//Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
				float rim		: TEXCOORD2;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float _IntersectThreshold;
			float4 _MainColor;
			float _Freshel;
			sampler2D _CameraDepthTexture;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				float3 viewDir = ObjSpaceViewDir(v.vertex);
				float rim = 1 - saturate(dot(v.normal,viewDir));
				o.rim = rim;
				COMPUTE_EYEDEPTH(o.screenPos.z);
				return o;
			}
			
			fixed4 frag (v2f i, fixed face : VFACE) : SV_Target
			{
				float cameraDepth = tex2Dproj(_CameraDepthTexture,i.screenPos).r;
				float eyeDepth = LinearEyeDepth(cameraDepth); 
				float intersect = saturate(abs(eyeDepth - i.screenPos.z))/_IntersectThreshold;
				i.rim *= intersect * clamp(0,1,face);
				fixed4 col = _MainColor * i.rim;
				return col;
			}
			ENDCG
		}
	}
}
