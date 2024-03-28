extends Control

@onready var Item = $"../ItemBase"

func _on_sim_throw_pressed():
	Item.StartThrow(1,true,Vector2(0,0),Vector2(0,0))


func _on_break_pressed():
	Item.BreakItem()


func _on_impact_pressed():
	pass # Replace with function body.


func _on_test_impact_particles_pressed():
	Item.SprayImpactParticles()


func _on_test_break_particles_pressed():
	Item.SprayBreakParticles()


func _on_fall_influence_toggled(toggled_on):
	Item.SlideInfluence = toggled_on


func _on_hand_held_action_button_down():
	Item.HandheldAction("EmitParticles")
