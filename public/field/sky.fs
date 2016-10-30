
precision mediump float;

varying vec2 vTextureCoord;

uniform vec3 uSunDir;
uniform vec3 uEyeDir;
uniform vec2 uResolution;

uniform vec2 uViewportScale;

void main(void)
{
	//vec2 xy = (2.0 * (gl_FragCoord.xy / uResolution.xy)) - 1.0;
	vec2 xy = gl_FragCoord.xy / uResolution.xy;
	//xy *= uViewportScale;

	vec3 skyColor = vec3(0.4,0.61,0.82);
	vec3 horizonColor = vec3(0.7,0.81,0.85);
	vec3 groundColor = vec3(0.2, 0.7, 0.2);				

	// Map -1 -> 1, bottom to top
	float x = (xy.y * 2.0) - 1.0;
	x += uEyeDir.y;

	//float topMix = pow(clamp(x, 0.0, 1.0), 0.5);
	float topMix = clamp(pow(x, 0.5), 0.0, 1.0);
	vec3 topColor = mix(horizonColor, skyColor, topMix);

	float bottomMix = pow(clamp(-x, 0.0, 1.0), 0.5);
	vec3 bottomColor = mix(horizonColor, groundColor, bottomMix);				

	// Map 0 -> 2
	x += 1.0;

	gl_FragColor = vec4(mix(bottomColor, topColor, clamp(floor(x), 0.0, 1.0)), 1.0);
}