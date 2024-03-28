@tool
class_name Huescroll
extends RichTextEffect


# To use this effect:
# - Enable BBCode on a RichTextLabel.
# - Register this effect on the label.
# - Use [Huescroll param=2.0]hello[/Huescroll] in text.
var bbcode = "Huescroll"


func _process_custom_fx(char_fx):
	char_fx.color.a = 1
	char_fx.color.s = 1
	char_fx.color.v = 1
#	char_fx.color.h = sin(char_fx.elapsed_time + char_fx.range.x) + 1
	char_fx.color.h = wrap(char_fx.elapsed_time,0,1) + (char_fx.range.x * .1)
