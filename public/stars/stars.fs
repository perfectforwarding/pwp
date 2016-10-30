
precision mediump float;

uniform vec3 uResolution;
uniform vec3 uLightDir;
uniform vec3 uCamEye;
uniform vec3 uCamFowward;
uniform vec3 uCamRight;
uniform vec3 uCamUp;
uniform float uVFov;

const float PI = 3.14159265358979323846264;
const float ZFAR = 1025.0;
const float SFAR = 300.0;
const float FOCAL_LENGTH = 1.0;
const int STEPS = 256;
const float EPSILON = 0.001;

const vec3 CAM_UP = vec3(0.0, 1.0, 0.0);

const vec3 BALL_CENTER = vec3(0.0, 0.0, 0.0);
const float BALL_RADIUS = 1.0;

const vec3 BOX_CENTER = vec3(3.0, 0.0, 0.0);
const vec3 BOX_HALF_EXTENTS = vec3(1.0, 1.0, 1.0);

float udSphere(vec3 p, vec3 sphere_center, float sphere_radius)
{
	return length(p - sphere_center) - sphere_radius;
}

float sdBox(vec3 p, vec3 box_center, vec3 box_half_extents)
{
	return length(max(abs(p - box_center) - box_half_extents, 0.0));	
}

float sdPlane(vec3 p, vec4 plane)
{
	return dot(p, plane.xyz) - plane.w;
}

vec2 selElem(vec2 p1, vec2 p2)
{
	return p1.x < p2.x ? p1 : p2;
}

vec2 map(vec3 p)
{
	vec2 d = vec2(sdBox(p, BOX_CENTER, BOX_HALF_EXTENTS), 1.0);
	d = selElem(vec2(udSphere(p, BALL_CENTER, BALL_RADIUS), 2.0), d);

	return d;
}

vec3 gnormal(vec3 p)
{
	float h = 0.0001;
	return normalize(vec3(
		map(p + vec3(h, 0, 0)).x - map(p - vec3(h, 0, 0)).x,
		map(p + vec3(0, h, 0)).x - map(p - vec3(0, h, 0)).x,
		map(p + vec3(0, 0, h)).x - map(p - vec3(0, 0, h)).x));
}

// cast a ray
// @param ro Right origin
// @param rd Right direction
// @out i number of iterations
// @out t distance from ro in rd direction to the mapped point
// @out m material idx 

void march(vec3 ro, vec3 rd, float mint, float maxt, out int i, out float t, out float m)
{
	t = mint;
	m = -1.0;

	for(int j = 0; j < STEPS; ++j)
	{
		vec3 p = ro + rd * t;
		vec2 d = map(p);
		
		if(d.x < EPSILON)
		{
			i = j;
			m = d.y;
			break;
		}
		else if(d.x > maxt)
		{
			i = j;
			break;
		}		

		t += max(d.x, EPSILON);
	}
}

float shadow(vec3 p, vec3 lightDir)
{
	vec3 ro = p;
	vec3 rd = -lightDir;
	
	float sf = 1.0;
	float t = EPSILON * 2.0;
	for(int i = 0; i < STEPS; ++i )
	{
		vec3 p = ro + rd * t;
		float h = map(p).x;

		sf = min(sf, 8.0 * h / t);				
		if(h < EPSILON || t >= SFAR)
		{
			break;
		}

		t += max(h, EPSILON);		
	}

	return clamp(sf, 0.0, 1.0);
}

vec3 checker_mat(vec3 p, float scale, vec3 color0, vec3 color1)
{
	p.xyz *= scale;
	
	float k = mod(floor(p.x) + floor(p.y) + floor(p.z), 2.0);
	return mix(color0, color1, k);
}

void mat_color(vec3 p, vec3 normal, float m, out vec3 color)
{
	color = vec3(0.1, 0.1, 0.2);

	if(m == 0.0)
	{
		color = vec3(0.5, 0.2, cos(m)) * (sin(m * 0.5) + 0.3);		
	}
	else if(m == 1.0)
	{
		color = checker_mat(p, 2.0, vec3(0.0), vec3(1.0));
	}
	else if(m == 2.0)
	{
		color = vec3(0.0);
	}
}

#define iterations 17
#define formuparam 0.53

#define volsteps 20
#define stepsize 0.1

#define tile   1.350
#define speed  0.010 

#define brightness 0.0015
#define darkmatter 0.300
#define distfading 0.730
#define saturation 0.850

vec3 stars(vec3 ro, vec3 rd)
{
	ro.xyz *= 0.0;

	//volumetric rendering
	float s = 0.1;
	float fade = 1.0;

	vec3 v = vec3(0.0);
	for (int r = 0; r<volsteps; r++) 
	{
		vec3 p = ro + s * rd * 0.5;
		p = abs(vec3(tile) - mod(p, vec3(tile * 2.0))); // tiling fold

		float pa = 0.0;
		float a = 0.0;
		for (int i = 0; i < iterations; ++i) 
		{ 
			p = abs(p) / dot(p, p) - formuparam; // the magic formula
			a += abs(length(p) - pa); // absolute sum of average change
			pa = length(p);
		}
		float dm = max(0.0, darkmatter - a * a * 0.001); //dark matter
		a *= a * a; // add contrast
		if (r > 6) fade *= 1.0 - dm; // dark matter, don't render near
		//v+=vec3(dm,dm*.5,0.);
		v += fade;
		v += vec3(s, s*s , s*s*s*s) * a * brightness * fade; // coloring based on distance
		fade *= distfading; // distance fading
		s += stepsize;
	}
	v=mix(vec3(length(v)), v, saturation); //color adjust
	
	return v.xyz * 0.01;
}

vec3 computeColor(vec3 ro, vec3 rd)
{
	vec3 color = vec3(0.1, 0.1, 0.2);

	float t, m;
	int i;	
	march(ro, rd, 0.0, ZFAR, i, t, m);

	if(m >= 0.0)
	{
		// surface point
		vec3 p = ro + rd * t;
		// shadow
		float s = 1.0;//shadow(p, uLightDir);
		// Catch normal
		vec3 normal = gnormal(p);

		// Calc mat color
		if(m == 2.0)
		{
			march(p, normal, 0.001, SFAR, i, t, m);
		}
		mat_color(p, normal, m, color);		

		// Lit
		vec3 H = normalize(uLightDir + rd); // half vector

		color *= vec3(max(-dot(normal, uLightDir), 0.1)); // diffuse
		color += pow(clamp(-dot(normal, H), 0.0, 1.0), 100.0) * 2.0;

		color *= vec3(s);
	}
	else
	{
		color = stars(ro, rd);		
	}

	return color;
}

void main(void)
{
	float aspectRatio = uResolution.x / uResolution.y;
	vec2 uv;
	uv.x = gl_FragCoord.x * 2.0 / uResolution.x - 1.0;
	uv.y = gl_FragCoord.y * 2.0 / uResolution.y - 1.0;

	// focal length
	float fl = 1.0 / tan(uVFov / 180.0 * PI / 2.0); // Distance between the eye and the image plane

	// ray
	vec3 ro = uCamEye;
	vec3 rd = normalize(uCamFowward * fl + uCamRight * uv.x * aspectRatio + uCamUp * uv.y);

	// calc color
	vec3 color = computeColor(ro, rd);

	// output color
	gl_FragColor = vec4(color, 1.0);	
}
