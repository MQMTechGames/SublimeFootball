Shader "MQMTech/Waterline" 
{
    Properties 
    {
    	_aspectRatio ("Aspect Ratio", Float) = 1.4
    
        _waterColor ("Water Color", Color) = (1, 1, 1, 1)
        _waterColor2 ("Water Color 2", Color) = (1, 1, 1, 1)
        
        _waterColor3 ("Water Color 3", Color) = (1, 1, 1, 1)
        _waterColor4 ("Water Color 4", Color) = (1, 1, 1, 1)
        
        _specColor ("Specular Color 2", Color) = (1.0, 1.0, 1.0, 1)
        
        _specPower ("Specular Power", Float) = 10
        _specIntensity ("Specular Intensity", Float) = 2
        _normalNoiseFactor ("Normal Noise Factor", Float) = 0
        _ambientFactor ("Ambient Factor", Float) = 0.25
        _sunDir ("Sun Direction", Vector) = (0.5, 0.5, 0.5)
        
        _coastPosition ("Coast Low Position", Float) = 1.3
        _coastDynamicPosition ("Coast Dynamic Position ", Float) = 0.5
        _coastDynamicSpeed ("Coast Dynamic Speed ", Float) = 0.5
        _coastPhase ("Coast Phase", Float) = 0.0
        
        _coastPosition2 ("Coast Low Position 2", Float) = 1.3
        _coastDynamicPosition2 ("Coast Dynamic Position 2", Float) = 0.5
        _coastDynamicSpeed2 ("Coast Dynamic Speed 2", Float) = 0.5
        _coastPhase2 ("Coast Phase 2", Float) = 0.0
    }
    
    SubShader {
        LOD 1
        Tags {"Queue" = "Transparent+10" }
        
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
            
            #define PI 3.14159265359

			half _aspectRatio;

            fixed4 _waterColor;
            fixed4 _waterColor2;
            
            fixed4 _waterColor3;
            fixed4 _waterColor4;
            
            fixed4 _specColor;
            float _ambientFactor;
            
            float _specPower;
            float _specIntensity;
            float _normalNoiseFactor;
            float3 _sunDir;
            
            float _coastPosition;
            float _coastDynamicPosition;
            float _coastDynamicSpeed;
            float _coastPhase;
            
            float _coastPosition2;
            float _coastDynamicPosition2;
            float _coastDynamicSpeed2;
            float _coastPhase2;

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
                worldPos.y += FBM(worldPos * 10000. + _Time.y) * 2.;
                
                o.pos = mul (UNITY_MATRIX_VP, float4(worldPos, 1));
                o.texcoord = v.texcoord.xy;
                
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);
                o.viewDir = viewDir;
                
                return o;
            }
            
            fixed4 ApplyWaterColor(float2 p, float distorsionFactor, fixed4 color, fixed4 waterColorA, fixed4 waterColorB, float coastPosition, float _coastDynamicPosition, float coastDynamicSpeed, float coastPhase)
            {
				float waterColorDistorsion = FBM(float2(length(p * 0.5))) * 0.3 + FBM(float2(p * 10.)) * 0.7;
				fixed4 waterColor = lerp(waterColorA, waterColorB, smoothstep(waterColorDistorsion, 0.3, 1.));
				
				float d = length(float2(p.x * _aspectRatio, p.y)) - coastPosition;
				float coastMovement = coastDynamicSpeed * _Time.y + coastPhase;
				d += (abs(sin(coastMovement)) * _coastDynamicPosition);
				d -= FBM(p * distorsionFactor) * 1.2;
				waterColor.a = lerp(0, 1., smoothstep(d, 0.001, 0.1));
				
				d = length(float2(p.x * _aspectRatio, p.y)) - coastPosition;
				d -= FBM(p * 2.2) * 1.0;
				waterColor.a *= lerp(1, 0., smoothstep(d, 0.001, 0.1));
				
				// Apply new water color
				float alphaBlend = waterColor.a / (waterColor.a + color.a);
				
				color.rgb = waterColor.rgb * alphaBlend + color.rgb * (1.-alphaBlend);
				color.a = max(color.a, waterColor.a);
				
				return color;
            }
            
            fixed4 CalculateColorField(float2 itexcoord, float3 viewDir)
			{
				float2 p = itexcoord.yx;
				float temp = p.y;
				p.y = p.x;
				p.x = temp;
				
				// Distance fields
				float2 q = p * 2. - 1.;
				
				fixed4 color = fixed4(0.);
				color = ApplyWaterColor(q, 2.01, color, _waterColor, _waterColor2, _coastPosition, _coastDynamicPosition, _coastDynamicSpeed, _coastPhase);
				color = ApplyWaterColor(q, 2.23, color, _waterColor3, _waterColor4, _coastPosition2, _coastDynamicPosition2, _coastDynamicSpeed2, _coastPhase2);
				color = ApplyWaterColor(q, 2.42, color, _waterColor, _waterColor4, _coastPosition2, _coastDynamicPosition, _coastDynamicSpeed2, (_coastPhase2 - _coastPhase) * 1.7);
				color = ApplyWaterColor(q, 2.71, color, _waterColor2, _waterColor4, _coastPosition, _coastDynamicPosition2, _coastDynamicSpeed, (_coastPhase2 - _coastPhase) * 1.3);
				
				// External part
				float d = length(float2(q.x * _aspectRatio, q.y)) - 1.0;
				d -= FBM(float2(q * 20.)) * 0.5;
				color.a *= lerp(1., 0., smoothstep(d, 0.001, 0.2));
				
				// Illumination
//				float3 l = normalize(_sunDir); 
//				float bumpNoise = FBM(p * 500);
//				float3 N = normalize(float3(0., 1., 0.) + float3(bumpNoise * 2. - 1., 0., FBM(float2(bumpNoise) * 2. -1. )) * _normalNoiseFactor);
//				float diffFactor = max(dot(l, N), 0);
//				
//				float3 refl = reflect(viewDir, N);
//				float specFactor = pow(max(dot(refl, l), 0), _specPower) * _specIntensity;
//				
//				color.rgb *= _ambientFactor + (1.-_ambientFactor) * diffFactor;
//				color += _specColor * specFactor;
//				color += _specColor * fixed(specFactor);
				
				return color;
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
