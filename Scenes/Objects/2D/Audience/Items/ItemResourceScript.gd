extends Resource
@export_category("Visuals")
@export var ItemImage : CompressedTexture2D
@export var BreakImage : CompressedTexture2D
@export var BreakSounds : Array[AudioStream]
@export var ImpactSounds : Array[AudioStream]

@export_category("Conditions")
@export var HandheldObject : bool
@export var CanBreak : bool
@export var CanImpact : bool
@export var SlideOnBreak : bool
@export var StretchWhenSlide : bool

@export_category("Physics")
@export var BreakAtApex : bool

@export_category("Callables")
@export var CallOnBreak : String
@export var CallOnImpact : String

@export_category("Particles")
@export var BreakParticleSystem : PackedScene
@export var ImpactParticleSystem : PackedScene
@export var HandheldActionParticleSystem : PackedScene
@export var HandheldParticlePosition : Vector2 = Vector2(0,0)
