using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Chess
{
    

    public class StateManager
    {
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch spriteBatch;
        public ContentManager Content;
        public Board board;
       

        public Texture2D chessBoard, chessPieces;

        public StateManager(SpriteBatch spriteBatch,GraphicsDevice GraphicsDevice, ContentManager Content)
        {
            this.spriteBatch = spriteBatch;
            this.GraphicsDevice = GraphicsDevice;
            this.Content = Content;
        }

        public virtual void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            

            chessBoard = Content.Load<Texture2D>("chessboard");
            chessPieces = Content.Load<Texture2D>("chess");
        } 

        public virtual void Update()
        {

        }

        public virtual void Draw()
        {
            
        }

        public void drawBoard()
        {
            spriteBatch.Begin();
            spriteBatch.Draw(chessBoard, new Rectangle(1, 1, 960, 960), Color.White);
            spriteBatch.End();
        }

        public void drawPieces()
        {
            spriteBatch.Begin();
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {


                    if (board.square[i][j].pieceColor == PieceColor.black)
                    {
                        if (board.square[i][j].piece == Piece.rook)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(360, 0, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.knight)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(120, 0, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.bishop)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(240, 0, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.queen)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(480, 0, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.king)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(600, 0, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.pawn)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(0, 0, 120, 120), Color.White);
                    }
                    else if (board.square[i][j].pieceColor == PieceColor.white)
                    {
                        if (board.square[i][j].piece == Piece.rook)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(360, 120, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.knight)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(120, 120, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.bishop)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(240, 120, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.queen)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(480, 120, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.king)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(600, 120, 120, 120), Color.White);
                        else if (board.square[i][j].piece == Piece.pawn)
                            spriteBatch.Draw(chessPieces, new Rectangle(120 * i, 120 * j, 120, 120), new Rectangle(0, 120, 120, 120), Color.White);
                    }


                }
            spriteBatch.End();
        }

        public bool mouseInRange(MouseState mouseState)
        {
            if (mouseState.X >= 0 && mouseState.Y >= 0 && mouseState.X <= 960 && mouseState.Y <= 960)
                return true;
            return false;
        }

        
    }

}
