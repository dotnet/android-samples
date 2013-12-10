#version 300 es

in highp vec2 textureCoordinate;
in highp vec3 vertexE;
in highp vec3 normalE;

out lowp vec4 fragColor;

uniform lowp sampler2D texture;
uniform highp vec3 light;

void main()
{
	highp vec3 L = normalize (light - vertexE);
    highp vec3 E = normalize (-vertexE);
    highp vec3 R = normalize (-reflect (L, normalE));

    lowp vec4 amb = vec4 (.2, .2, .2, 1.0);
    lowp vec4 diff = vec4 (.8, .8, .8, 1.0) * max(dot(normalE,L), 0.0);
    diff = clamp (diff, 0.0, 1.0);

    fragColor = texture (texture, textureCoordinate) * (amb + diff);
}
