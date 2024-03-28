extends Node2D
@export var AreaBounds : PackedVector2Array
var SelectedMan : Resource
var InstancedMan
var ManImator : AnimationPlayer
@onready var AudienceManReference = "res://Scenes/Objects/2D/Audience/AudienceMan.tscn"
var AudienceManTemplate

signal SummonAvatar(Avatar, BodyColor, EyeColor, Action)

var ValidAvatars : Dictionary = {
	"stickfigure" : "res://Scenes/Objects/2D/Audience/Avatars/StickFigureAvatar.tres",
	"wolf" : "res://Scenes/Objects/2D/Audience/Avatars/WolfAvatar.tres",
	"colortest" : "res://Scenes/Objects/2D/Audience/Avatars/ColorTestAvatar.tres"
}
#Functionality I want
# Slide in from bottom
# Bounce and bark
# Slide out.

func _ready():
	print(ValidAvatars.keys())
	SprayWolves()

func SpawnAudienceMan(DesiredAvatar : String, BodyColor : String, EyeColor : String, ItemToSpawn : String):
	#Randomise the position, sprite face, and node we're spawning.
	@warning_ignore("unassigned_variable")
	var spawnPosition : Vector2
	var AvatarResourceReference : String
	
	if ValidAvatars.has(DesiredAvatar):
		AvatarResourceReference = ValidAvatars.get(DesiredAvatar)
	else:
		return false
	
	spawnPosition.x = randf_range(AreaBounds[0].x,AreaBounds[1].x)
	spawnPosition.y = randf_range(AreaBounds[0].y,AreaBounds[1].y)

	AudienceManTemplate = load(AudienceManReference)
	
	SelectedMan = load(AvatarResourceReference) #	Select the man
	
	#Instance the man, load him into the scene under AudienceManager
	InstancedMan = AudienceManTemplate.instantiate()
	InstancedMan.ResourceToLoad = SelectedMan
	self.add_child(InstancedMan)
	
	# Set Material params.
	var ManMaterial = InstancedMan.get_node("Sprite2D").material
	ManMaterial.set_shader_parameter("BodyColor",Color(BodyColor,1))
	ManMaterial.set_shader_parameter("EyeColor",Color(EyeColor,1))
	
	#Set starting position and set reference variables.
	InstancedMan.position.x = spawnPosition.x
	InstancedMan.position.y = 1336
	
	ManImator = InstancedMan.get_node("AnimationPlayer")
	InstancedMan.ItemToSpawn = ItemToSpawn
	return true

func ManAppear(Anim : String):
	ManImator.play(Anim)
	InstancedMan.DesiredAction = Anim
	#You might need to do all of the movement, sounds, etc through the animation on each AudienceMan.
	# If you try and code stuff here, the reference will change as multiple redeems for barks come in
	# making it a nightmare to deal with since you'd have to work asyncronously.

func SprayWolves():
	await get_tree().create_timer(randf_range(0,10)).timeout
	if SpawnAudienceMan("stickfigure","000000","000000","egg"): #Make sure we loaded the avatar and it is valid.
		ManAppear("SummonItemTest")


func _draw():
	if Engine.is_editor_hint():
		draw_line(AreaBounds[0],AreaBounds[1],"BLUE",1.0)


func _on_summon_avatar(Avatar, BodyColor, EyeColor, Action, ItemToSpawn):
	if SpawnAudienceMan(Avatar,BodyColor,EyeColor, ItemToSpawn):
		ManAppear(Action)
	
	
