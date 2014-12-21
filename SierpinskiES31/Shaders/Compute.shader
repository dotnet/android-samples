#version 310 es

struct vertexData
{
	vec4 position;
	vec4 normal;
	vec3 baycentric;
};

layout (binding = 0) buffer verticesBuffer
{
	vertexData[] data;
} vertices;

struct triangleData
{
	uint a;
	uint b;
	uint c;
};

layout (binding = 1) buffer triangleBuffer
{
	triangleData[] triangle;
} triangles;

layout (local_size_x = 1, local_size_y = 1, local_size_z = 1) in;

vec4[4] offsets = {
	vec4 (0, 0, 0, 0),
	vec4 (1.0, 0, 0, 0),
	vec4 (0.5, sqrt (3.0) / 2.0, 0, 0),
	vec4 (0.5, 0.5 / sqrt (3.0), 2.0 / sqrt (6.0), 0)
};

vec4[4] points = {
	vec4 (-0.5, -0.5 / sqrt (3.0), -0.5 / sqrt (6.0), 1),
	vec4 (0.5, -0.5 / sqrt (3.0), -0.5 / sqrt (6.0), 1),
	vec4 (0, 1.0 / sqrt (3.0), -0.5 / sqrt (6.0), 1),
	vec4 (0, 0, 1.5 / sqrt (6.0), 1)
};

void tetrahedron (uint idx, uint tidx, vec4 off)
{
	vertices.data [idx].position = off + points[0];
	vertices.data [idx + 1u].position = off + points[1];
	vertices.data [idx + 2u].position = off + points[2];
	vertices.data [idx + 3u].position = off + points[3];

	vertices.data [idx + 4u].position = vertices.data [idx].position;
	vertices.data [idx + 5u].position = vertices.data [idx + 1u].position;
	vertices.data [idx + 6u].position = vertices.data [idx + 2u].position;
	vertices.data [idx + 7u].position = vertices.data [idx + 3u].position;

	vertices.data [idx + 8u].position = vertices.data [idx].position;
	vertices.data [idx + 9u].position = vertices.data [idx + 1u].position;
	vertices.data [idx + 10u].position = vertices.data [idx + 2u].position;
	vertices.data [idx + 11u].position = vertices.data [idx + 3u].position;

	triangles.triangle [tidx].a = 0u + idx;
	triangles.triangle [tidx].b = 2u + idx;
	triangles.triangle [tidx].c = 1u + idx;
	vertices.data [idx].normal = vec4 (0, 0, -1, 1);
	vertices.data [idx + 1u].normal = vec4 (0, 0, -1, 1);
	vertices.data [idx + 2u].normal = vec4 (0, 0, -1, 1);
	vertices.data [idx].baycentric = vec3 (1, 0, 0);
	vertices.data [idx + 1u].baycentric = vec3 (0, 1, 0);
	vertices.data [idx + 2u].baycentric = vec3 (0, 0, 1);

	triangles.triangle [tidx + 1u].a = 4u + idx;
	triangles.triangle [tidx + 1u].b = 5u + idx;
	triangles.triangle [tidx + 1u].c = 3u + idx;
	vertices.data [idx + 4u].normal = vec4 (0, -2.0 / sqrt (6.0), 0.5 / sqrt (3.0), 1);
	vertices.data [idx + 5u].normal = vertices.data [idx + 4u].normal;
	vertices.data [idx + 3u].normal = vertices.data [idx + 4u].normal;
	vertices.data [idx + 4u].baycentric = vec3 (1, 0, 0);
	vertices.data [idx + 5u].baycentric = vec3 (0, 1, 0);
	vertices.data [idx + 3u].baycentric = vec3 (0, 0, 1);

	triangles.triangle [tidx + 2u].a = 8u + idx;
	triangles.triangle [tidx + 2u].b = 7u + idx;
	triangles.triangle [tidx + 2u].c = 6u + idx;
	vertices.data [idx + 8u].normal = vec4 (-3.0 / (sqrt (3.0) * sqrt (6.0)), 1.0 / sqrt (6.0), 0.5 / sqrt (3.0), 1);
	vertices.data [idx + 6u].normal = vertices.data [idx + 8u].normal;
	vertices.data [idx + 7u].normal = vertices.data [idx + 8u].normal;
	vertices.data [idx + 8u].baycentric = vec3 (1, 0, 0);
	vertices.data [idx + 6u].baycentric = vec3 (0, 1, 0);
	vertices.data [idx + 7u].baycentric = vec3 (0, 0, 1);

	triangles.triangle [tidx + 3u].a = 9u + idx;
	triangles.triangle [tidx + 3u].b = 10u + idx;
	triangles.triangle [tidx + 3u].c = 11u + idx;
	vertices.data [idx + 9u].normal = vec4 (3.0 / (sqrt (3.0) * sqrt (6.0)), 1.0 / sqrt (6.0), 0.5 / sqrt (3.0), 1);
	vertices.data [idx + 10u].normal = vertices.data [idx + 9u].normal;
	vertices.data [idx + 11u].normal = vertices.data [idx + 9u].normal;
	vertices.data [idx + 9u].baycentric = vec3 (1, 0, 0);
	vertices.data [idx + 10u].baycentric = vec3 (0, 1, 0);
	vertices.data [idx + 11u].baycentric = vec3 (0, 0, 1);
}

void main()
{
	uint id = uint (gl_GlobalInvocationID.x);
	uint cid = id;
	uint dist = 1u;
	vec4 offset = vec4 (0, 0, 0, 0);
	do {
		uint m = cid & 3u;
		offset += float (dist) * offsets [m];
		cid = cid >> 2u;
		dist = dist << 1u;
	} while (cid > 0u);

	tetrahedron (id * 12u, id * 4u, offset);
}
