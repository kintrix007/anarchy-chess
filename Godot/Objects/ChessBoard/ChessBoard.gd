tool
extends Node2D

onready var board := $"%Board"
onready var tilemap_size: Vector2 = board.cell_size * Vector2(8, 8)

var parent: Control = null

func _ready() -> void:
	var tmp_parent := get_parent()
	if tmp_parent == null: return
	if not tmp_parent is Control: return
	
	parent = tmp_parent 
	var ok := parent.connect("resized", self, "_on_parent_resized")
	if ok != OK:
		printerr("'%s' Could not connect resize from parent '%s'" % [self, parent])
	
	resize_fit()

func get_fit_ratio(size: Vector2, target: Vector2) -> Vector2:
	var ratio := target / size
	var fit_ratio := min(ratio.x, ratio.y)
	
	return Vector2(fit_ratio, fit_ratio)

func resize_fit() -> void:
	if parent == null: return
	
	var ratio := get_fit_ratio(tilemap_size, parent.rect_size)
	self.scale = ratio

func _on_parent_resized() -> void:
	resize_fit()
