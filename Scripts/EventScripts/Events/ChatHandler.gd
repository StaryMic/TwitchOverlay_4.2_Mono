extends Node

signal AvatarCommand(Type : String,Message : String, Username : String)
signal SendChatMessage(Message : String)

var Chat = HTTPClient.new()

const AuthFile = "res://Auth.json"
const TwitchURL = "https://api.twitch.tv"
const MyUserID = "121720350"

var APIKeyFileToText = FileAccess.get_file_as_string(AuthFile)
var APIKeysToDict = JSON.parse_string(APIKeyFileToText)
var ReceivedSessionID
var AccessToken = APIKeysToDict.Access_Token
var Headers = ["Authorization: Bearer "+AccessToken,"Client-Id: hmhsbcxv5jd5qd6g7n4lktwlugns7z","Content-Type: application/json"]

@export var ChatSounds : Array[AudioStream]

func _ready():
	if Chat.connect_to_host(TwitchURL) == OK:
		print("We're connected to Twitch Chat!")
	else: print("Failed to connect to Twitch Chat...")
	Chat.blocking_mode_enabled = false

func _process(_delta):
	Chat.poll()
	if Chat.get_status() == Chat.STATUS_BODY and Chat.has_response():
		Chat.read_response_body_chunk()

func _on_event_handler_chatmessage(username : String, message : String, user_id : String, message_id : String):
	if user_id == MyUserID and message.contains("[BOT]:"): #Ignore anything coming back from the bot.
		print("Ignoring BOT reply")
		return
	ChatSound()
	print_rich("[color=green]",username,"[/color]")
	print_rich("[color=green]",message,"[/color]")
	match message.get_slice(" ",0):
		"!AvatarColor":
			AvatarCommand.emit("AvatarColor",message, username)
		
		"!Avatar":
			AvatarCommand.emit("Avatar",message,username)



func _on_send_chat_message(Message : String, _ReplyToMessageID : String):
	var BodyToInsert = {
		"broadcaster_id" : MyUserID,
		"sender_id": MyUserID,
		"message" : "",
		"reply_parent_message_id" : _ReplyToMessageID
	}
	BodyToInsert.message = "[BOT]: " + Message
	print(str(BodyToInsert))
	print(Message)
	await Chat.get_status() != Chat.STATUS_BODY
	if Chat.request(HTTPClient.METHOD_POST,"/helix/chat/messages",Headers,str(BodyToInsert)) == OK:
		print("Message sent successfully")
	else:
		print("Message failed to send. Current status: ",Chat.get_status())

func ChatSound():
	$ChatSound.stream = ChatSounds.pick_random()
	$ChatSound.play()
