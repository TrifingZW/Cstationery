extends Sprite2D

# 导出变量，可以在编辑器中调整
@export var offset_amount: Vector2 = Vector2(700, 700)

func _ready() -> void:
	# 确保鼠标可见
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)

func _process(_delta: float) -> void:
	var mouse_pos = get_global_mouse_position()
	
	# 计算最终位置：鼠标位置 + 自定义偏移
	var target_pos = mouse_pos + offset_amount
	
	# 如果有纹理，可以基于纹理中心调整
	if texture:
		var tex_size = Vector2(texture.get_width(), texture.get_height())
		target_pos -= tex_size / 2  # 如果希望鼠标在图片中心，取消这行注释
	
	global_position = target_pos
