
precision highp float;

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
const float EPSILON = 0.0001;

const vec3 CAM_UP = vec3(0.0, 1.0, 0.0);

const vec3 BALL_CENTER = vec3(0.0, 0.0, 0.0);
const float BALL_RADIUS = 1.0;

const vec3 BOX_CENTER = vec3(-5.0, 0.0, 5.0);
const vec3 BOX_HALF_EXTENTS = vec3(1.0, 1.0, 1.0);

const vec4 PLANE_F = vec4(0.0, 1.0, 0.0, -1.0);

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
	vec2 d = vec2(sdPlane(p, PLANE_F), 0.0);
	d = selElem(vec2(sdBox(p, BOX_CENTER, BOX_HALF_EXTENTS), 1.0), d);
	d = selElem(vec2(udSphere(p, BALL_CENTER, BALL_RADIUS), 2.0), d);

	return d;
}

vec3 getNormal(vec3 p)
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

void march(vec3 ro, vec3 rd, float maxt, out int i, out float t, out float m)
{
	t = 0.0;
	m = -1.0;

	for(int j = 0; j < STEPS; ++j)
	{
		vec3 p = ro + rd * t;
		vec2 d = map(p);
		t += d.x;
		//float d = udSphere(ro, rd, BALL_CENTER, BALL_RADIUS);
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
	}
}

float shadow(vec3 p, vec3 lightDir)
{
	vec3 ro = p - lightDir * SFAR;
/*		
	float t, m;
	int i;
	march(ro, lightDir, 100.0, i, t, m);
	return t < (100.0 - 0.01) ? 0.0 : 1.0;
*/	

	float sf = 1.0;
	float t = 0.0;
	for(int i = 0; i < STEPS; ++i )
	{
		vec3 p = ro + lightDir * t;
		float h = map(p).x;
		t += h;
		//sf = min(sf, 8.0 * h / t);		
		sf = min(sf, (SFAR - t) / SFAR);		
		if(h < EPSILON || t >= SFAR)
		{
			break;
		}
	}

	return clamp(sf, 0.0, 1.0);
}

vec3 computeColor(vec3 ro, vec3 rd)
{
	vec3 color = vec3(0.1, 0.1, 0.2);

	float t, m;
	int i;	
	march(ro, rd, ZFAR, i, t, m);

	if(m >= 0.0)
	{
		// surface point
		vec3 p = ro + rd * t;
		// shadow
		float s = shadow(p, uLightDir);

		// Calc mat color
		float cx = sign(mod(p.x * 2.0, 2.0) - 1.0);
		float cy = sign(mod(p.y * 2.0, 2.0) - 1.0);
		float cz = sign(mod(p.z * 2.0, 2.0) - 1.0);
		color = vec3(cy * cx * cz);

		color = vec3(0.5, 0.2, cos(m)) * (sin(m * 0.5) + 0.3);


		// Lit
		vec3 normal = getNormal(p); // surface normal
		vec3 H = normalize(uLightDir + rd); // half vector

		color *= vec3(max(-dot(normal, uLightDir), 0.0)); // diffuse
		color += pow(clamp(-dot(normal, H), 0.0, 1.0), 100.0) * 2.0;

		color *= vec3(s);

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
