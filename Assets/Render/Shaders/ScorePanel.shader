Shader "MQMTech/ScorePanel" 
{
    Properties 
    {
        _color ("Base Color", Color) = (1, 1, 1, 1)
        _color2 ("Top Color", Color) = (1, 1, 1, 1)
        _pendulumColor ("Pendulum Color", Color) = (1, 1, 1, 1)
        
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

            #define PI 3.1415
            
            fixed4 _color;
            fixed4 _color2;
            fixed4 _pendulumColor;
            
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
            
            float2 rotate2D(float2 p, float rad)
            {
            	return float2(p.x * cos(rad) - p.y * sin(rad), p.y * cos(rad) + p.x * sin(rad));
            }
            
            fixed4 CalculateColor(float2 texcoord, float3 worldPos)
            {
            	float2 p = texcoord;
            	
            	float3 bottomColor = _color;
            	bottomColor = lerp(bottomColor, _dirtColor, smoothstep(FBM((float2(p.x) + float2(0.31, 0.35)) * 20.5), 0.2, 1.) * 0.6);
				bottomColor = lerp(bottomColor, _dirtColor2, smoothstep(FBM(float2(p.x) * 0.8), 0.5, 1.) * 0.5);
				
            	fixed3 color = bottomColor;
            	
            	fixed3 topColor = _color2;
				topColor = lerp(topColor, fixed3(0.), smoothstep(FBM(float2(p.x) * 2.1), 0.0, 1.) * 0.8);
            	color = lerp(color, topColor, smoothstep(p.y, 0., 0.4));
            	
            	// setup pendulum position
            	float2 q = p * 2. - 1.;
            	
            	// print borders
            	float d = min(abs(abs(q.y) - 1.), abs(abs(q.x) - 1.)) - 0.05;
            	d = min(abs(length(q) - 0.8) - 0.02, d);
            	color = lerp(fixed3(0.), color, smoothstep(d, 0., 0.01));
            	
            	float2 iconPosition = float2(sin(_Time.y) * 0.7, -lerp(0.3, 1., abs(cos(_Time.y)) * 0.4)) * 0.6;
            	
            	// draw diagonal line
            	float lineThickness = 0.06;
            	float lineLength = 0.5;
            	float obliqueness = -iconPosition.x;
            	float2 qq = q - float2(0., 0.4);
            	d = max(abs( qq.x - qq.y * obliqueness * 1.6 ) - lineThickness, length(qq) - max(lineLength*(1.+abs(obliqueness)), 0.6));
            	d = max(d, qq.y - 0.1);
            	d += sin(p.x * 10.) * cos(p.y * 10.) * 0.05;
            	
            	fixed3 lineColor = fixed3(0.);
            	color = lerp(lineColor, color, smoothstep(d, 0., 0.001));
            	
            	// draw pendulum
            	float2 lpos = q - iconPosition;
            	
            	// Change scale :)
            	lpos *= lerp(0.5 + abs(sin(_Time.y * 0.5) * 0.5 ), 1., smoothstep(length(lpos), 0.1, 0.5));
            	
            	float lengthLPos = length(lpos);
            	d = lengthLPos - 0.26;
            	if(d < 0.001)
            	{
            		color = lerp(lineColor, color, smoothstep(d, 0., 0.001));
            		
            		float dist = sin(atan2(lpos.y, lpos.x) * 40. + FBM(lpos)* 200. );
            		color = lerp(color, fixed3(0.5, 0., 0.), dist);
            		
            		color = lerp(_color2, color, smoothstep(lengthLPos - 0.01, 0., 0.2));
            		
    				fixed3 insideEye = sin(25. * lengthLPos/0.1) * sin(FBM(lpos * 10.) * 5.);
    				
    				color = lerp(insideEye, color, smoothstep(lengthLPos, 0.01, 0.08));
    				
    				color = lerp(fixed3(0.5, 0., 0.), color, smoothstep(lengthLPos, 0.005, 0.01));
    			}
            	
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