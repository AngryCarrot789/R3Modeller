#version 330 core

uniform vec3 in_color;

out vec4 FragColor;

void main() {
   FragColor = vec4(in_color, 1.0f);
}