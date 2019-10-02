#version 330 core

in vec3 aPosition;
in vec3 aNormal;

out VS_OUT {
    vec4 normal;
} vs_out;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main(void)
{
	gl_Position = vec4(aPosition, 1.0) * model * view * projection;
    mat4 normalMatrix = mat4(mat3(transpose(inverse(model))));
    vs_out.normal = normalize(vec4(aNormal, 0.0) * normalMatrix * view * projection);
}