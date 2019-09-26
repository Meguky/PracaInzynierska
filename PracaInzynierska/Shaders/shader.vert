#version 330 core

in vec3 aPosition;

in vec3 aNormal;

in vec3 aColor;


out vec3 Color;
out vec3 Normal;
out vec3 FragPos;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;


void main(void)
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
	Color = aColor;
	Normal = aNormal;
	FragPos = vec3(model * vec4(aPosition, 1.0));
}