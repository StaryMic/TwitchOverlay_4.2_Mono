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

func _on_global_scene_signals_channel_prediction(Title, Outcomes, PredictionStatus, WinningOutcomeID):
	print("I hav received the signal")
	match PredictionStatus:
		"begin":
			print("PredictionHandler.gd: Prediction has begun")
			$VBoxContainer/WinnerLabel.text = ""
			if Outcomes.size() == 2: #If we have 2 outcomes
				tween = create_tween()
				tween.set_trans(Tween.TRANS_BOUNCE)
				tween.set_ease(Tween.EASE_OUT)
				tween.tween_property(self,"position",Vector2(0,0),1)
			
			$VBoxContainer/Title.text = str(Title)
			for outcome in Outcomes:
				if outcome.Color == "blue":
					$VBoxContainer/HBoxContainer/PredictionOne/Option.text = str(outcome.Title)
					$VBoxContainer/HBoxContainer/PredictionOne/ChannelPoints.text = str(0)
				else:
					$VBoxContainer/HBoxContainer/PredictionTwo/Option.text = str(outcome.Title)
					$VBoxContainer/HBoxContainer/PredictionTwo/ChannelPoints.text = str(0)
			slidermat.set_shader_parameter("Progress", 0.5)
		"end":
			$VBoxContainer/HBoxContainer/PredictionOne/ChannelPoints.text = str(Outcomes[0].ChannelPoints)
			$VBoxContainer/HBoxContainer/PredictionTwo/ChannelPoints.text = str(Outcomes[1].ChannelPoints)
			if tween.is_running() && tween != null:
				tween.stop() #Stop tweens if we kept some running
		
			if Outcomes[0].ChannelPoints != 0 or total_channel_points != 0 or Outcomes[0].ChannelPoints != null or total_channel_points != null:
				predictionpercent = Outcomes[0].ChannelPoints / total_channel_points
			else:
				predictionpercent = 0.5
			tween = create_tween()
			tween.set_ease(Tween.EASE_OUT)
			tween.set_trans(Tween.TRANS_EXPO)
			tween.tween_property(slidermat,"shader_parameter/Progress",predictionpercent,0.5)
			tween.play()
			if Outcomes[0].Id == WinningOutcomeID:
				$VBoxContainer/WinnerLabel.text = str(templateWinText,"Blue Wins!")
				winparticle.process_material.Color = BlueColor
				winparticle.emitting = true
				winaudio.play()
			
			else:
				$VBoxContainer/WinnerLabel.text = str(templateWinText,"Pink Wins!")
				winparticle.process_material.Color = PinkColor
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
			total_channel_points = Outcomes[0].ChannelPoints + Outcomes[1].ChannelPoints
			$VBoxContainer/HBoxContainer/PredictionOne/ChannelPoints.text = str(Outcomes[0].ChannelPoints)
			$VBoxContainer/HBoxContainer/PredictionTwo/ChannelPoints.text = str(Outcomes[1].ChannelPoints)
			predictionpercent = Outcomes[0].ChannelPoints / total_channel_points
			tween = create_tween()
			tween.set_ease(Tween.EASE_OUT)
			tween.set_trans(Tween.TRANS_EXPO)
			tween.tween_property(slidermat,"shader_parameter/Progress",predictionpercent,0.5)
			tween.play()
