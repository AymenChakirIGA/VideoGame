shader_type canvas_item;

uniform float fade_start : hint_range(0.0, 1.0, 0.01) = 0.7;
uniform float fade_end   : hint_range(0.0, 1.0, 0.01) = 1.0;
uniform bool vertical_fade = true;
uniform bool fade_from_end = true;

void fragment() {
    vec4 base_color = COLOR; // Only the ColorRect's color

    float coord = vertical_fade ? UV.y : UV.x;
    if (fade_from_end) {
        coord = 1.0 - coord;
    }

    float fade = smoothstep(fade_start, fade_end, coord);
    base_color.a *= (1.0 - fade);

    COLOR = base_color;
}
