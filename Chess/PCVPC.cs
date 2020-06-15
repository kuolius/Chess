using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace Chess
{
    public class PCVPC : StateManager
    {
        double[] data;
        bool whiteTurn, endgame, whiteWins, draw;
        List<string> whiteBoard,blackBoard;
        int speed ,counter;
        Piece promotionPiece;

        public PCVPC(SpriteBatch spriteBatch, GraphicsDevice GraphicsDevice, ContentManager Content) : base(spriteBatch, GraphicsDevice, Content)
        {
            whiteTurn = true;
        }

        public override void LoadContent()
        {
            base.LoadContent();
            speed = 0;
            counter = 0;
            endgame = false;
            whiteWins = false;
            draw = false;
            promotionPiece = Piece.empty;
            
            board = new Board();
            board.startGame();
            whiteBoard = new List<string>();
            blackBoard = new List<string>();

            //generateNeorons();

            data = readData();
            
        }

        public override void Update()
        {
            if (counter >= speed && !endgame)
            {
                if (whiteTurn)
                {
                    if (board.isCheckMate())
                    {
                        whiteWins = false;
                        endgame = true;

                        Console.WriteLine("Checkmate, black wins");
                        return;
                    }

                    

                    List<Point> move = getBestMove();

                    if (trifoldRepetition(move[0].X, move[0].Y, move[1].X, move[1].Y) || board.isStaleMate() || board.isDraw())
                    {
                        endgame = true;
                        draw = true;
                        Console.WriteLine("Draw");
                        return;
                    }

                    board.makeMove(move[0], move[1]);
                    if (board.isPromotion())
                        board.square[move[1].X][move[1].Y].piece = promotionPiece;

                    whiteBoard.Add(board.saveBoard());

                    
                    whiteTurn = !whiteTurn;
                }
                else
                {
                    board.rotate();
                    if (board.isCheckMate())
                    {
                        whiteWins = true;
                        endgame = true;
                        board.rotate();
                        Console.WriteLine("Checkmate, white wins");
                        return;

                    }

                    List<Point> move = getBestMove();

                    if (trifoldRepetition(move[0].X, move[0].Y, move[1].X, move[1].Y) || board.isStaleMate() || board.isDraw())
                    {
                        endgame = true;
                        draw = true;
                        board.rotate();
                        Console.WriteLine("Draw");
                        return;
                    }

                    board.makeMove(move[0], move[1]);
                    if (board.isPromotion())
                        board.square[move[1].X][move[1].Y].piece = promotionPiece;


                    blackBoard.Add(board.saveBoard());
                   

                    board.rotate();
                    whiteTurn = !whiteTurn;
                }

                

                counter = 0;
            }
            else if(!endgame)
                counter++;
            
            if(endgame) // && Keyboard.GetState().IsKeyDown(Keys.N))
            {

                adjust();
                endgame = false;
                whiteWins = false;
                whiteTurn = true;
                draw = false;
                promotionPiece = Piece.empty;
                data = readData();
                board = new Board();
                board.startGame();
                whiteBoard = new List<string>();
                blackBoard = new List<string>();
            }

            base.Update();

                
        }

        public override void Draw()
        {
            drawBoard();
            drawPieces();
            base.Draw();
        }
        
        
        public bool isCheckMatePossible(int i, int j, int k, int l)
        {
            string previousBoard = board.saveBoard();
            board.makeMove(new Point(i, j), new Point(k, l));

            board.rotate();
            if (board.isCheckMate())
            {
                board.rotate();
                board.loadBoard(previousBoard);
                return true;

            }
            board.rotate();


            for (int m = 0; m < 8; m++)
                for (int n = 0; n < 8; n++)
                {
                    int checkMate = 0;
                    if (!board.isAlly(m, n))
                        continue;
                    List<int> moves = board.possibleMoves(m, n);
                    if (moves.Count == 0)
                    {
                        board.loadBoard(previousBoard);
                        return false;
                    }
                    foreach (int move in moves)
                    {
                        if (board.square[move / 10][move % 10].piece == Piece.king)
                            continue;
                        board.makeMove(new Point(m, n), new Point(move / 10, move % 10));
                        board.rotate();
                        if (!board.isCheckMate())
                        {
                            board.rotate();
                            board.loadBoard(previousBoard);
                            return false;

                        }
                        else
                            checkMate++;
                        board.rotate();
                    }
                    if(checkMate>0)
                    {
                        board.loadBoard(previousBoard);
                        return true;
                    }

                }
            board.loadBoard(previousBoard);
            return false;
        }
        

        public List<Point> getBestMove()
        {
            Point bestEnd = new Point();
            Point bestStart = new Point();

            double prediction = -1;

            for (int i = 0; i < 8; i++)
                for (int j = 0; j < 8; j++)
                {
                    if (board.isAlly(i, j))
                    {
                        List<int> moves = board.possibleMoves(i, j);
                        foreach (int move in moves)
                        {
                            double newPrediction = predict(i, j, move / 10, move % 10)*(1-predictOpponent(i, j, move / 10, move % 10));
                            if (prediction == -1 || newPrediction > prediction  || trifoldRepetition(bestStart.X,bestStart.Y,bestEnd.X,bestEnd.Y))
                            {
                                
                                
                                bestEnd = new Point(move / 10, move % 10);
                                bestStart = new Point(i, j);
                                prediction = newPrediction;
                                
                                
                            }
                            //Console.WriteLine(newPrediction);


                            if (board.isPossiblePromotion(i, j, move / 10, move % 10))
                            {
                                
                                board.square[i][j].piece = Piece.queen;
                                newPrediction = predict(i, j, move / 10, move % 10) * (1 - predictOpponent(i, j, move / 10, move % 10));
                                if(newPrediction > prediction)
                                {
                                    promotionPiece = Piece.queen;
                                    bestEnd = new Point(move / 10, move % 10);
                                    bestStart = new Point(i, j);
                                    prediction = newPrediction;
                                }

                                board.square[i][j].piece = Piece.rook;
                                newPrediction = predict(i, j, move / 10, move % 10) * (1 - predictOpponent(i, j, move / 10, move % 10));
                                if (newPrediction > prediction)
                                {
                                    promotionPiece = Piece.rook;
                                    bestEnd = new Point(move / 10, move % 10);
                                    bestStart = new Point(i, j);
                                    prediction = newPrediction;
                                }

                                board.square[i][j].piece = Piece.bishop;
                                newPrediction = predict(i, j, move / 10, move % 10) * (1 - predictOpponent(i, j, move / 10, move % 10));
                                if (newPrediction > prediction)
                                {
                                    promotionPiece = Piece.bishop;
                                    bestEnd = new Point(move / 10, move % 10);
                                    bestStart = new Point(i, j);
                                    prediction = newPrediction;
                                }

                                board.square[i][j].piece = Piece.knight;
                                newPrediction = predict(i, j, move / 10, move % 10) * (1 - predictOpponent(i, j, move / 10, move % 10));
                                if (newPrediction > prediction)
                                {
                                    promotionPiece = Piece.knight;
                                    bestEnd = new Point(move / 10, move % 10);
                                    bestStart = new Point(i, j);
                                    prediction = newPrediction;
                                }

                                board.square[i][j].piece = Piece.pawn;

                            }



                            if(isCheckMatePossible(i, j, move / 10, move % 10))
                                return new List<Point> { new Point(i , j), new Point(move / 10, move % 10) };

                        }


                    }

                }

            return new List<Point> { bestStart, bestEnd };
        }


        public bool trifoldRepetition(int i, int j, int k, int l)
        {
            string previousBoard = board.saveBoard();
            board.makeMove(new Point(i, j), new Point(k,l));

            string currentBoard = board.saveBoard();

            if(whiteTurn)
            {
                int ct = 0;
                foreach(string wb in whiteBoard)
                {
                    if (wb == currentBoard)
                        ct++;
                }

                if (ct == 2)
                {
                    board.loadBoard(previousBoard);
                    return true;

                }
            }
            else
            {
                int ct = 0;
                foreach (string bb in blackBoard)
                {
                    if (bb == currentBoard)
                        ct++;
                }

                if (ct == 2)
                {
                    board.loadBoard(previousBoard);
                    return true;

                }
            }
            

            board.loadBoard(previousBoard);

            return false;
        }

        public List<string> connectedBoard()
        {
            List<string> connected = new List<string>();
            if(whiteTurn)
            {
                for(int i=Math.Max(0,whiteBoard.Count-6);i<whiteBoard.Count;i++)
                {
                    connected.Add(whiteBoard[i]);
                    connected.Add(board.rotateSave(blackBoard[i]));
                }
            }
            else
            {
                for (int i = Math.Max(0, blackBoard.Count-6); i < blackBoard.Count; i++)
                {
                    connected.Add(whiteBoard[i]);
                    connected.Add(board.rotateSave(blackBoard[i]));
                }

                connected.Add(whiteBoard[blackBoard.Count]);
            }

            return connected;
        }

        public double predict(int i, int j, int k, int l)
        {
            string previousBoard = board.saveBoard();
            board.makeMove(new Point(i, j), new Point(k, l));
            double[] z = new double[64];
            double sum = data[4224];
            for (int m = 0; m < 64; m++)
                z[m] = data[4160 + m];
            for (int m = 0; m < 64; m++)
                for (int n = 0; n < 64; n++)
                {
                    z[m] += data[64 * m + n] * board.getValue(n);
                }
            for (int m = 0; m < 64; m++)
                sum += Math.Tanh(z[m]) * data[4096 + m];

            board.loadBoard(previousBoard);

            return (Math.Tanh(sum) + 1) / 2;

        }

        public double predictOpponent(int i, int j, int k, int l)
        {
            string previousBoard = board.saveBoard();
            board.makeMove(new Point(i, j), new Point(k, l));
            board.rotate();
            double[] z = new double[64];
            double sum = data[4224];
            for (int m = 0; m < 64; m++)
                z[m] = data[4160 + m];
            for (int m = 0; m < 64; m++)
                for (int n = 0; n < 64; n++)
                {
                    z[m] += data[64 * m + n] * board.getValue(n);
                }
            for (int m = 0; m < 64; m++)
                sum += Math.Tanh(z[m]) * data[4096 + m];

            board.rotate();
            board.loadBoard(previousBoard);

            return (Math.Tanh(sum) + 1) / 2;
        }

        public double prediction(string str)
        {
            string previousBoard = board.saveBoard();
            board.loadBoard(str);
            double[] z = new double[64];
            double sum = data[4224];
            for (int m = 0; m < 64; m++)
                z[m] = data[4160 + m];
            for (int m = 0; m < 64; m++)
                for (int n = 0; n < 64; n++)
                {
                    z[m] += data[64 * m + n] * board.getValue(n);
                }
            for (int m = 0; m < 64; m++)
                sum += Math.Tanh(z[m]) * data[4096 + m];

            board.loadBoard(previousBoard);

            return (Math.Tanh(sum) + 1) / 2;

        }

        public double predictionOpponent(string str)
        {
            string previousBoard = board.saveBoard();
            board.loadBoard(str);
            board.rotate();
            double[] z = new double[64];
            double sum = data[4224];
            for (int m = 0; m < 64; m++)
                z[m] = data[4160 + m];
            for (int m = 0; m < 64; m++)
                for (int n = 0; n < 64; n++)
                {
                    z[m] += data[64 * m + n] * board.getValue(n);
                }
            for (int m = 0; m < 64; m++)
                sum += Math.Tanh(z[m]) * data[4096 + m];

            board.rotate();
            board.loadBoard(previousBoard);

            return (Math.Tanh(sum) + 1) / 2;

        }

        public double[] activation(string str)
        {
            string previousBoard = board.saveBoard();
            board.loadBoard(str);
            double[] z = new double[64];
            
            for (int m = 0; m < 64; m++)
                z[m] = data[4160 + m];
            for (int m = 0; m < 64; m++)
                for (int n = 0; n < 64; n++)
                {
                    z[m] += data[64 * m + n] * board.getValue(n);
                }
            for (int m = 0; m < 64; m++)
                z[m] =  Math.Tanh(z[m]);

            return z;
        }

        public void adjust()
        {
            Point y = new Point(0, 10);
            if (whiteWins)
            {
                y.X = 10;y.Y = 0;
            }
            
            if(draw)
            {
                y.X = 5; y.Y = 5;
            }

            double deltaLast = 0; 
            
            double[] active = new double[64];
            

            double m = Math.Min(whiteBoard.Count,10) + Math.Min(blackBoard.Count, 10) ;
            double alpha = 0.1;
            double previouscost = 0;
            double cost = 0;

            for (int k = 0; k < 1000; k++)
            {
                previouscost = cost;
                double[] delta = new double[64];
                double[] deltaW = new double[4225];
                cost = 0;
                double pred = 0;

                

                for (int n=0;n<Math.Min(whiteBoard.Count,10);n++)
                {
                    pred = prediction(whiteBoard[n]);// *(1-predictionOpponent(whiteBoard[n]));
                    active = activation(whiteBoard[n]);

                    board.loadBoard(whiteBoard[n]);

                    

                    deltaLast = (pred - (double)y.X/10) *  (1 - pred*pred);
                    for (int i = 0; i < 64; i++)
                        delta[i] += data[4096 + i] * deltaLast * (1 - Math.Pow(active[i],2));

                    for (int i = 0; i < 64; i++)
                        for (int j = 0; j < 64; j++)
                            deltaW[i * 64 + j] += board.getValue(j) * delta[i];
                    for (int i = 0; i < 64; i++)
                    {
                        deltaW[4096 + i] += active[i] * deltaLast;
                        deltaW[4160 + i] += delta[i];
                    }
                    deltaW[4224] += deltaLast;

                    cost +=  Math.Pow(pred - (double)y.X / 10, 2) / 2;

                }

                /*
                for (int n = Math.Max(whiteBoard.Count-5,0); n < whiteBoard.Count ; n++)
                {
                    pred = prediction(whiteBoard[n]);
                    active = activation(whiteBoard[n]);

                    board.loadBoard(whiteBoard[n]);



                    deltaLast = (pred - (double)y.X / 10) * (1 - pred * pred);
                    for (int i = 0; i < 64; i++)
                        delta[i] += data[4096 + i] * deltaLast * (1 - Math.Pow(active[i], 2));

                    for (int i = 0; i < 64; i++)
                        for (int j = 0; j < 64; j++)
                            deltaW[i * 64 + j] += board.getValue(j) * delta[i];
                    for (int i = 0; i < 64; i++)
                    {
                        deltaW[4096 + i] += active[i] * deltaLast;
                        deltaW[4160 + i] += delta[i];
                    }
                    deltaW[4224] += deltaLast;

                    cost += Math.Pow(pred - (double)y.X / 10, 2) / 2;

                }
                */

                board.isWhiteDown = false;
                for (int n=0;n< Math.Min(blackBoard.Count, 25); n++)
                {
                   
                    board.loadBoard(blackBoard[n]);
                    pred = (prediction(blackBoard[n]) +(1 - predictionOpponent(blackBoard[n])))/2;
                    active = activation(blackBoard[n]);

                    deltaLast = (pred - (double)y.Y/10) * (1 - pred*pred);
                    for (int i = 0; i < 64; i++)
                        delta[i] += data[4096 + i] * deltaLast *(1 - Math.Pow(active[i], 2));

                    for (int i = 0; i < 64; i++)
                        for (int j = 0; j < 64; j++)
                            deltaW[i * 64 + j] += board.getValue(j) * delta[i];
                    for (int i = 0; i < 64; i++)
                    {
                        deltaW[4096 + i] += active[i] * deltaLast;
                        deltaW[4160 + i] += delta[i];
                    }
                    deltaW[4224] += deltaLast;

                    cost +=  Math.Pow(pred - (double)y.Y / 10, 2) / 2;

                }
                /*
                for (int n = Math.Max(blackBoard.Count - 5, 0); n < blackBoard.Count; n++)
                {

                    board.loadBoard(blackBoard[n]);
                    pred = prediction(blackBoard[n]);
                    active = activation(blackBoard[n]);

                    deltaLast = (pred - (double)y.Y / 10) * (1 - pred * pred);
                    for (int i = 0; i < 64; i++)
                        delta[i] += data[4096 + i] * deltaLast * (1 - Math.Pow(active[i], 2));

                    for (int i = 0; i < 64; i++)
                        for (int j = 0; j < 64; j++)
                            deltaW[i * 64 + j] += board.getValue(j) * delta[i];
                    for (int i = 0; i < 64; i++)
                    {
                        deltaW[4096 + i] += active[i] * deltaLast;
                        deltaW[4160 + i] += delta[i];
                    }
                    deltaW[4224] += deltaLast;

                    cost += Math.Pow(pred - (double)y.Y / 10, 2) / 2;

                }

                */
                board.isWhiteDown = true;

                

                cost = cost / m;


                
                //Console.Write(" vector:"+deltaW[10]);
                //Console.WriteLine(" activation:" + active[10]);
                //Console.Write(" deltaLast:" + deltaLast);
                //Console.WriteLine(" pred:" + pred);
                //Console.WriteLine(" boardvalue:" + board.getValue(10));

               

                for (int i = 0; i < 4160; i++)
                    data[i] = data[i] - alpha * 1 / m  * deltaW[i];
                    //data[i] += data[i] - alpha * ((1 / m * deltaW[i]) +  0.001*data[i]);

                for (int i = 4160; i < 4225; i++)
                    data[i] = data[i] - alpha * (1 / m) * deltaW[i];

            }


            Console.Write("Cost:" + cost);
            Console.Write("PreviousCost:" + previouscost);

            using (StreamWriter sw = new StreamWriter("neurons.txt"))
            {

                for (int i = 0; i < 4225; i++)
                {
                    if (i!=4224)
                        sw.Write(Convert.ToString(data[i]) + ",");
                    else
                        sw.Write(Convert.ToString(data[i]));

                }

                
            }

            
           
            

        }
        
        public double[] readData()
        {
            double[] data = new double[4288];

            using (StreamReader sr = new StreamReader("neurons.txt"))
            {
                string[] line = sr.ReadToEnd().Split(',');
                for(int i=0;i<line.Length;i++)
                {
                    data[i] = Convert.ToDouble(line[i]);
                }
            }

            
            return data;
        }

        public void generateNeorons()
        {
            Random rand = new Random();
            using (StreamWriter sw = new StreamWriter("neurons.txt"))
            {
                for (int i = 0; i < 4225; i++)
                {
                    double z0 = rand.NextDouble();
                    double z1 = rand.NextDouble();

                    double normal =  Math.Abs(0.1*Math.Sqrt((-2) * Math.Log(z0)) * Math.Cos(2 * Math.PI * z1));

                    if (i != 4224)
                        sw.Write(Convert.ToString(normal) + ',');
                    else
                        sw.Write(Convert.ToString(normal));

                }
            }
        }
    }
}
