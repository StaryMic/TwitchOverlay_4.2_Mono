extends Node

var Client_ID
var Client_Secret # OUR KEYS FOR ACCESSIN THE APP, YARR
var Access_Token

var WebsocketUrl = ""
var TwitchSockClient = WebSocketPeer.new()
var JsonParseOutput : Dictionary
var SubscriptionSessionID : String #SHOULD ONLY BE SET ON "session_welcome"
var apiKeysFile = "res://Auth.json"

var APIKeyFileToText = FileAccess.get_file_as_string(apiKeysFile)
var APIKeysToDict = JSON.parse_string(APIKeyFileToText)

var LocalDebug : bool #FOR DEBUGGING ON THE TWITCH CLI LOCAL WEBSOCKET.

signal send_sub_session_id(SubSessionID : String,Token : String)
signal send_notification_to_event_handler(JsonData : Dictionary)

# Called when the node enters the scene tree for the first time.
func _ready():
	Client_Secret = APIKeysToDict.Client_Secret
	Client_ID = APIKeysToDict.Client_ID
	Access_Token = APIKeysToDict.Access_Token
	LocalDebug = APIKeysToDict.WSDebug

	if LocalDebug == true:
		WebsocketUrl = "ws://127.0.0.1:8080/ws"
	else:
		WebsocketUrl = "wss://eventsub.wss.twitch.tv/ws"

	#CONNECT TIME
	if TwitchSockClient.connect_to_url(WebsocketUrl) != OK:
		print("Nope. Couldn't connect.")
		set_process(false)

#Checked every frame
func _process(_delta):
	TwitchSockClient.poll()
	#print(TwitchSockClient.get_ready_state())

	if TwitchSockClient.get_available_packet_count() > 0:
		var IncomingPacketData = TwitchSockClient.get_packet().get_string_from_ascii()
		if !IncomingPacketData.is_empty():
			ParsePacketFromJsonToDictionary(IncomingPacketData)
			CheckSocketMessageType(JsonParseOutput)

# Connect to a button later.
func ReconnectToTwitch():
		if TwitchSockClient.connect_to_url(WebsocketUrl) != OK:
			print("Nope. Couldn't connect.")
			set_process(false)

func ParsePacketFromJsonToDictionary(packet):
	if typeof(packet) != 4:
		# Fire if we send something that isn't a String.
		print("Input not Json. Skipping. Fuck you.")
	else:
		#Parse to JSON (Dictionary)
		JsonParseOutput = JSON.parse_string(packet)
		print(JsonParseOutput)
		var debugfile = FileAccess.open("res://WebsocketOutput.txt",FileAccess.WRITE)
		debugfile.store_string(str(JsonParseOutput))

func CheckSocketMessageType(packetJson : Dictionary):
	if packetJson.metadata.message_type == "session_welcome":
		print("We got a session_welcome message!")
		SubscriptionSessionID = packetJson.payload.session.id
		print("Our session ID is : ",SubscriptionSessionID)
		send_sub_session_id.emit(SubscriptionSessionID) #Send off the session ID once we receive it so we can make API calls.
	elif packetJson.metadata.message_type == "session_keepalive":
		print("Still Alive.")
	elif packetJson.metadata.message_type == "notification":
		send_notification_to_event_handler.emit(JsonParseOutput)
		print_rich(str("[color=purple]",JsonParseOutput,"[/color]"))
	elif packetJson.metadata.message_type == "session_reconnect":
		print("Twitch needs to reconnect. XFinity moment.")
	elif packetJson.metadata.message_type == "revokation":
		print("Revoking subscription to ",packetJson.metadata.subscription_type)
