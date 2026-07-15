#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uBase;
uniform sampler2D uBloom;
uniform float uIntensity;

void main()
{
    vec3 base = texture(uBase, vUV).rgb;
    vec3 bloom = texture(uBloom, vUV).rgb;
    FragColor = vec4(base + bloom * uIntensity, 1.0);
}
