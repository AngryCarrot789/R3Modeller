#version 330
#define LIGHT vec3(0.36, 0.80, 0.48)

// Uniforms
uniform sampler2D tex;

// Inputs
in vec2 ex_uv;
in vec3 ex_normal;

void main(void) {
    float s = dot(ex_normal, LIGHT) * 0.25 + 0.75;
    gl_FragColor = vec4(texture(tex, ex_uv).rgb * s, 1.0);
}