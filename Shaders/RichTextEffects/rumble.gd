@tool
class_name Rumble
extends RichTextEffect


# To use this effect:
# - Enable BBCode on a RichTextLabel.
# - Register this effect on the label.
# - Use [Rumble param=2.0]hello[/Rumble] in text.
var bbcode = "Rumble"


func _process_custom_fx(char_fx):
	char_fx.offset.x = randf_range(-2,2)
	char_fx.offset.y = randf_range(-2,2)
	return true
