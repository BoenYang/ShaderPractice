Shader "Unlit/Shield"
{
	Properties
	{
		_MainColor("MainColor", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Fresnel("Fresnel Intensity", Range(0,200)) = 3.0
		_FresnelWidth("Fresnel Width", Range(0,2)) = 3.0
		_ScrollSpeed("Scroll Speed",float) = 0.03
		_IntersectThreshold("IntersectThreshold",Range(0,1)) = 0.1

		//SubShader
		//{
		//	Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		//	//GrabPass{ "_GrabTexture" }
		//	Pass
		//	{
		//		Lighting Off
		//		ZWrite Off
		//		Blend SrcAlpha OneMinusSrcAlpha
		//		Cull Back

		//		CGPROGRAM
		//		#pragma vertex vert
		//		#pragma fragment frag
		//		#include "UnityCG.cginc"

		//		struct appdata
		//		{
		//			fixed4 vertex : POSITION;
		//			fixed4 normal : NORMAL;
		//			fixed3 uv : TEXCOORD0;
		//		};

		//		struct v2f
		//		{
		//			fixed2 uv : TEXCOORD0;
		//			fixed4 vertex : SV_POSITION;
		//			fixed3 rimColor : TEXCOORD1;
		//			fixed4 screenPos : TEXCOORD2;
		//		};

		//		sampler2D _MainTex, _CameraDepthTexture, _GrabTexture;
		//		fixed4 _MainTex_ST,_MainColor,_GrabTexture_ST,_GrabTexture_TexelSize;
		//		fixed _Fresnel,_FresnelWidth,_IntersectThreshold;
		//		fixed _ScrollSpeed;

		//		v2f vert(appdata v)
		//		{
		//			v2f o;
		//			o.vertex = UnityObjectToClipPos(v.vertex);
		//			o.uv = TRANSFORM_TEX(v.uv, _MainTex);

		//			//fresnel 
		//			fixed3 viewDir = normalize(ObjSpaceViewDir(v.vertex));
		//			//dotProduct 为 0 - 1 为1则为视线与法线成90度，则为边缘
		//			fixed dotProduct = 1 - saturate(dot(v.normal, viewDir));
		//			o.rimColor = smoothstep(-_FresnelWidth, 1.0, dotProduct) * .5f;
		//			o.screenPos = ComputeScreenPos(o.vertex);
		//			COMPUTE_EYEDEPTH(o.screenPos.z);//eye space depth of the vertex 
		//			return o;
		//		}

		//		fixed4 frag(v2f i, fixed face : VFACE) : SV_Target
		//		{
		//			//intersection 1-0 0表示相交，1表示不相交
		//			fixed sceneDepth = LinearEyeDepth(tex2Dproj(_CameraDepthTexture,i.screenPos)).r;
		//			fixed intersect = saturate((sceneDepth - i.screenPos.z) / _IntersectThreshold);

		//			fixed2 uv = i.uv + _ScrollSpeed * _Time.y;
		//			//一般来说是一张黑白图
		//			fixed3 main = tex2D(_MainTex, uv);
		//			//distortion

		//			//fixed3 distortColor = tex2Dproj(_GrabTexture, i.screenPos);

		//			//intersect hightlight
		//			//越边缘的并且不相交的地方rim值越高，且背面的rim值为0
		//			//i.rimColor *= intersect * clamp(0,1,face);
		//			//主贴图中白的地方会显示成_mainColor,并且越边缘的会越接近_mainColor
		//			main *= _MainColor * pow(_Fresnel,i.rimColor);

		//			//lerp distort color & fresnel color
		//			//越接近边缘的部分显示main与_mainColor的混合颜色，否则显示背景
		//			//main = lerp(distortColor, main, i.rimColor.r);
		//			//相交的地方加上_MainColor，正面像素加的比例比较少，背面加比较多
		//			main += (1 - intersect) * (face > 0 ? .03 : .3)* _MainColor* _Fresnel;

		//			return fixed4(main,0.5);
		//		}
		//		ENDCG
		//	}
		//}
	}
}