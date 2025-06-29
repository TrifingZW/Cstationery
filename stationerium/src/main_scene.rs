use godot::classes::Camera2D;
use godot::prelude::*;

#[derive(GodotClass)]
#[class(base = Node2D)]
struct MainScene {
    base: Base<Node2D>,

    #[export]
    characters: Array<Gd<Node2D>>,

    #[export]
    camera2d: Option<Gd<Camera2D>>,

    #[export]
    margin: f32,

    #[export]
    position_interpolation_speed: f32,

    #[export]
    zoom_interpolation_speed: f32,
}

#[godot_api]
impl INode2D for MainScene {
    fn init(base: Base<Node2D>) -> Self {
        Self {
            base,
            characters: Array::new(),
            camera2d: None,
            margin: 100.0, // 默认边距为10
            position_interpolation_speed: 5.0, // 位置过渡速度
            zoom_interpolation_speed: 3.0, // 缩放过渡速度
        }
    }

    fn process(&mut self, _delta: f64) {
        // 每帧更新摄像机位置
        self.update_camera_position();
    }

    fn ready(&mut self) {
        // 初始化时更新摄像机位置
        self.update_camera_position();
    }
}

#[godot_api]
impl MainScene {
    fn update_camera_position(&mut self) {
        // 如果没有角色，不进行更新
        if self.characters.is_empty() {
            return;
        }

        // 如果没有摄像机则直接返回
        if self.camera2d.is_none() {
            return;
        }

        // 计算所有角色的边界框
        let mut min_x = f32::MAX;
        let mut min_y = f32::MAX;
        let mut max_x = f32::MIN;
        let mut max_y = f32::MIN;

        for character in self.characters.iter_shared() {
            let position = character.get_global_position();
            min_x = min_x.min(position.x);
            min_y = min_y.min(position.y);
            max_x = max_x.max(position.x);
            max_y = max_y.max(position.y);
        }

        // 添加边距
        min_x -= self.margin;
        min_y -= self.margin;
        max_x += self.margin;
        max_y += self.margin;

        // 计算中心点
        let center_x = (min_x + max_x) / 2.0;
        let center_y = (min_y + max_y) / 2.0;

        // 预先获取viewport信息以避免借用冲突
        let viewport = self.base().get_viewport().unwrap();
        let viewport_size = viewport.get_visible_rect().size;

        // 计算缩放比例
        let width_ratio = (max_x - min_x) / viewport_size.x;
        let height_ratio = (max_y - min_y) / viewport_size.y;

        // 取较大的比例，确保所有内容都可见
        let zoom_scale = f32::max(width_ratio, height_ratio);

        // 防止缩放太小或太大
        /*let zoom_scale = f32::max(0.1, zoom_scale);
        let zoom_scale = f32::min(2.0, zoom_scale);
*/
        // 计算目标缩放向量
        let target_zoom = if zoom_scale > 1.0 {
            Vector2::ONE / zoom_scale
        } else {
            Vector2::ONE
        };

        // 预先获取 delta_time 以避免借用冲突
        let delta_seconds = self.base().get_process_delta_time() as f32;
        let position_lerp_factor = f32::min(1.0, self.position_interpolation_speed * delta_seconds);
        let zoom_lerp_factor = f32::min(1.0, self.zoom_interpolation_speed * delta_seconds);

        // 现在可以安全地获取camera的可变引用
        if let Some(camera) = &mut self.camera2d {
            // 计算目标位置
            let target_position = Vector2::new(center_x, center_y);

            // 获取当前位置和缩放
            let current_position = camera.get_global_position();
            let current_zoom = camera.get_zoom();

            // 插值计算新位置和缩放
            let new_position = current_position.lerp(target_position, position_lerp_factor);
            let new_zoom = current_zoom.lerp(target_zoom, zoom_lerp_factor);

            // 应用新的位置和缩放
            camera.set_global_position(new_position);
            camera.set_zoom(new_zoom);
        }
    }
}
