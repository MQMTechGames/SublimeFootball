Shader "SocialPoint/Water" 
{
    Properties 
    {
        _EnvironmentMap ("Environment Map", CUBE) = "" {}
        _EnvironmentBias ("_Environment Bias", Float) = 0.06
        _EnvironmentDistortion ("Environment Distortion", Float) = 0.1
        
        _SunDir ("SunDir", Vector) = (0.3, -0.6, -1, 0)
        
        _SurfaceColor ("Surface Color", Color) = (1,1,1,0.5)
        _UnderWaterColor ("Under Water Color", Color) = (1,1,1,1.0)

        _FresnelPow ("Fresnel Pow", Float) = 4
        _FresnelBias ("Fresnel Bias", Float) = 0.06
            
        _RefractionBlend ("_RefractionBlend", Float) = 0.7
        _ReflectionColor ("_ReflectionColor", Color) = (1,1,1,1.0)
        
        _fresnelViewBias ("_fresnelViewBias", Float) = 0
        _RefractionDistortion ("Refraction Distortion", Float) = 1

        _WaveVelocity ("_WaveVelocity Velocity", Vector) = (0, 0, 0, 0)
        _WaveTiling("_WaveTiling tiling", Vector) = (1, 1, 0, 0)
        
        _SpecularColor ("SpecularColor", Color) = (1,1,1,1.0)
        _SpecularSize ("Specular Pow", Float) = 250
        _SpecularIntensity ("Specular Intensity", Float) = 1
        _SpecDistortion ("Specular Distorion", Float) = 1
        _specularWidth ("_specularWidth", Float) = 1
        _specularVel ("_specularVel", Float) = 1

        _Bump ("Bump (RGB)", 2D) = "bump" {}
        _BumpVel ("Bump Velocity", Vector) = (0, 0, 0, 0)
        _BumpTiling("Bump tiling", Vector) = (1, 1, 0, 0)
        
        _BumpB ("BumpB (RGB)", 2D) = "bump" {}
        _BumpBVel ("BumpB Velocity", Vector) = (0, 0, 0, 0)
        _BumpBTiling("BumpB tiling", Vector) = (1, 1, 0, 0)
        
        BumPhaseVel ("BumPhaseVel", Vector) = (0, 0, 0, 0)
        BumPhaseTile("BumPhaseTile", Vector) = (1, 1, 0, 0)
        _BumpPhaseMap ("Bump Noise (RGB)", 2D) = "bump" {}
        
        _BumpMixVel ("Bump Mix Velocity", Float) = 1
        _BumpMixMin ("Bump Mix Min", Float) = 0.2
        _BumpMixMax ("Bump Mix Max", Float) = 0.8
        
        _BumpAttenuanceByTransPow ("_BumpAttenuanceByTransPow", Float) = 1
        
        _WaveSimulation ("Wave Simulation", Float) = 0

        _WaveSpeed ("Wave Speed Scale", Float) = 100
        _WaveScale ("Wave Height Scale", Float) = 8
        _WaterHeight ("Water Height", Float) = 1
        
        _WaveDistance ("Wave Distance", Float) = 1
        _WaveOffset ("Wave Offset", Vector) = (0, 0, 0, 0)
    }
    
    SubShader {
        LOD 1
        Pass {
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma exclude_renderers xbox360
            #pragma vertex vert
            #pragma target 3.0
            #pragma glsl
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "ShaderFilters.cginc"
            
            #define PI 3.1415

            struct v2f 
            {
                float4 pos : SV_POSITION;
                float4  ssPos : TEXCOORD0;
                float4  bumpTexCoord : TEXCOORD1;
                float3  viewDir : TEXCOORD2;
                float3  objSpaceNormal : TEXCOORD3;
                float3  lightDir : TEXCOORD4;
                float2  uv_WaterMap : TEXCOORD6;
                float4  refl : TEXCOORD7;
            };
            
            float _specularWidth;
            float _specularVel;
            half _OpacityFactor = 1.0;
            
            float _BumpMixMin = 0.2;
            float _BumpMixMax = 0.2;

            float4 _SunDir;
            float _WaveSpeed = 100.0f;
            float _WaveScale = 8.0f;
            float4 _WaveOffset;
            
            half4 _BumpVel;
            half4 _BumpBVel;
            
            half4 _FoamColor;
            float _FoamPow = 1.0;
            half _FoamDistortion = 0.6;
            
            sampler2D _WaveMask;
            float4 _WaveMask_ST;
            
            sampler2D _Fresnel;
            
            samplerCUBE _EnvironmentMap;
            half _EnvironmentBias;
            float _RefractionBlend;
            half _EnvironmentDistortion;
            float4 _WaveDirection;
            
            float _SpecularSize = 250.0f;
            float _SpecularIntensity = 1.0f;
            half _WaveDistance = 1.0f;
            half _SpecDistortion = 0.1;
            
            half _WaterHeight = 0.0f;
            half4 _SpecularColor;
            
            float4 _BumpBTiling;
            float4 _BumpTiling;
            
            half4 _SurfaceColor;
            half4 _UnderWaterColor;
            half  _FresnelPow;
            half  _FresnelIntensity;
            half  _FresnelBias;
            float4 _FresnelColor;
            float4 _ReflectionColor;
            
            half _RefractionDistortion = 0.0f;
            
            sampler2D _Bump;
            sampler2D _BumpB;
            sampler2D _BumpPhaseMap;
            
            half4 BumPhaseVel;
            half4 BumPhaseTile;
            
            sampler2D _WaveTexture;
            sampler2D _Refraction;
            sampler2D _Reflection;

        	sampler2D _WaterColor;
            half4 _WaterVel;
            half4 _WaterColorTiling;
            half _WaterColorVisibily = 0;
            
            float _BumpMixVel = 1.0;
            half _WaterDistorsion;
            
            half coastTransparencySpeed;
            half coastTransparencyMin;
            half coastTransparencyMax;
            
            half _BumpAttenuanceByTransPow;
            
            half4 _WaveVelocity;
            half4 _WaveTiling;
            
            float _fresnelViewBias;
            
            float random(float2 co){
    			return frac(sin(dot(co.xy ,float2(12.9898,78.233))) * 43758.5453);
			}

            v2f vert (appdata_tan v)
            {
                v2f o;
                
    //#if defined(SHADER_API_OPENGL)
                // Move watter
                TRANSFORM_TEX(v.texcoord, _WaveMask);
                float2 waveMaskTransformed =  v.texcoord.xy * _WaveMask_ST.xy + _WaveMask_ST.zw;
                float4 WaveMaskColor = tex2Dlod (_WaveMask, float4(waveMaskTransformed.xy,0,0));
                
                float waveHeight = (_WaterHeight + WaveMaskColor.r * _WaveScale) / 100.0f;
                float waveSpeed = _WaveSpeed;
                
                float3 vertexPos = v.vertex.xyz;
                float wavePhase = WaveMaskColor.g * PI;
                
                float4 bumpPhase = tex2Dlod (_BumpPhaseMap, float4(v.texcoord.xy*BumPhaseTile.xy + BumPhaseVel.xy,0,0)).r;
                float bumpPhaseFactor = (bumpPhase.x + bumpPhase.y) / 2.0f;
                float deltaYNoise = sin(_Time.y + bumpPhaseFactor * 2 * PI);
                
                
                
                //vertexPos.y += sin(_Time.y * waveSpeed + wavePhase + deltaYNoise) * waveHeight;
                vertexPos.y += sin(_Time.y * waveSpeed + v.vertex.x) * waveHeight;
                
                o.refl.w = sin(_Time.y * _specularVel + v.vertex.x * _specularWidth);
                //vertexPos.y += sin(_Time.y + v.vertex.x);
                
                o.pos = mul (UNITY_MATRIX_MVP, float4(vertexPos, 1));
    //#else
                //o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
    //#endif        
                float3 objSpaceViewDir = ObjSpaceViewDir(v.vertex);
                float3 binormal = cross( normalize(v.normal), normalize(v.tangent.xyz) );
                float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
    
                o.objSpaceNormal = v.normal;
                o.viewDir = mul(rotation, objSpaceViewDir);
                o.lightDir = mul(rotation, float3(_SunDir.xyz)); // from local to tangent space
                
                o.uv_WaterMap = MultiplyUV (UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
                
                //o.bumpTexCoord.xy = v.vertex.xz/float2(1, 1);
                o.bumpTexCoord.xy = v.texcoord.xy * float2(1,1);
                o.bumpTexCoord.zw = v.texcoord.xy;
                
                // Environment Texture
                float3 worldPos = mul (_Object2World, v.vertex).xyz;
                float3 dirToCamera = normalize(_WorldSpaceCameraPos.xyz - worldPos);
                float3 refl = normalize(-dirToCamera + (2*dot(dirToCamera, v.normal) * v.normal));
				o.refl.xyz = refl;
				
				// _Refraction
				o.ssPos.xyzw = o.pos;
				
                return o;
            }
            
            half4 frag (v2f i) : COLOR
            {
                fixed4 waterMapValue = tex2D (_WaveMask, i.uv_WaterMap);
                float shine = 1-waterMapValue.r;
                float coastTransparency = 0;//waterMapValue.r;
                
                float transparencyAtt = 1-waterMapValue.b;
                float fresnelAttenuation = pow(transparencyAtt, _BumpAttenuanceByTransPow);
                transparencyAtt = 1-pow(1-transparencyAtt, _OpacityFactor);
                
                // Tangent Normal
                half4 buv = i.bumpTexCoord.xyxy;
                buv.xy = buv.xy / (_BumpTiling.xy/100.0f) + _Time.y * _BumpVel.xy;
                buv.zw = buv.zw / (_BumpBTiling.xy/100.0f) + _Time.y * _BumpBVel.xy;;
                
                float3 tangentNormal0 = (tex2D(_Bump, buv.xy).rgb * 2.0) - 1;
                float3 tangentNormal1 = (tex2D(_BumpB, buv.zw).rgb * 2.0) - 1;
                tangentNormal0 = normalize(tangentNormal0);
                tangentNormal1 = normalize(tangentNormal1);
                
                float bumpMixmin = _BumpMixMin;
                float bumpMixmax = _BumpMixMax;
                float bumpMixRange = bumpMixmax - bumpMixmin;
                
                float4 bumpPhase = tex2D (_BumpPhaseMap, i.bumpTexCoord.xy*BumPhaseTile.xy + BumPhaseVel.xy).r;
                float bumpPhaseFactor = (bumpPhase.x + bumpPhase.y) / 2.0f;
                float bumpMix = bumpMixmin + sin(_Time.y * _BumpMixVel + bumpPhaseFactor * 2 * PI) * bumpMixRange;
                
                float3 tangentNormal = normalize(lerp(tangentNormal0, tangentNormal1, bumpMix));
                half3 normViewDir = normalize(i.viewDir);
                
                // apply coas transparency
                float extraTransparencyRange = coastTransparencyMax - coastTransparencyMin;
                float extraTransparency = coastTransparencyMin + extraTransparencyRange* ( sin(_Time.y * coastTransparencySpeed  + bumpPhase.r));
                extraTransparency *= coastTransparency;
                
                extraTransparency = 1-extraTransparency;
                transparencyAtt *= extraTransparency;
                
                // fresnel
                float fresnelLookup = max(dot(tangentNormal.xyz, normalize(normViewDir.xyz + float3(0,0, _fresnelViewBias))), 0.0f);
                
                _FresnelBias /= 10;
                float fresnelTerm = _FresnelBias + (1.0-_FresnelBias)*pow(1.0 - fresnelLookup, (_FresnelPow/10.0f));
                
                // specular
                float3 specTangentNormal = normalize(float3(0,0,1) + tangentNormal * _SpecDistortion/10.0f);
                float3 halfVec = normalize(normViewDir - normalize(i.lightDir));
                float specular = pow(max(dot(halfVec, specTangentNormal.xyz), 0.0), _SpecularSize) * _SpecularIntensity/10.0f;
                
                float2 ssPos = i.ssPos.xy/i.ssPos.w;
                ssPos = (ssPos.xy +1) / 2;
                
                float3 envVector = i.refl + tangentNormal * _EnvironmentDistortion;
                float3 cube = texCUBE (_EnvironmentMap, envVector).rgb;
                
                float3 cubemap = texCUBE (_EnvironmentMap, envVector).rgb;
                
                half4 reflection = tex2D(_Reflection, ssPos.xy + tangentNormal * _EnvironmentDistortion );
                if(abs(reflection.a - 0.9) < 0.001)
                {
                	reflection = half4(1,0,0,1);
                }
                
                float reflectionLum = (reflection.r + reflection.g + reflection.g) / 3.0f;
                
                float3  environment = reflection.rgb;
                environment.rgb = alphaBlend(float4(_ReflectionColor.rgb, _RefractionBlend), float4(environment.rgb,1));

                // Refraction texture
                float2 RefractionUV = tangentNormal * _RefractionDistortion;
				
                float4 ssColor = tex2D(_Refraction, ssPos.xy + RefractionUV);
                
                float4 result = float4(0, 0, 0, 1);

                result.rgb = lerp( ssColor.rgb, environment.rgb * _UnderWaterColor, fresnelTerm );

                result.rgb += specular * _SpecularColor.rgb * (i.refl.w + 1)/2;

                
                result.a = 1;//extraTransparency;
                
                return result;
            }
            ENDCG
        }
    }
}
