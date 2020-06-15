using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chess
{
    enum GameState {pvp,pcvpc,pcvp };
   
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GameState gameState;
        PVP pvp;
        PCVPC pcvpc;
        PCVP pcvp;

        
        
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 962;
            graphics.PreferredBackBufferWidth = 962;
            graphics.ApplyChanges();

            
        }

        
        protected override void Initialize()
        {

            this.IsMouseVisible = true;

            pvp = new PVP(spriteBatch, GraphicsDevice, Content);
            pcvpc = new PCVPC(spriteBatch, GraphicsDevice, Content);
            pcvp = new PCVP(spriteBatch, GraphicsDevice, Content);
            gameState = GameState.pcvp;

            
            base.Initialize();
        }

        
        protected override void LoadContent()
        {
          
            spriteBatch = new SpriteBatch(GraphicsDevice);

            pvp.LoadContent();
            pcvpc.LoadContent();
            pcvp.LoadContent();
            
        }

       
        protected override void UnloadContent()
        {
            
        }

       
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (this.IsActive)
            {
                switch (gameState)
                {
                    case GameState.pvp:
                        pvp.Update();
                        break;
                    case GameState.pcvpc:
                        pcvpc.Update();
                        break;
                    case GameState.pcvp:
                        pcvp.Update();
                        break;

                }
            }

            base.Update(gameTime);
        }

       
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            switch (gameState)
            {
                case GameState.pvp:
                    pvp.Draw();
                    break;
                case GameState.pcvpc:
                    pcvpc.Draw();
                    break;
                case GameState.pcvp:
                    pcvp.Draw();
                    break;

            }

            base.Draw(gameTime);
        }
    }
}
