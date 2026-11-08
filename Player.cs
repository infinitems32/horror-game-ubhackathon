using Godot;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	private const float MouseSensitivity = 0.002f;

	private Camera3D camera;
	private RayCast3D paperRay;
	private Label todoLabel;
	private bool firstStormStarted;
	private bool firstStormFinished;
	private bool drankWater;
	private bool windowScarePlayed;

	public override void _Ready()
	{
		camera = GetNode<Camera3D>("Camera3D");
		paperRay = camera.GetNode<RayCast3D>("RayCast3D");
		paperRay.AddException(this);

		GetNode<Node3D>("../WindowFigure").Hide();
		GetNode<OmniLight3D>("../LightningFlash").Hide();

		var todoUi = new CanvasLayer();
		AddChild(todoUi);

		todoLabel = new Label
		{
			Position = new Vector2(24, 24),
			Text = "TO-DO LIST\n\nFinish the graphs assignment — overdue\n" +
				   "Get to work by 6:00 AM\nCheck midterm results\n\n[E] Close",
			Visible = false
		};
		todoLabel.AddThemeFontSizeOverride("font_size", 24);
		todoUi.AddChild(todoLabel);

		Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey key &&
			key.Pressed && !key.Echo && key.Keycode == Key.E)
		{
			if (todoLabel.Visible)
			{
				todoLabel.Hide();
				PlayFirstStorm();
			}
			else if (paperRay.GetCollider() is StaticBody3D target)
			{
				var targetName = target.GetParent()?.Name;

				if (targetName == "TodoPaper")
				{
					todoLabel.Show();
				}
				else if (targetName == "WaterCup" && !drankWater)
				{
					drankWater = true;
					PlayFirstStorm();
					GD.Print("Water cup detected");
				}
			}
		}

		if (@event is InputEventMouseMotion mouse &&
			Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			RotateY(-mouse.Relative.X * MouseSensitivity);
			camera.RotateX(-mouse.Relative.Y * MouseSensitivity);

			Vector3 rotation = camera.Rotation;
			rotation.X = Mathf.Clamp(
				rotation.X, Mathf.DegToRad(-80), Mathf.DegToRad(80)
			);
			camera.Rotation = rotation;
		}

		if (@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		Vector2 inputDir = Input.GetVector(
			"ui_left", "ui_right", "ui_up", "ui_down"
		);
		Vector3 direction = (
			Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)
		).Normalized();

		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();

		if ( drankWater && !windowScarePlayed &&
			paperRay.GetCollider() is StaticBody3D glass &&
			glass.GetParent()?.Name == "WindowGlass")
		{
			PlayWindowScare();
		}
	}

	private async void PlayFirstStorm()
	{
		if (firstStormStarted)
			return;

		firstStormStarted = true;

		var flash = GetNode<OmniLight3D>("../LightningFlash");
		var thunder = GetNodeOrNull<AudioStreamPlayer>("../ThunderSound");

		await ToSignal(GetTree().CreateTimer(1.0), "timeout");
		flash.Show();

		await ToSignal(GetTree().CreateTimer(0.15), "timeout");
		flash.Hide();
		thunder?.Play();

		firstStormFinished = true;
	}

	private async void PlayWindowScare()
	{
		if (windowScarePlayed)
			return;

		windowScarePlayed = true;

		var figure = GetNode<Node3D>("../WindowFigure");
		var flash = GetNode<OmniLight3D>("../LightningFlash");

		await ToSignal(GetTree().CreateTimer(0.15), "timeout");
		flash.Show();
		figure.Show();
		GetNodeOrNull<AudioStreamPlayer3D>("../ThunderSound")?.Play();
		await ToSignal(GetTree().CreateTimer(0.35), "timeout");
		figure.Hide();
		flash.Hide();
				 await ToSignal(GetTree().CreateTimer(1.0), "timeout");

var ending = new CanvasLayer { Layer = 10 };
AddChild(ending);

var blackScreen = new ColorRect { Color = Colors.Black };
ending.AddChild(blackScreen);
blackScreen.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

var title = new Label
{
	Text = "AFTER HOURS\n\nEnd of Demo",
	HorizontalAlignment = HorizontalAlignment.Center,
	VerticalAlignment = VerticalAlignment.Center
};
title.AddThemeFontSizeOverride("font_size", 36);
blackScreen.AddChild(title);
title.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);

SetPhysicsProcess(false);
SetProcessUnhandledInput(false);
Input.MouseMode = Input.MouseModeEnum.Visible;
	}
}
