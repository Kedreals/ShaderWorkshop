using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Workshop
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameObject m_Tree;
        GameObject m_Cat;
        GameObject m_Schorsch;
        GameObject m_Plane;

        Matrix m_View;
        Matrix m_Projection;

        Vector3 m_Directional_Light;
        float? m_ell = null;
        
        float m_ambient_light_ell;

        Camera m_Camera;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            m_Camera = new Camera(new Vector3(1, 1, 5), new Vector3(0, 2, 0), Vector3.Up, MathHelper.ToRadians(60));

            m_Directional_Light = new Vector3(-10, 10, 2);
            baseLightPos = m_Directional_Light;
            //m_ell = 150;
            m_ambient_light_ell = 0.25f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Effect effect = Content.Load<Effect>("NormalMapping");

            m_Cat = new GameObject(Content.Load<Model>("Cat"), effect, Content.Load<Texture2D>("CatTexture"), Content.Load<Texture2D>("CatNormalMap"));
            m_Schorsch = new GameObject(Content.Load<Model>("Schorsch"), effect, Content.Load<Texture2D>("SchorschTexture"), Content.Load<Texture2D>("SchorschNormalMap"));
            m_Tree = new GameObject(Content.Load<Model>("Birch"), effect, Content.Load<Texture2D>("BirchTexture1"), Content.Load<Texture2D>("BirchNormalMap"));
            m_Plane = new GameObject(Content.Load<Model>("Plane"), effect, Content.Load<Texture2D>("PlaneTexture"), Content.Load<Texture2D>("PlaneNormalMap"));

            m_Cat.Position = new Vector3(0.5f, 0, 2);
            m_Schorsch.Position = new Vector3(1, 0, 1);
            m_Tree.Position = new Vector3(0, -0.5f, 1);
        }

        float t = 0.0f;
        Vector3 baseLightPos;

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            t += (float)gameTime.ElapsedGameTime.TotalSeconds;

            m_Directional_Light = Vector3.Transform(baseLightPos, Matrix.CreateRotationY(-t*2*MathHelper.Pi/60.0f)) + ((float)Math.Cos(t * 2 * MathHelper.Pi / 60.0f) - 0.8f) * 10.0f * Vector3.Up;
            m_Directional_Light = m_Directional_Light - Vector3.Zero;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            m_Plane.Render(m_Camera.View, m_Camera.Projection, 20, 20, m_Directional_Light, m_ell, m_ambient_light_ell);
            m_Cat.Render(m_Camera.View, m_Camera.Projection, 1, 1, m_Directional_Light, m_ell, m_ambient_light_ell);
            m_Tree.Render(m_Camera.View, m_Camera.Projection, 1, 1, m_Directional_Light, m_ell, m_ambient_light_ell);
            m_Schorsch.Render(m_Camera.View, m_Camera.Projection, 1, 1, m_Directional_Light, m_ell, m_ambient_light_ell);

            base.Draw(gameTime);
        }
    }
}
