extends Node
@onready var GlobalSceneSignalRef = $"../../GlobalSceneSignals"
@onready var QueueTimer = $NotificationTimer
signal follow(username)
signal subscribe(username,tier,is_gift)
signal giftsub(username,tier,total_gifts)
signal submessage(username,tier,message,cumulative_months,duration_months)
signal cheer(username,message,bits)
signal raid(username,viewers)
signal pointredeem(username,reward_title,user_input)
signal ban()
signal channelpoll(title,choices,poll_status) #Choices should be an array of dicts.
signal prediction(title,outcomes,notification_type,winning_id) #https://dev.twitch.tv/docs/eventsub/eventsub-subscription-types/#channelpredictionlock
signal hypetrain(total,_progress,goal,top_contributions,level,notification_type)
signal goal(type,description,current_amount,target_amount)
signal chatmessage(username,message,user_id,message_id)
#THIS SCRIPT HANDLES **INDIVIDUAL** EVENTS 
#Any other events with multiple parts like Channel.poll.begin/end/progress will have their own script to handle them.
#This script is the hub for all events, sending those that can't be handled here somewhere else.

#Redirects
#Channel.poll
#Channel.prediction
#Channel.hype_train
#Channel.goal

var NotificationQueue : Array = []

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

func _on_websocket_client_send_notification_to_event_handler(JsonData):
	var CurrentNotificationType = JsonData.metadata.subscription_type
	var ConstructedDictionary : Dictionary
	ConstructedDictionary.clear() #Clear every call.
	match CurrentNotificationType:
		"channel.chat.message":
			GlobalSceneSignalRef.emit_signal("ChatMessage",JsonData.payload.event.chatter_user_name,JsonData.payload.event.message.text,JsonData.payload.event.chatter_user_id,JsonData.payload.event.message_id)
			chatmessage.emit(JsonData.payload.event.chatter_user_name,JsonData.payload.event.message.text,JsonData.payload.event.chatter_user_id,JsonData.payload.event.message_id)
		"channel.follow":
			ConstructedDictionary.timer_delay = 5
			ConstructedDictionary.type = "follow"
			
			ConstructedDictionary.user_name = JsonData.payload.event.user_name
			
			if CheckForRepeatedEvent(ConstructedDictionary.user_name,ConstructedDictionary.type):
				# If this is a repeated event
				print("Repeated Event. Ignoring.")
				return # Do nothing
			
			if !CheckForRepeatedEvent(ConstructedDictionary.user_name,ConstructedDictionary.type):
				NotificationQueue.append(ConstructedDictionary)
				AddToRepeatList(ConstructedDictionary.user_name,ConstructedDictionary.type)
		"channel.subscribe":
			ConstructedDictionary.timer_delay = 5
			ConstructedDictionary.type = "subscribe"
			
			ConstructedDictionary.user_name = JsonData.payload.event.user_name
			ConstructedDictionary.tier = JsonData.payload.event.tier
			ConstructedDictionary.is_gift = JsonData.payload.event.is_gift
			
			NotificationQueue.append(ConstructedDictionary)
		"channel.subscription.gift":
			ConstructedDictionary.timer_delay = 5
			ConstructedDictionary.type = "giftsub"
			
			match JsonData.payload.event.is_anonymous:
				true:
					ConstructedDictionary.user_name = "Anonymous"
				false:
					ConstructedDictionary.user_name = JsonData.payload.event.user_name
					
			ConstructedDictionary.total = JsonData.payload.event.total
			ConstructedDictionary.tier = JsonData.payload.event.tier
			
			NotificationQueue.append(ConstructedDictionary)
		"channel.subscription.message":
			ConstructedDictionary.timer_delay = 5
			ConstructedDictionary.type = "submessage"
			
			ConstructedDictionary.user_name = JsonData.payload.event.user_name
			ConstructedDictionary.tier = JsonData.payload.event.tier
			ConstructedDictionary.message = JsonData.payload.event.message.text
			ConstructedDictionary.cumulative_months = JsonData.payload.event.cumulative_months
			ConstructedDictionary.duration_months = JsonData.payload.event.duration_months
			
			NotificationQueue.append(ConstructedDictionary)
		"channel.cheer":
			ConstructedDictionary.timer_delay = 5
			ConstructedDictionary.type = "cheer"
			
			match JsonData.payload.event.is_anonymous:
				true:
					ConstructedDictionary.user_name = "Anonymous"
				false:
					ConstructedDictionary.user_name = JsonData.payload.event.user_name
			ConstructedDictionary.message = JsonData.payload.event.message
			ConstructedDictionary.bits = JsonData.payload.event.bits
			NotificationQueue.append(ConstructedDictionary)
		"channel.raid":
			ConstructedDictionary.timer_delay = 5
			ConstructedDictionary.type = "raid"
			
			ConstructedDictionary.from_broadcaster_user_name = JsonData.payload.event.from_broadcaster_user_name
			ConstructedDictionary.viewers = JsonData.payload.event.viewers
			
			if CheckForRepeatedEvent(ConstructedDictionary.user_name,ConstructedDictionary.type):
				# If this is a repeated event
				print("Repeated Event. Ignoring.")
				return # Do nothing
			
			if !CheckForRepeatedEvent(ConstructedDictionary.from_broadcaster_user_name,ConstructedDictionary.type):
				NotificationQueue.append(ConstructedDictionary)
				AddToRepeatList(ConstructedDictionary.from_broadcaster_user_name,ConstructedDictionary.type)
			
		"channel.channel_points_custom_reward_redemption.add":
			pointredeem.emit(JsonData.payload.event.user_name,JsonData.payload.event.reward.title,JsonData.payload.event.user_input)

		"channel.ban":
			ban.emit() #SHOOT HIM

		"channel.poll.begin":
			channelpoll.emit(JsonData.payload.event.title,JsonData.payload.event.choices,"begin")
		
		"channel.poll.progress":
			channelpoll.emit(JsonData.payload.event.title,JsonData.payload.event.choices,"progress")
		
		"channel.poll.end":
			channelpoll.emit(JsonData.payload.event.title,JsonData.payload.event.choices,"end")
		
		"channel.prediction.begin":
			prediction.emit(JsonData.payload.event.title,JsonData.payload.event.outcomes,"begin",null)
		
		"channel.prediction.end":
			prediction.emit(JsonData.payload.event.title,JsonData.payload.event.outcomes,"end",JsonData.payload.event.winning_outcome_id)

		"channel.prediction.lock":
			prediction.emit(JsonData.payload.event.title,JsonData.payload.event.outcomes,"lock",null)

		"channel.prediction.progress":
			prediction.emit(JsonData.payload.event.title,JsonData.payload.event.outcomes,"progress",null)

		"channel.hype_train.begin":
			hypetrain.emit(JsonData.payload.event.total,JsonData.payload.event.progress,JsonData.payload.event.goal,JsonData.payload.event.top_contributions,JsonData.payload.event.level, "begin")

		"channel.hype_train.progress":
			hypetrain.emit(JsonData.payload.event.total,JsonData.payload.event.progress,JsonData.payload.event.goal,JsonData.payload.event.top_contributions,JsonData.payload.event.level, "progress")

		"channel.hype_train.end":
			hypetrain.emit(JsonData.payload.event.total,null,JsonData.payload.event.goal,JsonData.payload.event.top_contributions,JsonData.payload.event.level, "end")

		"channel.goal.begin":
			goal.emit(JsonData.payload.event.type,JsonData.payload.event.description,JsonData.payload.event.current_amount,JsonData.payload.event.target_amount)

		"channel.goal.progress":
			goal.emit(JsonData.payload.event.type,JsonData.payload.event.description,JsonData.payload.event.current_amount,JsonData.payload.event.target_amount)

		"channel.goal.end":
			goal.emit(JsonData.payload.event.type,JsonData.payload.event.description,JsonData.payload.event.current_amount,JsonData.payload.event.target_amount)

		_:
			print("Unhandled notification type in EventHandler.gd")
	print_rich("[color=green]",NotificationQueue,"[/color]")


func _on_timer_timeout(): #Queue system here.
	if NotificationQueue.is_empty():
		QueueTimer.wait_time = 1
	if !NotificationQueue.is_empty():
		var CurrentNotification = NotificationQueue[0]
		match CurrentNotification.type:
			"follow":
				QueueTimer.wait_time = CurrentNotification.timer_delay
				follow.emit(CurrentNotification.user_name)
			"subscribe":
				QueueTimer.wait_time = CurrentNotification.timer_delay
				subscribe.emit(CurrentNotification.user_name,CurrentNotification.tier,CurrentNotification.is_gift)
			"giftsub":
				QueueTimer.wait_time = CurrentNotification.timer_delay
				giftsub.emit(CurrentNotification.user_name,CurrentNotification.tier,CurrentNotification.total)
			"submessage":
				QueueTimer.wait_time = CurrentNotification.timer_delay
				submessage.emit(CurrentNotification.user_name,CurrentNotification.tier,CurrentNotification.message,CurrentNotification.cumulative_months,CurrentNotification.duration_months)
			"raid":
				QueueTimer.wait_time = CurrentNotification.timer_delay
				raid.emit(CurrentNotification.from_broadcaster_user_name,CurrentNotification.viewers)
			"cheer":
				QueueTimer.wait_time = CurrentNotification.timer_delay
				cheer.emit(CurrentNotification.user_name,CurrentNotification.message,CurrentNotification.bits)
		NotificationQueue.remove_at(0)

var RepeatList : Array[Dictionary] # List of events we don't want repeats of.
# This will effect the following events
# Follows and Raids

func CheckForRepeatedEvent(username,event):
	var ConstructedDictionary = {
		"username" = null,
		"event" = null}
	ConstructedDictionary.event = event
	ConstructedDictionary.username = username
	if RepeatList.has(ConstructedDictionary):
		return true # REPEATED EVENT
	if !RepeatList.has(ConstructedDictionary):
		return false # Not repeated

func AddToRepeatList(username, event):
	var ConstructedDictionary = {
		"username" = null,
		"event" = null
	}
	ConstructedDictionary.event = event
	ConstructedDictionary.username = username
	RepeatList.append(ConstructedDictionary)
	
