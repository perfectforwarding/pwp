
precision mediump float;

varying vec4 vPosition;
varying vec3 vColor;

void main(void) 
{
	//float depth = (vPosition.z / vPosition.w);
	//float depth = gl_FragCoord.z;
	
	float depth = vPosition.z;
	//depth = depth / 100.0;

	//gl_FragColor = vec4(depth, depth, depth, 1);
	gl_FragColor = vec4(vColor, depth);
}