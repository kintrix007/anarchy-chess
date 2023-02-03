tool
extends Panel

export(float, 0.3, 1.0) var screen_width: float = 0.65

func _ready() -> void:
	set_width()

func set_width() -> void:
	var viewport_rect := get_viewport_rect().size
	self.rect_size.x = viewport_rect.x * screen_width
