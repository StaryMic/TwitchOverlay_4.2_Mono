extends Node
var TestDict = {
	"test1":[
		{
			"value":1
		},
		{
			"value":2
		},
		{
			"value":3
		}
	]
}
var CurrentIndex : int = 0

# Called when the node enters the scene tree for the first time.
func _ready():
	pass


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	$"../Index".text = str(CurrentIndex)
	$"../Output".text = str(TestDict.test1[CurrentIndex].value)

func _on_increment_index_pressed():
	CurrentIndex = CurrentIndex + 1
	print(CurrentIndex)
