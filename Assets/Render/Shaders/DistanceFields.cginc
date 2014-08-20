#ifndef INC_DISTANCE_FIELDS_CGINC
#define INC_DISTANCE_FIELDS_CGINC

float Line2D( float2 p, float width, float height )
{
	return max(abs(p.x) - width, abs(p.y) - height);
}

float Circle2D( float2 p, float r )
{
	return length(p) - r;
}

float CirclePerimeter2D( float2 p, float r, float thickness )
{
	return abs(length(p) - r) - thickness;
}

float DiagonalLine2D( float2 p, float size, float thickness )
{
	return max( abs(p.y - p.x) - thickness, length(p) - size );
}

#endif
