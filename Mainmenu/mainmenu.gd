extends Node2D
@onready var v_box_container: VBoxContainer = $VBoxContainer
@onready var panel: Panel = $Panel
@onready var panel_2: Panel = $Panel2
@onready var panel_3: Panel = $Panel3
@onready var panel_4: Panel = $Panel4
@onready var panel_5: Panel = $Panel5
@onready var audio_stream_player: AudioStreamPlayer = $AudioStreamPlayer
@onready var audio_stream_player_2: AudioStreamPlayer = $AudioStreamPlayer2

func _process(delt) :
	pass

func _ready():
	v_box_container.visible=true
	panel.visible=false
	panel_2.visible=false
	panel_3.visible=false
	panel_4.visible=false
	panel_5.visible=false
	audio_stream_player = get_node_or_null("AudioStreamPlayer")

	


func _on_play_button_down() -> void:
	panel_2.visible=true
	v_box_container.visible=false
	audio_stream_player.play()



func _on_setting_button_down() -> void:
	
	v_box_container.visible=false
	panel.visible=true
	audio_stream_player.play()


func _on_exit_button_down() -> void:
	get_tree().quit()
	audio_stream_player.play()


func _on_back_pressed() -> void:
	_ready()
	audio_stream_player.play()


# 使用预加载优化
var character_select_scene = preload("res://Select the Character screen/select_the_character_screen.tscn")

func _on_twopeople_pressed() -> void:
	# 确保这行使用制表符缩进
	panel_2.visible=false
	panel_4.visible=true
	audio_stream_player.play()


func _on_onepeople_pressed() -> void:
	# 确保这行使用制表符缩进
	panel_2.visible=false
	panel_4.visible=true
	audio_stream_player.play()


func _on_back1_pressed() -> void:
	panel_2.visible=false
	v_box_container.visible=true
	audio_stream_player.play()


func _on_play2_pressed() -> void:
	v_box_container.visible=false
	panel_3.visible=true
	audio_stream_player.play()


func _on_back_5_pressed() -> void:
	v_box_container.visible=true
	panel_3.visible=false
	audio_stream_player_2.play()


func _on_back_2_pressed() -> void:
	panel_4.visible=false
	panel_5.visible=true
	audio_stream_player_2.play()


func _on_back_3_pressed() -> void:
	var result=get_tree().change_scene_to_packed(character_select_scene)
	
