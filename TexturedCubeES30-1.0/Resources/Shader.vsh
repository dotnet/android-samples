#version 300 es

in vec4 position;
in vec4 normal;
in vec2 texcoord;

out vec2 textureCoordinate;
out vec3 vertexE;
out vec3 normalE;

uniform float translate;
uniform mat4 projection;
uniform mat4 view;
uniform mat4 normalMatrix;
uniform mat4 texProjection;
uniform int[24] texDepth;

void main()
{
	vec4 vpos = position;
	vertexE = (view * position).xyz;
	normalE = normalize((normalMatrix * normal).xyz);
    gl_Position = projection * vpos;

    textureCoordinate = texcoord;
}
