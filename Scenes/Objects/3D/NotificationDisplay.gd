extends Control
@onready var TopLabel = $NotificationPanel/VBoxContainer/TopLabel
@onready var BottomLabel = $NotificationPanel/VBoxContainer/VBoxContainer/BottomLabel
@onready var ImagePanel = $ImagePanel
@onready var NotificationPanel = $NotificationPanel
@onready var AnimPlayer = $AnimationPlayer

@export var ImagePanelPositions : PackedVector2Array

func _ready():
	AnimPlayer.play("Appear")

#Tween Functions
var tween : Tween

func ResetPosition():
	if tween.is_valid() and tween.is_running():
		tween.stop()
		tween.kill()
	ImagePanel.position = Vector2(1547.81,0)
	NotificationPanel.position = Vector2(2059.81,0)

func PlayTweenAnimation():
	tween = create_tween()
	tween.set_ease(Tween.EASE_IN).set_trans(Tween.TRANS_CUBIC)


# TODO: STRIP OUT BBCODE FROM INITIAL MESSAGE

func RandomizeTopMessage():
	var ChosenMessage = ""
	match randi_range(0,4):
		0:
			ChosenMessage = "[center][rumble]HOLY FUCKING BINGLE!!!"
		1:
			ChosenMessage = "[center][rainbow]AMAZING!!!"
		2:
			ChosenMessage = "[center][rainbow]INCREDIBLE!!!"
		3:
			ChosenMessage = "[center]I can't thank you enough!"
		4:
			ChosenMessage = "[center]Thank you for your support!"

func _on_event_handler_cheer(username, message, bits):
	RandomizeTopMessage()
	var message_template = "[center][Rainbow]{username}[/Rainbow] has sent [Rainbow]{bits}[/Rainbow] bits!"
	var message_fills = {"username":username,"message":message,"bits":bits}
	BottomLabel.text = message_template.format(message_fills)
	AnimPlayer.play("Appear")


func _on_follow(username):
	RandomizeTopMessage()
	var message_template = "[center]Thank you\n[Rainbow]{username}[/Rainbow]\n for the follow!"
	var message_fills = {"username":username}
	BottomLabel.text = message_template.format(message_fills)
	AnimPlayer.play("Appear")


func _on_giftsub(username, tier : String, total_gifts):
	RandomizeTopMessage()
	var message_template = "[center][Rainbow]{username}[/Rainbow] has gifted [Rainbow]{gifts}[/Rainbow] subs!"
	var message_fills = {"username":username,"gifts":total_gifts,"tier":tier}
	BottomLabel.text = message_template.format(message_fills)
	AnimPlayer.play("Appear")

func _on_event_handler_raid(username, viewers):
	TopLabel.text = "[center][Rumble]RAID INCOMING!!!"
	var message_template = "[center][Rainbow]{username}[/Rainbow] has sent [Rainbow]{viewers}[/Rainbow] viewers to our channel!\nBe kind to them!"
	var message_fills = {"username":username,"viewers":viewers}
	BottomLabel.text = message_template.format(message_fills)
	AnimPlayer.play("Appear")

func _on_event_handler_submessage(username, tier, message, cumulative_months, duration_months):
	RandomizeTopMessage()
	var message_template = "[center][Rainbow]{username}[/Rainbow] has subscribed!"
	var message_fills = {"username":username,"message":message}
	BottomLabel.text = message_template.format(message_fills)
	AnimPlayer.play("Appear")

func _on_event_handler_subscribe(username, tier, is_gift):
	RandomizeTopMessage()
	var message_template
	if is_gift == true:
		message_template = "[center][Rainbow]{username}[/Rainbow] was gifted a tier {tier} sub!"
	else: 
		message_template = "[center][Rainbow]{username}[/Rainbow] has subscribed!"
	var message_fills = {"username":username,"tier":tier,"gift":is_gift}
	BottomLabel.text = message_template.format(message_fills)
	AnimPlayer.play("Appear")
