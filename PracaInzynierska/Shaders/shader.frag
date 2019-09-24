#version 330

out vec4 outputColor;

uniform vec4 lightColor;
uniform vec4 terrainColor;
uniform float ambientStrength;

void main()
{
    vec4 ambient = ambientStrength * lightColor;
	outputColor = ambient * terrainColor;
}