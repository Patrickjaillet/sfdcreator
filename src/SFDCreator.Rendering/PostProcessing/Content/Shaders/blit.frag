#version 460 core

in vec2 vUV;
out vec4 FragColor;

uniform sampler2D uSource;

void main()
{
    FragColor = texture(uSource, vUV);
}
