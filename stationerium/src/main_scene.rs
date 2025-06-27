use godot::classes::{INode2D, Node2D};
use godot::prelude::*;

#[derive(GodotClass)]
#[class(base = Node2D)]
struct MainScene {
    base: Base<Node2D>,
}

#[godot_api]
impl INode2D for MainScene {
    fn init(base: Base<Node2D>) -> Self {
        Self { base }
    }
}
