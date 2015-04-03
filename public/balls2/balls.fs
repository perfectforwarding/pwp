
precision lowp float;

uniform vec3 uResolution;
uniform vec3 uLightDir;


const float PI = 3.14159265358979323846264;
const float ZNEAR = 0.0;
const float ZFAR = 25.0;
const float FOCAL_LENGTH = 1.0; // Distance between the eye and the image plane
const int STEPS = 64;
const float EPSILON = 0.001;

const vec3 CAM_EYE = vec3(0.0, 0.0, 0.0);
const vec3 CAM_FORWARD = vec3(0.0, 0.0, 1.0);
const vec3 CAM_RIGHT = vec3(1.0, 0.0, 0.0);
const vec3 CAM_UP = vec3(0.0, 1.0, 0.0);

const vec3 BALL_CENTER = vec3(0.0, 0.0, 5.0);
const float BALL_RADIUS = 1.3;

const vec3 BOX_CENTER = vec3(-5.0, 0.0, 5.0);
const vec3 BOX_HALF_EXTENTS = vec3(1.0, 1.0, 1.0);

float udSphere(vec3 p, vec3 sphere_center, float sphere_radius)
{
	return length(p - sphere_center) - sphere_radius;
}

float sdBox(vec3 p, vec3 box_center, vec3 box_half_extents)
{
	return length(max(abs(p - box_center) - box_half_extents, 0.0));	
}

float distScene(vec3 p)
{
	return min(
		udSphere(p, BALL_CENTER, BALL_RADIUS),
		sdBox(p, BOX_CENTER, BOX_HALF_EXTENTS)
	);
}

vec3 getNormal(vec3 p)
{
	float h = 0.0001;
	return normalize(vec3(
		distScene(p + vec3(h, 0, 0)) - distScene(p - vec3(h, 0, 0)),
		distScene(p + vec3(0, h, 0)) - distScene(p - vec3(0, h, 0)),
		distScene(p + vec3(0, 0, h)) - distScene(p - vec3(0, 0, h))));
}

void rayMarch(vec3 ro, vec3 rd, out int i, out float t)
{
	t = 0.0;
	for(int j = 0; j < STEPS; ++j)
	{
		vec3 p = ro + rd * t;
		float d = distScene(p);
		if(d < EPSILON || d > ZFAR)
		{
			i = j;
			break;
		}

		t += d;
	}
}

vec3 computeColor(vec3 ro, vec3 rd)
{
	vec3 color = vec3(0.1, 0.1, 0.2);

	float t;
	int i;	
	rayMarch(ro, rd, i, t);

	if(t < ZFAR)
	{
		vec3 p = ro + rd * t;
		vec3 normal = getNormal(p);

		vec3 H = normalize(uLightDir + rd);

		color = vec3(-dot(normal, uLightDir));
		color += pow(clamp(-dot(normal, H), 0.0, 1.0), 85.0);
	}

	return color;
}

void main(void)
{
	float aspectRatio = uResolution.x / uResolution.y;
	vec2 uv;
	uv.x = gl_FragCoord.x * 2.0 / uResolution.x - 1.0;
	uv.y = gl_FragCoord.y * 2.0 / uResolution.y - 1.0;

	vec3 ro = CAM_EYE;

	float fl = 1.0 / tan( 45.0 / PI / 2.0);
	//fl = FOCAL_LENGTH;

	vec3 rd = normalize(CAM_FORWARD * fl + CAM_RIGHT * uv.x * aspectRatio + CAM_UP * uv.y);

	vec3 color = computeColor(ro, rd);

	gl_FragColor = vec4(color, 1.0);
}
