#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uSource;
uniform float uThreshold;

void main()
{
    vec3 color = texture(uSource, vUV).rgb;
    float luminance = dot(color, vec3(0.2126, 0.7152, 0.0722));
    float contribution = max(luminance - uThreshold, 0.0) / max(luminance, 0.0001);
    FragColor = vec4(color * contribution, 1.0);
}
