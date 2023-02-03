extends Control

onready var tween := Tween.new()
onready var hamburger_menu := $"%HamburgerMenu"

var is_hamburger_open := false

func _ready() -> void:
	add_child(tween)
	close_hamburger_menu(true)
	hamburger_menu.rect_position = Vector2(-get_viewport_rect().size.x, 0)

func _on_hamburger_button_pressed() -> void:
	is_hamburger_open = not is_hamburger_open
	if is_hamburger_open:
		open_hamburger_menu()
	else:
		close_hamburger_menu()

func open_hamburger_menu(instantly := false) -> void:
	var _ok = tween.interpolate_property(
		hamburger_menu, "rect_position:x",
		null, 0,
		0.0 if instantly else 0.3, Tween.TRANS_CUBIC, Tween.EASE_OUT
	)
	_ok = tween.start()
	is_hamburger_open = true

func close_hamburger_menu(instantly := false) -> void:
	var menu_hidden_position := -get_viewport_rect().size.x
	var _ok = tween.interpolate_property(
		hamburger_menu, "rect_position:x",
		null, menu_hidden_position,
		0.0 if instantly else 0.3, Tween.TRANS_CUBIC, Tween.EASE_OUT
	)
	_ok = tween.start()
	is_hamburger_open = false

func _input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if not is_hamburger_open: return
		var hm := hamburger_menu
		var hamburger_left_side: float = hm.rect_size.x
		if event.position.x < hamburger_left_side: return
		_on_hamburger_button_pressed()
