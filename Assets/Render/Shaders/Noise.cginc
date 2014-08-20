#ifndef INC_NOISE_CGINC
#define INC_NOISE_CGINC

float Hash( float n )
{
	return frac(sin(n)*43758.5453);
}

float Hash(in float3 p)
{
    return frac(sin(dot(p,float3(37.1,61.7, 12.4)))*3758.5453123);
}

float Noise( in float3 p )
{
    float3 i = floor(p);
	float3 f = frac(p); 
	f *= f * (3.0-2.0*f);

    return lerp(
		lerp(lerp(Hash(i + float3(0.,0.,0.)), Hash(i + float3(1.,0.,0.)),f.x),
			lerp(Hash(i + float3(0.,1.,0.)), Hash(i + float3(1.,1.,0.)),f.x),
			f.y),
		lerp(lerp(Hash(i + float3(0.,0.,1.)), Hash(i + float3(1.,0.,1.)),f.x),
			lerp(Hash(i + float3(0.,1.,1.)), Hash(i + float3(1.,1.,1.)),f.x),
			f.y),
		f.z);
}

float Noise( in float2 x )
{
    float2 p = floor(x);
    float2 f = frac(x);
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*57.0;
    float res = lerp(lerp( Hash(n+  0.0), Hash(n+  1.0),f.x),
                    lerp( Hash(n+ 57.0), Hash(n+ 58.0),f.x),f.y);
    return res;
}

float FBM( float3 p)
{
	p *= .35;
    float f;
	
	f = 0.5000 * Noise(p); p = p * 3.02;
	f += 0.2500 * Noise(p); p = p * 3.03;
	f += 0.1250 * Noise(p); p = p * 3.01;
	f += 0.0625   * Noise(p); p =  p * 3.03;
	f += 0.03125  * Noise(p); p =  p * 3.02;
	//f += 0.015625 * Noise(p);
	
    return f;
}

float FBM( float2 p)
{
	p *= .35;
    float f;
	
	f = 0.5000 * Noise(p); p = p * 3.02;
	f += 0.2500 * Noise(p); p = p * 3.03;
	f += 0.1250 * Noise(p); p = p * 3.01;
	f += 0.0625   * Noise(p); p =  p * 3.03;
	f += 0.03125  * Noise(p); p =  p * 3.02;
	//f += 0.015625 * Noise(p);
	
    return f;
}
#endif
