@tool
class_name SmoothRumble
extends RichTextEffect


# To use this effect:
# - Enable BBCode on a RichTextLabel.
# - Register this effect on the label.
# - Use [SmoothRumble param=2.0]hello[/SmoothRumble] in text.
var bbcode = "SmoothRumble"

func _process_custom_fx(char_fx):
	char_fx.offset.x = sin(randf())*5
	char_fx.offset.y = sin(randf())*5
	return true
