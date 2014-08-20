Shader "MQMTech/Ball" 
{
    Properties 
    {
        _mainColor ("Main Color", Color) = (1, 1, 1, 1)
        _mainColor2 ("Main Color2", Color) = (1, 1, 1, 1)
        
        _topColor ("Top Color", Color) = (1, 1, 1, 1)
        _bottomColor ("Bottom Color", Color) = (1, 1, 1, 1)
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
            
            fixed4 _mainColor;
            fixed4 _mainColor2;
            
            fixed4 _topColor;
            fixed4 _bottomColor;
            
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
            
            	float a = 1.;
            	fixed3 color = _mainColor;
            	
            	p.x += (FBM((p + float2(302., 299.)) * 70.) * 2. - 1.) * 0.02;
            	float d = mod(p.x, 0.25);
            	fixed3 color2 = color * _mainColor2;	
            	color = lerp(color, color2, smoothstep(d, 0.125, 0.1251));
            	
            	p.y += (FBM((p + float2(193., 201.)) * 70.) * 2. - 1.) * 0.02;
            	d = mod(p.y, 0.25);
            	color2 = color * _mainColor2;
            	color = lerp(color, color2, smoothstep(d, 0.125, 0.1251));
            	
            	d = p.y - 0.9;
            	color = lerp(color, _topColor, smoothstep(d, -0.1, 0.01));
            	
            	d = 0.1 - p.y;
            	color = lerp(color, _bottomColor, smoothstep(d, -0.1, 0.01));
            	
            	return fixed4(color, a);
            }

            float4 frag (v2f i) : COLOR
            {
            	fixed4 color = fixed4(0.);

    			color = CalculateColor(i.texcoord, i.worldPos);
    			
            	return color;
            }
            
            ENDCG
        }
    }
}
