namespace MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Game1() as this =
    inherit Microsoft.Xna.Framework.Game()
    let graphics = new GraphicsDeviceManager(this)
    let mutable spriteBatch: SpriteBatch = null
    let mutable ballTexture: Texture2D = null
    do
        this.Content.RootDirectory <- "Content"
        base.IsMouseVisible <- true

    override this.Initialize() =
        // TODO: Add your initialization logic here
        base.Initialize()

    override this.LoadContent() =
        spriteBatch <- new SpriteBatch(this.GraphicsDevice);
        ballTexture <- this.Content.Load<Texture2D>("ball");

    override this.Update(gameTime: GameTime) =
        if GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) then
            this.Exit()
        // TODO: Add your update logic here
        base.Update(gameTime)

    override this.Draw(gameTime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        spriteBatch.Begin();
        spriteBatch.Draw(ballTexture, Vector2(0f, 0f), Color.White);
        spriteBatch.End();
        base.Draw(gameTime)
