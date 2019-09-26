#version 330

in vec3 Normal;
in vec3 Color;
in vec3 FragPos;

out vec4 outputColor;

uniform vec3 lightColor;
uniform vec3 lightPos;
uniform float ambientStrength;

void main()
{
	vec3 norm = normalize(Normal);
	vec3 lightDir = normalize(lightPos - FragPos);

	float diff = max(dot(norm, lightDir), 0.0);
	vec3 diffuse = diff * lightColor;
	
    vec3 ambient = ambientStrength * lightColor;
	vec3 result = (ambient + diffuse) * Color;
	outputColor = vec4(result, 1.0);
}