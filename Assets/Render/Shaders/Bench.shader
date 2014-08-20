Shader "MQMTech/Bench" 
{
    Properties 
    {
        _color ("Base Color", Color) = (1, 1, 1, 1)
        _color2 ("Top Color", Color) = (1, 1, 1, 1)
        
        _dirtColor ("Dirt Color", Color) = (0.5, 0, 0.5, 1)
        _dirtColor2 ("Dirt Color 2", Color) = (0.5, 0, 0.5, 1)
    }
    
    SubShader {
        LOD 1
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma target 3.0
            #pragma only_renderers d3d9
            #pragma glsl
            #pragma fragment frag
            
            #include "Noise.cginc"            
            #include "UnityCG.cginc"
            #include "ShaderFilters.cginc"
            #include "DistanceFields.cginc"
            #include "RealBadge.cginc"

            #define PI 3.1415
            
            fixed4 _color;
            fixed4 _color2;
            
            fixed4 _dirtColor;
            fixed4 _dirtColor2;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            v2f vert (appdata_tan v)
            {
                v2f o;
                
                float3 worldPos = mul(_Object2World, float4(v.vertex.xyz, 1)).xyz;
                o.worldPos = worldPos;
                
                o.pos = mul (UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
                o.texcoord = v.texcoord.xy;
                
                return o;
            }
            
            fixed4 CalculateColor(float2 texcoord, float3 worldPos)
            {
            	float2 p = texcoord;
            	
            	float3 groundColor = _color;
            	groundColor = lerp(groundColor, _dirtColor, smoothstep(FBM((p + float2(0.31, 0.35)) * 15.), 0.2, 1.) * 0.6);
				groundColor = lerp(groundColor, _dirtColor2, smoothstep(FBM(p * 10.), 0.5, 1.) * 0.5);
				
				float3 mastilColor = _color2;
				mastilColor = lerp(mastilColor, fixed3(0.), smoothstep(FBM(p * 10.), 0.0, 1.) * 0.8);
				
            	float3 color = groundColor;
            	
            	color = lerp(color, mastilColor, smoothstep(p.y, 0., 0.4));
            	
            	float a = 1.;
            	
            	return fixed4(color, a);
            }

            float4 frag (v2f i) : COLOR
            {
            	fixed4 color = CalculateColor(i.texcoord, i.worldPos);
            
            	return color;
            }
            
            ENDCG
        }
    }
}
