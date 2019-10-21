Shader "Unlit/BackfaceShader"
{

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		Cull Front

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
				float4 linearDepth : TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.linearDepth = float4(0.0, 0.0, COMPUTE_DEPTH_01, 0.0);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(i.linearDepth.z,0.0,0.0,0.0);
            }
            ENDCG
        }
    }
}
