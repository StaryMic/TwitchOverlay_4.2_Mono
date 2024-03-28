@tool
class_name Rainbow
extends RichTextEffect


# To use this effect:
# - Enable BBCode on a RichTextLabel.
# - Register this effect on the label.
# - Use [Rainbow param=2.0]hello[/Rainbow] in text.
var bbcode = "Rainbow"


func _process_custom_fx(char_fx):
	var offset = char_fx.offset
	char_fx.offset.y = (sin(char_fx.elapsed_time * 15 + char_fx.range.x)) * 5
	
	char_fx.color.a = 1
	char_fx.color.s = 1
	char_fx.color.v = 1
	char_fx.color.h = wrap(char_fx.elapsed_time,0,1) + (char_fx.range.x * .1)
	return true
