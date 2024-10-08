﻿
namespace BoardGameProject
{
    /// <summary>
    /// gomoku checker class
    /// </summary>
    internal class GomokuChecker : IChecker<GomokuBoard>
    {
        /// <summary>
        /// check draw
        /// </summary>
        /// <param name="board"></param>
        /// <returns></returns>
        public bool IsDraw(GomokuBoard board)
        {
            for (int i = 0; i < board.Size; i++)
            {
                for (int j = 0; j < board.Size; j++)
                {
                    if (board.Cells[i][j] == 0) return false;
                }
            }
            return true;
        }

        /// <summary>
        /// check valid
        /// </summary>
        /// <param name="board"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public bool IsValidPlace(GomokuBoard board, int row, int col)
        {
            if ((row == 996 && col == 996) || // Redo
                (row == 997 && col == 997) || // Undo
                (row == 998 && col == 998) || // Load
                (row == 999 && col == 999))   // Save
            {
                return true;  
            }

            if (row >= 0 && row < board.Size && col >= 0 && col < board.Size && board.Cells[row][col] == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

      
        /// <summary>
        /// check win
        /// </summary>
        /// <param name="board"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool IsWin(GomokuBoard board, int row, int col, int player)
        {
            if (CheckDirection(board, row, col, player, 1, 0) || CheckDirection(board, row, col, player, 0, 1) ||
               CheckDirection(board, row, col, player, 1, 1) || CheckDirection(board, row, col, player, 1, -1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// check direction
        /// </summary>
        /// <param name="board"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="player"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private bool CheckDirection(GomokuBoard board, int row, int col, int player, int v1, int v2)
        {
            int count = 1;
            count += CountDirection(board, row, col, player, v1, v2);
            count += CountDirection(board, row, col, player, -v1, -v2);
            return count >= 5;
        }

        /// <summary>
        /// count direction
        /// </summary>
        /// <param name="board"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="player"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        private int CountDirection(GomokuBoard board, int row, int col, int player, int v1, int v2)
        {
            int count = 0;
            row += v1;
            col += v2;
            while (row >= 0 && row < board.Size && col >= 0 && col < board.Size && board.Cells[row][col] == player)
            {
                count++;
                row += v1;
                col += v2;
            }
            return count;
        }

        public bool IsValidPlace(List<GomokuBoard> boardList, int boardIndex, int row, int col)
        {
            throw new NotImplementedException();
        }

        public bool IsWin(List<GomokuBoard> boardList)
        {
            throw new NotImplementedException();
        }

    }
}
