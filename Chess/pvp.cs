using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;


namespace Chess
{
    

    public class PVP : StateManager
    {
        MouseState mouseState, previousMS;
        Texture2D border, highlight, promotion, allow;
        bool isSelected;
        bool gameStarted;
        Point selectedCoords, previousCoordsStart,previousCoordsEnd;
        bool whiteTurn;
        
        


        public PVP(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, ContentManager Content): base( spriteBatch,GraphicsDevice, Content)
        {
            isSelected = false;
            gameStarted = false;
            whiteTurn = true;
           
            selectedCoords = new Point();
        }

        public override void LoadContent()
        {
           
            base.LoadContent();
            board = new Board();
            board.startGame();
            
            previousCoordsEnd = new Point();
            previousCoordsStart = new Point();

            highlight = Content.Load<Texture2D>("highlight");
            promotion = Content.Load<Texture2D>("promotion");
            allow = Content.Load<Texture2D>("allow");

            border = new Texture2D(GraphicsDevice, 120, 120);

            Color[] data = new Color[14400];
            for (int i = 0; i < 120; i++)
                for (int j = 0; j < 120; j++)
                {
                    if (i < 3 || i > 116)
                        data[120 * i + j] = Color.SteelBlue;
                    else if (j < 3 || j > 116)
                        data[120 * i + j] = Color.SteelBlue;
                    else
                        data[120 * i + j] = Color.Transparent;


                }

            border.SetData(data);

            

        }

        public override void Update()
        {
            mouseState = Mouse.GetState();

            Point promotionCoords = board.promotionCoords();
            if (board.isPromotion() && mouseInRange(mouseState) && mouseState.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released && mouseState.Y >= 60 + 120 * promotionCoords.Y && mouseState.Y <= 120 + 120 * promotionCoords.Y && mouseState.X >= 60 + 120 * promotionCoords.X && mouseState.X <= 300 + 120 * promotionCoords.X)
            {

                if (mouseState.X >= 60 + 120 * promotionCoords.X && mouseState.X <= 120 + 120 * promotionCoords.X)
                {
                    board.promote(Piece.queen);
                }
                else if (mouseState.X >= 120 + 120 * promotionCoords.X && mouseState.X <= 180 + 120 * promotionCoords.X)
                {
                    board.promote(Piece.rook);
                }
                else if (mouseState.X >= 180 + 120 * promotionCoords.X && mouseState.X <= 240 + 120 * promotionCoords.X)
                {
                    board.promote(Piece.bishop);
                }
                else if (mouseState.X >= 240 + 120 * promotionCoords.X && mouseState.X <= 300 + 120 * promotionCoords.X)
                {
                    board.promote(Piece.knight);
                }

                whiteTurn = !whiteTurn;
            }


            if (!board.isPromotion() && mouseInRange(mouseState) && mouseState.LeftButton==ButtonState.Pressed && previousMS.LeftButton==ButtonState.Released && !isSelected && board.square[mouseState.X / 120][mouseState.Y / 120].piece!=Piece.empty && (whiteTurn && board.square[mouseState.X / 120][mouseState.Y / 120].pieceColor==PieceColor.white || !whiteTurn && board.square[mouseState.X / 120][mouseState.Y / 120].pieceColor == PieceColor.black))
            {
                selectedCoords = new Point(mouseState.X/120, mouseState.Y/120);
                isSelected = true;
            }
            else if (!board.isPromotion() && mouseInRange(mouseState) && isSelected && mouseState.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released && (whiteTurn && board.square[selectedCoords.X][selectedCoords.Y].pieceColor == PieceColor.white || !whiteTurn && board.square[selectedCoords.X][selectedCoords.Y].pieceColor == PieceColor.black))
            {
                if ((whiteTurn && board.isWhiteDown || !whiteTurn && !board.isWhiteDown) )
                {
                    if (board.isPositionPossible(selectedCoords.X, selectedCoords.Y, mouseState.X / 120, mouseState.Y / 120))
                    {
                        board.makeMove(new Point(selectedCoords.X, selectedCoords.Y), new Point(mouseState.X / 120, mouseState.Y / 120));
                        if(!board.isPromotion())
                            whiteTurn = !whiteTurn;
                        previousCoordsStart = new Point(selectedCoords.X, selectedCoords.Y);
                        previousCoordsEnd = new Point(mouseState.X / 120, mouseState.Y / 120);
                        if (!gameStarted)
                            gameStarted = true;
                        
                    }

                }
                else if (!whiteTurn && board.isWhiteDown || whiteTurn && !board.isWhiteDown)
                {
                    board.rotate();
                    if (board.isPositionPossible(7 - selectedCoords.X, 7 - selectedCoords.Y, 7 - mouseState.X / 120, 7 - mouseState.Y / 120))
                    {

                        board.makeMove(new Point(7 - selectedCoords.X, 7 - selectedCoords.Y), new Point(7 - mouseState.X / 120, 7 - mouseState.Y / 120));
                        if (!board.isPromotion())
                            whiteTurn = !whiteTurn;
                        previousCoordsStart = new Point(selectedCoords.X, selectedCoords.Y);
                        previousCoordsEnd = new Point(mouseState.X / 120, mouseState.Y / 120);
                        if (!gameStarted)
                            gameStarted = true;
                    }

                    board.rotate();
                }

                
                if(whiteTurn && board.square[mouseState.X / 120][mouseState.Y / 120].pieceColor == PieceColor.white || !whiteTurn && board.square[mouseState.X / 120][mouseState.Y / 120].pieceColor == PieceColor.black)
                    selectedCoords = new Point(mouseState.X / 120, mouseState.Y / 120);
                else
                    isSelected = false;
            }
            else if (!board.isPromotion() && mouseInRange(mouseState) && isSelected && mouseState.LeftButton == ButtonState.Pressed && previousMS.LeftButton == ButtonState.Released)
                isSelected = false;

            

            previousMS = mouseState;
            base.Update();
        }

        public override void Draw()
        {
            drawBoard();
            spriteBatch.Begin();
            if (isSelected)
            {
                spriteBatch.Draw(border, new Rectangle(1 + selectedCoords.X * 120, 1 + selectedCoords.Y * 120, 120, 120), Color.White);
                
            }


            if (gameStarted)
            {
                float opacity;
                if ((previousCoordsStart.X + previousCoordsStart.Y) % 2 == 0)
                    opacity = 0.2f;
                else
                    opacity = 0.7f;
                spriteBatch.Draw(highlight, new Rectangle(1 + previousCoordsStart.X * 120, 1 + previousCoordsStart.Y * 120, 120, 120), Color.White * opacity);

                if ((previousCoordsEnd.X + previousCoordsEnd.Y) % 2 == 0)
                    opacity = 0.2f;
                else
                    opacity = 0.7f;
                spriteBatch.Draw(highlight, new Rectangle(1 + previousCoordsEnd.X * 120, 1 + previousCoordsEnd.Y * 120, 120, 120), Color.White * opacity);
            }

            spriteBatch.End();

            drawPieces();

            if (isSelected)
            {
                spriteBatch.Begin();
                List<int> possibleMoves;
                if ((whiteTurn && board.isWhiteDown || !whiteTurn && !board.isWhiteDown))
                    possibleMoves = board.possibleMoves(selectedCoords.X, selectedCoords.Y);
                else
                {
                    board.rotate();
                    possibleMoves = board.possibleMoves(7 - selectedCoords.X,7 -selectedCoords.Y);
                    board.rotate();
                   
                    for(int i=0; i< possibleMoves.Count();i++)
                    {
                        
                        possibleMoves[i] = (7 - possibleMoves[i] / 10) * 10 + 7 - possibleMoves[i] % 10;
                    }

                }
                foreach (int i in possibleMoves)
                {
                    spriteBatch.Draw(allow, new Rectangle(1 + i / 10 * 120, 1 + i % 10 * 120, 120, 120), Color.White);
                }
                spriteBatch.End();

            }

            if (board.isPromotion())
            {
                spriteBatch.Begin();
                Point promotionCoords = board.promotionCoords();
                spriteBatch.Draw(promotion, new Rectangle(60 + 120 * promotionCoords.X, 60 + 120 * promotionCoords.Y, 240, 60), Color.White);
                spriteBatch.Draw(chessPieces, new Rectangle(60 + 120 * promotionCoords.X, 60 + 120 * promotionCoords.Y, 60, 60), new Rectangle(480, 0, 120, 120),Color.White);
                spriteBatch.Draw(chessPieces, new Rectangle(120 + 120 * promotionCoords.X, 60 + 120 * promotionCoords.Y, 60, 60), new Rectangle(360, 0, 120, 120), Color.White);
                spriteBatch.Draw(chessPieces, new Rectangle(180 + 120 * promotionCoords.X, 60 + 120 * promotionCoords.Y, 60, 60), new Rectangle(240, 0, 120, 120), Color.White);
                spriteBatch.Draw(chessPieces, new Rectangle(240 + 120 * promotionCoords.X, 60 + 120 * promotionCoords.Y, 60, 60), new Rectangle(120, 0, 120, 120), Color.White);

                spriteBatch.End();
            }
            base.Draw();
        }

        

    }
}
