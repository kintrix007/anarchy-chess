extends TileMap

class Pos:
	var x: int
	var y: int
	
	func _init(x = null, y = null) -> void:
		if x == null and y == null:
			x = 0
			y = 0
		elif x is int and y == null:
			y = x
		
		self.x = x
		self.y = y

var pieces := {}

func _ready() -> void:
	Pos.new()
