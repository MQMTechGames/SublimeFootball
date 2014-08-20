Shader "MQMTech/LightTowerTorch" 
{
    Properties 
    {
        _color ("Base Color", Color) = (1, 1, 1, 1)
        _color2 ("Top Color", Color) = (1, 1, 1, 1)
        
        _lightColor ("Light Color", Color) = (0.5, 0, 0.5, 1)
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
            fixed4 _lightColor;

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
                
                o.pos = mul (UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
                o.texcoord = v.texcoord.xy;
                
                return o;
            }
            
            fixed4 CalculateColor(float2 texcoord, float3 vertexPos)
            {
            	float2 p = texcoord;
            	
            	float2 q = p * 2. - 1.;
            	
            	float lightIntensity = length(q);
            	fixed3 color = lerp(_color, _color2, lightIntensity);
            	color += _lightColor * (1.-lightIntensity);
            	color += fixed3(1) * pow((1.-lightIntensity), 3.);
            	
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
