#version 310 es

in highp vec4 vpos;
in highp vec2 textureCoordinate;
in highp vec3 vertexE;
in highp vec3 normalE;
in highp vec3 vbaycentric;

out lowp vec4 fragColor;

uniform highp vec3 light;
uniform lowp float level;

void main()
{
	highp vec3 L = normalize (light - vertexE);
    highp vec3 E = normalize (-vertexE);
    highp vec3 R = normalize (-reflect (L, normalE));

    lowp vec4 amb = vec4 (.2, .2, .2, 1.0);
    lowp vec4 diff = vec4 (.8, .8, .8, 1.0) * max (dot (normalE, L), 0.0);
    diff = clamp (diff, 0.0, 1.0);

    highp float fdist = distance(vpos.xyz, light);
    highp float falloff = 150f / (fdist * fdist);

    if (any (lessThan (vbaycentric, vec3 (0.005f * (level + 1f)))))
        falloff /= 2f;

    fragColor = falloff * (amb + diff);
}
