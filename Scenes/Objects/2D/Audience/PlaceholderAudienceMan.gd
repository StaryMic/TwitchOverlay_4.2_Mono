extends Node2D

@export var ResourceToLoad : Resource

var randomypos = randf_range(1000,1080)
@onready var AnimPlayer = $AnimationPlayer
@onready var AudioPlayer = $AudioStreamPlayer2D
@onready var Sprite = $Sprite2D

@export var PopSounds : Array[AudioStream]

var ThrowInfluence : bool = false
var TimeProcessed : float = 0
var ThrowTime : float = 0
var ThrowStartPoint : Vector2
var ThrowMidPoint : Vector2
var ThrowEndPoint : Vector2

var SpeenInfluence : bool = false

var SlideInfluence : bool = false
var slidetween

var DesiredAction : String

var ItemToSpawn : String
@onready var ItemBaseReference = "res://Scenes/Objects/2D/Audience/Items/ItemBase.tscn"
var ItemBaseTemplate
var SpawnedItem
var InstancedItem

func _ready():
	Sprite.texture = ResourceToLoad.Standing
	if Sprite.texture != null:
		print("Texture loaded I think lmao")
	else:
		print("Failed to load resource. Killing Audience Member.")
		queue_free()
	
	match randi_range(0,1):
		0:
			Sprite.flip_h = false
		1:
			Sprite.flip_h = true

func _process(delta):
	if ThrowInfluence == true:
		HandleBezierCurve(delta)
		
	if SpeenInfluence == true:
		self.rotation = self.rotation + .1
		print(self.rotation)
	
	if SlideInfluence == true:
		if randi_range(0,50) == 42:
			SlideInfluence = false
			slidetween.kill()
			PopOffWindow()

# Move Functions

func TweenIn():
	var tween = create_tween()
	tween.set_trans(Tween.TRANS_CIRC)
	tween.set_ease(Tween.EASE_IN_OUT)
	tween.tween_property(self,"position:y",randomypos,1)
	ChangeSprite("Standing")

func TweenOut():
	var tween = create_tween()
	tween.set_trans(Tween.TRANS_CIRC)
	tween.set_ease(Tween.EASE_IN_OUT)
	tween.tween_property(self,"position:y",1500,1)
	tween.tween_callback(queue_free)
	ChangeSprite("Standing")
	
func StartThrow(timelength : float, randomizepoints : bool, _midpoint : Vector2, _endpoint : Vector2):
	ThrowInfluence = true
	ThrowStartPoint = self.position
	ThrowTime = timelength
	TimeProcessed = 0
	if randomizepoints != true:
		ThrowMidPoint = _midpoint
		ThrowEndPoint = _endpoint
	else:
		ThrowEndPoint.x = randf_range(150,1800)
		ThrowEndPoint.y = randf_range(150,900)
		
		ThrowMidPoint = (ThrowStartPoint+ThrowEndPoint)/2
		ThrowMidPoint.x = ThrowMidPoint.x + randf_range(-600,600)
		ThrowMidPoint.y = ThrowMidPoint.y + randf_range(0,600)
	
	if DesiredAction == "Throw":
		ChangeSprite("Thrown")
		Sprite.scale = Vector2(0.5,0.5)
		var tween = create_tween()
		tween.set_trans(Tween.TRANS_CUBIC)
		tween.set_ease(Tween.EASE_IN)
		tween.tween_property(Sprite,"scale",Vector2(0.25,0.25),timelength)
		tween.tween_callback(Thunk)
		tween.tween_callback(SlideDownScreen).set_delay(randf_range(0,1.5))
	if DesiredAction == "Falling":
		ChangeSprite("Falling")

func HandleBezierCurve(deltatime : float):
	#Bezier curve code
	if ThrowTime == null:
		ThrowInfluence = false
		return
	TimeProcessed = TimeProcessed + deltatime
	var NormalizedTime = TimeProcessed/ThrowTime
	if NormalizedTime >= 1:
		NormalizedTime = 1
		ThrowInfluence = false
	
	#Quadratic Bezier. Get the position between each point.
	var lerp1 : Vector2 = ThrowStartPoint.lerp(ThrowMidPoint,NormalizedTime)
	var lerp2 : Vector2 = ThrowMidPoint.lerp(ThrowEndPoint,NormalizedTime)
	
	#Lerp those together
	var resultingposition : Vector2 = lerp1.lerp(lerp2,NormalizedTime)
	self.position = resultingposition

func PopOffWindow():
	var MidPointPosition : Vector2
	var EndPointPosition : Vector2
	var HorizontalDirection : float
	
	if randi_range(0,1) == 0:
		HorizontalDirection = -1
		Sprite.flip_h = false
	else:
		HorizontalDirection = 1
		Sprite.flip_h = true
	
	MidPointPosition.x = self.position.x + (randf_range(100,300)* HorizontalDirection)
	MidPointPosition.y = self.position.y + randf_range(200,400)
	
	EndPointPosition.x = MidPointPosition.x + (randf_range(100,300)* HorizontalDirection)
	EndPointPosition.y = MidPointPosition.y + 1000
	
	DesiredAction = "Falling"
	
	StartThrow(randf_range(.6,1.2),false,MidPointPosition,EndPointPosition)
	SFX_WindowPop()
	await get_tree().create_timer(1.2).timeout
	queue_free()

func SlideDownScreen():
	SlideInfluence = true
	WindowSqueak()
	slidetween = create_tween()
	slidetween.set_ease(Tween.EASE_IN)
	slidetween.set_trans(Tween.TRANS_SINE)
	slidetween.tween_property(self,"position:y",1500,randf_range(0.8,1.5))
	slidetween.tween_callback(queue_free)
	ChangeSprite("Sliding")

# Action Functions

func ChangeSprite(LoadSprite : String):
	var SpriteToLoad
	match LoadSprite:
		"Standing":
			SpriteToLoad = ResourceToLoad.Standing
		"Bark":
			SpriteToLoad = ResourceToLoad.Bark
		"Thrown":
			SpriteToLoad = ResourceToLoad.Thrown
		"Splat":
			SpriteToLoad = ResourceToLoad.Splat
		"Sliding":
			SpriteToLoad = ResourceToLoad.Sliding
		"Falling":
			SpriteToLoad = ResourceToLoad.Falling
		"Jeering":
			SpriteToLoad = ResourceToLoad.Jeering
		"PreThrowItem":
			SpriteToLoad = ResourceToLoad.PreThrowItem
		"ThrowItem":
			SpriteToLoad = ResourceToLoad.ThrowItem
		_:
			print("Failed to change sprite. Make sure the name of the sprite exists under ChangeSprite().")
	Sprite.texture = SpriteToLoad

func SummonItem(Item : String):
	var ValidItemList = {
		"egg" : "res://Scenes/Objects/2D/Audience/Items/Resources/Egg.tres",
		"spraybottle" : "res://Scenes/Objects/2D/Audience/Items/Resources/SprayBottle.tres"
	}
	if !ValidItemList.has(Item.to_lower()):
		return false
	else:
		ItemBaseTemplate = load(ItemBaseReference)
		InstancedItem = ItemBaseTemplate.instantiate()
		self.add_child(InstancedItem)
		InstancedItem.ItemPropertyFile = load(ValidItemList.get(Item.to_lower()))
		SpawnedItem = self.get_node("ItemBase")

func ThrowItem():
	$ItemBase.reparent(get_node(".."),true)
	InstancedItem.StartThrow(1,true,Vector2(0,0),Vector2(0,0))

func Thunk():
	var thunksound = load("res://Audio/SFX/snowball_hit_window.wav")
	var impactplayer = AudioStreamPlayer2D.new()
	impactplayer.pitch_scale = randf_range(.7,1.4)
	impactplayer.stream = thunksound
	impactplayer.bus = "AudAvatarSounds"
	impactplayer.max_distance = 1000
	add_child(impactplayer)
	impactplayer.play()
	ChangeSprite("Splat")

func WindowSqueak():
	var SlideSound = load("res://Audio/SFX/windowsqueak.wav")
	var impactplayer = AudioStreamPlayer2D.new()
	impactplayer.pitch_scale = randf_range(.7,1.4)
	impactplayer.stream = SlideSound
	impactplayer.bus = "AudAvatarSounds"
	impactplayer.max_distance = 1000
	add_child(impactplayer)
	impactplayer.play()

func Bark():
	#Play Bark sound from resource.
	var BarkSound : AudioStream = ResourceToLoad.BarkSounds.pick_random()
	var impactplayer = AudioStreamPlayer2D.new()
	impactplayer.pitch_scale = randf_range(.7,1.4)
	impactplayer.stream = BarkSound
	impactplayer.bus = "AudAvatarSounds"
	impactplayer.panning_strength = 1.3
	add_child(impactplayer)
	impactplayer.play()
	ChangeSprite("Bark")
	
	#Bounce Avatar
	StartThrow(BarkSound.get_length(),false,Vector2(self.position.x,self.position.y-150),self.position)
	await get_tree().create_timer(BarkSound.get_length()).timeout
	ChangeSprite("Standing")


#Audio Functions
func SFX_WindowPop():
	var PopSound : AudioStream = PopSounds.pick_random()
	var impactplayer = AudioStreamPlayer2D.new()
	impactplayer.pitch_scale = randf_range(.7,1.4)
	impactplayer.stream = PopSound
	impactplayer.bus = "AudAvatarSounds"
	impactplayer.panning_strength = 1.3
	add_child(impactplayer)
	impactplayer.play()
