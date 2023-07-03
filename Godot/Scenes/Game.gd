extends Control

onready var tween := Tween.new()
onready var hamburger_menu := $"%HamburgerMenu"

var is_hamburger_open := false

func _ready() -> void:
	add_child(tween)
	hamburger_menu.close_hamburger_menu(true)

func _on_hamburger_button_pressed() -> void:
	is_hamburger_open = not is_hamburger_open
	if is_hamburger_open:
		hamburger_menu.open_hamburger_menu()
	else:
		hamburger_menu.close_hamburger_menu()

func _input(event: InputEvent) -> void:
	if event is InputEventScreenTouch:
		if not is_hamburger_open: return
		var hm := hamburger_menu
		var hamburger_left_side: float = hm.rect_size.x
		if event.position.x < hamburger_left_side: return
		
		is_hamburger_open = false
		hamburger_menu.close_hamburger_menu()
