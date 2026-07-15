#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uSource;
uniform vec2 uBlurDirection;
uniform int uSampleCount;

void main()
{
    vec3 color = vec3(0.0);
    float total = 0.0;
    int count = max(uSampleCount, 1);

    for (int i = 0; i < count; i++)
    {
        float t = (float(i) / float(max(count - 1, 1))) - 0.5;
        vec2 offset = uBlurDirection * t;
        color += texture(uSource, vUV + offset).rgb;
        total += 1.0;
    }

    FragColor = vec4(color / max(total, 0.0001), 1.0);
}
