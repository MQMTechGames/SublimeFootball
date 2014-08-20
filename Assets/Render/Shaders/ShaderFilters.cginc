#ifndef INC_SHADER_FILTERS_CGINC
#define INC_SHADER_FILTERS_CGINC

float doFadeIn( float dt, float fade_in )
{
	return saturate( dt / fade_in );
}

float doFadeOut( float dt, float fade_out )
{
	return saturate( (1 - dt) / fade_out );
}

float smoothstep(float x, float min, float max )
{
    // Scale, bias and saturate x to 0..1 range
    x = saturate((x - min)/(max - min)); 
    // Evaluate polynomial
    return x*x*(3 - 2*x);
}

float smootherstep(float x, float min, float max )
{
	// Scale, and clamp x to 0..1 range
    x = clamp((x - min)/(max - min), 0.0, 1.0);
    // Evaluate polynomial
    return x*x*x*(x*(x*6 - 15) + 10);
}

// mod returns the value of x modulo y. This is computed as x - y * floor(x/y).
float mod( float x, float y )
{
	float res = x - y * floor( x/y );

	return res;
}

float3 colorOverlay( float3 Target , float3 Blend )
{
	return (Target > 0.5) * (1 - (1-2*(Target-0.5)) * (1-Blend)) + (Target <= 0.5) * ((2*Target) * Blend);
}

float3 lighten( float3 Target , float3 Blend )
{
	return max(Target,Blend);
}

float3 darken( float3 Target , float3 Blend )
{
	return min(Target,Blend);
}

float3 linearBurn( float3 Target , float3 Blend )
{
	return Target + Blend - 1;
}

float3 softLight( float3 Target , float3 Blend )
{
	return (Blend > 0.5) * (1 - (1-Target) * (1-(Blend-0.5))) + (Blend <= 0.5) * (Target * (Blend+0.5));
}

float3 hardLight( float3 Target , float3 Blend )
{
	return (Blend > 0.5) * (1 - (1-Target) * (1-2*(Blend-0.5))) + (Blend <= 0.5) * (Target * (2*Blend));
}

float3 vividLight( float3 Target , float3 Blend )
{
	return (Blend > 0.5) * (1 - (1-Target) / (2*(Blend-0.5))) + (Blend <= 0.5) * (Target / (1-2*Blend));
}

float3 linearLight( float3 Target , float3 Blend )
{
	return (Blend > 0.5) * (Target + 2*(Blend-0.5)) + (Blend <= 0.5) * (Target + 2*Blend - 1);
}

float3 colorBurn( float3 Target , float3 Blend )
{
	return Target + Blend - 1;
}

float3 exclusion( float3 Target , float3 Blend )
{
	return float3(0.5,0.5,0.5) - 2*(Target-0.5)*(Blend-0.5);
}
float3 pinLight( float3 Target , float3 Blend )
{
	return (Blend > 0.5) * (max(Target,2*(Blend-0.5))) + (Blend <= 0.5) * (min(Target,2*Blend));
}

float3 colorDodge( float3 Target , float3 Blend )
{
	return Target / (1-Blend);
}

/**
* Source is the incoming color, Target is the background color
*/
float4 alphaBlend( float4 Source, float4 Target )
{
	return (Source * Source.a) + Target * ( 1 - Source.a );
}

float3 alphaBlend( float3 Source, float3 Target, float a )
{
	return (Source * a) + Target * ( 1 - a );
}

float avg3( float3 input )
{
	return (input.r + input.g + input.b) / 3.;
}

float3 RGBToHSL(float3 color)
{
	float3 hsl; // init to 0 to avoid warnings ? (and reverse if + remove first part)
	
	float fmin = min(min(color.r, color.g), color.b);    //Min. value of RGB
	float fmax = max(max(color.r, color.g), color.b);    //Max. value of RGB
	float delta = fmax - fmin;             //Delta RGB value

	hsl.z = (fmax + fmin) / 2.0; // Luminance

	if (delta == 0.0)		//This is a gray, no chroma...
	{
		hsl.x = 0.0;	// Hue
		hsl.y = 0.0;	// Saturation
	}
	else                                    //Chromatic data...
	{
		if (hsl.z < 0.5)
			hsl.y = delta / (fmax + fmin); // Saturation
		else
			hsl.y = delta / (2.0 - fmax - fmin); // Saturation
		
		float deltaR = (((fmax - color.r) / 6.0) + (delta / 2.0)) / delta;
		float deltaG = (((fmax - color.g) / 6.0) + (delta / 2.0)) / delta;
		float deltaB = (((fmax - color.b) / 6.0) + (delta / 2.0)) / delta;

		if (color.r == fmax )
			hsl.x = deltaB - deltaG; // Hue
		else if (color.g == fmax)
			hsl.x = (1.0 / 3.0) + deltaR - deltaB; // Hue
		else if (color.b == fmax)
			hsl.x = (2.0 / 3.0) + deltaG - deltaR; // Hue

		if (hsl.x < 0.0)
			hsl.x += 1.0; // Hue
		else if (hsl.x > 1.0)
			hsl.x -= 1.0; // Hue
	}

	return hsl;
}

float HueToRGB(float f1, float f2, float hue)
{
	if (hue < 0.0)
		hue += 1.0;
	else if (hue > 1.0)
		hue -= 1.0;
	float res;
	if ((6.0 * hue) < 1.0)
		res = f1 + (f2 - f1) * 6.0 * hue;
	else if ((2.0 * hue) < 1.0)
		res = f2;
	else if ((3.0 * hue) < 2.0)
		res = f1 + (f2 - f1) * ((2.0 / 3.0) - hue) * 6.0;
	else
		res = f1;
	return res;
}

float3 HSLToRGB(float3 hsl)
{
	float3 rgb;
	
	if (hsl.y == 0.0)
		rgb = float3(hsl.z,hsl.z,hsl.z); // Luminance
	else
	{
		float f2;
		
		if (hsl.z < 0.5)
			f2 = hsl.z * (1.0 + hsl.y);
		else
			f2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);
			
		float f1 = 2.0 * hsl.z - f2;
		
		float temp = (1.0/3.0);
		rgb.r = HueToRGB(f1, f2, hsl.x + float3(temp,temp,temp));
		rgb.g = HueToRGB(f1, f2, hsl.x);
		rgb.b= HueToRGB(f1, f2, hsl.x - (temp,temp,temp));
	}
	
	return rgb;
}

// Hue Blend mode creates the result color by combining the luminance and saturation of the base color with the hue of the blend color.
float3 BlendHue(float3 base, float3 blend)
{
	float3 baseHSL = RGBToHSL(base);
	return HSLToRGB(float3(RGBToHSL(blend).r, baseHSL.g, baseHSL.b));
}

// Saturation Blend mode creates the result color by combining the luminance and hue of the base color with the saturation of the blend color.
float3 BlendSaturation(float3 base, float3 blend)
{
	float3 baseHSL = RGBToHSL(base);
	return HSLToRGB(float3(baseHSL.r, RGBToHSL(blend).g, baseHSL.b));
}

// Color Mode keeps the brightness of the base color and applies both the hue and saturation of the blend color.
float3 BlendColor(float3 base, float3 blend)
{
	float3 blendHSL = RGBToHSL(blend);
	return HSLToRGB(float3(blendHSL.r, blendHSL.g, RGBToHSL(base).b));
}

// Luminosity Blend mode creates the result color by combining the hue and saturation of the base color with the luminance of the blend color.
float3 BlendLuminosity(float3 base, float3 blend)
{
	float3 baseHSL = RGBToHSL(base);
	return HSLToRGB(float3(baseHSL.r, baseHSL.g, RGBToHSL(blend).b));
}

float calculateLum( float3 input )
{
	return ( abs(input.r) + abs(input.g) + abs(input.b) );
}

float smoothclamp( float x, float min, float max )
{
	return clamp( ( min + ( x / ( max - min ) ) ), min, max );
}

float contrast( float color, float ref, float power )
{
	color = color + (1-ref);
	color = pow( color, power );

	color = saturate( color - (1-ref) );

	return color;
}

float3 blendLinearColor( float3 base, float3 blend, float t )
{
	return base * ( 1-t ) + blend * t;
}

float4 SmoothCurve( float4 x ) {  
  return x * x *( 3.0 - 2.0 * x );  
}

float4 TriangleWave( float4 x ) {  
  return abs( frac( x + 0.5 ) * 2.0 - 1.0 );  
}

float4 SmoothTriangleWave( float4 x ) {  
  return SmoothCurve( TriangleWave( x ) );  
}

#endif
