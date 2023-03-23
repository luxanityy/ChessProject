
using Chess.Common;
using Microsoft.AspNetCore.Components;
using System.Diagnostics;

namespace Chess.Controls
{
    // TODO: show last move
    // TODO: Implement UI for timers
    // TODO: Implement menu to choose time control
    public partial class BoardControlls : ComponentBase
    {
        public bool PromotionModalWhiteVisible { get; set; } = false;
        public Chessboard Board { get; set; }
        public bool PromotionModalBlackVisible { get; set; } = false;

        public bool[,] ValidMoves = new bool[8, 8];

        public bool GameOver { get; set; } = false;
        public BoardControlls()
        {
            NewGame();
        }
        private static void Test(object sender, EventArgs e)
        {

        }
        public void NewGame()
        {
            Board = new(600, 5);
            GameOver = false;
        }

        public bool IsDarkTile(int row, int col)
        {
            return (row + col) % 2 == 1;
        }
        
        public string GetTileClass(int row, int col)
        {

            string result = string.Empty;
            if (Board.getBoard()[row, col].RightClicked)
            {
                string tileRightClickClass = IsDarkTile(row, col) ? "tile-rightC-black " : "tile-rightC-white ";
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

            return result;
        }

        private string _move { get; set; } = string.Empty;
        char[] parsed = new char[4];
        public async Task MovePieceAsync(int row, int col)
        {

            if (_move.Length == 4)
            { 
                _move = string.Empty;
                parsed = new char[4];
            }
            if (string.IsNullOrEmpty(_move))
            {
                parsed[0] = (char)(col + 'a');
                parsed[1] = (char)(8 - row + '0');
                _move = new string(parsed).Trim('\0');
                PromotionModalWhiteVisible = false;
                PromotionModalBlackVisible = false;

                ValidMoves = Board.pieceCanMove(Board.getBoard()[row, col].getPiece());
                StateHasChanged();
                // TODO: krugciki kad se klikne na drugu figuru njeni krugciki
            }
            else
            {
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

                if(!Board.move(_move, "Q"))
                {
                    Console.WriteLine($"Illegal move {_move}");
                }
                ValidMoves = new bool[8,8]; 
                
                // update frontend 
                StateHasChanged();
                parsed = new char[4]; 
                _move = string.Empty;

                if (Board.isGameOver())
                {
                    await Task.Delay(3000);
                    GameOver = true;
                    StateHasChanged();
                }
            }
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
            StateHasChanged();
            parsed = new char[4];
            _move = string.Empty;
        }

        public void HandleRightClick(int row, int col)
        {
            Board.getBoard()[row, col].RightClicked = !Board.getBoard()[row, col].RightClicked;
        }
    }
}
