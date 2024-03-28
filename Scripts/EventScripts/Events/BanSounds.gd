extends Node

var AudioPlayers : Array
# Called when the node enters the scene tree for the first time.
func _ready():
	var children = self.get_children()
	for i in children:
		if i is AudioStreamPlayer3D:
			AudioPlayers.append(i)

func _on_event_handler_ban():
	AudioPlayers.pick_random().play()
	await get_tree().create_timer(.5).timeout
	$Crowsounds/Crow.play()
