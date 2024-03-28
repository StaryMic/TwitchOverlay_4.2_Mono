extends Node2D
@export var ItemPropertyFile : Resource

@onready var Sprite = $Sprite2D
@onready var Audio = $AudioStreamPlayer2D

var Flipped : bool

var ThrowInfluence : bool
var SlideInfluence : bool

var ThrowStartPoint : Vector2
var ThrowTime
var TimeProcessed
var ThrowMidPoint : Vector2
var ThrowEndPoint : Vector2

@onready var BreakCallable = Callable(self,ItemPropertyFile.CallOnBreak)
@onready var ImpactCallable = Callable(self,ItemPropertyFile.CallOnImpact)

var BreakParticleScene
var ImpactParticleScene
var HandheldParticleSystem
# Called when the node enters the scene tree for the first time.
func _ready():
	Sprite.texture = ItemPropertyFile.ItemImage
	if ItemPropertyFile.CanBreak == true:
		BreakParticleScene = ItemPropertyFile.BreakParticleSystem.instantiate()
	if ItemPropertyFile.CanImpact == true:
		ImpactParticleScene = ItemPropertyFile.ImpactParticleSystem.instantiate()
	if ItemPropertyFile.HandheldObject == true:
		Sprite.texture = ItemPropertyFile.ItemImage
		HandheldParticleSystem = ItemPropertyFile.HandheldActionParticleSystem.instantiate()
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if ThrowInfluence == true:
		HandleBezierCurve(delta)
	if SlideInfluence == true:
		self.position.y = self.position.y + 5
		if ItemPropertyFile.StretchWhenSlide == true:
			self.scale.y = self.scale.y + (0.15 * delta)

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
		if ItemPropertyFile.BreakAtApex == true:
			BreakItem()
	
	#Quadratic Bezier. Get the position between each point.
	var lerp1 : Vector2 = ThrowStartPoint.lerp(ThrowMidPoint,NormalizedTime)
	var lerp2 : Vector2 = ThrowMidPoint.lerp(ThrowEndPoint,NormalizedTime)
	
	#Lerp those together
	var resultingposition : Vector2 = lerp1.lerp(lerp2,NormalizedTime)
	self.position = resultingposition
	
func SprayImpactParticles():
	self.add_child(ImpactParticleScene)
	ImpactParticleScene.emitting = true
	

func SprayBreakParticles():
	self.add_child(BreakParticleScene)
	BreakParticleScene.emitting = true

func BreakItem():
	Sprite.texture = ItemPropertyFile.BreakImage
	SprayBreakParticles()
	Audio.stream = ItemPropertyFile.BreakSounds.pick_random()
	Audio.play()
	await get_tree().create_timer(randf_range(.5,2)).timeout
	if ItemPropertyFile.SlideOnBreak == true:
		SlideInfluence = true

func ItemImpact():
	SprayImpactParticles()
	Audio.stream = ItemPropertyFile.ImpactSounds.pick_random()
	Audio.play()

func HandheldAction (type : String):
	match type:
		"EmitParticles":
			self.add_child(HandheldParticleSystem)
			HandheldParticleSystem.position = ItemPropertyFile.HandheldParticlePosition
			if Flipped == false:
				HandheldParticleSystem.rotation = 180
			else:
				HandheldParticleSystem.rotation = 0
			HandheldParticleSystem.emitting = true
		_:
			print("Nope. Invalid handheld action.")
			return false
