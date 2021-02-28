namespace MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Vector = { X: float; Y: float }

module Vector =
    let zero = { X = 0.0; Y = 0.0 }
    let add v1 v2 = { X = v1.X + v2.X; Y = v1.Y + v2.Y }
    let scale factor v = { X = v.X * factor; Y = v.Y * factor }
    let min v1 v2 = { X = min v1.X v2.X; Y = min v1.Y v2.Y }
    let max v1 v2 = { X = max v1.X v2.X; Y = max v1.Y v2.Y }
    let toVector2 v = Vector2(float32 v.X, float32 v.Y)

type Game1() as this =
    inherit Microsoft.Xna.Framework.Game()
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch: SpriteBatch = null
    let mutable ballTexture: Texture2D = null
    let mutable ballPosition = Vector.zero
    let mutable ballVelocity = Vector.zero
    let oneGee = 9.81
    let fluidDensity = 1.2
    let dragCoefficient = 1.0
    let ballMass = 70.0
    let ballArea = 0.85
    let gravity = { X = 0.0; Y = oneGee }
    let pixelsPerMeter = 64.0 / 1.7
    do
        this.Content.RootDirectory <- "Content"
        base.IsMouseVisible <- true

    override this.Initialize() =
        base.Initialize()

    override this.LoadContent() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice);
        ballTexture <- this.Content.Load<Texture2D>("ball");

    override this.Update(gameTime: GameTime) =
        if GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) then
            this.Exit()

        let forceValue = 2.0 * oneGee * ballMass
        let maxPosition =
            { X = float (this.GraphicsDevice.Viewport.Width - ballTexture.Width) / pixelsPerMeter
              Y = float (this.GraphicsDevice.Viewport.Height - ballTexture.Height) / pixelsPerMeter }

        let kstate = Keyboard.GetState()
        let getForceDirection key = if kstate.IsKeyDown(key) then 1.0 else 0.0
        let computeDrag velocity = (if velocity < 0.0 then 0.5 else -0.5) * fluidDensity * velocity * velocity * dragCoefficient * ballArea
        let collide pos vel =
            let restitution = 0.5
            let px, vx =
                match pos.X with
                | v when v < 0.0 -> 0.0, vel.X * -restitution
                | v when v > maxPosition.X -> maxPosition.X, vel.X * -restitution
                | v -> v, vel.X
            let py, vy =
                match pos.Y with
                | v when v < 0.0 -> 0.0, vel.Y * -restitution
                | v when v > maxPosition.Y -> maxPosition.Y, vel.Y * -restitution
                | v -> v, vel.Y
            { X = px; Y = py }, { X = vx; Y = vy }

        let drag = { X = computeDrag ballVelocity.X
                     Y = computeDrag ballVelocity.Y }
        let acceleration =
            { X = getForceDirection Keys.Right - getForceDirection Keys.Left
              Y = getForceDirection Keys.Down - getForceDirection Keys.Up }
            |> Vector.scale forceValue
            |> Vector.add drag
            |> Vector.scale (1.0 / ballMass)
            |> Vector.add gravity
        let velocity =
            acceleration
            |> Vector.scale gameTime.ElapsedGameTime.TotalSeconds
            |> Vector.add ballVelocity
        let position =
            velocity
            |> Vector.scale gameTime.ElapsedGameTime.TotalSeconds
            |> Vector.add ballPosition
        
        let finalPosition, finalVelocity = collide position velocity
        ballVelocity <- finalVelocity
        ballPosition <- finalPosition
        base.Update(gameTime)

    override this.Draw(gameTime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin();
        spriteBatch.Draw(ballTexture, ballPosition |> Vector.scale pixelsPerMeter |> Vector.toVector2, Color.White);
        spriteBatch.End();
        base.Draw(gameTime)
