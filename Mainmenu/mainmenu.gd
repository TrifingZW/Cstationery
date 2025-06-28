extends Node2D
@onready var v_box_container: VBoxContainer = $VBoxContainer
@onready var panel: Panel = $Panel
@onready var panel_2: Panel = $Panel2

func _process(delt) :
	pass

func _ready():
	v_box_container.visible=true
	panel.visible=false
	panel_2.visible=false


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


func _on_twopeople_pressed() -> void:
# get_tree().change_scene_to_file(" ")
	print("双人")


func _on_onepeople_pressed() -> void:
# get_tree().change_scene_to_file(" ")
	print("单人")


func _on_back1_pressed() -> void:
	panel_2.visible=false
	v_box_container.visible=true
