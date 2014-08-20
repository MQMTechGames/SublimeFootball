Shader "MQMTech/SquareGrassField" 
{
    Properties 
    {
    	_aspectRatio ("Aspect Ratio", Float) = 1.4
    
        _color ("Color", Color) = (1, 1, 1, 1)
        _linesColor ("Lines Color", Color) = (1, 1, 1, 1)
        
        _dirtColor ("Dirt Color", Color) = (0.5, 0, 0.5, 1)
        _dirtColor2 ("Dirt Color 2", Color) = (0.5, 0, 0.5, 1)
        _dirtColor3 ("Dirt Color 3", Color) = (1, 1, 1, 1)
        
        _specColor ("Specular Color 2", Color) = (1.0, 1.0, 1.0, 1)
        
        _numLinesX ("Number of Lines X", Float) = 18
        _numLinesY ("Number of Lines Y", Float) = 12
        _lineThickness ("Line Thickness", Float) = 0.0008
        
        _specPower ("Specular Power", Float) = 10
        _specIntensity ("Specular Intensity", Float) = 2
        _normalNoiseFactor ("Normal Noise Factor", Float) = 0
        
        _ambientFactor ("Ambient Factor", Float) = 0.25
        
        _sunDir ("Sun Direction", Vector) = (0.5, 0.5, 0.5)
    }
    
    SubShader {
        LOD 1
        Tags {"Queue" = "Transparent" }
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

			half _aspectRatio;

            fixed4 _color;
            fixed4 _linesColor;
            
            fixed4 _dirtColor;
            fixed4 _dirtColor2;
            fixed4 _dirtColor3;
            
            fixed4 _specColor;
            
            float _numLinesX;
            float _numLinesY;
            float _lineThickness;
            
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
                //worldPos.y += FBM(v.vertex.xyz * 10000. + _Time.y) * 1.;
                //worldPos.y += smoothstep((1. - length(v.texcoord.x * 2. - 1.)), 0.25, 1.) * 2.;
                
                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos, 1));
                o.texcoord = v.texcoord.xy;
                
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                o.viewDir = viewDir;
                
                return o;
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
				
				float lineThickness = _lineThickness;
				float lineGlow = lineThickness;
				
				// Distance fields
				float2 q = p * 2. - 1.;
				float d = 1.;
				
				// Inner square
				float innersField = max(abs(q.x) - 0.99, abs(q.y) - 0.99);
				
				// Middle line
				d = Line2D(q, lineThickness, 1.0);
				d = max(d, innersField);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
//				// Middle Circle
				d = CirclePerimeter2D(float2(q.x * _aspectRatio, q.y), 0.3, lineThickness);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Middle spot
				d = Circle2D(q* float2(_aspectRatio, 1.), lineThickness * 6.);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Goal lines
				float2 qx = q;
				qx.x = abs(q.x);
				
//				// Front Line
				d = Line2D(qx - float2(0.65, 0.), lineThickness, 0.60);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
//				// Semi Circle 1
				d = CirclePerimeter2D(float2((q.x - 0.75) * _aspectRatio, q.y), 0.3, lineThickness);
				float d2 = q.x - 0.65;
				d = max(d, d2);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
//				// Semi Circle 2
				d = CirclePerimeter2D(float2((q.x + 0.75) * _aspectRatio, q.y), 0.3, lineThickness);
				d2 = q.x + 0.65;
				d = max(d, -d2);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));

//				// Wings
				float2 qxy = qx;
				qxy.y = abs(qxy.y);
				d = Line2D(qxy - float2(0.85, 0.60), 0.2, lineThickness);
				d = max(d, innersField);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
//				// Inner Front Line
				d = Line2D(qxy - float2(0.90, 0.), lineThickness, 0.30);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
//				// Inner Wings
				d = Line2D(qxy - float2(0.95, 0.30), 0.05, lineThickness);
				d = max(d, innersField);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Side Lines
				d = Line2D(qxy - float2(0., 0.99), 0.99, lineThickness);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Bottom Lines
				d = Line2D(qxy - float2(0.99, 0.), lineThickness, 0.99);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Penalty spot
				d = Circle2D((qxy - float2(0.80, 0.)) * float2(_aspectRatio, 1.), lineThickness * 6.);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Squares
				d = CirclePerimeter2D(float2((qxy.x - 0.99) * _aspectRatio, qxy.y - 0.99), 0.03, lineThickness);
				d = max(d, innersField);
				color = lerp(_linesColor, color, smoothstep(d, 0.0, lineGlow));
				
				// Drawn grass lines
				fixed3 color2 = color * 0.7;
				color = CalculateLineColor(p.x, color, color2, _numLinesX);
				
				color2 = color * 0.7;
				color = CalculateLineColor(p.y, color, color2, _numLinesY);
				
//				// Dirtiness
				color = lerp(color, _dirtColor3, smoothstep(FBM((p + float2(0.31, 0.35)) * 15.), 0.2, 1.) * 0.6);
				
				fixed3 goalDirt = lerp(color, _dirtColor, smoothstep(FBM(p * 100.), 0.55, 1.25));
				goalDirt = lerp(goalDirt, _dirtColor, smoothstep(FBM(p * 31.), 0.5, 1.45));
				d = max(0.7 - length(q.x), length(q.y) - 0.5);
				d = min(d, length(float2(q.x * _aspectRatio, q.y)) - 0.2);
				
				color = lerp(goalDirt, color, smoothstep(d, 0., 0.1));
				
				return fixed4(color.rgb, 1.);
				color = lerp(color, _dirtColor2, smoothstep(FBM((p + float2(10)) * 40.), 0.4, 1.) * 0.5);
				
//				// Illumination
				float3 l = normalize(_sunDir); 
				float bumpNoise = FBM(p*500);
				float3 N = normalize(float3(0., 1., 0.) + float3(bumpNoise * 2. - 1., 0., FBM(float2(bumpNoise) * 2. -1. )) * _normalNoiseFactor);
				float diffFactor = max(dot(l, N), 0);
				
				float3 refl = reflect(viewDir, N);
				float specFactor = pow(max(dot(refl, l), 0), _specPower) * _specIntensity;
				
				color *= _ambientFactor + (1.-_ambientFactor) * diffFactor;
				color += _specColor * specFactor;
				
//				fixed4 badgeColor = GetBadgeColor(float2(-q.x * _aspectRatio, q.y) * 40.0);
//				fixed3 mixedWitBadge = badgeColor * badgeColor.a + color * (1.-badgeColor.a);
//				color = lerp(color, mixedWitBadge, smoothstep(sin(_Time.y * 0.5) * 0.15, 0., 1.));
				
				// vigneting
				float a = 1.;
				a *= 1. - smoothstep(length(q.x), 0.97, 1.0);
				a *= 1. - smoothstep(length(q.y), 0.97, 1.0);
				a *= 1. - smoothstep(FBM(p * 24.35), 0.3, 1.);
				
				return fixed4(color, 1);
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
