extends Node

var QueueArray : Array
var ArrayCount : int = 0
var DictionaryDummy = {
	"var" = ArrayCount
}
var LargeDictionary = {
  "total": 2,
  "data": [
	{
	  "id": "26b1c993-bfcf-44d9-b876-379dacafe75a",
	  "status": "enabled",
	  "type": "stream.online",
	  "version": "1",
	  "condition": {
		"broadcaster_user_id": "1234"
	  },
	  "created_at": "2020-11-10T20:08:33.12345678Z",
	  "transport": {
		"method": "webhook",
		"callback": "https://this-is-a-callback.com"
	  },
	  "cost": 1
	},
	{
	  "id": "35016908-41ff-33ce-7879-61b8dfc2ee16",
	  "status": "webhook_callback_verification_pending",
	  "type": "user.update",
	  "version": "1",
	  "condition": {
		"user_id": "1234"
	  },
	  "created_at": "2020-11-10T14:32:18.730260295Z",
	  "transport": {
		"method": "webhook",
		"callback": "https://this-is-a-callback.com"
	  },
	  "cost": 0
	}
  ],
  "total_cost": 1,
  "max_total_cost": 10000,
  "pagination": {}
}
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	$"../../SplitContainer/Label".set("text",QueueArray)

func _on_add_button_pressed():
	QueueArray.append(DictionaryDummy)
	ArrayCount+=1

func _on_remove_button_pressed():
	QueueArray.remove_at(0)

func _on_loop_button_pressed():
	for i in LargeDictionary.data:
		var idStore = i.id
		print(idStore)
