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
    let ballMass = 50.0
    let oneGee = 9.81
    let pixelsPerMeter = 10.0
    let gravity = { X = 0.0; Y = oneGee }
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

        let kstate = Keyboard.GetState()
        let getInc key = if kstate.IsKeyDown(key) then 1.0 else 0.0
        let forceValue = 2.0 * oneGee / ballMass
        let acceleration =
            { X = getInc Keys.Right - getInc Keys.Left
              Y = getInc Keys.Down - getInc Keys.Up }
            |> Vector.scale forceValue
            |> Vector.scale ballMass
            |> Vector.add gravity
        let velocity =
            acceleration
            |> Vector.scale (pixelsPerMeter * gameTime.ElapsedGameTime.TotalSeconds)
            |> Vector.add ballVelocity
        let maxPosition =
            { X = float (this.GraphicsDevice.Viewport.Width - ballTexture.Width)
              Y = float (this.GraphicsDevice.Viewport.Height - ballTexture.Height) }
        let position =
            velocity
            |> Vector.scale gameTime.ElapsedGameTime.TotalSeconds
            |> Vector.add ballPosition
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
        
        let finalPosition, finalVelocity = collide position velocity
        ballVelocity <- finalVelocity
        ballPosition <- finalPosition
        base.Update(gameTime)

    override this.Draw(gameTime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin();
        spriteBatch.Draw(ballTexture, Vector.toVector2 ballPosition, Color.White);
        spriteBatch.End();
        base.Draw(gameTime)
