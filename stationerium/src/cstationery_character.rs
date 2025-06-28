use godot::prelude::*;
use godot::classes::{CharacterBody2D, ICharacterBody2D};

#[derive(GodotClass)]
#[class(base = CharacterBody2D)]
pub struct CstationeryCharacter {
    base: Base<CharacterBody2D>,
}

#[godot_api]
impl ICharacterBody2D for CstationeryCharacter {
    fn init(base: Base<CharacterBody2D>) -> Self {
        Self { base }
    }
}