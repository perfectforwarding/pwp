
precision mediump float;

varying vec2 vTextureCoord;

uniform sampler2D uSampler;
uniform vec2 uResolution;
uniform float elapsedTime;

void main(void)
{
	vec2 px = vec2(1.0 / uResolution.x, 1.0 / uResolution.y);

	vec4 current_color = texture2D(uSampler, vTextureCoord);
	vec4 bottom_color = texture2D(uSampler, vec2(vTextureCoord.x, max(vTextureCoord.y - px.y, 0.0 + px.y)));	
	vec4 top_color = texture2D(uSampler, vec2(vTextureCoord.x, min(vTextureCoord.y + px.y, 1.0 - px.y)));	

	if(current_color.r < 0.5)
	{
		if(bottom_color.r > 0.5)
		{
			current_color = bottom_color;
		}
	}
	else
	{
		if(top_color.r < 0.5)
		{
			current_color = top_color;
		}
	}

	gl_FragColor = current_color;
	//gl_FragColor.r = vTextureCoord.x;
}