
attribute vec3 aVertexPosition;
attribute vec4 aVertexExtra;

uniform mat4 uWMatrix;
uniform mat4 uMVMatrix;
uniform mat4 uViewInvMatrix;
uniform mat4 uPMatrix;
uniform vec2 uGrassSpacing;
uniform float uGrassWidth;
uniform float uGrassHeight;
uniform float uGrassHeightVariation;
uniform float uTime;
uniform float uSwayRange;
uniform float uFreqAnim;
uniform float uPeriodAnimX;
uniform float uPeriodAnimZ;
uniform vec3 uDarkColor;
uniform vec3 uBrightColor;
uniform vec3 uPatchRandom;

varying vec4 vPosition;
varying vec3 vColor;



#define MOD2 vec2(3.07965, 7.4235)

//--------------------------------------------------------------------------
// Noise functions...
float Hash( float p )
{
	vec2 p2 = fract(vec2(p) / MOD2);
    p2 += dot(p2.yx, p2.xy+19.19);
	return fract(p2.x * p2.y);
}

float Hash(vec2 p)
{
	p  = fract(p / MOD2);
    p += dot(p.xy, p.yx+19.19);
    return fract(p.x * p.y);
}

float Noise( in vec2 x )
{
    vec2 p = floor(x);
    vec2 f = fract(x);
    f = f*f*(3.0-2.0*f);
    float n = p.x + p.y*57.0;
    float res = mix(mix( Hash(n+  0.0), Hash(n+  1.0),f.x),
                    mix( Hash(n+ 57.0), Hash(n+ 58.0),f.x),f.y);
    
    return res;
}


//--------------------------------------------------------------------------
// Main function
void main(void) 
{
	// Get random numbers
	float r1 = mix(aVertexExtra.z, aVertexExtra.w, uPatchRandom.x);
	float r2 = mix(aVertexExtra.z, aVertexExtra.w, uPatchRandom.y);
	float r3 = mix(uPatchRandom.x, uPatchRandom.y, aVertexExtra.z);
	float r4 = mix(uPatchRandom.x, uPatchRandom.y, aVertexExtra.w);

	// Set grass width
	vec4 pt = vec4(aVertexPosition, 1.0);
	pt.x *= uGrassWidth;

	// Billboarding
	pt = uViewInvMatrix * pt;

	// Set in position
	pt.x += (uGrassSpacing.x * aVertexExtra.x) + ((r1 - 0.5) * uGrassSpacing.x);


	pt.z += (uGrassSpacing.y * aVertexExtra.y) + ((r2 - 0.5) * uGrassSpacing.y);
	pt.y *= uGrassHeight;
	pt.y += aVertexPosition.y * uGrassHeightVariation * (r1 - 0.5);

	pt.x += aVertexPosition.y * aVertexPosition.y * (pt.y / uGrassHeight) * (r4 - 0.5) * uGrassHeight / 3.0;				
	pt.z += aVertexPosition.y * aVertexPosition.y * (pt.y / uGrassHeight) * (r3 - 0.5) * uGrassHeight / 3.0;				

    vec4 wp = uWMatrix * pt;
	//pt.y += sin(wp.x * 0.1) + cos(wp.z * 0.2) * 2.0;		//****		
	float base = Noise(vec2(wp.x, wp.z) * 0.02) * 20.0;
	float base2 = Noise(vec2(wp.x + 0.5, wp.z + 0.5) * 0.02) * 20.0; 

	pt.y += base;
	float shadow = (base2 - base) * 1.0;

	float sway = aVertexPosition.y * aVertexPosition.y * uSwayRange;
	//sway *= sway;
	//sway = 0.0;

	float r2h = (r2 + 1.0) * 0.5;
	//float r2h = (r2 * 0.2) + 0.9;
	float timeX = uTime * uFreqAnim;// * r2h;
	float timeZ = uTime * uFreqAnim;// * r2h;

    //float r3h = (r3 + 1.0) * 0.5;
    float r3h = r3 * 2.0;
	float periodX = wp.x * uPeriodAnimX * r3h;
	float periodZ = wp.z * uPeriodAnimZ * r3h;

	float r4h = (r4 + 1.0) * 0.5;
	float sx = timeX + periodX;
	float sz = timeZ + periodZ;
	pt.x += (sin(sx) + (sin((r1 + sx) * 3.0) * 0.2 * r2)) * sway * r4h;
	pt.z += (sin(sz) + (sin((r2 + sz) * 3.0) * 0.2 * r3)) * sway * r4h;
	//pt.x += uTime;



    gl_Position = uPMatrix * uMVMatrix * pt;

    r1 = mod(r1 + wp.x * r2 + wp.z * r3, 2.0);
    r1 = abs(r1 - 1.0);

		//rand2 = mod(rand2 + wp.z * rand1Mult + wp.x * rand2Mult, 1.0);

    vColor.r = mix(uDarkColor.r, uBrightColor.r, r1);
    vColor.g = mix(uDarkColor.g, uBrightColor.g, r1);
    vColor.b = mix(uDarkColor.b, uBrightColor.b, r1);

    vColor += shadow;

    vPosition = gl_Position;
    vPosition.z /= 100.0;
}