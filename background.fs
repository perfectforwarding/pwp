
precision mediump float;

varying vec2 vTextureCoord;
uniform vec2 uResolution;

void main(void)
{
	vec2 xy = gl_FragCoord.xy - (uResolution.xy / 2.0);
	xy /= 500.0;

	vec3 col = 0.4 * vec3(0.4, 0.6, 0.7) * (1.0 - 0.4 * length(xy));

	float s = 160.0;
	col *= 1.0 - 0.2 * smoothstep(0.0, 1.0, sin(xy.x * s) * sin(xy.y * s));

	gl_FragColor = vec4(col.r, col.g, col.b, 1.0);
}