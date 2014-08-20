Shader "MQMTech/LightTowerBase" 
{
    Properties 
    {
        _color ("Base Color", Color) = (1, 1, 1, 1)
        _color2 ("Top Color", Color) = (1, 1, 1, 1)
        
        _dirtColor ("Dirt Color", Color) = (1, 1, 1, 1)
        _dirtColor2 ("Dirt Color 2", Color) = (1, 1, 1, 1)
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
            
            fixed4 _dirtColor;
            fixed4 _dirtColor2;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 vertexPos : TEXCOORD1;
            };

            v2f vert (appdata_tan v)
            {
                v2f o;
                
                o.vertexPos = v.vertex.xyz;
                
                o.pos = mul (UNITY_MATRIX_MVP, float4(o.vertexPos, 1));
                o.texcoord = v.texcoord.xy;
                
                return o;
            }
            
            fixed4 CalculateColor(float2 texcoord, float3 worldPos)
            {
            	float2 p = texcoord;

				fixed3 color = lerp(_color, _color2, smoothstep(worldPos.z, 0., 100.));

				fixed3 dirtyColor = lerp(_dirtColor.rgb, _dirtColor2.rgb, FBM(float2(worldPos.x)));
				color = lerp(color, dirtyColor, 1.0);
				
				float2 q = p * 2. - 1.;
				float d = abs(min(abs(q.x) - 1., abs(q.y) - 1.)) - 0.01;
				color = lerp(fixed3(0.), color, d);
				
            	float a = 1.;
            	
            	return fixed4(color, a);
            }

            float4 frag (v2f i) : COLOR
            {
            	fixed4 color = CalculateColor(i.texcoord, i.vertexPos);
            
            	return color;
            }
            
            ENDCG
        }
    }
}
