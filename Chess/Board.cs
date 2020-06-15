using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public enum Piece : int { knight = 55, bishop = 56, pawn = 30 , king = 50, queen = 99, rook = 75, empty = 0 };
    public enum PieceColor : int { white = 1, black = -1, empty = 0};

    public class Board 
    {
        public List<List<Square>> square;
        public bool isWhiteDown;
        public int repetitions;

        public Board()
        {
            isWhiteDown = true;
            repetitions = 0;
            square = new List<List<Square>>();
            for (int i = 0; i < 8; i++)
            {
                square.Add(new List<Square>());
                for (int j = 0; j < 8; j++)
                {
                    square[i].Add(new Square());
                }
            }
            
        }

        public void startGame()
        {
            

            square[0][0].piece = Piece.rook;
            square[1][0].piece = Piece.knight;
            square[2][0].piece = Piece.bishop;
            square[3][0].piece = Piece.queen;
            square[4][0].piece = Piece.king;
            square[5][0].piece = Piece.bishop;
            square[6][0].piece = Piece.knight;
            square[7][0].piece = Piece.rook;
            square[0][7].piece = Piece.rook;
            square[1][7].piece = Piece.knight;
            square[2][7].piece = Piece.bishop;
            square[3][7].piece = Piece.queen;
            square[4][7].piece = Piece.king;
            square[5][7].piece = Piece.bishop;
            square[6][7].piece = Piece.knight;
            square[7][7].piece = Piece.rook;


            for (int i = 0; i < 8; i++)
            {
                square[i][0].pieceColor = PieceColor.black;
                square[i][7].pieceColor = PieceColor.white;
                square[i][1].piece = Piece.pawn;
                square[i][6].piece = Piece.pawn;
                
                square[i][1].pieceColor = PieceColor.black;
                square[i][6].pieceColor = PieceColor.white;
                
               
            }

        }

        public void rotate()
        {
            Piece tempPiece = Piece.empty;
            PieceColor tempPieceColor = PieceColor.empty;
            int temp = 0;
            for(int i=0;i<4;i++)
                for(int j=0;j<8;j++)
                {
                    tempPiece = square[7 - i][j].piece;
                    tempPieceColor = square[7 - i][j].pieceColor;
                    temp = square[7 - i][j].count;
                    square[7 - i][j].piece = square[i][j].piece;
                    square[7 - i][j].pieceColor = square[i][j].pieceColor;
                    square[7 - i][j].count = square[i][j].count;
                    square[i][j].piece = tempPiece;
                    square[i][j].pieceColor = tempPieceColor;
                    square[i][j].count = temp;
                }
            
            for (int j = 0; j < 4; j++)
                for (int i = 0; i < 8; i++)
                {
                    tempPiece = square[i][7 - j].piece;
                    tempPieceColor = square[i][7 - j].pieceColor;
                    temp = square[i][7 - j].count;
                    square[i][7 - j].piece = square[i][j].piece;
                    square[i][7 - j].pieceColor = square[i][j].pieceColor;
                    square[i][7 - j].count = square[i][j].count;
                    square[i][j].piece = tempPiece;
                    square[i][j].pieceColor = tempPieceColor;
                    square[i][j].count = temp;
                }
            isWhiteDown = !isWhiteDown;
        }

        public bool isPositionPossible(int i, int j, int k, int l)
        {
            if (i < 0 || i > 7 || j < 0 || j > 7 || k < 0 || k > 7 || l < 0 || l > 7)
                return false;

            if (i == k && j == l)
                return false;

            // Castling
            if (square[i][j].piece == Piece.king && square[i][j].count == 0)
            {
                
                if (k - i == 2 && j - l == 0 && square[7][7].count == 0 && !amongInLine(new Point(i,j),new Point(7,7)))
                {
                    if (!isKingInCheck(i, j, i, j) && !isKingInCheck(i, j, i + 1, j) && !isKingInCheck(i, j, i + 2, j))
                        return true;
                    

                }
                else if(i - k == 2 && j - l == 0 && square[0][7].count == 0 && !amongInLine(new Point(i, j), new Point(0, 7)))
                {
                    if (!isKingInCheck(i, j, i, j) && !isKingInCheck(i, j, i - 1, j) && !isKingInCheck(i, j, i - 2, j))
                        return true;
                }
            }

            // En passant
            if (square[i][j].piece == Piece.pawn && j == 3 && l==2 && (i-k==1 && square[i - 1][j].piece == Piece.pawn && !isAlly(i - 1, j) && square[i - 1][j].count == 1 || k-i==1 && square[i + 1][j].piece == Piece.pawn && !isAlly(i + 1, j) && square[i + 1][j].count == 1))
                return true;

            if (isAlly(k,l))
                return false;

            if (square[i][j].piece == Piece.knight)
                if ((Math.Abs(k-i)!=1 || Math.Abs(l-j)!=2) && ( Math.Abs(k-i)!=2 || Math.Abs(l-j)!=1))
                    return false;

            if (square[i][j].piece == Piece.bishop)
                if (Math.Abs(k - i) != Math.Abs(l - j) || amongInLine(new Point(i, j), new Point(k, l)))
                    return false;

            if (square[i][j].piece == Piece.rook)
                if (Math.Abs(k - i) != 0 && Math.Abs(l - j) != 0 || amongInLine(new Point(i, j), new Point(k, l)))
                    return false;

            if (square[i][j].piece == Piece.queen)
                if (Math.Abs(k - i) != 0 && Math.Abs(l - j) != 0 && Math.Abs(k - i) != Math.Abs(l - j) || amongInLine(new Point(i, j), new Point(k, l)))
                    return false;

            if (square[i][j].piece == Piece.pawn)
                if (( k - i != 0 || j-l!=1 || square[k][l].piece != Piece.empty) && (Math.Abs(k - i) != 1 || j-l != 1 || square[k][l].piece == Piece.empty) && (j-l!=2 || k-i!=0 || j!=6 || square[k][l].piece != Piece.empty || square[k][l+1].piece != Piece.empty))
                    return false;

            if (square[i][j].piece == Piece.king)
                if (Math.Abs(k - i) > 1 || Math.Abs(l - j) > 1)
                    return false;

            if (isKingInCheck(i, j, k, l))
                return false;

            



            return true;
        }

        public bool isStaleMate()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (isAlly(i, j) && possibleMoves(i, j).Count != 0)
                        return false;

            if (isCheckMate())
                return false; 

            return true;

        }

        public bool isDraw()
        {
            if (repetitions >= 50)
                return true;
            return false;
        }

        public bool isAlly(int k, int l)
        {
            if (square[k][l].pieceColor == PieceColor.white && isWhiteDown || square[k][l].pieceColor == PieceColor.black && !isWhiteDown)
                return true;
            return false;
        }

        public bool isKingInCheck(int i, int j, int k, int l)
        {
            Piece tempPiece = square[k][l].piece;
            PieceColor tempPieceColor = square[k][l].pieceColor;

            square[k][l].piece = square[i][j].piece;
            square[k][l].pieceColor = square[i][j].pieceColor;
            if (i != k || j!=l)
            {
                square[i][j].piece = Piece.empty;
                square[i][j].pieceColor = PieceColor.empty;
            }
            Point king = findKing();


            for (int m = 0; m <= Math.Min(7 - king.X, 7 - king.Y) + Math.Min(king.X, king.Y); m++)
            {
                if ((square[king.X - Math.Min(king.X, king.Y) + m][king.Y - Math.Min(king.X, king.Y) + m].piece == Piece.queen || square[king.X - Math.Min(king.X, king.Y) + m][king.Y - Math.Min(king.X, king.Y) + m].piece == Piece.bishop) && !isAlly(king.X - Math.Min(king.X, king.Y) + m, king.Y - Math.Min(king.X, king.Y) + m) && !amongInLine(new Point(king.X - Math.Min(king.X, king.Y) + m, king.Y - Math.Min(king.X, king.Y) + m), new Point(king.X, king.Y)) || (Math.Min(king.X, king.Y) - m == 1 && (square[king.X - 1][king.Y - 1].piece == Piece.king || square[king.X - 1][king.Y - 1].piece == Piece.pawn) && !isAlly(king.X - 1,king.Y - 1)) || m - Math.Min(king.X, king.Y) == 1 && square[king.X + 1][king.Y + 1].piece == Piece.king && !isAlly(king.X + 1,king.Y + 1))
                {
                    square[i][j].piece = square[k][l].piece;
                    square[i][j].pieceColor = square[k][l].pieceColor;
                    square[k][l].piece = tempPiece;
                    square[k][l].pieceColor = tempPieceColor;
                    return true;
                }
            }


            for (int m = 0; m <= Math.Min(king.X, 7 - king.Y) + Math.Min(7 - king.X, king.Y); m++)
            {
                if ((square[king.X - Math.Min(king.X, 7 - king.Y) + m][king.Y + Math.Min(king.X, 7 - king.Y) - m].piece == Piece.queen || square[king.X - Math.Min(king.X, 7 - king.Y) + m][king.Y + Math.Min(king.X, 7 - king.Y) - m].piece == Piece.bishop) && !isAlly(king.X - Math.Min(king.X, 7 - king.Y) + m,king.Y + Math.Min(king.X, 7 - king.Y) - m) && !amongInLine(new Point(king.X - Math.Min(king.X, 7 - king.Y) + m, king.Y + Math.Min(king.X, 7 - king.Y) - m), new Point(king.X, king.Y)) || (m - Math.Min(king.X, 7 - king.Y) == 1 && (square[king.X + 1][king.Y - 1].piece == Piece.king || square[king.X + 1][king.Y - 1].piece == Piece.pawn) && !isAlly(king.X + 1,king.Y - 1)) || m - Math.Min(king.X, 7 - king.Y) == -1 && square[king.X - 1][king.Y + 1].piece == Piece.king && !isAlly(king.X - 1,king.Y + 1))
                {
                    square[i][j].piece = square[k][l].piece;
                    square[i][j].pieceColor = square[k][l].pieceColor;
                    square[k][l].piece = tempPiece;
                    square[k][l].pieceColor = tempPieceColor;
                    return true;
                }
            }

            for (int m = 0; m < 8; m++)
            {
                if ((square[m][king.Y].piece == Piece.queen || square[m][king.Y].piece == Piece.rook) && !isAlly(m,king.Y) && !amongInLine(new Point(king.X, king.Y), new Point(m, king.Y)) || Math.Abs(king.X - m) == 1 && square[m][king.Y].piece == Piece.king && !isAlly(m,king.Y))
                {
                    square[i][j].piece = square[k][l].piece;
                    square[i][j].pieceColor = square[k][l].pieceColor;
                    square[k][l].piece = tempPiece;
                    square[k][l].pieceColor = tempPieceColor;
                    return true;
                }
            }

            for (int m = 0; m < 8; m++)
            {
                if ((square[king.X][m].piece == Piece.queen || square[king.X][m].piece == Piece.rook) && !isAlly(king.X,m) && !amongInLine(new Point(king.X, king.Y), new Point(king.X, m)) || Math.Abs(king.Y - m) == 1 && square[king.X][m].piece == Piece.king && !isAlly(king.X,m))
                {
                    square[i][j].piece = square[k][l].piece;
                    square[i][j].pieceColor = square[k][l].pieceColor;
                    square[k][l].piece = tempPiece;
                    square[k][l].pieceColor = tempPieceColor;
                    return true;
                }
            }



            if (king.X + 2 < 8 && king.Y + 1 < 8 && square[king.X + 2][king.Y + 1].piece == Piece.knight && !isAlly(king.X + 2,king.Y + 1) || king.X - 2 >= 0 && king.Y - 1 >= 0 && square[king.X - 2][king.Y - 1].piece == Piece.knight && !isAlly(king.X - 2,king.Y - 1)|| king.X - 2 >= 0 && king.Y + 1 < 8 && square[king.X - 2][king.Y + 1].piece == Piece.knight && !isAlly(king.X - 2,king.Y + 1) || king.X + 2 < 8 && king.Y - 1 >= 0 && square[king.X + 2][king.Y - 1].piece == Piece.knight && !isAlly(king.X + 2,king.Y - 1) || king.X + 1 < 8 && king.Y + 2 < 8 && square[king.X + 1][king.Y + 2].piece == Piece.knight && !isAlly(king.X + 1,king.Y + 2) || king.X - 1 >= 0 && king.Y - 2 >= 0 && square[king.X - 1][king.Y - 2].piece == Piece.knight && !isAlly(king.X - 1,king.Y - 2) || king.X + 1 < 8 && king.Y - 2 >= 0 && square[king.X + 1][king.Y - 2].piece == Piece.knight && !isAlly(king.X + 1,king.Y - 2) || king.X - 1 >= 0 && king.Y + 2 < 8 && square[king.X - 1][king.Y + 2].piece == Piece.knight && !isAlly(king.X - 1,king.Y + 2))
            {
                square[i][j].piece = square[k][l].piece;
                square[i][j].pieceColor = square[k][l].pieceColor;
                square[k][l].piece = tempPiece;
                square[k][l].pieceColor = tempPieceColor;
                return true;
            }


            square[i][j].piece = square[k][l].piece;
            square[i][j].pieceColor = square[k][l].pieceColor;
            square[k][l].piece = tempPiece;
            square[k][l].pieceColor = tempPieceColor;

            return false;
        }

        public void makeMove(Point start, Point end)
        {
            if (!isAlly(end.X, end.Y))
                repetitions = 0;
            else
                repetitions++;


            if (square[start.X][start.Y].piece == Piece.king && end.X - start.X == 2)
            {
                square[end.X][end.Y].piece = square[start.X][start.Y].piece;
                square[end.X][end.Y].pieceColor = square[start.X][start.Y].pieceColor;
                square[end.X][end.Y].count = square[start.X][start.Y].count + 1;

                square[end.X - 1][end.Y].piece = square[7][7].piece;
                square[end.X - 1][end.Y].pieceColor = square[7][7].pieceColor;
                square[end.X - 1][end.Y].count = square[7][7].count + 1;

                if (square[end.X][end.Y].count > 2)
                    square[end.X][end.Y].count = 2;

                if (square[end.X - 1][end.Y].count > 2)
                    square[end.X - 1][end.Y].count = 2;

                square[7][7].piece = Piece.empty;
                square[7][7].pieceColor = PieceColor.empty;
                square[7][7].count = 0;

                square[start.X][start.Y].piece = Piece.empty;
                square[start.X][start.Y].pieceColor = PieceColor.empty;
                square[start.X][start.Y].count = 0;

            }
            else if (square[start.X][start.Y].piece == Piece.king && start.X - end.X == 2)
            {
                square[end.X][end.Y].piece = square[start.X][start.Y].piece;
                square[end.X][end.Y].pieceColor = square[start.X][start.Y].pieceColor;
                square[end.X][end.Y].count = square[start.X][start.Y].count + 1;

                square[end.X + 1][end.Y].piece = square[0][7].piece;
                square[end.X + 1][end.Y].pieceColor = square[0][7].pieceColor;
                square[end.X + 1][end.Y].count = square[0][7].count + 1;

                if (square[end.X][end.Y].count > 2)
                    square[end.X][end.Y].count = 2;

                if (square[end.X + 1][end.Y].count > 2)
                    square[end.X + 1][end.Y].count = 2;

                square[0][7].piece = Piece.empty;
                square[0][7].pieceColor = PieceColor.empty;
                square[0][7].count= 0;

                square[start.X][start.Y].piece = Piece.empty;
                square[start.X][start.Y].pieceColor = PieceColor.empty;
                square[start.X][start.Y].count = 0;
            }
            else if (square[start.X][start.Y].piece == Piece.pawn && Math.Abs(end.X - start.X) == 1 && square[end.X][end.Y].piece==Piece.empty)
            {
                square[end.X][end.Y].piece = Piece.pawn;
                square[end.X][end.Y].pieceColor = square[start.X][start.Y].pieceColor;
                square[end.X][end.Y].count = square[start.X][start.Y].count + 1;


                if (square[end.X][end.Y].count > 2)
                    square[end.X][end.Y].count = 2;

                square[end.X][end.Y+1].piece = Piece.empty;
                square[end.X][end.Y+1].pieceColor = PieceColor.empty;
                square[end.X][end.Y+1].count = 0;

                square[start.X][start.Y].piece = Piece.empty;
                square[start.X][start.Y].pieceColor = PieceColor.empty;
                square[start.X][start.Y].count = 0;


            }
            else
            {
                disablePassant();
                square[end.X][end.Y].piece = square[start.X][start.Y].piece;
                square[end.X][end.Y].pieceColor = square[start.X][start.Y].pieceColor;
                square[end.X][end.Y].count= square[start.X][start.Y].count + 1;

                if (square[end.X][end.Y].count > 2)
                    square[end.X][end.Y].count = 2;

                square[start.X][start.Y].piece = Piece.empty;
                square[start.X][start.Y].pieceColor = PieceColor.empty;
                square[start.X][start.Y].count = 0;
                
            }
        }

        public bool isPossiblePromotion(int i, int j, int k, int l)
        {

            string previousBoard = saveBoard();
            makeMove(new Point(i, j), new Point(k, l));
            if (isPromotion())
            {
                loadBoard(previousBoard);
                return true;

            }
            loadBoard(previousBoard);
            return false;

        }

        public bool isCheckMate()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (isAlly(i, j) && possibleMoves(i, j).Count != 0)
                        return false;
            


            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (square[i][j].piece == Piece.empty && isKingInCheck(i, j, i, j))
                        return true;


            return false;
        }

        public void disablePassant()
        {
            for (int i = 0; i < 8; i++)
                if (square[i][4].piece == Piece.pawn && square[i][4].count == 1)
                    square[i][4].count++;
        }

        public bool amongInLine(Point start, Point end)
        {
            for (int i = 1; i < Math.Max(Math.Abs(end.X - start.X), Math.Abs(end.Y - start.Y)); i++)
                if (square[Math.Sign(end.X - start.X)* i + start.X][Math.Sign(end.Y - start.Y)  * i + start.Y].piece != Piece.empty)
                    return true;

            return false;
        }

        public bool isPromotion()
        {
            for (int i = 0; i < 8; i++)
                if (square[i][0].piece == Piece.pawn || square[i][7].piece == Piece.pawn)
                    return true;
            return false;
        }

        public Point promotionCoords()
        {
            for (int i = 0; i < 8; i++)
                if (square[i][0].piece == Piece.pawn)
                    return new Point(i, 0);
                else if (square[i][7].piece == Piece.pawn)
                    return new Point(i, 7);
            return new Point();
        }


        public void promote(Piece piece)
        {
            for (int i = 0; i < 8; i++)
                if (square[i][0].piece == Piece.pawn)
                    square[i][0].piece = piece;
                else if(square[i][7].piece == Piece.pawn)
                    square[i][7].piece = piece;
        }


        public Point findKing()
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                    if (square[i][j].piece == Piece.king && isAlly(i,j))
                        return new Point(i, j);

            return new Point();
                    
        }

        public List<int> possibleMoves(int i, int j)
        {
            List<int> moves = new List<int>();
            for (int k = 0; k < 8; k++)
                for (int l = 0; l < 8; l++)
                {
                    if ((k != i || l != j) && isPositionPossible(i, j, k, l))
                        moves.Add(k * 10 + l);
                }
            return moves;
        }


        public string saveBoard()
        {
            string board = "";

            for(int j=0;j<8;j++)
                for(int i=0;i<8;i++)
                {
                    switch(square[i][j].piece)
                    {
                        case Piece.bishop:
                            if (square[i][j].pieceColor == PieceColor.white)
                                board += "B" + Convert.ToString(square[i][j].count);
                            else
                                board += "b" + Convert.ToString(square[i][j].count);
                            break;
                        case Piece.king:
                            if (square[i][j].pieceColor == PieceColor.white)
                                board += "K" + Convert.ToString(square[i][j].count);
                            else
                                board += "k" + Convert.ToString(square[i][j].count);
                            break;
                        case Piece.knight:
                            if (square[i][j].pieceColor == PieceColor.white)
                                board += "N" + Convert.ToString(square[i][j].count);
                            else
                                board += "n" + Convert.ToString(square[i][j].count);
                            break;
                        case Piece.pawn:
                            if (square[i][j].pieceColor == PieceColor.white)
                                board += "P" + Convert.ToString(square[i][j].count);
                            else
                                board += "p" + Convert.ToString(square[i][j].count);
                            break;
                        case Piece.queen:
                            if (square[i][j].pieceColor == PieceColor.white)
                                board += "Q" + Convert.ToString(square[i][j].count);
                            else
                                board += "q" + Convert.ToString(square[i][j].count);
                            break;
                        case Piece.rook:
                            if (square[i][j].pieceColor == PieceColor.white)
                                board += "R" + Convert.ToString(square[i][j].count);
                            else
                                board += "r" + Convert.ToString(square[i][j].count);
                            break;
                        case Piece.empty:
                            board += "E" + Convert.ToString(square[i][j].count);
                            break;
                    }
                }
            return board;
        }

        public void loadBoard(string state)
        {
            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    switch (state[16 * j + 2 * i])
                    {
                        case 'Q':
                            square[i][j].piece = Piece.queen;
                            square[i][j].pieceColor = PieceColor.white;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'q':
                            square[i][j].piece = Piece.queen;
                            square[i][j].pieceColor = PieceColor.black;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'R':
                            square[i][j].piece = Piece.rook;
                            square[i][j].pieceColor = PieceColor.white;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'r':
                            square[i][j].piece = Piece.rook;
                            square[i][j].pieceColor = PieceColor.black;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'B':
                            square[i][j].piece = Piece.bishop;
                            square[i][j].pieceColor = PieceColor.white;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'b':
                            square[i][j].piece = Piece.bishop;
                            square[i][j].pieceColor = PieceColor.black;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'N':
                            square[i][j].piece = Piece.knight;
                            square[i][j].pieceColor = PieceColor.white;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'n':
                            square[i][j].piece = Piece.knight;
                            square[i][j].pieceColor = PieceColor.black;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'P':
                            square[i][j].piece = Piece.pawn;
                            square[i][j].pieceColor = PieceColor.white;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'p':
                            square[i][j].piece = Piece.pawn;
                            square[i][j].pieceColor = PieceColor.black;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'K':
                            square[i][j].piece = Piece.king;
                            square[i][j].pieceColor = PieceColor.white;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'k':
                            square[i][j].piece = Piece.king;
                            square[i][j].pieceColor = PieceColor.black;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                        case 'E':
                            square[i][j].piece = Piece.empty;
                            square[i][j].pieceColor = PieceColor.empty;
                            square[i][j].count = (int)char.GetNumericValue(state[16 * j + 2 * i + 1]);
                            break;
                    }
                }
        }

        public double getValue(int i)
        {
            if (isWhiteDown)
                return (double)(int)square[i % 8][i / 8].piece * (int)square[i % 8][i / 8].pieceColor/100;
            else
                return (double)(int)square[i % 8][i / 8].piece * (int)square[i % 8][i / 8].pieceColor*(-1)/100;

        }

        public string rotateSave(string save)
        {
            string rotated;

            string previousBoard = saveBoard();
            loadBoard(save);
            rotate();
            rotated = saveBoard();
            rotate();
            loadBoard(previousBoard);

            return rotated;
        }

    }


    public class Square
    {
        public Piece piece;
        public PieceColor pieceColor;
        public int count;

        public Square()
        {
            piece = Piece.empty;
            pieceColor = PieceColor.empty;
            count = 0;
        }

        
    }

    
}
