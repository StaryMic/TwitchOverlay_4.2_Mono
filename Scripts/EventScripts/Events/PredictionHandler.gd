extends Node

@onready var slidermat = $VBoxContainer/Slider/Clipper/ProgressBar.material
@onready var locksprite = $Lock
@onready var winparticle = $WinParticles
@onready var winaudio = $WinAudio
var BlueColor = Color(0.196, 0.424, 0.965)
var PinkColor = Color(0.808, 0.184, 0.545)
var predictionpercent
var total_channel_points

var tween : Tween
const templateWinText = "[Rainbow][center]"

func _on_event_handler_prediction(title, outcomes, notification_type, winning_id):
	match notification_type:
		"begin":
			$VBoxContainer/WinnerLabel.text = ""
			if outcomes.size() == 2: #If we have 2 outcomes
				tween = create_tween()
				tween.set_trans(Tween.TRANS_BOUNCE)
				tween.set_ease(Tween.EASE_OUT)
				tween.tween_property(self,"position",Vector2(0,0),1)
				
				$VBoxContainer/Title.text = str(title)
				for outcome in outcomes:
					if outcome.color == "blue":
						$VBoxContainer/HBoxContainer/PredictionOne/Option.text = str(outcome.title)
						$VBoxContainer/HBoxContainer/PredictionOne/ChannelPoints.text = str(0)
					else:
						$VBoxContainer/HBoxContainer/PredictionTwo/Option.text = str(outcome.title)
						$VBoxContainer/HBoxContainer/PredictionTwo/ChannelPoints.text = str(0)
			slidermat.set_shader_parameter("Progress", 0.5)
		"end":
			$VBoxContainer/HBoxContainer/PredictionOne/ChannelPoints.text = str(outcomes[0].channel_points)
			$VBoxContainer/HBoxContainer/PredictionTwo/ChannelPoints.text = str(outcomes[1].channel_points)
			if tween.is_running() == true:
				tween.stop() #Stop tweens if we kept some running
			
			if outcomes[0].channel_points != 0 or total_channel_points != 0 or outcomes[0].channel_points != null:
				predictionpercent = outcomes[0].channel_points / total_channel_points
			else:
				predictionpercent = 0.5
			tween = create_tween()
			tween.set_ease(Tween.EASE_OUT)
			tween.set_trans(Tween.TRANS_EXPO)
			tween.tween_property(slidermat,"shader_parameter/Progress",predictionpercent,0.5)
			tween.play()
			if outcomes[0].id == winning_id:
				$VBoxContainer/WinnerLabel.text = str(templateWinText,"Blue Wins!")
				winparticle.process_material.color = BlueColor
				winparticle.emitting = true
				winaudio.play()
				
			else:
				$VBoxContainer/WinnerLabel.text = str(templateWinText,"Pink Wins!")
				winparticle.process_material.color = PinkColor
				winparticle.emitting = true
				winaudio.play()
			
			await get_tree().create_timer(10).timeout

			winparticle.emitting = false
			tween = create_tween()
			tween.set_trans(Tween.TRANS_CIRC)
			tween.set_ease(Tween.EASE_IN)
			tween.tween_property(self,"position",Vector2(0,-500),.5)
			tween.tween_property(locksprite,"modulate:a",0,1)
			tween.play()
			
		"lock":
			if tween.is_running() == true:
					tween.stop() #Stop tweens if we kept some running
			locksprite.play("Wiggle")
			
			tween = create_tween()
			tween.tween_property(locksprite,"modulate:a",1,1)
			tween.set_trans(Tween.TRANS_QUAD)
			tween.set_ease(Tween.EASE_IN_OUT)
			tween.play()
			
		"progress":
			if tween.is_running() == true:
					tween.stop() #Stop tweens if we kept some running
			total_channel_points = outcomes[0].channel_points + outcomes[1].channel_points
			$VBoxContainer/HBoxContainer/PredictionOne/ChannelPoints.text = str(outcomes[0].channel_points)
			$VBoxContainer/HBoxContainer/PredictionTwo/ChannelPoints.text = str(outcomes[1].channel_points)
			predictionpercent = outcomes[0].channel_points / total_channel_points
			tween = create_tween()
			tween.set_ease(Tween.EASE_OUT)
			tween.set_trans(Tween.TRANS_EXPO)
			tween.tween_property(slidermat,"shader_parameter/Progress",predictionpercent,0.5)
			tween.play()
