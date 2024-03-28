extends Node2D
@export var Points : Array[Vector2]
var time : float = 0
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time = time + delta
	_quadratic_bezier(Points[0],Points[1],Points[2],time)

func _quadratic_bezier(p0: Vector2, p1: Vector2, p2: Vector2, t: float):
	print(t)
	var clamptime = clampf(t,0,1)
	var q0 = p0.lerp(p1, clamptime)
	var q1 = p1.lerp(p2, clamptime)
	var r = q0.lerp(q1, clamptime)
	$Icon.position = r

func _draw():
	for i in Points:
		draw_circle(i,15,"BLUE")
