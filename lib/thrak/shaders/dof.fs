
precision mediump float;

varying vec2 vTextureCoord;
uniform float uDofBegin;
uniform float uDofEnd;
uniform sampler2D uFullResTexture;
uniform sampler2D uLowResTexture;

void main(void)
{
	vec4 hiResColor = texture2D(uFullResTexture, vTextureCoord);
	vec4 loResColor = texture2D(uLowResTexture, vTextureCoord);
	float mixval = clamp((hiResColor.a * 100.0 - uDofBegin) / (uDofEnd - uDofBegin), 0.0, 1.0);

	gl_FragColor = vec4(mix(hiResColor.rgb, loResColor.rgb, mixval), 1.0);				
	//gl_FragColor = vec4(loResColor.r, loResColor.g, loResColor.b, 1.0);

}		