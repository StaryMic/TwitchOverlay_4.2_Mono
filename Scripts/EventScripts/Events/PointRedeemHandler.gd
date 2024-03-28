extends Node
@onready var eventhandler = $".."
@onready var db = %Database
@onready var AudienceManager = %AudienceManager
func _on_event_handler_pointredeem(username, reward_title, user_input):
	match reward_title:
		"Bark":
			var EyeColor : String
			var BodyColor : String
			var Avatar : String
			
			var query = db.QueryDB("SELECT * FROM AvatarSettings WHERE Username is '{username}'".format({"username" : username.to_lower()}))
			
			Avatar = query[0].AvatarType
			BodyColor = query[0].BodyColor
			EyeColor = query[0].EyeColor
			
			AudienceManager.SummonAvatar.emit(Avatar,BodyColor,EyeColor,"Bark","")
		"Throw":
			var EyeColor : String
			var BodyColor : String
			var Avatar : String
			
			var query = db.QueryDB("SELECT * FROM AvatarSettings WHERE Username is '{username}'".format({"username" : username.to_lower()}))
			
			Avatar = query[0].AvatarType
			BodyColor = query[0].BodyColor
			EyeColor = query[0].EyeColor
			
			AudienceManager.SummonAvatar.emit(Avatar,BodyColor,EyeColor,"Throw","")
