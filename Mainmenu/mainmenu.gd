extends Node2D
@onready var v_box_container: VBoxContainer = $VBoxContainer
@onready var panel: Panel = $Panel
@onready var panel_2: Panel = $Panel2
@onready var panel_3: Panel = $Panel3

func _process(delt) :
	pass

func _ready():
	v_box_container.visible=true
	panel.visible=false
	panel_2.visible=false
	panel_3.visible=false


func _on_play_button_down() -> void:
	panel_2.visible=true
	v_box_container.visible=false
	



func _on_setting_button_down() -> void:
	
	v_box_container.visible=false
	panel.visible=true


func _on_exit_button_down() -> void:
	get_tree().quit()


func _on_back_pressed() -> void:
	_ready()


# 使用预加载优化
var character_select_scene = preload("res://Select the Character screen/select_the_character_screen.tscn")

func _on_twopeople_pressed() -> void:
	# 确保这行使用制表符缩进
	var result = get_tree().change_scene_to_packed(character_select_scene)
	if result != OK:
		printerr("场景加载失败: ", result)

func _on_onepeople_pressed() -> void:
	# 确保这行使用制表符缩进
	var result = get_tree().change_scene_to_packed(character_select_scene)
	if result != OK:
		printerr("场景加载失败: ", result)

func _on_back1_pressed() -> void:
	panel_2.visible=false
	v_box_container.visible=true


func _on_play2_pressed() -> void:
	v_box_container.visible=false
	panel_3.visible=true


func _on_back_5_pressed() -> void:
	v_box_container.visible=true
	panel_3.visible=false
