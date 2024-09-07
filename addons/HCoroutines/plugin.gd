@tool
extends EditorPlugin


const AUTOLOAD_NAME = "CoroutineManager"


func _enter_tree() -> void:
	add_autoload_singleton(AUTOLOAD_NAME, "res://addons/HCoroutines/coroutine_manager.tscn")


func _exit_tree() -> void:
	remove_autoload_singleton(AUTOLOAD_NAME)
