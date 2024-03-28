extends Node3D


# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


func _on_event_handler_follow(username):
	print_rich("[b]GOT A FOLLOW SIGNAL[/b]")
	$Label3D.text = username



func _on_event_handler_cheer(username, message, bits):
	$Label3D.text = str(username,message,bits)


func _on_event_handler_giftsub(username, tier, total_gifts):
	$Label3D.text = str(username,tier,total_gifts)


func _on_event_handler_pointredeem(username, reward_title, user_input):
	$Label3D.text = str(username,reward_title,user_input)


func _on_event_handler_submessage(username, tier, message, cumulative_months, duration_months):
	$Label3D.text = str(username,tier,message,cumulative_months,duration_months)


func _on_event_handler_subscribe(username, tier, is_gift):
	$Label3D.text = str(username,tier,is_gift)


func _on_event_handler_raid(username, viewers):
	$Label3D.text = str(username,viewers)
