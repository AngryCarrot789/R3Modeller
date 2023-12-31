#version 330

precision mediump float;

// Uniforms
uniform mat4 mvp;
uniform mat4 mv;

// Inputs
in vec3 in_pos;
in vec2 in_uv;
in vec3 in_normal;

// Outputs
out vec2 ex_uv;
out vec3 ex_normal;

void main(void) {
    gl_Position = mvp * vec4(in_pos, 1.0);
    ex_uv = in_uv;
    ex_normal = normalize((mv * vec4(in_normal, 0.0)).xyz);
}