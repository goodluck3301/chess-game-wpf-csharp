using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Diagnostics;
using System.Threading;

namespace Chess
{
    public class Move
    {
        public int x;
        public int y;
        public int r;
        public int c;
    }

    public partial class MainWindow : Window
    {
      
       
        int[,] CurrentAvailable = new int[8, 8];
        double DeltaX, DeltaY;
        bool FigureClicked;
        int TempX, TempY;
        int  s;
        bool check;
        int Value,tempbsec,tempwsec;
        bool Move;


        int[,] board =
        {
           {-50,-30,-33,-90,-1000,-33,-30,-50 },
           {-10,-10,-10,-10,  -10,-10,-10,-10 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           { 10, 10, 10, 10,   10, 10, 10, 10 },
           { 50, 30, 33, 90, 1000, 33, 30, 50 }
        };

        int[,] resetBoard =
        {
           {-50,-30,-33,-90,-1000,-33,-30,-50 },
           {-10,-10,-10,-10,  -10,-10,-10,-10 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           {  0,  0,  0,  0,    0,  0,  0,  0 },
           { 10, 10, 10, 10,   10, 10, 10, 10 },
           { 50, 30, 33, 90, 1000, 33, 30, 50 }
        };

        int[,] WAttack = new int[8, 8];
        int[,] BAttack = new int[8, 8];

        int[,] WCAttack = new int[8, 8];
        int[,] BCAttack = new int[8, 8];

        bool finded;

        public MainWindow()
        {
            Move = true;
            InitializeComponent();
            InitialPositions(board);
        }

        void checker()
        {
            bool win = false;
            int sum = 0;
            int sum2 = 0;
            for (int i = 0; i < 8; i++) {
                for (int j = 0; j < 8; j++) {
                    if (board[i,j]>0) {
                        sum += board[i, j];
                    }
                    if (board[i, j] < 0)
                    {
                        sum2 += board[i, j];
                    }
                }
            }
            if (sum < 1000 || sum2 > -1000) {
                board = resetBoard;
                MessageBox.Show("Game Over");
            }
        }

       private Move minimaxWOR(int depth, int[,] taxtak1, bool MaximisingPlayer)
        {
            Move bestMoveFound = new Move();

            List<Move> newGameMoves = new List<Move>();
            newGameMoves = CheckAttack(MaximisingPlayer, taxtak1);
            decimal bestMove = -100000;
            for (int i = 0; i < newGameMoves.Count(); i++)
            {
                Move newGameMove = newGameMoves[i];
                decimal value;
                value = 0;
                int[,] temp = new int[8, 8];
                copy(temp, taxtak1);
                ugly_Move(newGameMove, taxtak1);

                copy(taxtak1, temp);
                if (value >= bestMove)
                {
                    bestMove = value;
                    bestMoveFound = newGameMove;
                }
            }
                return bestMoveFound;
        }



        private decimal minimax(int depth, int[,] taxtak,decimal alpha,decimal beta, bool MaximisingPlayer)
        {
            if (depth == 0)
            {
                return -evaluateBoard(taxtak);            
            }
            List<Move> newGameMoves = new List<Move>();
            newGameMoves = CheckAttack(MaximisingPlayer, taxtak);
            if (MaximisingPlayer)
            {
                decimal bestMove = -100000;
                for (int i = 0; i < newGameMoves.Count(); i++)
                {
                    Move newGameMove = newGameMoves[i];
                    int[,] temp = new int[8, 8];
                    copy(temp, taxtak);
                    ugly_Move(newGameMove, taxtak);
                    bestMove = Math.Max(bestMove, minimax(depth - 1, taxtak, alpha, beta, false)  );

                    copy(taxtak, temp);
                    alpha = Math.Max(alpha, bestMove);
                    if(beta<=alpha)
                    {
                        return bestMove;
                    }
                }          
                return bestMove;
            }
            else
            {              
                decimal bestMove = 100000;
                for (int i = 0; i < newGameMoves.Count(); i++)
                {  
                    Move newGameMove = newGameMoves[i];
                    int[,] temp = new int[8, 8];
                    copy(temp, taxtak);
                    ugly_Move(newGameMove, taxtak);
                    bestMove =Math.Min(bestMove, minimax(depth - 1, taxtak,alpha,beta, true) );
                    copy(taxtak, temp);
                    beta = Math.Min(beta, bestMove);
                    if(beta<=alpha)
                    {
                        return bestMove;
                    }
                }
                return bestMove;
            }
        }

        private Move minimaxRoot(int depth, int[,] taxtak1, bool MaximisingPlayer)
        {
            List<Move> newGameMoves = new List<Move>();
            newGameMoves = CheckAttack(MaximisingPlayer, taxtak1);
            decimal bestMove = -100000;
            Move bestMoveFound = new Move();
            for (int i = 0; i < newGameMoves.Count(); i++)
            {
                Move newGameMove = newGameMoves[i];
                int[,] temp = new int[8, 8];
                copy(temp, taxtak1);
                ugly_Move(newGameMove, taxtak1);
       
                decimal value = minimax(depth - 1, taxtak1, -10000, 10000, false);
                copy(taxtak1, temp);
                if (value >= bestMove)
                {
                    bestMove = value;
                    bestMoveFound = newGameMove;
                }
            }
            return bestMoveFound;
        }

        private decimal evaluateBoard(int[,] boardd)
        {
            decimal totalEvaluation;
            totalEvaluation = 0;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                        totalEvaluation += boardd[i, j];   
                }
            }
            return totalEvaluation- MoveCount(true, boardd)+ MoveCount(false, boardd);
        }

        private List<Move> CheckAttack(bool isMaximisingPlayer, int[,] taxtak)
        {
            List<Move> validMoves = new List<Move>();
            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    ////
                    checker();
                    if ((isMaximisingPlayer == false) && (taxtak[r, c] > 0))
                    {
                        switch (taxtak[r, c])
                        {
                            case 10:
                                
                                if (r - 1 >= 0)
                                {
                                    if (taxtak[r - 1, c] == 0)
                                    {               
                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 1; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak,false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);        
                                    }
                                }
                                else
                                {  
                                    taxtak[r, c] = 90;
                                }
                                if (r - 1 >= 0)
                                {
                                    if (c - 1 >= 0) { if (taxtak[r - 1, c - 1] < 0) { Move ob = new Move(); ob.x = r - 1; ob.y = c - 1; ob.r = r; ob.c = c; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }
                                    if (c + 1 <= 7) { if (taxtak[r - 1, c + 1] < 0) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 1; ob.y = c + 1; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        } }
                                }

                                if (r == 6 && taxtak[r - 1, c] == 0 && taxtak[r - 2, c] == 0) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 2; ob.y = c; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                }
                                break;

                            case 30:

                                if (((r - 2) >= 0) && ((c + 1) < 8) && (taxtak[r - 2, c + 1] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 2; ob.y = c + 1; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r - 2) >= 0) && ((c - 1) >= 0) && (taxtak[r - 2, c - 1] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 2; ob.y = c - 1; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 2) < 8) && ((c + 1) < 8) && (taxtak[r + 2, c + 1] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 2; ob.y = c + 1; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 2) < 8) && ((c - 1) >= 0) && (taxtak[r + 2, c - 1] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 2; ob.y = c - 1; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r - 1) >= 0) && ((c - 2) >= 0) && (taxtak[r - 1, c - 2] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 1; ob.y = c - 2; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r - 1) >= 0) && ((c + 2) < 8) && (taxtak[r - 1, c + 2] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 1; ob.y = c + 2; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 1) < 8) && ((c - 2) >= 0) && (taxtak[r + 1, c - 2] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c - 2; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 1) < 8) && ((c + 2) < 8) && (taxtak[r + 1, c + 2] <= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c + 2; int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                break;

                            case 33:

                                int s20;
                                s20 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s20++;
                                    if (s20 > 7) break;
                                    else
                                    {
                                        if (taxtak[i, s20] > 0)
                                        {
                                            break;
                                        }
                                        else

                                        if (taxtak[i, s20] < 0)
                                        {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s20; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }

                                        else
                                        {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s20; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                int s9;
                                s9 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s9--;
                                    if (s9 < 0) break;
                                    else
                                    {
                                        if (taxtak[i, s9] > 0)
                                        {
                                            break;
                                        }
                                        else

                                        if (taxtak[i, s9] < 0)
                                        {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s9; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s9; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                int s10;
                                s10 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    s10++;
                                    if (s10 > 7) break;
                                    else
                                    {

                                        if (taxtak[i, s10] > 0)
                                        {
                                            break;
                                        }
                                        else

                                        if (taxtak[i, s10] < 0)
                                        {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s10; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s10; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                int s11;
                                s11 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    s11--; if (s11 < 0) break;
                                    else
                                    {
                                        if (taxtak[i, s11] > 0) { break; }
                                        else
                                       if (taxtak[i, s11] < 0) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s11; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp); break; }
                                        else { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s11; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }
                                }

                                break;


                            case 50:
                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] < 0)
                                    {
                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);

                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                { 
                                    if (taxtak[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }

                                break;


                            case 90:
                                int s12;
                                s12 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s12++;
                                    if (s12 > 7) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s12] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s12] < 0)
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s12; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }

                                        else
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s12; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                 
                                int s13;
                                s13 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s13--;
                                    if (s13 < 0) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s13] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s13] < 0)
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s13; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s13; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                
                                int s14;
                                s14 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    s14++;
                                    if (s14 > 7) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s14] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s14] < 0)
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s14; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s14; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                   
                                int s15;
                                s15 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    s15--;
                                    if (s15 < 0) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s15] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s15] < 0)
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s15; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {

                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = s15; int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }

                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = c; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] < 0)
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {

                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r; ob.y = i; int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }


                                break;

                            case 1000:

                                for (int i = r - 1; i < r + 2; i++)
                                {
                                    if (i < 0) continue;
                                    if (i > 7) break;
                                    for (int j = c - 1; j < c + 2; j++)
                                    {
                                        if (j < 0) continue;
                                        if (j > 7) break;
                                        if (i != r || j != c)
                                        {
                                            if (taxtak[i, j] == 0 || taxtak[i, j] < 0)
                                            {
                                                Move ob = new Move(); ob.r = r; ob.c = c; ob.x = i; ob.y = j; int[,] temp = new int[8, 8];
                                                copy(temp, taxtak);
                                                ugly_Move(ob, taxtak);
                                                if (!CheckCheck(taxtak, false)) { validMoves.Add(ob); }
                                                copy(taxtak, temp);
                                            }
                                        }
                                    }
                                }
                                break;
                        }
                    }
                    else if ((isMaximisingPlayer == true) && (taxtak[r, c] < 0))
                    {
                        switch (taxtak[r, c])
                        {
                            case -10:

                                if (r + 1 < 8) { if (taxtak[r + 1, c] == 0) {
                                        Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { if (r + 1 == 7) { taxtak[r + 1, c] = -90; }; validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    } }
                     
                                if (r == 1 && taxtak[r + 1, c] == 0 && taxtak[r + 2, c] == 0)
                                { 
                                    Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 2; ob.y = c;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                }
                                if (r + 1 < 8)
                                {
                                    if (c - 1 >= 0) { if (taxtak[r + 1, c - 1] > 0) {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c - 1;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        } }

                                    if (c + 1 <= 7) { if (taxtak[r + 1, c + 1] > 0) {
                                            Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c + 1;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        } }
                                }
                                break;

                            case -30:

                                if (((r - 2) >= 0) && ((c + 1) < 8) && (taxtak[r - 2, c + 1] >= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 2; ob.y = c + 1;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r - 2) >= 0) && ((c - 1) >= 0) && (taxtak[r - 2, c - 1] >= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 2; ob.y = c - 1;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 2) < 8) && ((c + 1) < 8) && (taxtak[r + 2, c + 1] >= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 2; ob.y = c + 1;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 2) < 8) && ((c - 1) >= 0) && (taxtak[r + 2, c - 1] >= 0)) {
                                    Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 2; ob.y = c - 1;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r - 1) >= 0) && ((c - 2) >= 0) && (taxtak[r - 1, c - 2] >= 0)) { Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 1; ob.y = c - 2;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r - 1) >= 0) && ((c + 2) < 8) && (taxtak[r - 1, c + 2] >= 0)) {
                                    Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r - 1; ob.y = c + 2;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 1) < 8) && ((c - 2) >= 0) && (taxtak[r + 1, c - 2] >= 0)) {
                                    Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c - 2;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };
                                if (((r + 1) < 8) && ((c + 2) < 8) && (taxtak[r + 1, c + 2] >= 0))
                                {
                                    Move ob = new Move(); ob.r = r; ob.c = c; ob.x = r + 1; ob.y = c + 2;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, taxtak);
                                    ugly_Move(ob, taxtak);
                                    if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                    copy(taxtak, temp);
                                };

                              
                                break;

                            case -33:

                                 
                                int s77;
                                s77 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s77++;
                                    if (s77 > 7) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s77] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s77] > 0)
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s77;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);                                        
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s77;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (CheckCheck(taxtak, true) == false) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                 
                                int ss;
                                ss = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    ss--;
                                    
                                    if (ss < 0) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, ss] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, ss] > 0)
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = ss;                                     
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }     
                                            copy(taxtak, temp);
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = ss;                                        
                                            int[,] tempp = new int[8, 8];
                                            copy(tempp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }                                       
                                            copy(taxtak, tempp);                                      
                                        }
                                    }

                                }
                                   
                                int sss;
                                sss = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sss++;
                                    if (sss > 7) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, sss] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, sss] > 0)
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = sss;                                    
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }                                        
                                            copy(taxtak, temp);                                         
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = sss;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }                                      
                                            copy(taxtak, temp);                      
                                        }
                                    }

                                }
                                   
                                int ss1;
                                ss1 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    ss1--;
                                    if (ss1 < 0) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, ss1] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, ss1] > 0)
                                        {                                
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = ss1;                                       
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }                                  
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = ss1;     
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }                                         
                                            copy(taxtak, temp);                                         
                                        }
                                    }

                                }
                              
                                break;

                            case -50:

                                
                              
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] > 0)
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] > 0)
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] > 0)
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] > 0)
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;                                 
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }                            
                                break;


                            case -90:

                                int s2;
                                s2 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s2++;
                                    if (s2 > 7) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s2] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s2] > 0)
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s2;                                     
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }

                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s2;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                 
                                int s3;
                                s3 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    s3--;
                                    if (s3 < 0) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s3] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s3] > 0)
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s3;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s3;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                   
                                int s4;
                                s4 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    s4++;
                                    if (s4 > 7) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s4] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s4] > 0)
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s4;                                     
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s4;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                   
                                int s5;
                                s5 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    s5--;
                                    if (s5 < 0) break;
                                    else
                                    {
                                         
                                        if (taxtak[i, s5] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (taxtak[i, s5] > 0)
                                        {

                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s5;

                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                            break;
                                        }
                                        else
                                        {
                                            Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = s5;
                                            int[,] temp = new int[8, 8];
                                            copy(temp, taxtak);
                                            ugly_Move(ob, taxtak);
                                            if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                            copy(taxtak, temp);
                                        }
                                    }

                                }
                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] > 0)
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[i, c] > 0)
                                    {

                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = c;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (taxtak[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] > 0)
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (taxtak[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (taxtak[r, i] > 0)
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                        break;
                                    }
                                    else
                                    {
                                        Move ob = new Move(); ob.x = r; ob.r = r; ob.c = c; ob.y = i;
                                        int[,] temp = new int[8, 8];
                                        copy(temp, taxtak);
                                        ugly_Move(ob, taxtak);
                                        if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                        copy(taxtak, temp);
                                    }
                                }


                             
                                break;

                            case -1000:

                                for (int i = r - 1; i < r + 2; i++)
                                {
                                    if (i < 0) continue;
                                    if (i > 7) break;
                                    for (int j = c - 1; j < c + 2; j++)
                                    {
                                        if (j < 0) continue;
                                        if (j > 7) break;
                                        if (i != r || j != c)
                                        {
                                            if (taxtak[i, j] == 0 || taxtak[i, j] > 0)
                                            {
                                                Move ob = new Move(); ob.x = i; ob.r = r; ob.c = c; ob.y = j;
                                                int[,] temp = new int[8, 8];
                                                copy(temp, taxtak);
                                                ugly_Move(ob, taxtak);
                                                if (!CheckCheck(taxtak, true)) { validMoves.Add(ob); }
                                                copy(taxtak, temp);
                                            }
                                        }
                                    }
                                }

                                break;

                        }
                    }


                }

            }
          

            return validMoves;

        }

        private void ResetSquareArray(int[,] array)
        {
           

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    array[i, j] = 0;
                }
            }
        }

        private void copy(int[,] array1, int[,] array2)//array 2-ic 1-@
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    array1[i, j] = array2[i, j];
                }
            }
        }

        private void ugly_Move(Move obj, int[,] boardd)
        {
            boardd[obj.x, obj.y] = boardd[obj.r, obj.c];
            boardd[obj.r, obj.c] = 0;
        }

        private void CoordMove(int x1, int y1, int x2, int y2)//x1y1-vorteghic      x2y2-vortegh
        {
            board[x2, y2] = board[x1, y1];
            board[x1, y1] = 0;
        }

        private void MoveObj(Move obj, int[,] board)//obj-i r,c-ic obj-i x,y
        {

            board[obj.x, obj.y] = board[obj.r, obj.c];
            board[obj.r, obj.c] = 0;



           InitialPositions(board);
        }

        private void AvailableMoves(int r, int c, int Value)
        {

            switch (Value)
            {
                case 10:
                    if (Move)
                    {

                        if (r - 1 >= 0)
                        {
                            if (c - 1 >= 0) { if (board[r - 1, c - 1] < 0) { CurrentAvailable[r - 1, c - 1] = 1; } }

                            if (c + 1 <= 7) { if (board[r - 1, c + 1] < 0) { CurrentAvailable[r - 1, c + 1] = 1; } }
                        }
                        if (r == 6 && board[r - 1, c] == 0 && board[r - 2, c] == 0) { CurrentAvailable[r - 1, c] = 1; CurrentAvailable[r - 2, c] = 1; }
                        if (r - 1 >= 0) { if (board[r - 1, c] == 0) { CurrentAvailable[r - 1, c] = 1; } }
                        else { board[r, c] = 90; } 




                    }
                    break;



                case 30:
                    if (Move)
                    {
                        if (((r - 2) >= 0) && ((c + 1) < 8) && (board[r - 2, c + 1] <= 0)) { CurrentAvailable[r - 2, c + 1] = 1; };
                        if (((r - 2) >= 0) && ((c - 1) >= 0) && (board[r - 2, c - 1] <= 0)) { CurrentAvailable[r - 2, c - 1] = 1; };
                        if (((r + 2) < 8) && ((c + 1) < 8) && (board[r + 2, c + 1] <= 0)) { CurrentAvailable[r + 2, c + 1] = 1; };
                        if (((r + 2) < 8) && ((c - 1) >= 0) && (board[r + 2, c - 1] <= 0)) { CurrentAvailable[r + 2, c - 1] = 1; };
                        if (((r - 1) >= 0) && ((c - 2) >= 0) && (board[r - 1, c - 2] <= 0)) { CurrentAvailable[r - 1, c - 2] = 1; };
                        if (((r - 1) >= 0) && ((c + 2) < 8) && (board[r - 1, c + 2] <= 0)) { CurrentAvailable[r - 1, c + 2] = 1; };
                        if (((r + 1) < 8) && ((c - 2) >= 0) && (board[r + 1, c - 2] <= 0)) { CurrentAvailable[r + 1, c - 2] = 1; };
                        if (((r + 1) < 8) && ((c + 2) < 8) && (board[r + 1, c + 2] <= 0)) { CurrentAvailable[r + 1, c + 2] = 1; };


                    }
                    break;

                case 33:
                    if (Move)
                    {
                        CurrentAvailable[r, c] = 0;
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {

                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else

                                if (board[i, s] < 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }

                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else

                                if (board[i, s] < 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }

                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {

                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else

                                if (board[i, s] < 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }

                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {

                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else

                                if (board[i, s] < 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }

                    }
                    break;

                case 50:
                    if (Move)
                    {
                        
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                             
                            if (board[i, c] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] < 0)
                            {

                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                             
                            if (board[i, c] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] < 0)
                            {

                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c + 1; i < 8; i++)
                        {
                             
                            if (board[r, i] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] < 0)
                            {

                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[r, i] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c - 1; i >= 0; i--)
                        {
                             
                            if (board[r, i] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] < 0)
                            {

                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[r, i] = 1;
                            }
                        }
                    }
                    break;

                case 90:
                    if (Move)
                    {
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {
                                 
                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] < 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }

                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                         
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                 
                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] < 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                           
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {
                                 
                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] < 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                           
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                 
                                if (board[i, s] > 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] < 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                         

                         


                        
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                             
                            if (board[i, c] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] < 0)
                            {

                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                             
                            if (board[i, c] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] < 0)
                            {

                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c + 1; i < 8; i++)
                        {
                             
                            if (board[r, i] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] < 0)
                            {

                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[r, i] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c - 1; i >= 0; i--)
                        {
                             
                            if (board[r, i] > 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] < 0)
                            {

                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {

                                CurrentAvailable[r, i] = 1;
                            }
                        }

                        ;
                    }
                    break;

                case 1000:
                    if (Move)
                    {
                        for (int i = r - 1; i < r + 2; i++)
                        {
                            if (i < 0) continue;
                            if (i > 7) break;
                            for (int j = c - 1; j < c + 2; j++)
                            {
                                if (j < 0) continue;
                                if (j > 7) break;
                                if (i != r || j != c)
                                {
                                    if (board[i, j] == 0 || board[i, j] < 0)
                                    {
                                        CurrentAvailable[i, j] = 1;
                                    }
                                }
                            }
                        }

                    }
                    break;

                case -10:
                    if (!Move)
                    {
                        if (r + 1 < 8)
                        {
                            if (c - 1 >= 0) { if (board[r + 1, c - 1] > 0) { CurrentAvailable[r + 1, c - 1] = 1; } }//board[r - 1, c - 1] == 0 ||

                            if (c + 1 <= 7) { if (board[r + 1, c + 1] > 0) { CurrentAvailable[r + 1, c + 1] = 1; } }//board[r - 1, c + 1] == 0 || 
                        }
                        if (r == 1 && board[r + 1, c] == 0 && board[r + 2, c] == 0) { CurrentAvailable[r + 1, c] = 1; CurrentAvailable[r + 2, c] = 1; }
                        if (r + 1 < 8) { if (board[r + 1, c] == 0) { CurrentAvailable[r + 1, c] = 1; } }
                        else { /*board[r, c] = -90;*/ }    

                    }
                    break;

                case -30:
                    if (!Move)
                    {
                        if (((r - 2) >= 0) && ((c + 1) < 8) && (board[r - 2, c + 1] >= 0)) { CurrentAvailable[r - 2, c + 1] = 1; };
                        if (((r - 2) >= 0) && ((c - 1) >= 0) && (board[r - 2, c - 1] >= 0)) { CurrentAvailable[r - 2, c - 1] = 1; };
                        if (((r + 2) < 8) && ((c + 1) < 8) && (board[r + 2, c + 1] >= 0)) { CurrentAvailable[r + 2, c + 1] = 1; };
                        if (((r + 2) < 8) && ((c - 1) >= 0) && (board[r + 2, c - 1] >= 0)) { CurrentAvailable[r + 2, c - 1] = 1; };
                        if (((r - 1) >= 0) && ((c - 2) >= 0) && (board[r - 1, c - 2] >= 0)) { CurrentAvailable[r - 1, c - 2] = 1; };
                        if (((r - 1) >= 0) && ((c + 2) < 8) && (board[r - 1, c + 2] >= 0)) { CurrentAvailable[r - 1, c + 2] = 1; };
                        if (((r + 1) < 8) && ((c - 2) >= 0) && (board[r + 1, c - 2] >= 0)) { CurrentAvailable[r + 1, c - 2] = 1; };
                        if (((r + 1) < 8) && ((c + 2) < 8) && (board[r + 1, c + 2] >= 0)) { CurrentAvailable[r + 1, c + 2] = 1; };
                    }
                    break;

                case -33:
                    if (!Move)
                    {
                 
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }

                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                         
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                           
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                           
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {
                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {
                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }

                    }
                    break;

                case -50:
                    if (!Move)
                    {
                        
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                             
                            if (board[i, c] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] > 0)
                            {
                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                             
                            if (board[i, c] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] > 0)
                            {
                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c + 1; i < 8; i++)
                        {
                             
                            if (board[r, i] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] > 0)
                            {
                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[r, i] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c - 1; i >= 0; i--)
                        {
                             
                            if (board[r, i] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] > 0)
                            {
                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[r, i] = 1;
                            }
                        }

                    }
                    break;

                case -90:
                    if (!Move)
                    {
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }

                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                         
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                           
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s++;
                            if (s > 7) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                           
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                            s--;
                            if (s < 0) break;
                            else
                            {
                                 
                                if (board[i, s] < 0)
                                {
                                    break;
                                }
                                else
                                
                                if (board[i, s] > 0)
                                {

                                    CurrentAvailable[i, s] = 1;
                                    break;
                                }
                                else
                                {

                                    CurrentAvailable[i, s] = 1;
                                }
                            }

                        }
                         

                         


                        
                        s = c;
                        for (int i = r - 1; i >= 0; i--)
                        {
                             
                            if (board[i, c] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] > 0)
                            {
                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = c;
                        for (int i = r + 1; i < 8; i++)
                        {
                             
                            if (board[i, c] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[i, c] > 0)
                            {
                                CurrentAvailable[i, c] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[i, c] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c + 1; i < 8; i++)
                        {
                             
                            if (board[r, i] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] > 0)
                            {
                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[r, i] = 1;
                            }
                        }
                         
                        s = r;
                        for (int i = c - 1; i >= 0; i--)
                        {
                             
                            if (board[r, i] < 0)
                            {
                                break;
                            }
                            else
                            
                            if (board[r, i] > 0)
                            {
                                CurrentAvailable[r, i] = 1;
                                break;
                            }
                            else
                            {
                                CurrentAvailable[r, i] = 1;
                            }
                        }
                    }
                    break;

                case -1000:
                    if (!Move)
                    {
                        for (int i = r - 1; i < r + 2; i++)
                        {
                            if (i < 0) continue;
                            if (i > 7) break;
                            for (int j = c - 1; j < c + 2; j++)
                            {
                                if (j < 0) continue;
                                if (j > 7) break;
                                if (i != r || j != c)
                                {
                                    if (board[i, j] == 0 || board[i, j] > 0)
                                    {
                                        CurrentAvailable[i, j] = 1;
                                    }
                                }
                            }
                        }
                    }
                    break;
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (CurrentAvailable[i, j] == 1)
                    {
                        Image Frame = new Image();
                        Uri Frameuri = new Uri("wpf_chess/ramka.gif", UriKind.Relative);
                        Frame.Source = new BitmapImage(Frameuri);
                        Frame.Width = 50;
                        Frame.Height = 50;
                        Frame.HorizontalAlignment = HorizontalAlignment.Left;
                        Frame.VerticalAlignment = VerticalAlignment.Top;
                        Frame.Margin = new Thickness(j * 50, i * 50, 0, 0);
                        BoardGrid.Children.Add(Frame);
                    }
                }
            }

        }

        private void InitialPositions(int[,] arr)
        {

          
            BoardGrid.Children.Clear();

          

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (arr[7, j] == -10) { arr[7, j] = -90; }
                    if (arr[0, j] == 10) { arr[0, j] = 90; }

                    switch (arr[i, j])
                    {
                        case 10:

                            Image WPawn = new Image();
                            Uri WPawnuri = new Uri("wpf_chess/wp.gif", UriKind.Relative);
                            WPawn.Source = new BitmapImage(WPawnuri);
                            WPawn.Width = 50;
                            WPawn.Height = 50;
                            WPawn.HorizontalAlignment = HorizontalAlignment.Left;
                            WPawn.VerticalAlignment = VerticalAlignment.Top;
                            WPawn.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(WPawn);

                            WPawn.MouseDown += Figure_MouseDown;
                            WPawn.MouseUp += Figure_MouseUp;
                            WPawn.MouseMove += Figure_MouseMove;


                            break;

                        case 30:
                            Image WKnight = new Image();
                            Uri WKnighturi = new Uri("wpf_chess/wn.gif", UriKind.Relative);
                            WKnight.Source = new BitmapImage(WKnighturi);
                            WKnight.Width = 50;
                            WKnight.Height = 50;
                            WKnight.HorizontalAlignment = HorizontalAlignment.Left;
                            WKnight.VerticalAlignment = VerticalAlignment.Top;
                            WKnight.Margin = new Thickness(j * 50, i * 50, 0, 0);

                            BoardGrid.Children.Add(WKnight);

                            WKnight.MouseDown += Figure_MouseDown;
                            WKnight.MouseUp += Figure_MouseUp;
                            WKnight.MouseMove += Figure_MouseMove;


                            break;

                        case 33:
                            Image WBishop = new Image();
                            Uri WBishopuri = new Uri("wpf_chess/wb.gif", UriKind.Relative);
                            WBishop.Source = new BitmapImage(WBishopuri);
                            WBishop.Width = 50;
                            WBishop.Height = 50;
                            WBishop.HorizontalAlignment = HorizontalAlignment.Left;
                            WBishop.VerticalAlignment = VerticalAlignment.Top;
                            WBishop.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(WBishop);

                            WBishop.MouseDown += Figure_MouseDown;
                            WBishop.MouseUp += Figure_MouseUp;
                            WBishop.MouseMove += Figure_MouseMove;
                            break;

                        case 50:
                            Image WRook = new Image();
                            Uri WRookuri = new Uri("wpf_chess/wr.gif", UriKind.Relative);
                            WRook.Source = new BitmapImage(WRookuri);
                            WRook.Width = 50;
                            WRook.Height = 50;
                            WRook.HorizontalAlignment = HorizontalAlignment.Left;
                            WRook.VerticalAlignment = VerticalAlignment.Top;
                            WRook.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(WRook);

                            WRook.MouseDown += Figure_MouseDown;
                            WRook.MouseUp += Figure_MouseUp;
                            WRook.MouseMove += Figure_MouseMove;

                            break;

                        case 90:
                            Image WQueen = new Image();
                            Uri WQueenuri = new Uri("wpf_chess/wq.gif", UriKind.Relative);
                            WQueen.Source = new BitmapImage(WQueenuri);
                            WQueen.Width = 50;
                            WQueen.Height = 50;
                            WQueen.HorizontalAlignment = HorizontalAlignment.Left;
                            WQueen.VerticalAlignment = VerticalAlignment.Top;
                            WQueen.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(WQueen);

                            WQueen.MouseDown += Figure_MouseDown;
                            WQueen.MouseUp += Figure_MouseUp;
                            WQueen.MouseMove += Figure_MouseMove;

                            break;

                        case 1000:
                            Image WKing = new Image();
                            Uri WKinguri = new Uri("wpf_chess/wk.gif", UriKind.Relative);
                            WKing.Source = new BitmapImage(WKinguri);
                            WKing.Width = 50;
                            WKing.Height = 50;
                            WKing.HorizontalAlignment = HorizontalAlignment.Left;
                            WKing.VerticalAlignment = VerticalAlignment.Top;
                            WKing.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(WKing);

                            WKing.MouseDown += Figure_MouseDown;
                            WKing.MouseUp += Figure_MouseUp;
                            WKing.MouseMove += Figure_MouseMove;
                            break;

                        case -10:
                            Image BPawn = new Image();
                            Uri BPawnuri = new Uri("wpf_chess/bp.gif", UriKind.Relative);
                            BPawn.Source = new BitmapImage(BPawnuri);
                            BPawn.Width = 50;
                            BPawn.Height = 50;
                            BPawn.HorizontalAlignment = HorizontalAlignment.Left;
                            BPawn.VerticalAlignment = VerticalAlignment.Top;
                            BPawn.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(BPawn);


                            BPawn.MouseDown += Figure_MouseDown;
                            BPawn.MouseUp += Figure_MouseUp;
                            BPawn.MouseMove += Figure_MouseMove;


                            break;

                        case -30:
                            Image BKnight = new Image();
                            Uri BKnighturi = new Uri("wpf_chess/bn.gif", UriKind.Relative);
                            BKnight.Source = new BitmapImage(BKnighturi);
                            BKnight.Width = 50;
                            BKnight.Height = 50;
                            BKnight.HorizontalAlignment = HorizontalAlignment.Left;
                            BKnight.VerticalAlignment = VerticalAlignment.Top;
                            BKnight.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(BKnight);


                            BKnight.MouseDown += Figure_MouseDown;
                            BKnight.MouseUp += Figure_MouseUp;
                            BKnight.MouseMove += Figure_MouseMove;
                            break;

                        case -33:
                            Image BBishop = new Image();
                            Uri BBishopuri = new Uri("wpf_chess/bb.gif", UriKind.Relative);
                            BBishop.Source = new BitmapImage(BBishopuri);
                            BBishop.Width = 50;
                            BBishop.Height = 50;
                            BBishop.HorizontalAlignment = HorizontalAlignment.Left;
                            BBishop.VerticalAlignment = VerticalAlignment.Top;
                            BBishop.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(BBishop);

                            BBishop.MouseDown += Figure_MouseDown;
                            BBishop.MouseUp += Figure_MouseUp;
                            BBishop.MouseMove += Figure_MouseMove;
                            break;

                        case -50:
                            Image BRook = new Image();
                            Uri BRookuri = new Uri("wpf_chess/br.gif", UriKind.Relative);
                            BRook.Source = new BitmapImage(BRookuri);
                            BRook.Width = 50;
                            BRook.Height = 50;
                            BRook.HorizontalAlignment = HorizontalAlignment.Left;
                            BRook.VerticalAlignment = VerticalAlignment.Top;
                            BRook.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(BRook);

                            BRook.MouseDown += Figure_MouseDown;
                            BRook.MouseUp += Figure_MouseUp;
                            BRook.MouseMove += Figure_MouseMove;
                            break;

                        case -90:
                            Image BQueen = new Image();
                            Uri BQueenuri = new Uri("wpf_chess/bq.gif", UriKind.Relative);
                            BQueen.Source = new BitmapImage(BQueenuri);
                            BQueen.Width = 50;
                            BQueen.Height = 50;
                            BQueen.HorizontalAlignment = HorizontalAlignment.Left;
                            BQueen.VerticalAlignment = VerticalAlignment.Top;
                            BQueen.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(BQueen);

                            BQueen.MouseDown += Figure_MouseDown;
                            BQueen.MouseUp += Figure_MouseUp;
                            BQueen.MouseMove += Figure_MouseMove;
                            break;

                        case -1000:
                            Image BKing = new Image();
                            Uri BKinguri = new Uri("wpf_chess/bk.gif", UriKind.Relative);
                            BKing.Source = new BitmapImage(BKinguri);
                            BKing.Width = 50;
                            BKing.Height = 50;
                            BKing.HorizontalAlignment = HorizontalAlignment.Left;
                            BKing.VerticalAlignment = VerticalAlignment.Top;
                            BKing.Margin = new Thickness(j * 50, i * 50, 0, 0);
                            BoardGrid.Children.Add(BKing);

                            BKing.MouseDown += Figure_MouseDown;
                            BKing.MouseUp += Figure_MouseUp;
                            BKing.MouseMove += Figure_MouseMove;
                            break;
                    }
                }
            }
        }

        private void Figure_MouseMove(object sender, MouseEventArgs e)
        {
            Image Figure = sender as Image;
            if (FigureClicked) { Figure.Margin = new Thickness(e.GetPosition(this).X - DeltaX, e.GetPosition(this).Y - DeltaY, 0, 0); }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DispatcherTimer dt = new DispatcherTimer();
            dt.Interval = TimeSpan.FromSeconds(1);
            dt.Tick += Dt_Tick;
            dt.Start();
            box3.Text = "White:  " + "0:0:0";
            box2.Text = "Black:  " + "0:0:0:0";
           
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            checker(); //
            tempwsec++;
            int sec = tempwsec;
            int minutes = sec / 60;
            int newSec = sec - minutes * 60;
            int hour = minutes / 60;
            int newMinnutes = minutes - hour * 60;
            TimeSpan time = new TimeSpan(0, hour, newMinnutes, newSec);
            box3.Text = "White:  " + time.Hours.ToString() + ":" + time.Minutes.ToString() + ":" + time.Seconds.ToString();
        }

        private void Figure_MouseUp(object sender, MouseButtonEventArgs e)
        {
            FigureClicked = false;
            Image Figure = sender as Image;

            Figure.Margin = new Thickness((int)Figure.Margin.Left + 25, (int)Figure.Margin.Top + 25, 0, 0);
            StackPanel.SetZIndex(BoardGrid, 0);
            StackPanel.SetZIndex(Figure, 0);

            int x = (int)Figure.Margin.Left;
            int y = (int)Figure.Margin.Top;

            x /= 50;
            y /= 50;

            if (y < 8 && x < 8)
            {
                if (CurrentAvailable[y, x] == 1)
                {
                    board[TempY, TempX] = 0;
                    board[y, x] = Value;
                }
            }

            InitialPositions(board);
            
            var timer = Stopwatch.StartNew();
      
            if (y < 8 && x < 8)
            {
                if ((TempX != x || TempY != y) && CurrentAvailable[y, x] == 1)
                {
                    MoveObj(minimaxRoot(1, board, true), board);
                    Move = true;
                }
            }
            timer.Stop();

            tempbsec += Convert.ToInt32(timer.ElapsedMilliseconds);
            int milliseconds = tempbsec;
            int sec = tempbsec / 1000;
            int newmilliseconds = milliseconds - sec * 1000;
            int minutes = sec / 60;
            int newSec = sec - minutes * 60;
            int hour = minutes / 60;
            int newMinnutes = minutes - hour * 60;
            TimeSpan time = new TimeSpan(0, hour, newMinnutes, newSec, newmilliseconds);
            box2.Text = "Black:  " + time.Hours.ToString() + ":" + time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
            ResetSquareArray(CurrentAvailable);
        }
      

        private void Figure_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Image Figure = sender as Image;
            if (e.ButtonState == e.LeftButton)
            {
                TempX = (int)Figure.Margin.Left;
                TempY = (int)Figure.Margin.Top;
                TempX /= 50;
                TempY /= 50;

                Value = board[TempY, TempX];
                StackPanel.SetZIndex(BoardGrid, 0);
                StackPanel.SetZIndex(Figure, 1);
                FigureClicked = true;
                DeltaX = e.GetPosition(this).X - Figure.Margin.Left;
                DeltaY = e.GetPosition(this).Y - Figure.Margin.Top;

                AvailableMoves(TempY, TempX, Value);
            }
        }

        private void Reset_Board(object sender, RoutedEventArgs e)
        {
/////
            copy(board, resetBoard);
            tempbsec = 0;
            tempwsec = 0;
            box2.Text = "Black:  " + "0:0:0:0";
            InitialPositions(board);
        }

        private bool CheckCheck(int[,] chboard, bool black)//false - withs, true - black (shax) 
        {
            UWB_Attacks(!black, chboard);
            finded = false;

            if (black)
            {
                if (!finded)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (!finded)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (!finded)
                                {
                                    if ((chboard[i, j] == -1000) && WAttack[i, j] == 1) { check = true; finded = true; } else { check = false; }



                                }
                            }
                        }
                    }
                }


            }
            else
            {
                if (!finded)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (!finded)
                        {
                            for (int j = 0; j < 8; j++)
                            {
                                if (!finded)
                                {

                             
                                    if (chboard[i, j] == 1000 && BAttack[i, j] == -1) { check = true; finded = true; } else { check = false; }


                                }
                            }
                        }
                    }
                }
            }
            return check;
        }

        private void U_Attacks(bool black, int[,] mqboard)
        {
            ResetSquareArray(WCAttack);
            ResetSquareArray(BCAttack);

            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {
                    if ((black == false) && (mqboard[r, c] > 0))
                    {
                        switch (mqboard[r, c])
                        {
                            case 10:

                                if (r - 1 >= 0)
                                {
                                    if (c - 1 >= 0)
                                    {
                                        if (mqboard[r - 1, c - 1] <= 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, r - 1, c - 1);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[r - 1, c - 1] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                    if (c + 1 <= 7)
                                    {
                                        if (mqboard[r - 1, c + 1] <= 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, r - 1, c + 1);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[r - 1, c + 1] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                }


                                break;

                            case 30:

                                if (((r - 2) >= 0) && ((c + 1) < 8) && (mqboard[r - 2, c + 1] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 2, c + 1);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r - 2, c + 1] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r - 2) >= 0) && ((c - 1) >= 0) && (mqboard[r - 2, c - 1] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 2, c - 1);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r - 2, c - 1] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 2) < 8) && ((c + 1) < 8) && (mqboard[r + 2, c + 1] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 2, c + 1);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r + 2, c + 1] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 2) < 8) && ((c - 1) >= 0) && (mqboard[r + 2, c - 1] <= 0))
                                {
                                    WAttack[r + 2, c - 1] = 1;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 2, c - 1);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r + 2, c - 1] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r - 1) >= 0) && ((c - 2) >= 0) && (mqboard[r - 1, c - 2] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 1, c - 2);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r - 1, c - 2] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r - 1) >= 0) && ((c + 2) < 8) && (mqboard[r - 1, c + 2] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 1, c + 2);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r - 1, c + 2] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 1) < 8) && ((c - 2) >= 0) && (mqboard[r + 1, c - 2] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 1, c - 2);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r + 1, c - 2] = 1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 1) < 8) && ((c + 2) < 8) && (mqboard[r + 1, c + 2] <= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 1, c + 2);
                                    if (!CheckCheck(mqboard, false)) { WCAttack[r + 1, c + 2] = 1; }
                                    copy(mqboard, temp);
                                };
                                break;

                            case 33:

                                int skm401;
                                skm401 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    skm401++;
                                    if (skm401 > 7) break;
                                    else
                                    {
                                        if (mqboard[i, skm401] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skm401] < 0)
                                        {

                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm401);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm401] = 1; }
                                            copy(mqboard, temp);

                                            break;
                                        }

                                        else
                                        {

                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm401);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm401] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                           
                                int skm501;
                                skm501 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    skm501--;
                                    if (skm501 < 0) break;
                                    else
                                    {
                                        if (mqboard[i, skm501] > 0)
                                        {
                                            break;
                                        }
                                        else
                                      
                                        if (mqboard[i, skm501] < 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm501);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm501] = 1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm501);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm501] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                }
                       
                                int skm601;
                                skm601 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    skm601++;
                                    if (skm601 > 7) break;
                                    else
                                    {
                                        if (mqboard[i, skm601] > 0)
                                        {
                                            break;
                                        }
                                        else
                                   
                                        if (mqboard[i, skm601] < 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm601);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm601] = 1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm601);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm601] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                }
                           
                                int skm701;
                                skm701 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    skm701--;
                                    if (skm701 < 0) break;
                                    else
                                    {
                                        if (mqboard[i, skm701] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skm701] < 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm701);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm701] = 1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm701);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm701] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                break;


                            case 50:
                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);

                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);

                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);

                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);

                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);


                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);

                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                break;


                            case 90:
                                int skm4;
                                skm4 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    skm4++;
                                    if (skm4 > 7) break;
                                    else
                                    {
                                        if (mqboard[i, skm4] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skm4] < 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm4);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm4] = 1; }
                                            copy(mqboard, temp);

                                            break;
                                        }

                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm4);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm4] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                }
                                 
                                int skm5;
                                skm5 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    skm5--;
                                    if (skm5 < 0) break;
                                    else
                                    {    
                                        if (mqboard[i, skm5] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skm5] < 0)
                                        {

                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm5);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm5] = 1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm5);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm5] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                }
                                   
                                int skm6;
                                skm6 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    skm6++;
                                    if (skm6 > 7) break;
                                    else
                                    {  
                                        if (mqboard[i, skm6] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skm6] < 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm6);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm6] = 1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm6);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm6] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                   
                                int skm7;
                                skm7 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    skm7--;
                                    if (skm7 < 0) break;
                                    else
                                    {   
                                        if (mqboard[i, skm7] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skm7] < 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm7);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm7] = 1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skm7);
                                            if (!CheckCheck(mqboard, false)) { WCAttack[i, skm7] = 1; }
                                            copy(mqboard, temp);
                                        }
                                    }
                                }
                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[i, c] = 1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, false)) { WCAttack[r, i] = 1; }
                                        copy(mqboard, temp);
                                    }
                                }

                                break;

                            case 1000:

                                for (int i = r - 1; i < r + 2; i++)
                                {
                                    if (i < 0) continue;
                                    if (i > 7) break;
                                    for (int j = c - 1; j < c + 2; j++)
                                    {
                                        if (j < 0) continue;
                                        if (j > 7) break;
                                        if (i != r || j != c)
                                        {
                                            if (mqboard[i, j] == 0 || mqboard[i, j] < 0)
                                            {

                                                int[,] temp = new int[8, 8];
                                                copy(temp, mqboard);
                                                CoordMove(r, c, i, j);
                                                if (!CheckCheck(mqboard, false)) { WCAttack[i, j] = 1; }
                                                copy(mqboard, temp);
                                            }
                                        }
                                    }
                                }

                                break;
                        }
                    }
                    else if ((black == true) && (mqboard[r, c] < 0))
                    {
                        switch (mqboard[r, c])
                        {
                            case -10:

                                if (r + 1 < 8)
                                {

                                    if (c - 1 >= 0)
                                    {
                                        if (mqboard[r + 1, c - 1] >= 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, r + 1, c - 1);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[r + 1, c - 1] = -1; }
                                            copy(mqboard, temp);

                                        }
                                    }

                                    if (c + 1 <= 7)
                                    {
                                        if (mqboard[r + 1, c + 1] >= 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, r + 1, c + 1);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[r + 1, c + 1] = -1; }
                                            copy(mqboard, temp);

                                        }
                                    }
                                }
                                break;

                            case -30:

                                if (((r - 2) >= 0) && ((c + 1) < 8) && (mqboard[r - 2, c + 1] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 2, c + 1);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r - 2, c + 1] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r - 2) >= 0) && ((c - 1) >= 0) && (mqboard[r - 2, c - 1] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 2, c - 1);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r - 2, c - 1] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 2) < 8) && ((c + 1) < 8) && (mqboard[r + 2, c + 1] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 2, c + 1);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r + 2, c + 1] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 2) < 8) && ((c - 1) >= 0) && (mqboard[r + 2, c - 1] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 2, c - 1);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r + 2, c - 1] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r - 1) >= 0) && ((c - 2) >= 0) && (mqboard[r - 1, c - 2] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 1, c - 2);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r - 1, c - 2] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r - 1) >= 0) && ((c + 2) < 8) && (mqboard[r - 1, c + 2] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r - 1, c + 2);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r - 1, c + 2] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 1) < 8) && ((c - 2) >= 0) && (mqboard[r + 1, c - 2] >= 0))
                                {
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 1, c - 2);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r + 1, c - 2] = -1; }
                                    copy(mqboard, temp);
                                };
                                if (((r + 1) < 8) && ((c + 2) < 8) && (mqboard[r + 1, c + 2] >= 0))
                                {
                                    BAttack[r + 1, c + 2] = -1;
                                    int[,] temp = new int[8, 8];
                                    copy(temp, mqboard);
                                    CoordMove(r, c, r + 1, c + 2);
                                    if (!CheckCheck(mqboard, true)) { BCAttack[r + 1, c + 2] = -1; }
                                    copy(mqboard, temp);
                                };
                                break;

                            case -33:

                             
                                int sk1;
                                sk1 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sk1++;
                                    if (sk1 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sk1] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sk1] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sk1);

                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sk1] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }

                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sk1);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sk1] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                 
                                int sk12;
                                sk12 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sk12--;
                                    if (sk12 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sk12] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sk12] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sk12);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sk12] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sk12);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sk12] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                   
                                int sk3;
                                sk3 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sk3++;
                                    if (sk3 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sk3] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sk3] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sk3);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sk3] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sk3);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sk3] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                   
                                int skmm4;
                                skmm4 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    skmm4--;
                                    if (skmm4 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, skmm4] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, skmm4] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skmm4);

                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, skmm4] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, skmm4);


                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, skmm4] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                break;

                            case -50:

                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, c] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {


                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, c] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, c] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, c] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                break;

                            case -90:
                                int sq;
                                sq = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sq++;
                                    if (sq > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sq] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sq] > 0)
                                        {

                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sq);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sq] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }

                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sq);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sq] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                 
                                int kms;
                                kms = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    kms--;
                                    if (kms < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, kms] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, kms] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, kms);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, kms] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, kms);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, kms] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                   
                                int sq1;
                                sq1 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sq1++;
                                    if (sq1 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sq1] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sq1] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sq1);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sq1] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {

                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sq1);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sq1] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                   
                                int sq2;
                                sq2 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sq2--;
                                    if (sq2 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sq2] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sq2] > 0)
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sq2);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sq2] = -1; }
                                            copy(mqboard, temp);
                                            break;
                                        }
                                        else
                                        {
                                            int[,] temp = new int[8, 8];
                                            copy(temp, mqboard);
                                            CoordMove(r, c, i, sq2);
                                            if (!CheckCheck(mqboard, true)) { BCAttack[i, sq2] = -1; }
                                            copy(mqboard, temp);
                                        }
                                    }

                                }
                                   

                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, s] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, s] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, s] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, i, c);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[i, s] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {

                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                        break;
                                    }
                                    else
                                    {
                                        int[,] temp = new int[8, 8];
                                        copy(temp, mqboard);
                                        CoordMove(r, c, r, i);
                                        if (!CheckCheck(mqboard, true)) { BCAttack[r, i] = -1; }
                                        copy(mqboard, temp);
                                    }
                                }

                                break;


                            case -1000:

                                for (int ii = r - 1; ii < r + 2; ii++)
                                {
                                    if (ii < 0) continue;
                                    if (ii > 7) break;
                                    for (int jj = c - 1; jj < c + 2; jj++)
                                    {

                                        if (jj < 0) continue;
                                        if (jj > 7) break;
                                        if (ii != r || jj != c)
                                        {

                                            if (mqboard[ii, jj] == 0 || mqboard[ii, jj] > 0)
                                            {


                                                int[,] temp = new int[8, 8];
                                                copy(temp, mqboard);
                                                CoordMove(r, c, ii, jj);
                                                if (!CheckCheck(mqboard, true)) { BCAttack[ii, jj] = -1; }
                                                copy(mqboard, temp);
                                            }
                                        }
                                    }

                                }

                                break;

                        }
                    }
                }
            }



        }

        private void UWB_Attacks(bool black, int[,] mqboard)
        {

            ResetSquareArray(WAttack);
            ResetSquareArray(BAttack);


            for (int r = 0; r < 8; r++)
            {
                for (int c = 0; c < 8; c++)
                {

                    if ((black == false) && (mqboard[r, c] > 0))
                    {
                        switch (mqboard[r, c])
                        {


                            case 10:

                                if (r - 1 >= 0)
                                {
                                    if (c - 1 >= 0)
                                    {
                                        if (mqboard[r - 1, c - 1] <= 0)
                                        {
                                            WAttack[r - 1, c - 1] = 1;


                                        }
                                    }
                                    if (c + 1 <= 7)
                                    {
                                        if (mqboard[r - 1, c + 1] <= 0)
                                        {
                                            WAttack[r - 1, c + 1] = 1;


                                        }
                                    }
                                }


                                break;

                            case 30:

                                if (((r - 2) >= 0) && ((c + 1) < 8) && (mqboard[r - 2, c + 1] <= 0))
                                {
                                    WAttack[r - 2, c + 1] = 1;


                                };
                                if (((r - 2) >= 0) && ((c - 1) >= 0) && (mqboard[r - 2, c - 1] <= 0)) { WAttack[r - 2, c - 1] = 1; };
                                if (((r + 2) < 8) && ((c + 1) < 8) && (mqboard[r + 2, c + 1] <= 0)) { WAttack[r + 2, c + 1] = 1; };
                                if (((r + 2) < 8) && ((c - 1) >= 0) && (mqboard[r + 2, c - 1] <= 0)) { WAttack[r + 2, c - 1] = 1; };
                                if (((r - 1) >= 0) && ((c - 2) >= 0) && (mqboard[r - 1, c - 2] <= 0)) { WAttack[r - 1, c - 2] = 1; };
                                if (((r - 1) >= 0) && ((c + 2) < 8) && (mqboard[r - 1, c + 2] <= 0)) { WAttack[r - 1, c + 2] = 1; };
                                if (((r + 1) < 8) && ((c - 2) >= 0) && (mqboard[r + 1, c - 2] <= 0)) { WAttack[r + 1, c - 2] = 1; };
                                if (((r + 1) < 8) && ((c + 2) < 8) && (mqboard[r + 1, c + 2] <= 0)) { WAttack[r + 1, c + 2] = 1; };
                                break;

                            case 33:

                                int sub;
                                sub = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sub++;
                                    if (sub > 7) break;
                                    else
                                    {

                                        if (mqboard[i, sub] > 0)
                                        {
                                            break;
                                        }
                                        else

                                        if (mqboard[i, sub] < 0)
                                        {
                                            WAttack[i, sub] = 1;
                                            break;
                                        }

                                        else
                                        {
                                            WAttack[i, sub] = 1;
                                        }
                                    }

                                }
                                int sub1;
                                sub1 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sub1--;
                                    if (sub1 < 0) break;
                                    else
                                    {
                                        if (mqboard[i, sub1] > 0)
                                        {
                                            break;
                                        }
                                        else

                                        if (mqboard[i, sub1] < 0)
                                        {
                                            WAttack[i, sub1] = 1;
                                            break;
                                        }
                                        else
                                        {
                                            WAttack[i, sub1] = 1;
                                        }
                                    }

                                }

                                int sub2;
                                sub2 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub2++;
                                    if (sub2 > 7) break;
                                    else
                                    {

                                        if (mqboard[i, sub2] > 0)
                                        {
                                            break;
                                        }
                                        else

                                        if (mqboard[i, sub2] < 0)
                                        {
                                            WAttack[i, sub2] = 1;
                                            break;
                                        }
                                        else
                                        {
                                            WAttack[i, sub2] = 1;
                                        }
                                    }

                                }
                                int sub3;
                                sub3 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub3--; if (sub3 < 0) break;
                                    else
                                    {
                                        if (mqboard[i, sub3] > 0) { break; }
                                        else
                                       if (mqboard[i, sub3] < 0) { WAttack[i, sub3] = 1; break; }
                                        else { WAttack[i, sub3] = 1; }
                                    }
                                }

                                break;


                            case 50:
                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {
                                        WAttack[i, c] = 1;
                                        break;
                                    }
                                    else
                                    {
                                        WAttack[i, c] = 1;
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {
                                        WAttack[i, c] = 1;
                                        break;
                                    }
                                    else
                                    {
                                        WAttack[i, c] = 1;
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {
                                        WAttack[r, i] = 1;
                                        break;
                                    }
                                    else
                                    {
                                        WAttack[r, i] = 1;
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {
                                        WAttack[r, i] = 1;
                                        break;
                                    }
                                    else
                                    {
                                        WAttack[r, i] = 1;
                                    }
                                }

                                break;


                            case 90:
                                int sub5;
                                sub5 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sub5++;
                                    if (sub5 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub5] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub5] < 0)
                                        {

                                            WAttack[i, sub5] = 1;
                                            break;
                                        }

                                        else
                                        {

                                            WAttack[i, sub5] = 1;
                                        }
                                    }

                                }
                                 
                                int sub6;
                                sub6 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sub6--;
                                    if (sub6 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub6] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub6] < 0)
                                        {

                                            WAttack[i, sub6] = 1;
                                            break;
                                        }
                                        else
                                        {

                                            WAttack[i, sub6] = 1;
                                        }
                                    }

                                }
                                   
                                int sub7;
                                sub7 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub7++;
                                    if (sub7 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub7] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub7] < 0)
                                        {

                                            WAttack[i, sub7] = 1;
                                            break;
                                        }
                                        else
                                        {

                                            WAttack[i, sub7] = 1;
                                        }
                                    }

                                }
                                   
                                int sub8;
                                sub8 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub8--;
                                    if (sub8 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub8] > 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub8] < 0)
                                        {

                                            WAttack[i, sub8] = 1;
                                            break;
                                        }
                                        else
                                        {

                                            WAttack[i, sub8] = 1;
                                        }
                                    }

                                }
                                 

                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {

                                        WAttack[i, c] = 1;
                                        break;
                                    }
                                    else
                                    {

                                        WAttack[i, c] = 1;
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] < 0)
                                    {

                                        WAttack[i, c] = 1;
                                        break;
                                    }
                                    else
                                    {

                                        WAttack[i, c] = 1;
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {

                                        WAttack[r, i] = 1;
                                        break;
                                    }
                                    else
                                    {

                                        WAttack[r, i] = 1;
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] > 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] < 0)
                                    {

                                        WAttack[r, i] = 1;
                                        break;
                                    }
                                    else
                                    {

                                        WAttack[r, i] = 1;
                                    }
                                }


                                break;

                            case 1000:

                                for (int i = r - 1; i < r + 2; i++)
                                {
                                    if (i < 0) continue;
                                    if (i > 7) break;
                                    for (int j = c - 1; j < c + 2; j++)
                                    {
                                        if (j < 0) continue;
                                        if (j > 7) break;
                                        if (i != r || j != c)
                                        {
                                            if (mqboard[i, j] == 0 || mqboard[i, j] < 0)
                                            {
                                                WAttack[i, j] = 1;


                                            }
                                        }
                                    }
                                }


                                break;
                        }
                    }
                    else if ((black == true) && (mqboard[r, c] < 0))
                    {
                        switch (mqboard[r, c])
                        {
                            case -10:

                                if (r + 1 < 8)
                                {

                                    if (c - 1 >= 0) { if (mqboard[r + 1, c - 1] >= 0) { BAttack[r + 1, c - 1] = -1; } }

                                    if (c + 1 <= 7) { if (mqboard[r + 1, c + 1] >= 0) { BAttack[r + 1, c + 1] = -1; } }
                                }

                                break;

                            case -30:

                                if (((r - 2) >= 0) && ((c + 1) < 8) && (mqboard[r - 2, c + 1] >= 0)) { BAttack[r - 2, c + 1] = -1; };
                                if (((r - 2) >= 0) && ((c - 1) >= 0) && (mqboard[r - 2, c - 1] >= 0)) { BAttack[r - 2, c - 1] = -1; };
                                if (((r + 2) < 8) && ((c + 1) < 8) && (mqboard[r + 2, c + 1] >= 0)) { BAttack[r + 2, c + 1] = -1; };
                                if (((r + 2) < 8) && ((c - 1) >= 0) && (mqboard[r + 2, c - 1] >= 0)) { BAttack[r + 2, c - 1] = -1; };
                                if (((r - 1) >= 0) && ((c - 2) >= 0) && (mqboard[r - 1, c - 2] >= 0)) { BAttack[r - 1, c - 2] = -1; };
                                if (((r - 1) >= 0) && ((c + 2) < 8) && (mqboard[r - 1, c + 2] >= 0)) { BAttack[r - 1, c + 2] = -1; };
                                if (((r + 1) < 8) && ((c - 2) >= 0) && (mqboard[r + 1, c - 2] >= 0)) { BAttack[r + 1, c - 2] = -1; };
                                if (((r + 1) < 8) && ((c + 2) < 8) && (mqboard[r + 1, c + 2] >= 0)) { BAttack[r + 1, c + 2] = -1; };
                                break;

                            case -33:

                                 
                                int ks;
                                ks = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    ks++;
                                    if (ks > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, ks] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, ks] > 0)
                                        {
                                            BAttack[i, ks] = -1;
                                            break;
                                        }

                                        else
                                        {
                                            BAttack[i, ks] = -1;
                                        }
                                    }

                                }
                                 
                                int k1s;
                                k1s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    k1s--;
                                    if (k1s < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, k1s] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, k1s] > 0)
                                        {
                                            BAttack[i, k1s] = -1;
                                            break;
                                        }
                                        else
                                        {
                                            BAttack[i, k1s] = -1;
                                        }
                                    }

                                }
                                   
                                int sub9;
                                sub9 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub9++;
                                    if (sub9 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub9] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub9] > 0)
                                        {
                                            BAttack[i, sub9] = -1;
                                            break;
                                        }
                                        else
                                        {
                                            BAttack[i, sub9] = -1;
                                        }
                                    }

                                }
                                   
                                int sub10;
                                sub10 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub10--;
                                    if (sub10 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub10] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub10] > 0)
                                        {
                                            BAttack[i, sub10] = -1;
                                            break;
                                        }
                                        else
                                        {
                                            BAttack[i, sub10] = -1;
                                        }
                                    }

                                }
                                break;

                            case -50:

                                
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {
                                        BAttack[i, c] = -1;


                                    }
                                    else
                                    {
                                        BAttack[i, c] = -1;

                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {
                                        BAttack[i, c] = -1;

                                    }
                                    else
                                    {
                                        BAttack[i, c] = -1;


                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {
                                        BAttack[r, i] = -1;

                                        break;
                                    }
                                    else
                                    {
                                        BAttack[r, i] = -1;


                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {
                                        BAttack[r, i] = -1;

                                        break;
                                    }
                                    else
                                    {
                                        BAttack[r, i] = -1;


                                    }
                                }
                                break;

                            case -90:
                                int sub23;
                                sub23 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sub23++;
                                    if (sub23 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub23] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub23] > 0)
                                        {

                                            BAttack[i, sub23] = -1;
                                            break;
                                        }

                                        else
                                        {
                                            BAttack[i, sub23] = -1;
                                        }
                                    }

                                }
                                 
                                int sub24;
                                sub24 = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                    sub24--;
                                    if (sub24 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub24] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub24] > 0)
                                        {
                                            BAttack[i, sub24] = -1;
                                            break;
                                        }
                                        else
                                        {
                                            BAttack[i, sub24] = -1;
                                        }
                                    }

                                }
                                   
                                int sub25;
                                sub25 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub25++;
                                    if (sub25 > 7) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub25] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub25] > 0)
                                        {
                                            BAttack[i, sub25] = -1;
                                            break;
                                        }
                                        else
                                        {

                                            BAttack[i, sub25] = -1;
                                        }
                                    }

                                }
                                   
                                int sub26;
                                sub26 = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                    sub26--;
                                    if (sub26 < 0) break;
                                    else
                                    {
                                         
                                        if (mqboard[i, sub26] < 0)
                                        {
                                            break;
                                        }
                                        else
                                        
                                        if (mqboard[i, sub26] > 0)
                                        {
                                            BAttack[i, sub26] = -1;
                                            break;
                                        }
                                        else
                                        {
                                            BAttack[i, sub26] = -1;
                                        }
                                    }

                                }
                                 

                                                         
                                s = c;
                                for (int i = r - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {
                                        BAttack[i, c] = -1;
                                        break;
                                    }
                                    else
                                    {

                                        BAttack[i, c] = -1;
                                    }
                                }
                                 
                                s = c;
                                for (int i = r + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[i, c] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[i, c] > 0)
                                    {

                                        BAttack[i, c] = -1;
                                        break;
                                    }
                                    else
                                    {
                                        BAttack[i, c] = -1;
                                    }
                                }
                                 
                                s = r;
                                for (int i = c + 1; i < 8; i++)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {
                                        BAttack[r, i] = -1;
                                        break;
                                    }
                                    else
                                    {

                                        BAttack[r, i] = -1;
                                    }
                                }
                                 
                                s = r;
                                for (int i = c - 1; i >= 0; i--)
                                {
                                     
                                    if (mqboard[r, i] < 0)
                                    {
                                        break;
                                    }
                                    else
                                    
                                    if (mqboard[r, i] > 0)
                                    {
                                        BAttack[r, i] = -1;
                                        break;
                                    }
                                    else
                                    {
                                        BAttack[r, i] = -1;
                                    }
                                }

                                break;


                            case -1000:

                                for (int ii = r - 1; ii < r + 2; ii++)
                                {
                                    if (ii < 0) continue;
                                    if (ii > 7) break;
                                    for (int jj = c - 1; jj < c + 2; jj++)
                                    {

                                        if (jj < 0) continue;
                                        if (jj > 7) break;
                                        if (ii != r || jj != c)
                                        {

                                            if (mqboard[ii, jj] == 0 || mqboard[ii, jj] > 0)
                                            {

                                                BAttack[ii, jj] = -1;


                                            }
                                        }
                                    }

                                }

                                break;

                        }
                    }
                }
            }
        }

        private decimal MoveCount(bool black, int[,] mqboard)
        {
            U_Attacks(black, mqboard);
            decimal MoveCount;
            MoveCount = 0;
            if (black == false)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {

                        if (WCAttack[i, j] == 1) { MoveCount++; }
                    }
                }
            }
            else if (black == true)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {

                   
                        if (BCAttack[i, j] == -1) { MoveCount++; }
                    }
                }
            }
            MoveCount /= 10;
            return MoveCount;
        }
    }
}
