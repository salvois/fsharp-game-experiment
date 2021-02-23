namespace MyGame

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Input

type Game1() as this =
    inherit Microsoft.Xna.Framework.Game()
    let _graphics = new GraphicsDeviceManager(this)
    let mutable _spriteBatch: SpriteBatch = null
    do
        this.Content.RootDirectory <- "Content"
        base.IsMouseVisible <- true

    override this.Initialize() =
        // TODO: Add your initialization logic here
        base.Initialize()

    override this.LoadContent() =
        _spriteBatch <- new SpriteBatch(this.GraphicsDevice);
        // TODO: use this.Content to load your game content here
        ()

    override this.Update(gameTime: GameTime) =
        if GamePad.GetState(PlayerIndex.One).Buttons.Back = ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape) then
            this.Exit()
        // TODO: Add your update logic here
        base.Update(gameTime)

    override this.Draw(gameTime: GameTime) =
        this.GraphicsDevice.Clear(Color.CornflowerBlue)
        // TODO: Add your drawing code here
        base.Draw(gameTime)
