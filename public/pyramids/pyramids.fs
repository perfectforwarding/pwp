
precision mediump float;

uniform vec3 uResolution;
uniform vec3 uLightDir;
uniform vec3 uCamEye;
uniform vec3 uCamFowward;
uniform vec3 uCamRight;
uniform vec3 uCamUp;
uniform float uVFov;
uniform float elapsedTime;

const float PI = 3.14159265358979323846264;
const float ZFAR = 1025.0;
const float SFAR = 300.0;
const float FOCAL_LENGTH = 1.0;
const int STEPS = 256;
const float EPSILON = 0.0001;

const vec3 CAM_UP = vec3(0.0, 1.0, 0.0);

const vec3 BALL_CENTER = vec3(0.0, 0.0, 0.0);
const float BALL_RADIUS = 1.0;

const vec3 BOX_CENTER = vec3(3.0, 0.0, 0.0);
const vec3 BOX_HALF_EXTENTS = vec3(1.0, 1.0, 1.0);

mat3 rotMZ(float angle)
{
	return mat3( 	cos( angle ),	-sin( angle ),	0.0,
			    	sin( angle ),	cos( angle ),	0.0,
			    	0.0,			0.0,			1.0);	
}

mat3 rotMY(float angle)
{
	return mat3( 	cos( angle ),	0.0,			sin( angle ),
			    	0.0,			1.0,			0.0,
			    	-sin(angle),	0.0,			cos(angle));
}

float maxcomp(vec3 p)
{
  float m1 = max(p.x, p.y);
  return max(m1, p.z);
}

float udSphere(vec3 p, vec3 sphere_center, float sphere_radius)
{
	return length(p - sphere_center) - sphere_radius;
}

float udBox(vec3 p, vec3 box_center, vec3 box_half_extents)
{	
	vec3 h = abs(p - box_center);
	return length(max(h - box_half_extents, 0.0));	
}

float sdBox( vec3 p, vec3 b )
{
  vec3 d = abs(p) - b;
  return min(max(d.x,max(d.y,d.z)),0.0) +
         length(max(d,0.0));
}


float sdPlane(vec3 p, vec4 plane)
{
	return dot(p, plane.xyz) - plane.w;
}

float pyramid( vec3 p, float h, float yscale) 
{
	vec3 t;
	float k = 0.70710678118654752440084436210485;//cos = sin(PI / 4.0);
	
	t.x = p.x * k - p.z * k;
	t.y = p.y / yscale;
	t.z = p.x * k + p.z * k;

	t.y = abs(t.y);

	vec3 q = abs(t);
	return (q.x + q.y + q.z - h) / 3.0;
}

//
float cappedPyramid( vec3 p, float h, float yscale, float ps) 
{
	return max(-sdPlane(p, vec4(0.0, ps, 0.0, 0.0)), pyramid(p, h, yscale));
}

float generator(vec3 p, float h)
{
	float aurea = 1.618034;
	float pillar_width = h / 70.0;
	float pillar_length = pillar_width * aurea * 2.0;
	float pillar_offset = h / (aurea * 3.0);	

	//--- Base is a capped pyramid
	float cap = cappedPyramid(p, h, 1.0, 1.0);
	//float b1 = udBox(p, vec3(0.0, 0.0, 0.0), vec3(h / 5.0, h, h));
	float b1 = min(sdBox(p, vec3(pillar_width, h, h)), sdBox(p, vec3(h, h, pillar_width)));
	float base = max(-b1, cap);		

	//--- add pillars
	float pillars_x = min(
		sdBox(p + vec3(pillar_offset, -h, 0.0), vec3(pillar_length, h * 1.1, pillar_width)),
		sdBox(p + vec3(-pillar_offset, -h, 0.0), vec3(pillar_length, h * 1.1, pillar_width))
		);

	float pillars_y = min(
		sdBox(p + vec3(0.0, -h, pillar_offset), vec3(pillar_width, h * 1.1, pillar_length)),
		sdBox(p + vec3(0.0, -h, -pillar_offset), vec3(pillar_width, h * 1.1, pillar_length))
		);	

	float pillars = min(pillars_x, pillars_y);

	//--- inverted pyramid
	float top = cappedPyramid(p - vec3(0.0, h * 2.1, 0.0), h / 2.5, 2.0, -1.0);

	//--- pillars, base and top
	float gen = min(base, min(pillars, top));
	return gen;
}

float pyramidLine(vec3 p)
{
	vec3 c = vec3(10.0);
	vec3 h = mod(p, c) - 0.5 * c;
	h.y = p.y;
	h.x = p.x;

	return generator(h, 2.0);
}

float pyramidFloor(vec3 p)
{
	return sdBox(p + vec3(0.0, 10.0, 0.0), vec3(100.0, 10.0, 100.0));
}

vec2 selElem(vec2 p1, vec2 p2)
{
	return p1.x < p2.x ? p1 : p2;
}

vec2 map(vec3 p)
{
	vec2 d = vec2(udBox(p, BOX_CENTER, BOX_HALF_EXTENTS), 1.0);
	d = selElem(vec2(udSphere(p, BALL_CENTER, BALL_RADIUS), 2.0), d);
	d = selElem(vec2(pyramidLine(p), 3.0), d);
	d = selElem(vec2(pyramidFloor(p), 4.0), d);

	return d;
}

vec3 gnormal(vec3 p)
{
	float h = 0.001;

	return normalize(vec3(
		map(p + vec3(h, 0, 0)).x - map(p - vec3(h, 0, 0)).x,
		map(p + vec3(0, h, 0)).x - map(p - vec3(0, h, 0)).x,
		map(p + vec3(0, 0, h)).x - map(p - vec3(0, 0, h)).x));
/*
	vec3 e=vec3(0.01,-0.01,0.0);
	return normalize( vec3(	e.xyy*map(p+e.xyy).x +	e.yyx*map(p+e.yyx).x +	e.yxy*map(p+e.yxy).x +	e.xxx*map(p+e.xxx).x));		
*/	
}

// cast a ray
// @param ro Right origin
// @param rd Right direction
// @out i number of iterations
// @out t distance from ro in rd direction to the mapped point
// @out m material idx 

void march(vec3 ro, vec3 rd, float mint, float maxt, out int i, out float t, out float m)
{
	t = 0.0;
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
			m = -1.0;
			break;
		}		

		//t += max(d.x, EPSILON);
		t += d.x;
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

	color = vec3(0.5, 0.2, cos(m)) * (sin(m * 0.5) + 0.3);		
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

		color *= vec3(max(-dot(normal, uLightDir), 0.0)); // diffuse
		color += pow(clamp(-dot(normal, H), 0.0, 1.0), 100.0) * 2.0;

		color *= vec3(s);

		color = normal;

		//if(length(normal) > 1.0)
		//	color = vec3(1.0);
		//else if(length(color) > 1.0)
		//	color = vec3(0.1); 

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
