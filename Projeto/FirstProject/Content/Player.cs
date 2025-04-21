using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using static FirstProject.Game1;

namespace FirstProject.Content
{
    internal class Player
    {
        private Texture2D[][] sprites;
        private Game1 game;
        private bool keysReleased = true;


        enum Direction
        {
            Up, Down, Left, Right // 0, 1, 2, 3
        }
        public const int tileSize = 64; //potencias de 2 (operações binárias)

        private Direction direction = Direction.Down;
        // Current player position in the matrix (multiply by tileSize prior to drawing)

        private Point position; //Point = Vector2, mas são inteiros
        public Point Position => position; //auto função (equivalente a ter só get sem put) - AUTOPROPERTY
                                           //public Vector2 Position
                                           //{
                                           // get{return position;}
                                           //}
        public Player(Game1 game1, int x, int y) //constructor que dada a as posições guarda a sua posição
        {
            position = new Point(x, y);
            game = game1;

        }
        public void Update(GameTime gameTime)
        {
            Point lastPosition = position;
            KeyboardState kState = Keyboard.GetState();
            if ((kState.IsKeyDown(Keys.A)) || (kState.IsKeyDown(Keys.Left)))
            {
                position.X--;
                //game.direction = Direction.Left;
                direction = Direction.Left;
            }
            else if ((kState.IsKeyDown(Keys.W)) || (kState.IsKeyDown(Keys.Up)))
            {
                position.Y--;
                //game.direction = Direction.Up;
                direction = Direction.Up;
            }
            else if ((kState.IsKeyDown(Keys.S)) || (kState.IsKeyDown(Keys.Down)))
            {
                position.Y++;
                //game.direction = Direction.Down;
                direction = Direction.Down;
            }
            else if ((kState.IsKeyDown(Keys.D)) || (kState.IsKeyDown(Keys.Right)))
            {
                position.X++;
                //game.direction = Direction.Right;
                direction = Direction.Right;
            }
            else
            {
                if (kState.IsKeyUp(Keys.A) && kState.IsKeyUp(Keys.W) &&
                kState.IsKeyUp(Keys.S) && kState.IsKeyUp(Keys.D))
                {
                    keysReleased = true;
                }
            }

            // destino é caixa?
            if (game.HasBox(position.X, position.Y))
            {
                int deltaX = position.X - lastPosition.X;
                int deltaY = position.Y - lastPosition.Y;
                Point boxTarget = new Point(deltaX + position.X, deltaY + position.Y);
                // se sim, caixa pode mover-se?
                if (game.FreeTile(boxTarget.X, boxTarget.Y))
                {
                    for (int i = 0; i < game.boxes.Count; i++)
                    {
                        if (game.boxes[i].X == position.X && game.boxes[i].Y == position.Y)
                        {
                            game.boxes[i] = boxTarget;
                        }
                    }
                }
                else
                {
                    position = lastPosition;
                }
            }
            else
            {
                // se não é caixa, se não está livre, parado!
                if (!game.FreeTile(position.X, position.Y))
                    position = lastPosition;
            }
        }

        public void LoadContents()
        {
            sprites = new Texture2D[4][];
            sprites[(int)Direction.Up] = new Texture2D[]
                {
                game.Content.Load<Texture2D>("Character7"),
                game.Content.Load<Texture2D>("Character8"),
                game.Content.Load<Texture2D>("Character9")
                };
            sprites[(int)Direction.Down] = new Texture2D[] {
                game.Content.Load<Texture2D>("Character4"),
                game.Content.Load<Texture2D>("Character5"),
                game.Content.Load<Texture2D>("Character6") };
            sprites[(int)Direction.Left] = new Texture2D[] {
                game.Content.Load<Texture2D>("Character1"),
                game.Content.Load<Texture2D>("Character10") };
            sprites[(int)Direction.Right] = new Texture2D[] {
                game.Content.Load<Texture2D>("Character2"),
                game.Content.Load<Texture2D>("Character3") };   
        }


        public void Draw(SpriteBatch sb)
        {
            Rectangle rect = new Rectangle(Game1.tileSize * position.X,
                                     Game1.tileSize * position.Y,
                                    Game1.tileSize, Game1.tileSize);
            sb.Draw(sprites[(int)direction][0], rect, Color.White); //desenha o Player
        }


    }
}
