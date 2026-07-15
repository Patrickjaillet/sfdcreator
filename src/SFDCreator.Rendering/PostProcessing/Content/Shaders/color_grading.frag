#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uSource;
uniform vec3 uLift;
uniform vec3 uGamma;
uniform vec3 uGain;
uniform float uSaturation;
uniform float uContrast;

void main()
{
    vec3 color = texture(uSource, vUV).rgb;

    color = color * uGain + uLift;
    color = pow(max(color, vec3(0.0)), 1.0 / max(uGamma, vec3(0.0001)));

    float luminance = dot(color, vec3(0.2126, 0.7152, 0.0722));
    color = mix(vec3(luminance), color, uSaturation);
    color = (color - 0.5) * uContrast + 0.5;

    FragColor = vec4(clamp(color, 0.0, 1.0), 1.0);
}
