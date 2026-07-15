#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uSource;
uniform vec2 uResolution;
uniform float uScanlineStrength;
uniform float uVignetteStrength;
uniform float uCurvature;

vec2 warp(vec2 uv)
{
    vec2 centered = uv * 2.0 - 1.0;
    centered *= 1.0 + uCurvature * dot(centered, centered);
    return centered * 0.5 + 0.5;
}

void main()
{
    vec2 uv = warp(vUV);

    if (uv.x < 0.0 || uv.x > 1.0 || uv.y < 0.0 || uv.y > 1.0)
    {
        FragColor = vec4(0.0, 0.0, 0.0, 1.0);
        return;
    }

    vec3 color = texture(uSource, uv).rgb;

    float scanline = sin(uv.y * uResolution.y * 3.14159) * 0.5 + 0.5;
    color *= mix(1.0, scanline, uScanlineStrength);

    vec2 centered = uv - 0.5;
    float vignette = 1.0 - dot(centered, centered) * uVignetteStrength;
    color *= clamp(vignette, 0.0, 1.0);

    FragColor = vec4(color, 1.0);
}
