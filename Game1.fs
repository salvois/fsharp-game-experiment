namespace MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Game1() as this =
    inherit Microsoft.Xna.Framework.Game()
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch: SpriteBatch = null
    let mutable ballTexture: Texture2D = null
    let mutable ballPosition = Vector2.Zero
    let ballSpeed = 100.0
    let oneGee = 9.81
    let pixelsPerMeter = 10.0
    let mutable ballVelocity = Vector2.Zero
    do
        this.Content.RootDirectory <- "Content"
        base.IsMouseVisible <- true

    override this.Initialize() =
        ballPosition <- Vector2((float32)(graphics.PreferredBackBufferWidth / 2), (float32)(graphics.PreferredBackBufferHeight / 2))
        base.Initialize()

    override this.LoadContent() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice);
        ballTexture <- this.Content.Load<Texture2D>("ball");

    override this.Update(gameTime: GameTime) =
        if GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) then
            this.Exit()

        let kstate = Keyboard.GetState()
        let getInc key inc = if kstate.IsKeyDown(key) then inc else 0.0
        let xAcceleration = getInc Keys.Right oneGee - getInc Keys.Left oneGee
        let yAcceleration = getInc Keys.Down oneGee - getInc Keys.Up (2.0 * oneGee) + oneGee
        // let getInc key = if kstate.IsKeyDown(key) then 1.0 else 0.0
        // let delta = ballSpeed * gameTime.ElapsedGameTime.TotalSeconds
        // let x =
        //     float ballPosition.X + (getInc Keys.Right - getInc Keys.Left) * delta 
        //     |> max 0.0
        //     |> min (float (this.GraphicsDevice.Viewport.Width - ballTexture.Width))
        //     |> float32
        // let y =
        //     float ballPosition.Y + (getInc Keys.Down - getInc Keys.Up) * delta 
        //     |> max 0.0
        //     |> min (float (this.GraphicsDevice.Viewport.Height - ballTexture.Height))
        //     |> float32
        // ballPosition <- Vector2(x, y)
        ballVelocity <- Vector2(
            float ballVelocity.X + (xAcceleration * pixelsPerMeter * gameTime.ElapsedGameTime.TotalSeconds) |> float32,
            float ballVelocity.Y + (yAcceleration * pixelsPerMeter * gameTime.ElapsedGameTime.TotalSeconds) |> float32)
        ballPosition <- Vector2(
            float ballPosition.X + (float ballVelocity.X * gameTime.ElapsedGameTime.TotalSeconds) |> float32,
            float ballPosition.Y + (float ballVelocity.Y * gameTime.ElapsedGameTime.TotalSeconds) |> float32)
        base.Update(gameTime)

    override this.Draw(gameTime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin();
        spriteBatch.Draw(ballTexture, ballPosition, Color.White);
        spriteBatch.End();
        base.Draw(gameTime)
