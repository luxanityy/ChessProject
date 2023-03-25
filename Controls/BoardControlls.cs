
using Chess.Common;
using Chess.Models;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using System.Media;

namespace Chess.Controls

// TODO: Da se pojedene figure pokazujev dole koje su i materijal
// U chessboard ima pieceCounter[10]
// pieceCounter[0] = beliTop na tablu
// pieceCounter[1] = beliKonj na tablu
// pieceCounter[2] = beliLovac na tablu
// pieceCounter[3] = beliKraljica na tablu
// pieceCounter[4] = beliPijun na tablu
{
    public partial class BoardControlls : ComponentBase
    {
        public StartGameModel StartGameInput { get; set; } = new();
        public bool PromotionModalWhiteVisible { get; set; } = false;
        public Chessboard Board { get; set; }
        public bool PromotionModalBlackVisible { get; set; } = false;

        public bool[,] ValidMoves = new bool[8, 8];

        public int lastMFRow { get; set; } = -1;
        public int lastMFCol { get; set; } = -1;
        public int lastMTRow { get; set; } = -1;
        public int lastMTCol { get; set; } = -1;

        public string WhiteTimer { get; set; }
        public string BlackTimer { get; set; }
        public Timer? Timer;
        SoundPlayer checkMateSound = new SoundPlayer(@"wwwroot/Sounds/checkmate.wav");

        public bool GameOver { get; set; } = false;
        public BoardControlls()
        {
            Board = new(0, 0);
            Board.gameOver = true;
        }
        public async Task<bool> GameoverAsync()
        {
            if(Board != null)
            {
                if (Board.isGameOver())
                {
                    checkMateSound.Play();
                    Dispose();
                    if (Board.GetWhiteTimer() <= 0 || Board.GetBlackTimer() <= 0)
                        return true;

                    await Task.Delay(200);
                    return true;
                }
            }
            return false;
        }
        public string GetWhiteTimer()
        {
            if(Board != null)
            {
                int time = Board.GetWhiteTimer()/10;
                int mins = time / 60;
                int secs = time % 60;

                return $"{mins.ToString("D2")}:{secs.ToString("D2")}";
            }
            return "00:00";

        }
        public string GetBlackTimer()
        {
            if (Board != null)
            {
                int time = Board.GetBlackTimer() / 10;
                int mins = time / 60;
                int secs = time % 60;

                return $"{mins.ToString("D2")}:{secs.ToString("D2")}";
            }
                
            return "00:00";
        }
        public void NewGame()
        {
            if(StartGameInput.Timer == 0)
            {
                StartGameInput.Timer = 600;
            }
            Board = new(StartGameInput.Timer, StartGameInput.Increment);
            GameOver = false;
            parsed = new char[4];
            _move = string.Empty;
            lastMFRow = -1;
            lastMFCol = -1;
            lastMTRow = -1;
            lastMTCol = -1;
            Timer = new Timer((_) =>
            {
                InvokeAsync(async () =>
                {
                    WhiteTimer = GetWhiteTimer();
                    BlackTimer = GetBlackTimer();
                    GameOver = await GameoverAsync();
                    StateHasChanged();
                });
            }, null, 0, 100);
        }
        public void Dispose()
        {
            Timer?.Dispose();
            Timer = null;
        }
        public bool IsDarkTile(int row, int col)
        {
            return (row + col) % 2 == 1;
        }
        
        public string GetTileClass(int row, int col)
        {
            if (Board == null)
                return string.Empty;
            string result = string.Empty;
            if (Board.getBoard()[row, col].RightClicked)
            {
                string tileRightClickClass = IsDarkTile(row, col) ? "tile -rightC-black " : "tile-rightC-white ";
                result += tileRightClickClass;
            }

            result += IsDarkTile(row, col) ? "dark" : "light";
            result += " ";
            result += GetPieceType(row, col);

            

            return result;
        }

        public string GetPieceType(int row, int col)
        {
            var piece = Board.getBoard()[row,col].getPiece();

            if (piece != null)
            {
                string color = piece.white ? "White" : "Black";
                return color + piece.GetType().Name + " piece";
            }

            return string.Empty;
        }
        public string GetValidMoves(int row, int col)
        {
            string result = string.Empty;
            if (ValidMoves[row, col])
            {
                result += " ";
                result += "valid-move ";
            }
            if(Board != null && Board.getBoard()[row, col].getPiece() != null)
            {
                result += "take ";
            }

            return result;
        }

        private string _move { get; set; } = string.Empty;
        char[] parsed = new char[4];
        public async Task MovePieceAsync(int row, int col)
        {
            if (Board.gameOver)
                return;
            if (_move.Length == 4)
            { 
                _move = string.Empty;
                parsed = new char[4];
            }
            if (string.IsNullOrEmpty(_move))
            {
                FirstClick(row, col);
                PromotionModalWhiteVisible = false;
                PromotionModalBlackVisible = false;

                StateHasChanged();
            }
            else
            {
                if (Board.getBoard()[row, col].getPiece() != null && Board.whiteToMove && Board.getBoard()[row,col].getPiece().white)
                {
                    FirstClick(row, col);
                    StateHasChanged();
                    return;
                }
                if (Board.getBoard()[row, col].getPiece() != null && !Board.whiteToMove && !Board.getBoard()[row, col].getPiece().white)
                {
                    FirstClick(row, col);
                    StateHasChanged();
                    return;
                }
                parsed[2] = (char)(col + 'a');
                parsed[3] = (char)(8 - row + '0');
                _move = new string(parsed);

                if (Board.isPromotion(_move))
                {
                    if(Board.whiteToMove)
                        PromotionModalWhiteVisible = true;
                    else
                        PromotionModalBlackVisible = true;
                    StateHasChanged();
                    return;
                }
                ValidMoves = new bool[8,8];

                if(!Board.move(_move, "Q"))
                {
                    Console.WriteLine($"Illegal move {_move}");
                    return;
                }
                lastMFRow = 8 - (_move[1] - '0');
                lastMFCol = _move[0] - 'a';
                lastMTRow = row;
                lastMTCol = col;

                StateHasChanged();
                parsed = new char[4]; 
                _move = string.Empty;

                if (Board.isGameOver())
                {
                    await Task.Delay(3000);
                    GameOver = true;
                    Dispose();
                    StateHasChanged();
                }
            }
        }

        private void FirstClick(int row, int col)
        {
            if (Board == null)
                return;
            parsed[0] = (char)(col + 'a');
            parsed[1] = (char)(8 - row + '0');
            _move = new string(parsed).Trim('\0');
            var piece = Board.getBoard()[row, col].getPiece();
            ValidMoves = Board.pieceCanMove(piece);
        }

        public void PromotePiece(PromotionSelection piece)
        {
            if (piece == PromotionSelection.Queen)
            {
                Board.move(_move, "Q");
            }
            if (piece == PromotionSelection.Rook)
            {
                Board.move(_move, "R");
            }
            if (piece == PromotionSelection.Knight)
            {
                Board.move(_move, "N");
            }
            if (piece == PromotionSelection.Bishop)
            {
                Board.move(_move, "B");
            }
            PromotionModalWhiteVisible = false;
            PromotionModalBlackVisible = false;
            ValidMoves = new bool[8, 8];
            lastMFRow = 8 - (_move[1] - '0');
            lastMFCol = _move[0] - 'a';
            lastMTRow = 8 - (_move[3] - '0'); ;
            lastMTCol = _move[2] - 'a';
            StateHasChanged();
            parsed = new char[4];
            _move = string.Empty;
        }

        public void HandleRightClick(int row, int col)
        {
            Board.getBoard()[row, col].RightClicked = !Board.getBoard()[row, col].RightClicked;
        }

        public string GetLastMoveFrom(int row, int col)
        {
            if (row == lastMFRow && col == lastMFCol)
                return "last-move-from";
            return string.Empty;
        }
        public string GetLastMoveTo(int row, int col)
        {
            if (row == lastMTRow && col == lastMTCol)
                return "last-move-to";
            return string.Empty;
        }
    }
}
