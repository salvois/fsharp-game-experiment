open System

[<EntryPoint>]
let main argv =
    use game = new MyGame.Game1()
    game.Run()
    0
