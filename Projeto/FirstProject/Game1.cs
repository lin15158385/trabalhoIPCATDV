using FirstProject.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace FirstProject;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    private int nrLinhas = 0;
    private int nrColunas = 0;
    private Texture2D player, dot, box, wall; //Load images Texture

    private Player sokoban;
    public char[,] level;
    public List<Point> boxes = new();

    public static int tileSize = 64; //potencias de 2 (operações binárias)


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        LoadLevel("level1.txt");
        _graphics.PreferredBackBufferHeight = tileSize * (1 + level.GetLength(1)); //definição da altura
        _graphics.PreferredBackBufferWidth = tileSize * level.GetLength(0); //definição da largura
        _graphics.ApplyChanges(); //aplica a atualização da janela
        sokoban.LoadContents();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        // Use the name of your sprite font file here instead of 'File'.
        font = Content.Load<SpriteFont>("File");    
        dot = Content.Load<Texture2D>("EndPoint_Blue");
        box = Content.Load<Texture2D>("Crate_Brown");
        wall = Content.Load<Texture2D>("Wall_Brown");

    }
    void LoadLevel(string levelFile)
    {

        boxes = new List<Point>();
        string[] linhas = File.ReadAllLines($"Content/{levelFile}"); // "Content/" + level
        nrLinhas = linhas.Length;
        nrColunas = linhas[0].Length;
        level = new char[nrColunas, nrLinhas];
        for (int x = 0; x < nrColunas; x++)
        {
            for (int y = 0; y < nrLinhas; y++)
            {
                if (linhas[y][x] == '#')
                {
                    boxes.Add(new Point(x, y));
                    level[x, y] = ' '; // put a blank instead of the box '#'
                }
                else if (linhas[y][x] == 'Y')
                {
                    sokoban = new Player(this, x, y);
                    level[x, y] = ' '; // put a blank instead of the sokoban 'Y'
                }
                else
                {
                    level[x, y] = linhas[y][x];
                }
            }
        }
    }
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        if (Keyboard.GetState().IsKeyDown(Keys.R)) Initialize();

        base.Update(gameTime);
        sokoban.Update(gameTime);

        if (Victory()) Exit(); // FIXME: Change current level
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        // Draw text info
        _spriteBatch.DrawString(font, "Sokoban Game", new Vector2(0, 40), Color.White);
        _spriteBatch.DrawString(font, $"Level Size: {nrColunas}x{nrLinhas}", new Vector2(0, 0), Color.White);

        // Draw level tiles
        Rectangle position = new Rectangle(0, 0, tileSize, tileSize);
        for (int x = 0; x < level.GetLength(0); x++)
        {
            for (int y = 0; y < level.GetLength(1); y++)
            {
                position.X = x * tileSize;
                position.Y = y * tileSize;

                switch (level[x, y])
                {
                    case '.':
                        _spriteBatch.Draw(dot, position, Color.White);
                        break;
                    case 'X':
                        _spriteBatch.Draw(wall, position, Color.White);
                        break;
                }
            }
        }

        // Draw boxes
        foreach (Point b in boxes)
        {
            position.X = b.X * tileSize;
            position.Y = b.Y * tileSize;
            _spriteBatch.Draw(box, position, Color.White);
        }

        // Draw player (THIS WAS MISSING)
        sokoban.Draw(_spriteBatch);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public bool HasBox(int x, int y) // x e y é a posição do Player
    {
        foreach (Point b in boxes)
        {
            if (b.X == x && b.Y == y) return true; // se a caixa tiver a mesma posição do Player
        }
        return false;
    }
    public bool FreeTile(int x, int y)
    {
        if (level[x, y] == 'X') return false; // se for uma parede está ocupada
        if (HasBox(x, y)) return false; // verifica se é uma caixa
        return true;
        /* The same as: return level[x,y] != 'X' && !HasBox(x,y); */
    }

    public bool Victory()
    {
        foreach (Point b in boxes) // pecorrer a lista das caixas
        {
            if (level[b.X, b.Y] != '.') return false; // verifica se há caixas sem pontos
        }
        return true;
    }
}
