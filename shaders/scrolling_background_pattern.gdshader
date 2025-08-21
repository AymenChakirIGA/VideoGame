shader_type canvas_item;

uniform float speed = .4;
void vertex() {
	// Called for every vertex the material is visible on.
		UV.x += TIME * speed;
		UV.y += TIME *speed;
	}
