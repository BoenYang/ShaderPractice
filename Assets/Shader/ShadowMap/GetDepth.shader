Shader "Lighting/ShadowMapGetDepth"
{
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{	
			//Cull Front
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"



			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 depth : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.depth = o.vertex.zw;
				return o;
			}
			
			float4 frag (v2f i) : COLOR
			{
				float depth = i.depth.x/i.depth.y ;
				return fixed4(depth,depth,depth,1.0);
                //return EncodeFloatRGBA(depth) ;
			}
			ENDCG
		}
	}
}
