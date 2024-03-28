extends Node

const SubDataJsonTemplate = []
const apiKeysFile = "res://Auth.json"

const TwitchURL = "https://api.twitch.tv"
const DebugURL = "http://localhost:8082"
const MyUserID = "121720350"
var CurrentURL

var APIKeyFileToText = FileAccess.get_file_as_string(apiKeysFile)
var APIKeysToDict = JSON.parse_string(APIKeyFileToText)

var httpClient = HTTPClient.new()
var ReceivedSessionID
var AccessToken = APIKeysToDict.Access_Token
var Headers = ["Authorization: Bearer "+AccessToken,"Client-Id: hmhsbcxv5jd5qd6g7n4lktwlugns7z","Content-Type: application/json"]

var HttpRequestQueue : Array
var ClientCurrentStatus
var ErrorCheck
var CurrentSubscriptions : Dictionary
const SubscriptionStorageTemplate : Dictionary = {
	"SubId" : "",
	"Type" : ""
}
var CurrentRequest
var SubRequestBody : Dictionary

func _on_websocket_client_send_sub_session_id(SubSessionID): #Our equivalent of _Ready
	print("Received session ID: ",SubSessionID)
	ReceivedSessionID = SubSessionID

#HOO BOY HERE I GO MAKING A BIG FUCKING ARRAY OF DICTIONARIES FOR EVERY SUB TYPE YAYYYYY
# https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#subscription-types
# The Type field determines what we do with the data passed in.
# Valid types are "subscribe", "kill", "query" (POST, DELETE, GET respectively)
var StarterSubRequests : Array = [
	{
		"Name": "channel.follow",
		"Version": 2,
		"Type": "subscribe"
	},
	{
		"Name": "channel.subscribe",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.subscription.gift",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.subscription.message",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.cheer",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.raid",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.channel_points_custom_reward_redemption.add",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.poll.begin",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.poll.progress",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.poll.end",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.prediction.begin",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.prediction.end",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.prediction.lock",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.prediction.progress",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.hype_train.begin",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.hype_train.progress",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.hype_train.end",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.goal.begin",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.goal.progress",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.goal.end",
		"Version": 1,
		"Type": "subscribe"
	},
	{
		"Name": "channel.ban",
		"Version": 1,
		"Type": "subscribe"
	},
		{
		"Name": "channel.chat.message",
		"Version": 1,
		"Type": "subscribe"
	}
		]
var QueryTest = [
	{
		"Type":"query"
	}]
var KillTest = [
	{
		"Type":"kill",
		"SubId":""
	}
]
var QueryTestType = ""

func _init():
	if APIKeysToDict.APIDebug == true:
		CurrentURL = DebugURL
	else:
		CurrentURL = TwitchURL
	
	
	#Change starter query if we are testing.
	match QueryTestType:
		"Query":
			HttpRequestQueue = QueryTest
		"KillTest":
			HttpRequestQueue = KillTest
		_:#If we have nothing or something that doesn't match.
			HttpRequestQueue = StarterSubRequests
	
	ErrorCheck = httpClient.connect_to_host(CurrentURL) #Connect before doin shit.
	assert(ErrorCheck == OK,"FAILED TO CONNECT")
	
	while httpClient.get_status() == httpClient.STATUS_CONNECTING or httpClient.get_status() == httpClient.STATUS_RESOLVING:
		httpClient.poll()
		print(str(httpClient.get_status()))
		print("Connecting...")
		OS.delay_msec(100)
	
	assert(httpClient.get_status() == HTTPClient.STATUS_CONNECTED) #Are we connected?
	

func _process(_delta):#Every frame, check status, if not busy then run next request in queue.
	#Some code stolen from https://docs.godotengine.org/en/stable/tutorials/networking/http_client_class.html
	var LastClientStatus
	ClientCurrentStatus = httpClient.get_status()
	if ClientCurrentStatus != LastClientStatus:
#		print(ClientCurrentStatus)
		LastClientStatus = ClientCurrentStatus

	if ClientCurrentStatus == HTTPClient.STATUS_CONNECTED: #If we are connected or not processing the queue
		ProcessQueue()

func Jsonificator(input):
	return JSON.parse_string(input)

func _on_reconnect_pressed():
	httpClient.close()
	HttpRequestQueue = StarterSubRequests
	httpClient.connect_to_host("https://api.twitch.tv")
	while httpClient.get_status() == HTTPClient.STATUS_CONNECTING or HTTPClient.STATUS_RESOLVING:
		httpClient.poll()

func ProcessQueue():

	if !HttpRequestQueue.is_empty() and httpClient.get_status() == HTTPClient.STATUS_CONNECTED and ReceivedSessionID != null:
		CurrentRequest = HttpRequestQueue[0]
		match CurrentRequest.Type:
			"query":
				const _QueryTemplate = {"Type":"query"}
				print("Sending Query for current subs.")
				httpClient.request(HTTPClient.METHOD_GET,"/helix/eventsub/subscriptions",Headers)
			"kill":
				const _KillTemplate = {"Type":"kill", "SubId":""}
				httpClient.request(HTTPClient.METHOD_DELETE,str("/helix/eventsub/subscriptions?id=",CurrentRequest.SubId),Headers)
			"subscribe":
				#Fill in Request Body
				SubRequestBody = {
					"type": CurrentRequest.Name,
					"version": CurrentRequest.Version,
					"condition": {
						"to_broadcaster_user_id": MyUserID,
						"broadcaster_user_id": MyUserID,
						"user_id": MyUserID,
						"moderator_user_id": MyUserID},
						"transport": {
							"method": "websocket",
							"session_id": ReceivedSessionID
							}
							}
				print("Sub Request Body looks like this: ",SubRequestBody)
				httpClient.request(httpClient.METHOD_POST,"/helix/eventsub/subscriptions",Headers,str(SubRequestBody))

	while httpClient.get_status() == HTTPClient.STATUS_REQUESTING:
		httpClient.poll()

	if httpClient.get_status() == HTTPClient.STATUS_BODY: #Make damn sure this gets ALL the data from the API
		var FullResponse = ""
		var JsonifyResponse

		while httpClient.get_status() == HTTPClient.STATUS_BODY:
			httpClient.poll()
			print(str(httpClient.get_response_body_length()))
			var ReceivedChunk = httpClient.read_response_body_chunk().get_string_from_ascii()
			print("Got a chunk!!")
			print_rich("[color=green]",ReceivedChunk,"[/color]")
			if ReceivedChunk.length() == 0:
				await get_tree().create_timer(.1).timeout
			else:
				FullResponse = FullResponse + ReceivedChunk
				var TestFile = FileAccess.open("res://FullResponseTest.txt", FileAccess.WRITE)
				TestFile.store_string(FullResponse) #Just to make sure we're getting the whole output since Godot's Print function has a limit.
				print_rich("[color=green]WE GOT THE WHOLE RESPONSE BABY!!![/color]")
				TestFile.close()
				JsonifyResponse = Jsonificator(FullResponse)

		match CurrentRequest.Type:
			"query": #NOTE! I'd HIGHLY recommend not to use this. The query function doesn't output and we should really be getting a sub list from the websocket client.
				if httpClient.get_response_code() == 200: #If we have successfully gotten the subs.
					print("This does nothing. Subs are processed when we successfully subscribe.")
				else:
					print("Couldn't get subs. Error code was: ",httpClient.get_response_code())
			"kill":
				if httpClient.get_response_code() == 204:
					print("Successfully killed subscription with id: ", CurrentRequest.SubId)
				else: 
					print("Failed to kill subscription. Error code was: ",httpClient.get_response_code())
			"subscribe":
				if httpClient.get_response_code() == 202:
					print("You successfully subbed to ",CurrentRequest.Name)
					print(str(CurrentSubscriptions))
					CurrentSubscriptions[JsonifyResponse.data[0].type] = JsonifyResponse.data[0].id
				else:
					print("You failed to sub to ", CurrentRequest.Name, ". Error code is: ",httpClient.get_response_code())
		
		print(httpClient.get_response_code())
		if !HttpRequestQueue.is_empty():
			HttpRequestQueue.remove_at(0) #remove the request we just processed.
#	await get_tree().create_timer(0.1).timeout
