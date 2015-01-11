#version 310 es

in vec4 position;
in vec4 normal;
in vec3 baycentric;

out vec4 vpos;
out vec2 textureCoordinate;
out vec3 vertexE;
out vec3 normalE;
out vec3 vbaycentric;

uniform float translate;
uniform mat4 projection;
uniform mat4 view;
uniform mat4 normalMatrix;
uniform mat4 texProjection;

void main()
{
    vpos = view * position;
	vertexE = vpos.xyz;
	normalE = normalize ((normalMatrix * normal).xyz);
	vbaycentric = baycentric;
    gl_Position = projection * position;

    textureCoordinate = vec2(0, 0);
}
