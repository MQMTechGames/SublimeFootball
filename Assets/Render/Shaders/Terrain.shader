Shader "MQMTech/Terrain" 
{
    Properties 
    {
    	_aspectRatio ("Aspect Ratio", Float) = 1.4
    	_badgeSize ("Badge Size", Float) = 10.0
    
        _color ("Color", Color) = (1, 1, 1, 1)
        _color2 ("Color 2", Color) = (1, 1, 1, 1)
        _color3 ("Color 3", Color) = (1, 1, 1, 1)
        _waterColor ("Water Color", Color) = (1, 1, 1, 1)
        _linesColor ("Lines Color", Color) = (1, 1, 1, 1)
        
        _dirtColor ("Dirt Color", Color) = (0.5, 0, 0.5, 1)
        _dirtColor2 ("Dirt Color 2", Color) = (0.5, 0, 0.5, 1)
        _dirtColor3 ("Dirt Color 3", Color) = (1, 1, 1, 1)
        
        _specColor ("Specular Color 2", Color) = (1.0, 1.0, 1.0, 1)
        
        _specPower ("Specular Power", Float) = 10
        _specIntensity ("Specular Intensity", Float) = 2
        _normalNoiseFactor ("Normal Noise Factor", Float) = 0
        
        _ambientFactor ("Ambient Factor", Float) = 0.25
        
        _sunDir ("Sun Direction", Vector) = (0.5, 0.5, 0.5)
    }
    
    SubShader {
        LOD 1
        Tags {"Queue" = "Geometry" }
        
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma target 3.0
            #pragma only_renderers d3d9
            #pragma glsl
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Noise.cginc"  
            #include "ShaderFilters.cginc"
            #include "DistanceFields.cginc"
            #include "RealBadge.cginc"
            
            #define PI 3.14159265359

			half _aspectRatio;
			half _badgeSize;

            fixed4 _color;
            fixed4 _color2;
            fixed4 _color3;
            
            fixed4 _waterColor;
            fixed4 _linesColor;
            
            fixed4 _dirtColor;
            fixed4 _dirtColor2;
            fixed4 _dirtColor3;
            
            fixed4 _specColor;
            float _ambientFactor;
            
            float _specPower;
            float _specIntensity;
            float _normalNoiseFactor;
            float3 _sunDir;

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 viewDir : TEXCOORD1;
            };

            v2f vert (appdata_tan v)
            {
                v2f o;
                
                float3 worldPos = mul(_Object2World, float4(v.vertex.xyz, 1)).xyz;
                //worldPos.y += FBM(worldPos * 100.) * 5.;
                worldPos.y += FBM(worldPos * 10000. + _Time.y) * 2.;
                
                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos, 1));
                o.texcoord = v.texcoord.xy;
                
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                o.viewDir = viewDir;
                
                return o;
            }
            
       		float2 rotate2D(float2 p, float rad)
            {
            	return float2(p.x * cos(rad) - p.y * sin(rad), p.y * cos(rad) + p.x * sin(rad));
            }
            
        	fixed3 CalculateLineColor(float p, fixed3 colora, fixed3 colorb, float numLines)
            {
            	fixed3 color = colora;
            
				// Drawn circular lines
				float lengthPoint = length(p) * numLines;
				int lengthPointFloor = floor(lengthPoint);
				float lengthPointFrac = frac(lengthPoint);

				fixed3 mainColor = lengthPointFloor % 2 == 0 ? colorb.rgb : color.rgb;
				fixed3 secondaryColor = lengthPointFloor % 2 == 0 ? color.rgb : colorb.rgb;
				color = lerp(mainColor, secondaryColor, smoothstep(lengthPointFrac, 0.7, 1.0));
				
				return color;
            }
            
            fixed4 CalculateColorField(float2 itexcoord, float3 viewDir)
			{
				float2 p = itexcoord.yx;
				float temp = p.y;
				p.y = p.x;
				p.x = temp;
				
				fixed3 color = _color.rgb;
				float a = 1.;
				
				// Distance fields
				float2 q = p * 2. - 1.;
				float d = 1.;
				
				// Inner Athletic color
				d = length(float2(q.x * 1.3, q.y)) - 0.6;
				color = lerp(_color2, color, smoothstep((d), 0.0, 0.05));
				
				// Athetic lines
				d = abs(length(float2(q.x * 1.3, q.y)) - 0.8) - 0.2;
				if(d <= 0.)
				{
					d = sin(5 + 10 * PI * (1.-abs(d)/0.2));
					color = lerp(color, fixed3(1.), smoothstep(d, 0.95, 0.96));
				}
				// Out Athletic color
				d = -(length(float2(q.x * 1.3, q.y)) - 1.0);
				color = lerp(_color3, color, smoothstep(d, 0.0, 0.05));
				
				// Badge
				float2 badgeLocalPos = rotate2D(q, PI * 0.5);
				float signe = badgeLocalPos.x > 0 ? -1 : 1.;
				badgeLocalPos = float2(abs(badgeLocalPos.x), badgeLocalPos.y);
				badgeLocalPos = badgeLocalPos - float2(0.5, 0.);
				fixed4 badgeColor = GetBadgeColor(float2(signe * badgeLocalPos.x * _aspectRatio, badgeLocalPos.y) * _badgeSize);
				fixed3 mixedWitBadge = badgeColor * badgeColor.a + color * (1.-badgeColor.a);
				color = lerp(color, mixedWitBadge, 0.1 + smoothstep(sin(_Time.y * 0.5) * 0.15, 0., 1.));
				
				// Dirtiness
				color = lerp(color, _dirtColor3, smoothstep(FBM((p + float2(0.31, 0.35)) * 15.), 0.2, 1.) * 0.6);
				color = lerp(color, _dirtColor, smoothstep(FBM(p * 10.), 0.5, 1.) * 0.5);
				color = lerp(color, _dirtColor2, smoothstep(FBM((p + float2(10)) * 40.), 0.4, 1.) * 0.5);
				color = lerp(color, _dirtColor3, smoothstep(FBM((p + float2(10)) * 15.), 0.4, 1.) * 1.0);
				
				// External part
				d = length(float2(q.x * _aspectRatio, q.y)) - 1.0;
				d -= FBM(float2(q * 20.)) * 0.5;
				a = lerp(1., 0., smoothstep(d, 0.001, 0.2));
				
				// Illumination
				float3 l = normalize(_sunDir); 
				float bumpNoise = FBM(p * 500);
				float3 N = normalize(float3(0., 1., 0.) + float3(bumpNoise * 2. - 1., 0., FBM(float2(bumpNoise) * 2. -1. )) * _normalNoiseFactor);
				float diffFactor = max(dot(l, N), 0);
				
				float3 refl = reflect(viewDir, N);
				float specFactor = pow(max(dot(refl, l), 0), _specPower) * _specIntensity;
				
				color *= _ambientFactor + (1.-_ambientFactor) * diffFactor;
				color += _specColor * specFactor;
				
				return fixed4(color, a);
			}

            float4 frag (v2f i) : COLOR
            {
            	fixed4 color = CalculateColorField(i.texcoord.xy, i.viewDir);
                
                return color;
            }
            ENDCG
        }
    }
}
