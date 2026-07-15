#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uSource;
uniform vec2 uDirection;
uniform vec2 uTexelSize;

void main()
{
    float weights[5] = float[](0.227027, 0.1945946, 0.1216216, 0.054054, 0.016216);
    vec3 result = texture(uSource, vUV).rgb * weights[0];

    for (int i = 1; i < 5; i++)
    {
        vec2 offset = uDirection * uTexelSize * float(i);
        result += texture(uSource, vUV + offset).rgb * weights[i];
        result += texture(uSource, vUV - offset).rgb * weights[i];
    }

    FragColor = vec4(result, 1.0);
}
