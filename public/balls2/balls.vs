
attribute vec2 aVertexPosition;

void main(void)
{
	gl_Position = vec4(aVertexPosition, 0, 1);
}