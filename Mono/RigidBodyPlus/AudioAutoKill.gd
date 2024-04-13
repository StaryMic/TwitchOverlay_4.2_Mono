extends AudioStreamPlayer3D


# Called when the node enters the scene tree for the first time.
func _ready():
	finished.connect(die)
func die():
	queue_free()

