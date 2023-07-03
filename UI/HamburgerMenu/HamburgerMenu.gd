tool
extends Panel

export(float, 0.3, 1.0) var screen_width: float = 0.65
export(float, 350.0, 1500.0) var max_width: float = 650.0

onready var tween := Tween.new()

var is_hamburger_open := false

func _ready() -> void:
	add_child(tween)
	set_width()
	
	var ok := get_viewport().connect("size_changed", self, "_on_viewport_size_changed")
	if ok != OK:
		printerr("'%s' could not connect resize from the viewport" % self)

func set_width() -> void:
	var viewport_rect := get_viewport_rect().size
	var target_size := viewport_rect.x * screen_width
	target_size = min(target_size, max_width)
	
	self.rect_size.x = target_size

func open_hamburger_menu(instantly := false) -> void:
	var _ok = tween.interpolate_property(
		self, "rect_position:x",
		null, 0,
		0.0 if instantly else 0.3, Tween.TRANS_CUBIC, Tween.EASE_OUT
	)
	_ok = tween.start()
	is_hamburger_open = true

func close_hamburger_menu(instantly := false) -> void:
	var menu_hidden_position := -get_viewport_rect().size.x
	var _ok = tween.interpolate_property(
		self, "rect_position:x",
		null, menu_hidden_position,
		0.0 if instantly else 0.3, Tween.TRANS_CUBIC, Tween.EASE_OUT
	)
	_ok = tween.start()
	is_hamburger_open = false

func _on_viewport_size_changed() -> void:
	set_width()
	if is_hamburger_open:
		open_hamburger_menu(true)
	else:
		close_hamburger_menu(true)
