extends Node

# VARIABLES
var QuitTimer: float;
var IsTimerGoing: bool;

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
# If ESC is held then turn on QuitTimer
	if Input.is_action_just_pressed("ui_cancel"):
		IsTimerGoing = true;
	if Input.is_action_just_released("ui_cancel"):
		IsTimerGoing = false;
		QuitTimer = 0;
# If the timer has been running for 3 or more seconds, commit suicide.
	if IsTimerGoing == true:
		QuitTimer += delta;
		if QuitTimer >= 1:
			get_tree().quit()
