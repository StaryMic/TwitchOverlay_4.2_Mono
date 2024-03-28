shader_type canvas_item;

uniform float scale: hint_range(0.0,1.0) = 1.0;
uniform float time_scale: hint_range(0.0,2.0) = 1.0;


void fragment()
{
    //set the uv to a proper size on the center of screen 
    vec2 uv = (FRAGCOORD.xy - 0.5 * (scale / SCREEN_PIXEL_SIZE.xy)) / min(scale / SCREEN_PIXEL_SIZE.x, scale / SCREEN_PIXEL_SIZE.y);
    // set rotation angle value
    float rot = radians(0.0);
    // this set rotation by time and 
    rot = TIME * time_scale;
    // algebra formula for rotation by matrix , https://en.wikipedia.org/wiki/Rotation_matrix
    mat2 m = mat2( vec2( cos(rot), -sin(rot) ), vec2( sin(rot), cos(rot) ) );
    // rotation of uv with matrix algebra formula where is set the rotation angle 
    uv = m* uv;
    // define float by function module from 1.0 and uv.x - uv.y
    float d = mod(uv.x - uv.y, 1.0);
    // color for fragColor is value of fload d by module function
    vec4 col = vec4(vec3(d), 1.0);
    COLOR = vec4(col);
}