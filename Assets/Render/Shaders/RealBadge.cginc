#ifndef INC_REAL_BADGE_CGINC
#define INC_REAL_BADGE_CGINC

fixed4 GetBadgeColor(float2 p)
{
		fixed4 color =  fixed4(0.);
		float2 q = p;
		
		// foreground
		float d = length(q.xy)- 8.2;
		color = mix(fixed4(1.), color, step(0., d));
		
		float d2 = max(length(q.xy - float2(q.x, q.x))- 3.5, length(q.xy) - 8.);
		color = mix(fixed4(0., 0., 0.7, 1.), color, step(0., d2));
		d = min(d, d2);
		
		// main circle
		d2 = CirclePerimeter2D(q, 8., 0.4);
		// color + border
		color = mix(mix(fixed4(fixed3(0.), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.2, abs(d2))), color, step(0., d2));
		d = min(d, d2);

		// sub circles
		float2 q2 = q - float2(-0.5, 0.0);
		d2 = mix(CirclePerimeter2D(q2, 5.6, 0.4), 1., step(-3.0, q2.x));
		q2 = q - float3(0.5, 0.0, 0.);
		float d3 = mix(CirclePerimeter2D(q2, 5.6, 0.4), 1., step(-3.0, -q2.x));
		d2 = min(d2, d3);
		
		// m2
		q2 = q - float2(-1.75, 2.9);
		d3 = max(length(q2.xy - float2(q2.x, -q2.x))- 0.6, length(q2.xy) - 2.9);
		d2 = min(d2, d3);
		
		// c
		q2 = q - float2(0.5, 0.);
		d3 = mix(CirclePerimeter2D(q2, 2.5, 0.4), 1., step(1.8, -q2.x));
		d2 = min(d2, d3);
		
		// m1
		q2 = q - float2(1.75, 2.9);
		d3 = max(length(q2.xy - float2(q2.x, q2.x))- 0.6, length(q2.xy) - 2.9);
		d2 = min(d2, d3);
		
		// f
		q2 = q - float2(-0.5, 0.);
		d3 = min(min(Line2D(q2, 0.8, 0.5), Line2D(q2 - float2(1., -2.5), 0.5, 3.0)), Line2D(q2 - float2(0.,-1.2), 0.8, 0.5));
		d2 = min(d2, d3);
		// color + border
		color = mix(mix(fixed4(fixed3(0.), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.2, abs(d2))), color, step(0., d2));
		
		q2 = q - float2(0., 0.5);
		d3 = mix(CirclePerimeter2D(q2, 10.0, 1.6) + mix(0., sin(p.x*2.0)*sin(p.y)*0.2, step(10., q2.y)), 1., max(step(q2.y, -q2.x*1.5 + 2.5), step(q2.y, q2.x*1.5 + 2.5)));
		if(d3 < 0.)
		{
			color = mix(color, fixed4(0.8,0.,0., 1.), step(10., length(q.xy)));
			color = mix(fixed4(1., 0.84, 0., 1.), color, step(10., length(q.xy) + sin(p.x * 4.) * sin(p.y*5.) * 0.6 ));
			color = mix(fixed4(fixed3(0.), 1.), color, smoothstep(0., 0.4, abs(d3)));
		}
		d2 = min(d2, d3);
		
		// cross
		q2 = q - float2(0., 13.7);
		d3 = Line2D(q2, 0.35, 2.6);
		color = mix(mix(fixed4(fixed3(0.5), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.3, abs(d3))), color, step(0., d3));
		d2 = min(d2, d3);
		
		q2 = q - float2(0., 15.6);
		d3 = Line2D(q2, 1.0, 0.25);
		color = mix(mix(fixed4(fixed3(0.5), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.3, abs(d3))), color, step(0., d3));
		d2 = min(d2, d3);
		
		// top
		q2 = q - float2(3.0, 11.);
		d3 = mix(CirclePerimeter2D(q2, 2.0, 0.5), 1., step(q2.y, -q2.x*0.8 + 1.5)) + sin(p.x * 6.) * sin(p.y*6.) * 0.1;
		color = mix(mix(fixed4(fixed3(0.5), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.3, abs(d3))), color, step(0., d3));
		d2 = min(d2, d3);
		
		q2 = q - float2(-3.0, 11.);
		d3 = mix(CirclePerimeter2D(q2, 2.0, 0.5), 1., step(q2.y, q2.x*0.8 + 1.5)) + sin(p.x * 6.) * sin(p.y*6.) * 0.1;
		color = mix(mix(fixed4(fixed3(0.5), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.3, abs(d3))), color, step(0., d3));
		d2 = min(d2, d3);
		
		q2 = q - float2(0., 12.);
		d3 = mix(CirclePerimeter2D(q2, 2.5, 0.5), 1., step(q2.y, -1.)) + sin(p.x * 6.) * sin(p.y*6.) * 0.1;
		color = mix(mix(fixed4(fixed3(0.5), 1.), fixed4(1., 0.84, 0., 1.), smoothstep(0., 0.3, abs(d3))), color, step(0., d3));	
		
		return color;
}

#endif
