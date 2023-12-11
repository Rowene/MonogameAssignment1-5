using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Net.Sockets;

namespace MonogameAssignment1_5
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        SoundEffect menuMusic;
        SoundEffect citySounds;
        SoundEffect wingFlap;
        SoundEffect fireBreath;

        SoundEffectInstance menuMusicIn;
        SoundEffectInstance citySoundsIn;
        SoundEffectInstance wingFlapIn;

        int fireTime = 0;
        bool dragonAppear = false;
        bool spitFireScene = false;

        Texture2D fireScreenTexture;
        Texture2D introTexture;
        Texture2D outroTexture;

        SpriteFont instructionText;

        Rectangle dragonLocation = new Rectangle(400, 200, 300, 160);

        List<Texture2D> dragonFrames = new List<Texture2D>();
        int numFrameDragon = 0;
        float dragonTimeStamp = 0;
        float dragonTime = 0;
        float dragonInterval = 0.13f;

        List <Texture2D> cityFrames = new List<Texture2D> ();

        int numCityFrames = 28;
        float cityTimeStamp = 0;
        float cityTime = 0;
        float cityInterval = 0.15f;

        List <Texture2D> spitFrames = new List<Texture2D> ();

        int numSpitFrames = 0;
        float spitTimeStamp = 0;
        float spitTime = 0;
        float spitInterval = 0.1f;

        enum Screen
        {
            intro1, intro2, main, fire, end
        }
        Screen screen;
        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);

            _graphics.PreferredBackBufferHeight = 550;
            _graphics.PreferredBackBufferWidth = 1080;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            screen = Screen.intro1;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            introTexture = Content.Load<Texture2D>("introbackground");
            instructionText = Content.Load<SpriteFont>("instruction");
            fireScreenTexture = Content.Load<Texture2D>("fire1");
            outroTexture = Content.Load<Texture2D>("outroText");

            menuMusic = Content.Load<SoundEffect>("menuMusic");
            menuMusicIn = menuMusic.CreateInstance();
            menuMusicIn.IsLooped = true;
            menuMusicIn.Volume = 1.0f;

            citySounds = Content.Load<SoundEffect>("cityAmbience");
            citySoundsIn = citySounds.CreateInstance();
            citySoundsIn.IsLooped = true;
            citySoundsIn.Volume = 0.2f;

            wingFlap = Content.Load<SoundEffect>("wingFlap");
            wingFlapIn = wingFlap.CreateInstance();
            wingFlapIn.IsLooped = true;


            fireBreath = Content.Load<SoundEffect>("dragonFire");

            

            //dragon
            for (int i = 1; i <= 4; i++)
            {
                dragonFrames.Add(Content.Load<Texture2D>("dragonframe" + i));
            }

            //city
            for (int i = 0; i <= 28; i++)
            {
                cityFrames.Add(Content.Load<Texture2D>("QHJ-" + i));
            }

            //spit fire
            for (int i = 0; i <= 44; i++)
            {
                spitFrames.Add(Content.Load<Texture2D>(""+i));
            }
        }

        protected override void Update(GameTime gameTime)
        {
            
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            MouseState mouse = Mouse.GetState();
            KeyboardState keys = Keyboard.GetState();
            if (screen == Screen.intro1) { menuMusicIn.Play(); }
            if ((screen == Screen.intro1) & (keys.IsKeyDown(Keys.Space))) {  screen = Screen.intro2; }
            else if ((screen == Screen.intro2) & (keys.IsKeyDown(Keys.Enter))){ screen = Screen.main; }
            else if (screen == Screen.main)
            {
                menuMusicIn.Stop();
                citySoundsIn.Play();
                dragonTime = (float)gameTime.TotalGameTime.TotalSeconds - dragonTimeStamp;
                if (dragonTime > dragonInterval)
                {
                    numFrameDragon++;
                    dragonTimeStamp = (float)gameTime.TotalGameTime.TotalSeconds;
                    if (numFrameDragon == 4) { numFrameDragon = 0; wingFlap.Play(); }
                }
                cityTime = (float)gameTime.TotalGameTime.TotalSeconds - cityTimeStamp;
                if (cityTime > cityInterval)
                {
                    numCityFrames--;
                    cityTimeStamp = (float)gameTime.TotalGameTime.TotalSeconds;
                    if (numCityFrames == 0) { numCityFrames = 28; }
                }

                if (keys.IsKeyDown(Keys.A) || keys.IsKeyDown(Keys.Left))
                {
                    dragonInterval = 0.08f; cityInterval = 0.06f;
                }
                else if (keys.IsKeyDown(Keys.D) || keys.IsKeyDown(Keys.Right))
                {
                    dragonInterval = 0.24f; cityInterval = 0.21f;
                }
                else
                { cityInterval = 0.15f; dragonInterval = 0.13f; }

                if (keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up))
                {
                    if (dragonLocation.Y >-25) { dragonLocation.Y--; }
                }
                else if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S))
                {
                    if (dragonLocation.Y < 420) { dragonLocation.Y++; }
                }

                if (keys.IsKeyDown(Keys.Space)) { screen = Screen.fire; }
            }
            else if (screen == Screen.fire) 
            {
                wingFlapIn.Play();
                citySoundsIn.Stop();
                fireTime++;
                if (fireTime == 60)
                {
                    fireScreenTexture = Content.Load<Texture2D>("fire2");
                    dragonLocation = new Rectangle(425, -100, 750, 540);
                    dragonAppear = true;
                }

                else if (fireTime > 95)
                {
                    dragonTime = (float)gameTime.TotalGameTime.TotalSeconds - dragonTimeStamp;
                    if (dragonTime > dragonInterval)
                    {
                        numFrameDragon++;
                        dragonTimeStamp = (float)gameTime.TotalGameTime.TotalSeconds;
                        if (numFrameDragon == 4) { numFrameDragon = 0; }
                    }
                }
                        
                if (fireTime > 95 & fireTime < 200) 
                {
                    dragonLocation.X -=4 ; dragonLocation.Y+=2;
                }
                else if (fireTime > 200)
                {
                    wingFlapIn.Stop();
                    fireBreath.Play();
                    spitFireScene = true;
                    dragonAppear = false;
                    spitTime = (float)gameTime.TotalGameTime.TotalSeconds - spitTimeStamp;
                    if (spitTime > spitInterval)
                    {
                        numSpitFrames++;
                        spitTimeStamp = (float)gameTime.TotalGameTime.TotalSeconds;
                        if (numSpitFrames == 44) { screen = Screen.end; }
                    }
                }
                
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin();

            if (screen == Screen.intro1)
            {
                _spriteBatch.Draw(introTexture, new Vector2(0, 0), Color.White);
                
            }

            else if (screen == Screen.intro2)
            {
                _spriteBatch.DrawString(instructionText, "Arrow Keys (or WASD) to Move, Space Bar to Attack!", new Vector2(50, 50), Color.DarkMagenta);
                _spriteBatch.DrawString(instructionText, "Game Ends after Attacking.", new Vector2(50, 150), Color.DarkMagenta);
                _spriteBatch.DrawString(instructionText, "Press 'Enter' to Play!", new Vector2(350, 300), Color.DarkMagenta);
            }

            else if (screen == Screen.main)
            {
                _spriteBatch.Draw(cityFrames[numCityFrames], new Rectangle(0, 0, 1080, 550), Color.White);
                _spriteBatch.Draw(dragonFrames[numFrameDragon], dragonLocation, Color.White);
            }

            else if (screen == Screen.fire)
            {
                _spriteBatch.Draw(fireScreenTexture, new Rectangle(0, 0, 1080, 550), Color.White);
                if (dragonAppear)
                { _spriteBatch.Draw(dragonFrames[numFrameDragon], dragonLocation, Color.White); }
                if (spitFireScene)
                {
                    _spriteBatch.Draw(spitFrames[numSpitFrames], new Rectangle(0, 0, 1080, 550), Color.White);
                }
            }

            else if (screen == Screen.end)
            {
                _spriteBatch.Draw(outroTexture, new Rectangle(100,100,900, 180), Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}