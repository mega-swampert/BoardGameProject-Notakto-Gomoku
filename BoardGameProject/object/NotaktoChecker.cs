﻿
namespace BoardGameProject
{
    public class NotaktoChecker : IChecker<NotaktoBoard>
    {
        public bool IsDraw(NotaktoBoard board)
        {
            // Not applicable for Notakto.
            return false;
        }

        public bool IsValidPlace(NotaktoBoard board, int row, int col)
        {
            if (board.IsBoardLocked(board.CurrentBoardIndex))
            {
                return false; // 如果棋盘已被锁定，则该棋盘不再允许放置棋子。
            }

            return row >= 0 && row < board.Size && col >= 0 && col < board.Size && board.Boards[board.CurrentBoardIndex][row][col] == 0;
        }

        public bool IsWin(NotaktoBoard board, int row, int col, int player)
        {
            bool isWin = CheckDirection(board, row, col, player, 1, 0) ||  // 水平
                         CheckDirection(board, row, col, player, 0, 1) ||  // 垂直
                         CheckDirection(board, row, col, player, 1, 1) ||  // 主对角线
                         CheckDirection(board, row, col, player, 1, -1);   // 副对角线

            if (isWin)
            {
                board.LockBoard(board.CurrentBoardIndex); // 锁定当前棋盘。
            }

            return isWin;
        }

        private bool CheckDirection(NotaktoBoard board, int row, int col, int player, int v1, int v2)
        {
            int count = 1;
            count += CountDirection(board, row, col, player, v1, v2);
            count += CountDirection(board, row, col, player, -v1, -v2);
            return count >= 3;
        }
        private int CountDirection(NotaktoBoard board, int row, int col, int player, int v1, int v2)
        {
            int count = 0;
            row += v1;
            col += v2;
            while (row >= 0 && row < board.Size && col >= 0 && col < board.Size 
                && board.Boards[board.CurrentBoardIndex][row][col] == player)
            {
                count++;
                row += v1;
                col += v2;
            }
            return count;
        }
    }
}
