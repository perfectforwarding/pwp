
precision mediump float;

uniform sampler2D uSampler;
uniform float rtw;	// Render target width

varying vec2 vTextureCoord;

float offset[3];
float weight[3];

void main(void)
{
	offset[0] = 0.0; offset[1] = 1.3846153846 / rtw; offset[2] = 3.2307692308 / rtw;
	weight[0] = 0.2270270270; weight[1] = 0.3162162162; weight[2] = 0.0702702703;

	vec3 tc = texture2D(uSampler, vTextureCoord).rgb * weight[0];
	for(int i = 1; i < 3; ++i)
	{
		tc += texture2D(uSampler, vTextureCoord + vec2(offset[i], 0.0)).rgb * weight[i];
		tc += texture2D(uSampler, vTextureCoord - vec2(offset[i], 0.0)).rgb * weight[i];
	}

	//gl_FragColor = texture2D(uSampler, vTextureCoord);
	gl_FragColor = vec4(tc, 1.0);
}