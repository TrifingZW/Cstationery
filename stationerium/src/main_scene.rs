use crate::cstationery_character::CstationeryCharacter;
use godot::classes::Camera2D;
use godot::prelude::*;

#[derive(GodotClass)]
#[class(base = Node2D)]
struct MainScene {
    base: Base<Node2D>,

    #[export]
    character: Option<Gd<CstationeryCharacter>>,

    #[export]
    camera2d: Option<Gd<Camera2D>>,
}

#[godot_api]
impl INode2D for MainScene {
    fn init(base: Base<Node2D>) -> Self {
        Self {
            base,
            character: None,
            camera2d: None,
        }
    }
}
