Shader "MQMTech/Skybox" 
{
    Properties 
    {
    	_aspectRatio ("Aspect Ratio", Float) = 1.4
    	
    	_skyColor ("Sky Color", Color) = (1, 1, 1, 1)
    	_cloudsColor ("Clouds Color", Color) = (1, 1, 1, 1)
    	
    	_waterColor ("Water Color", Color) = (1, 1, 1, 1)
    	_surfaceWaterColor ("Surface Water Color", Color) = (1, 1, 1, 1)
    	
    	_sunColor ("Sun Color", Color) = (1, 1, 1, 1)
    }
    
    SubShader {
        //LOD 1
        Tags { "Queue"="Background" "RenderType"="Background" }
        
        Pass {

			Cull Off 
			ZWrite Off 
			//Fog { Mode Off }

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
            
            fixed4 _skyColor;
            fixed4 _cloudsColor;
            
            fixed4 _waterColor;
            fixed4 _surfaceWaterColor;
            fixed4 _sunColor;
            
            static float3 sunDir = normalize(float3(-1.5, 0.6, 1.));
            
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float3 localPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 ssPos : TEXCOORD3;
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
                
                o.localPos = v.vertex.xyz;
                
                o.ssPos = o.pos;
                
                return o;
            }
            
       		float2 rotate2D(float2 p, float rad)
            {
            	return float2(p.x * cos(rad) - p.y * sin(rad), p.y * cos(rad) + p.x * sin(rad));
            }
            
            #define SKY_START_Y_POS 8000.
            #define SKY_END_Y_POS 9000.
            #define SKY_START_END_DIST SKY_END_Y_POS - SKY_START_Y_POS
            #define SKY_NUM_STEPS 10.
            #define SKY_MAX_DISTANCE 40000.
            fixed3 DrawSky(float3 ro, float3 rd)
            {
            	fixed3 color = fixed3(0);
            	float distToStartYPos = SKY_START_Y_POS - ro.y;
            	float3 startPos = ro + rd * (distToStartYPos / rd.y);
            	float3 endPos = startPos + rd * (SKY_START_END_DIST / rd.y);
            	float3 stepDir = (endPos - startPos) / SKY_NUM_STEPS;
            	
            	float3 currPos = float3(0.);
            	for(int i = 0; i < ((int) SKY_NUM_STEPS); ++i)
            	{
            		currPos = startPos + ((float) i) * stepDir;
            		float d = FBM(currPos * 0.001 + _Time.y * 0.2);
            		
            		fixed3 currSkyColor = color + _cloudsColor * smoothstep(d, 0.4, 1.) / ((float) SKY_NUM_STEPS);
            		color = lerp(currSkyColor, color, smoothstep(length(currPos.xy) - SKY_MAX_DISTANCE, 0., 1000.));
            	}
            	color += _skyColor;
            	
            	// Sun
            	//float3 sunDir = normalize(float3(-1.5, 0.6, 1.));
            	float sunIntensity = clamp(pow(dot(sunDir, rd), 7.) * 0.7, 0.0, 1.0);
            	color += _sunColor * sunIntensity;
            	
            	sunIntensity = clamp(pow(dot(sunDir, rd), 100.) * 0.7, 0.0, 1.0);
            	color += _sunColor * sunIntensity;
            	
            	sunIntensity = clamp(pow(dot(sunDir, rd), 1000.) * 0.8, 0.0, 1.0);
            	color += _sunColor * sunIntensity;
            	
            	return color;
            }
            
        	#define WATER_START_Y_POS 0.
            #define WATER_END_Y_POS -2000.
            #define WATER_START_END_DIST SKY_END_Y_POS - SKY_START_Y_POS
            #define WATER_NUM_STEPS 10.
            #define WATER_MAX_DISTANCE 40000.
            fixed3 DrawWater(float3 ro, float3 rd)
            {
            	fixed3 color = fixed3(0.);
            	float distToStartYPos = WATER_START_Y_POS - ro.y;
            	float3 startPos = ro + rd * (distToStartYPos / rd.y);
            	float3 endPos = startPos + rd * (WATER_START_END_DIST / rd.y);
            	float3 stepDir = (endPos - startPos) / WATER_NUM_STEPS;
            	
            	float3 currPos = float3(0.);
            	for(int i = 0; i < ((int) WATER_NUM_STEPS); ++i)
            	{
            		currPos = startPos + ((float) i) * stepDir;
            		float d = FBM(currPos * 0.002);
            		
            		fixed3 currWaterColor = color + _waterColor * smoothstep(d, 0.5, 1.) / ((float) WATER_NUM_STEPS);
            		color = lerp(currWaterColor, color, smoothstep(length(currPos.xy) - WATER_MAX_DISTANCE, 0., 100.));
            	}
            	
            	color += _surfaceWaterColor;
            	
            	return color;
            }
            
            float2 rdCircle(float3 p)
            {
            	return float2(length(p) - 100., 1.);
            }
            
            float2 rdTube(float3 p, float width, float height)
            {
            	float d = max(length(p.xz) - width, abs(p.y) - height);
            	return float2(d, 2.);
            }
            
            float2 rdHeightMap(float3 p, float height)
            {
            	float d = p.y + height;
            	d += FBM(p * 0.0001) * 100.;
            	
            	return float2(d, 3.);
            }
            
            float2 Map(float3 p)
            {
            	float2 res = rdCircle(p - float3(0., 1000., 0.));
            	float2 res2 = rdTube(p - float3(0., 1000., 0.), 100, 200);
            	if(res2.x < res.x)
            	{
            		res = res2;
            	}
            	res2 = rdHeightMap(p, 1.);
            	if(res2.x < res.x)
            	{
            		res = res2;
            	}
            	
            	return res;
            }
            
            float2 Raytrace(float3 ro, float3 rd)
            {
            	float d = 0.1;
            	float k = d;
            	float2 res = float2(0.);
            	for(int i = 0; i < 40; ++i)
            	{
            		if( d < 0.01) continue;
            		res = Map(ro + k * rd);
            		
            		d = res.x;
            		k += d;
            		
            	}
            	
            	res.x = k;
            	if(d > 0.01)
            	{
            		res.y = -1.;
            	}
            	
            	return res;
            }
            
            fixed3 GetColor(float2 res, float3 p)
            {
            	return float3(1);
            }
            
            float3 CalculateNormal(float3 p, float3 rd)
            {
            	float3 ep = float3(0.001, 0., 0.);
            	
            	return float3
            	(
            		Map(p + rd * ep.xyy).x - Map(p - rd * ep.xyy).x,
            		Map(p + rd * ep.yxy).x - Map(p - rd * ep.yxy).x,
            		Map(p + rd * ep.yyx).x - Map(p - rd * ep.yyx).x
            	);
            }
            
            fixed4 CalculateColorField(float2 itexcoord, float3 viewDir, float3 ssPos, float3 localPos)
			{
				float3 cameraFront = -normalize(float3(UNITY_MATRIX_V[2][0], UNITY_MATRIX_V[2][1], UNITY_MATRIX_V[2][2]));
				float3 cameraRight = normalize(float3(UNITY_MATRIX_V[0][0], UNITY_MATRIX_V[0][1], UNITY_MATRIX_V[0][2]));
				float3 cameraUp = normalize(float3(UNITY_MATRIX_V[1][0], UNITY_MATRIX_V[1][1], UNITY_MATRIX_V[1][2]));
				
				float3 ro = _WorldSpaceCameraPos;
				float3 rd = normalize(cameraFront + cameraRight * ssPos.x * 1.4 + cameraUp * ssPos.y);
				
				fixed3 color = fixed3(0.);
				float a = 1.;
				
				float diffuseFactor = 1.;
				
				if(rd.y > 0.)
				{
					color = DrawSky(ro, rd);
				}
				else
				{
					color = DrawWater(ro, rd);
				}
				
				float2 res = Raytrace(ro, rd);
				if(res.y > 0.)
				{
					float p = ro + res.x * rd;
					color = GetColor(res, p);
					float3 normal = CalculateNormal(p, rd);
					
					// Lighting
					//diffuseFactor = clamp(dot(normal, sunDir), 0., 1.);
					color.x = color.x * diffuseFactor;
				}
				
				return fixed4(color, a);
			}
			
            float4 frag (v2f i) : COLOR
            {
            	float3 ssPos = i.ssPos.xyz / i.ssPos.w;
            	ssPos.y = -ssPos.y;
            	fixed4 color = CalculateColorField(i.texcoord.xy, i.viewDir, ssPos, i.localPos);
                
                return fixed4(0.);
                //return color;
            }
            ENDCG
        }
    }
}
