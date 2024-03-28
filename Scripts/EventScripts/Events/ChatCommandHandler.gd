extends Node

@onready var db = %Database
@onready var AudienceManager = %AudienceManager

func _on_chat_handler_avatar_command(Type : String, Message : String, Username : String):
	Message = Message.to_lower()
	Username = Username.to_lower()
	var format = {"Username" : Username}
	if Type == "AvatarColor":
		if Message.get_slice_count(" ") < 3:
			%ChatHandler.SendChatMessage.emit("@{Username}AvatarColor needs 2 hex codes for body and eye color. EXAMPLE: !AvatarColor FF0FF0 00a50d".format(format),"")
		if Message.get_slice_count(" ") == 3:
			var Bodycolor = Message.get_slice(" ",1)
			var Eyecolor = Message.get_slice(" ",2)
			if Bodycolor.is_valid_html_color() == false or Eyecolor.is_valid_html_color() == false:
				%ChatHandler.SendChatMessage.emit("@{Username} One of your hex codes is invalid. Double check your values and try again.".format(format),"")
			if Bodycolor.is_valid_html_color() == true and Eyecolor.is_valid_html_color() == true:
				if db.DoesUserExistInDB(Username):
					var UpdateData = {
						"BodyColor" : Bodycolor,
						"EyeColor" : Eyecolor
					}
					db.UpdateDataToDB("AvatarSettings", "Username = '{Username}'".format(format), UpdateData)
					%ChatHandler.SendChatMessage.emit("@{Username} Avatar colors successfully set.".format(format),"")
				else:
					var AddedData = {
						"Username" : Username,
						"BodyColor" : Bodycolor,
						"EyeColor" : Eyecolor
					}
					db.AddNewDataToDB("AvatarSettings", AddedData)
	if Type == "Avatar":
		var DesiredAvatar
		if Message.get_slice_count(" ") != 2:
			%ChatHandler.SendChatMessage.emit("@{Username} Valid avatar names: {AvatarList}".format({"Username" : Username, "AvatarList" : AudienceManager.ValidAvatars.keys()}),"")
			return
		if Message.get_slice_count(" ") == 2:
			DesiredAvatar = Message.get_slice(" ",1)
		if AudienceManager.ValidAvatars.has(DesiredAvatar):
			if db.DoesUserExistInDB(Username):
				var UpdateData = {
					"AvatarType" = DesiredAvatar
				}
				if db.UpdateDataToDB("AvatarSettings","username = '{username}'".format({"username" : Username}), UpdateData):
					%ChatHandler.SendChatMessage.emit("@{Username} Data changed successfully".format(format),"")
				else:
					%ChatHandler.SendChatMessage.emit("@{Username} Failed to store change. Try again in a moment.".format(format),"")
			else:
				var AddedData = {
						"Username" : Username,
						"AvatarType" : DesiredAvatar
					}
				if db.AddNewDataToDB("AvatarSettings", AddedData):
					%ChatHandler.SendChatMessage.emit("@{Username} Data changed successfully".format(format),"")
				else:
					%ChatHandler.SendChatMessage.emit("@{Username} Failed to store change. Try again in a moment.".format(format),"")
		if !AudienceManager.ValidAvatars.has(DesiredAvatar):
			%ChatHandler.SendChatMessage.emit("@{Username} Invalid avatar was specified.".format(format),"")
			return
