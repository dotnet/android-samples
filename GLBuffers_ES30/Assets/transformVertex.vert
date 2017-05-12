#version 300 es

uniform cbObject
{
	 mat4 tW;
};

uniform cbCamera 
{
	 mat4 tV;
	 mat4 tP;
	 mat4 tVP;
};

layout (location = 0) in vec4 vertexPosition;

void main()
{
	mat4 wvp = tP * tV * tW;
	vec4 vpos = vertexPosition;
    gl_Position = wvp * vpos;
}
