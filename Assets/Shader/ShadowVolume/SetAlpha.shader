Shader "Lighting/SetAlpha"
{
	SubShader
	{
		 ColorMask A
		 ZTest Always Cull Off ZWrite Off
		 Pass {
		    Color (0,0,0,0.25)
		 }
		 Pass {
		   Blend DstColor One
		   Color (0,0,0,1)
		 } 
		 Pass {
		    Blend OneMinusDstColor Zero
		    Color (0,0,0,1)
		 } 
		 Pass {
		    Blend One One
		    Color (0,0,0,0.5)
		 } 
		 Pass {
		    ColorMask RGB
		    Blend Zero DstAlpha
		    Color (1,1,1,1)
		 } 
	}
}
