# Item.gd
extends Area2D
class_name Item  # 确保声明类名

# 道具属性
@export var item_name: String = "道具"

# 状态
var is_picked_up: bool = false
var holder: Node = null

func _ready():
	# 连接碰撞信号
	area_entered.connect(_on_area_entered)
	
	# 设置碰撞层
	collision_layer = 2  # 道具层
	collision_mask = 1   # 玩家层

# 当玩家接触道具
func _on_area_entered(area):
	if is_picked_up: 
		return
		
	# 获取玩家节点
	var player = area.get_parent()
	
	# 确保是玩家并且可以拾取道具
	if player.is_in_group("player") and player.has_method("CanPickupItem"):
		if player.CanPickupItem():
			player.PickupItem(self)
			is_picked_up = true
			queue_free() # 销毁场景中的道具
		else:
			print("玩家无法拾取更多道具")

# 当道具被拾取
func on_picked_up(player):
	holder = player
	print("道具被拾取")

# 当道具被使用
func on_used(player):
	print("道具被使用")
	# 具体效果在子类实现
