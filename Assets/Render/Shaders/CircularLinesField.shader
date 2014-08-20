Shader "MQMTech/CircularLinesField" 
{
    Properties 
    {
        _color ("Color", Color) = (1, 1, 1, 1)
        _color2 ("Color 2", Color) = (1, 1, 1, 1)
        
        _dirtColor ("Dirt Color", Color) = (0.5, 0, 0.5, 1)
        _dirtColor2 ("Dirt Color 2", Color) = (0.5, 0, 0.5, 1)
        
        _numLines ("Number of Lines", Float) = 4
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
            
            #include "Noise.cginc"            
            #include "UnityCG.cginc"
            #include "ShaderFilters.cginc"

            #define PI 3.1415
            
            fixed4 _color;
            fixed4 _color2;
            
            fixed4 _dirtColor;
            fixed4 _dirtColor2;
            
            float _numLines;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            v2f vert (appdata_tan v)
            {
                v2f o;
                
                o.pos = mul (UNITY_MATRIX_MVP, float4(v.vertex.xyz, 1));
                o.texcoord = v.texcoord.xy;
                
                return o;
            }
            
            fixed4 CalculateColorField(float2 itexcoord)
			{
				float2 p = itexcoord.yx * 2.0 - 1.0;
				p.x *= 1.5;
				fixed3 color = _color.rgb;

				float distorsion = (1.+(sin(_Time.y) + 1.) * 0.5);

				// Drawn circular lines
				float lengthPoint = length(p) * _numLines;
				int lengthPointFloor = floor(lengthPoint);
				float lengthPointFrac = frac(lengthPoint);

				fixed3 mainColor = lengthPointFloor % 2 == 0 ? _color2.rgb : _color.rgb;
				fixed3 secondaryColor = lengthPointFloor % 2 == 0 ? _color.rgb : _color2.rgb;

				color = lerp(secondaryColor, mainColor, smoothstep(lengthPointFrac, 0., 0.3));
				color = lerp(mainColor, secondaryColor, smoothstep(lengthPointFrac, 0.7, 1.0));

				// Dirtiness
				color = lerp(color, _dirtColor, smoothstep(FBM(p * 10.), 0.5, 1.) * 0.5);
				color = lerp(color, _dirtColor2, smoothstep(FBM((p + float2(10)) * 50.), 0.65, 1.) * 0.5);
				                
				return fixed4(color, 1.);
			}

            float4 frag (v2f i) : COLOR
            {
            	fixed4 color = CalculateColorField(i.texcoord.xy);
                
                return color;
            }
            ENDCG
        }
    }
}
