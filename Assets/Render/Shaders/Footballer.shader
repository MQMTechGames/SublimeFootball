Shader "MQMTech/Footballer" 
{
    Properties 
    {
    	_baseColor ("Base Color", Color) = (1, 1, 1, 1)
        
        _mainColor ("Main Color", Color) = (1, 1, 1, 1)
        _mainColor2 ("Main Color2", Color) = (1, 1, 1, 1)
        
        _DirectionColor ("Direction Color", Color) = (1, 1, 1, 1)
        
        _partId ("Part Id", Int) = 1
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
            
            fixed4 _baseColor;
            
            fixed4 _mainColor;
            fixed4 _mainColor2;
            
            fixed4 _DirectionColor;
            
            half _partId;

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
            
            fixed4 CalculateBaseColor(float2 texcoord, float3 worldPos)
            {
            	float2 p = texcoord;
            
            	float a = 1.;
            	fixed3 color = _mainColor;
            	
            	float d = sin(p.y * 21.);
            	d += (FBM(p * 50.) * 2. - 1.) * 0.5;
            	
            	color = lerp(color, _mainColor2.rgb, smoothstep(d, 0., 0.001));
            	
            	return fixed4(color, a);
            }

            float4 frag (v2f i) : COLOR
            {
            	fixed4 color = fixed4(0.);

    			color = CalculateBaseColor(i.texcoord, i.worldPos);
    			
            	return color;
            }
            
            ENDCG
        }
    }
}
