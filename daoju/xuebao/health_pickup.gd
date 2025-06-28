# health_pickup.gd
extends Area2D

@export var heal_amount: int = 100
@export var respawn_time: float = 1.0  # 重生时间（秒）
@export var spawn_area_min: Vector2 = Vector2(150, 0)  # 生成区域最小坐标
@export var spawn_area_max: Vector2 = Vector2(1750, 0)  # 生成区域最大坐标
@export var spawn_y_position: float = 750 # 固定Y轴位置

var is_collected: bool = false
var original_position: Vector2  # 原始位置

func _ready():
	# 保存原始位置
	original_position = position
	
	# 连接信号 - 使用 area_entered
	area_entered.connect(_on_area_entered)
	
	# 设置碰撞层
	collision_layer = 2  # 道具在第2层
	collision_mask = 1   # 检测第1层（玩家层）
	
	# 调试输出碰撞设置
	print("血包碰撞层: ", collision_layer)
	print("血包碰撞掩码: ", collision_mask)
	
	# 首次生成时随机位置
	randomize_spawn_position()

func randomize_spawn_position():
	# 生成随机X位置
	var random_x = randf_range(spawn_area_min.x, spawn_area_max.x)
	
	# 设置新位置（保持Y轴不变）
	position = Vector2(random_x, spawn_y_position if spawn_y_position != 0.0 else original_position.y)
	print("[血包] 生成在随机位置: ", position)

func _on_area_entered(area):
	# 调试输出：进入的区域
	print("[血包] 检测到区域进入: ", area.name)
	
	# 尝试获取玩家节点
	var player = null
	
	# 方法1：检查区域的所有者是否有"Heal"方法
	var owner = area.get_owner()
	if owner and owner.has_method("Heal") and owner.is_in_group("player"):
		player = owner
		print("[血包] 通过所有者找到玩家: ", owner.name)
	
	# 方法2：检查区域父节点是否有"Heal"方法
	if !player:
		var parent = area.get_parent()
		if parent and parent.has_method("Heal") and parent.is_in_group("player"):
			player = parent
			print("[血包] 通过父节点找到玩家: ", parent.name)
	
	# 方法3：在场景树中搜索玩家组
	if !player:
		var players = get_tree().get_nodes_in_group("player")
		for p in players:
			if p.has_method("Heal"):
				player = p
				print("[血包] 通过组找到玩家: ", p.name)
				break
	
	# 确保是玩家且道具未被收集
	if not is_collected and player and player.is_in_group("player"):
		collect(player)
	else:
		if !player:
			print("[血包] 错误：未找到玩家节点")
		elif is_collected:
			print("[血包] 警告：血包已被收集")
		else:
			print("[血包] 警告：找到的节点不是玩家或不在'player'组中")

func collect(player):
	is_collected = true
	
	# 禁用碰撞，防止重复触发
	$CollisionShape2D.set_deferred("disabled", true)
	
	# 播放收集动画（如果有）
	if has_node("AnimationPlayer"):
		$AnimationPlayer.play("collect")
		# 等待动画完成
		await $AnimationPlayer.animation_finished
	else:
		# 如果没有动画，直接隐藏
		visible = false
	
	# 调试：准备调用治疗
	print("[血包] 尝试治疗玩家: ", player.name)
	print("[血包] 玩家是否有Heal方法: ", player.has_method("Heal"))
	
	# 调用玩家的治疗函数 - 注意C#方法名大小写
	if player.has_method("Heal"):  # 注意大写H
		print("[血包] 调用Heal方法")
		player.Heal(heal_amount)  # 注意大写H
	else:
		push_error("玩家对象没有Heal方法: " + str(player))
	
	# 等待一段时间后重生
	await get_tree().create_timer(respawn_time).timeout
	respawn()

func respawn():
	is_collected = false
	
	# 启用碰撞
	$CollisionShape2D.disabled = false
	
	# 设置随机位置
	randomize_spawn_position()
	
	# 播放重生动画（如果有）
	if has_node("AnimationPlayer"):
		$AnimationPlayer.play("respawn")
	else:
		# 如果没有动画，直接显示
		visible = true
