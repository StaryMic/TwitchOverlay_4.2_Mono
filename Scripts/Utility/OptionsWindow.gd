@icon("res://icon.svg")
extends Node

@onready var eventhandler = $"../EventHandler"

var output_devices
@onready var output_dropdown = $"../../OptionsWindow/Control/VBoxContainer/HBoxContainer/VBoxContainer3/AudioDeviceList"

func _init():
	output_devices = AudioServer.get_output_device_list()
	if output_devices.has("Wave Link SFX (Elgato Wave:3)"):
		AudioServer.output_device = "Wave Link SFX (Elgato Wave:3)"

# Called when the node enters the scene tree for the first time.
func _ready():
	for devices in output_devices:
		output_dropdown.add_item(devices)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	#If TAB is pressed Hide/Show the Options window.
	if Input.is_action_just_pressed("ui_text_indent"):
		%OptionsWindow.visible = !%OptionsWindow.visible

func _on_close_button_pressed():
	get_tree().quit();

func _on_hide_panel_button_pressed():
	%OptionsWindow.visible = false;

func _on_auth_pressed():
	OS.shell_open("https://id.twitch.tv/oauth2/authorize?client_id=hmhsbcxv5jd5qd6g7n4lktwlugns7z&redirect_uri=http://localhost:3000&force_verify=true&response_type=token&scope=bits:read+channel:read:subscriptions+channel_subscriptions+user:read:subscriptions+user:edit+chat:read+chat:edit+channel:moderate+moderation:read+moderator:manage:chat_settings+whispers:edit+user:manage:whispers+whispers:read+channel:manage:redemptions+channel:read:redemptions+channel:edit:commercial+channel_commercial+channel:manage:broadcast+channel_editor+user:edit:broadcast+clips:edit+channel:manage:extensions+channel:read:hype_train+analytics:read:extensions+analytics:read:games+user:read:follows+moderator:read:followers+user:read:broadcast+user:read:email+channel:read:polls+channel:manage:polls+channel:read:predictions+channel:manage:predictions+moderator:manage:announcements+channel:manage:moderators+channel:read:vips+channel:manage:vips+user:manage:chat_color+moderator:manage:chat_messages+channel:read:goals+channel:read:charity+moderator:read:chatters+moderator:manage:shield_mode+channel:manage:raids+moderator:manage:banned_users+moderator:manage:shoutouts+moderator:manage:automod_settings+moderator:read:automod_settings+moderator:manage:blocked_terms+moderator:read:blocked_terms")

func _on_audio_device_list_item_selected(index):
	AudioServer.output_device = output_dropdown.get_item_text(index)


func _on_follow_pressed():
	eventhandler.follow.emit("25characternameaaaaaaaaaa")

func _on_subscribe_pressed():
	eventhandler.subscribe.emit("25characternameaaaaaaaaaa","2",true)


func _on_giftsub_pressed():
	eventhandler.giftsub.emit("25characternameaaaaaaaaaa","2","10")


func _on_submessage_pressed():
	eventhandler.submessage.emit("25characternameaaaaaaaaaa",2,"nipples rule!!!!","2","4")


func _on_cheer_pressed():
	eventhandler.cheer.emit("25characternameaaaaaaaaaa","Fuck yeah sex!!!!!!","4892")


func _on_raid_pressed():
	eventhandler.raid.emit("25characternameaaaaaaaaaa","8")

func _on_ban_pressed():
	eventhandler.ban.emit()
	
	
	
	
#Redeem test buttons

func _on_move_opila_pressed():
	eventhandler.pointredeem.emit("peepee","Display Opila","")
