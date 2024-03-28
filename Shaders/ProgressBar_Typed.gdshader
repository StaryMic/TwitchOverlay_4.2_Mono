shader_type canvas_item;
//https://docs.godotengine.org/en/stable/tutorials/shaders/your_first_shader/your_first_2d_shader.html#introduction
uniform vec4 PrimaryColor = vec4(0,0,0,1);
uniform vec4 SecondaryColor = vec4(1,1,1,1);
void fragment() {
	vec2 modified_uvs;
	vec4 final_color;
	if (UV.x >= 0.5){
		COLOR = PrimaryColor;
	}
	else{
		COLOR = SecondaryColor;
	}
	modified_uvs.x = sin(TIME);
	modified_uvs.y = sin(TIME+UV.x+UV.y);
	COLOR.xy=modified_uvs;
}