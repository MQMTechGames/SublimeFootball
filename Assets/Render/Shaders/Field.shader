Shader "MQMTech/Field" 
{
    Properties 
    {
        _color ("_color", Color) = (1, 1, 1, 1)
        _color2 ("_color", Color) = (1, 1, 1, 1)
        _numLines ("_numLines", Float) = 4
    }
    
    SubShader {
        LOD 1
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma target 3.0
            #pragma glsl
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "ShaderFilters.cginc"
            
            #define PI 3.1415
            
            fixed4 _color;
            fixed4 _color2;
            half _numLines;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                half2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_tan v)
            {
                v2f o;
                
                o.pos = mul (UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
                o.texcoord = v.texcoord.xy;
                
                return o;
            }
            
            fixed4 CalculateColor(half2 texcoord)
            {
            	half2 p = texcoord.yx * 2.0 - 1.0;
                fixed3 color = fixed3(0.0, 1.0, 0.0);
                
                half lengthP = length(p);
                
                color = lerp(color, fixed3(1.0, 0., 0.), length(p));
                color = lerp(fixed3(0.0, 0., 1.), color, smoothstep(lengthP, 0., 0.2));
                
                color *= _color.rgb;
                fixed a = (1.-length(p)) * _color.a;
                
                return fixed4(color, a);
            }

            half4 frag (v2f i) : COLOR
            {
            	fixed4 color = CalculateColor(i.texcoord.xy);
                
                return color;
            }
            ENDCG
        }
    }
}
