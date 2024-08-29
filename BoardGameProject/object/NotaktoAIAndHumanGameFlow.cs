﻿

using System.Text.Json;

namespace BoardGameProject
{
    public class NotaktoAIAndHumanGameFlow : GameFlowBase
    {
        private string gameType;
        private string gameMode;
        private UIBase ui;

        public string GameType
        {
            get { return gameType; }
            set { gameType = value; }
        }
        public string GameMode
        {

            get { return gameMode; }
            set { gameMode = value; }
        }
        public UIBase UI
        {
            get { return ui; }
            set { ui = value; }
        }

        public NotaktoAIAndHumanGameFlow(string aGameType, string aGameMode, UIBase aUi)
        {
            gameMode = aGameMode;
            gameType = aGameType;
            ui = aUi;
        }

        private PlayerBase player1;
        private PlayerBase player2;
        public NotaktoBoard notaktoBoard;
        private NotaktoChecker checker;
        private NotaktoSaver saver;
        private NotaktoActionManager am;
        private List<int[,]> boardHistory = new List<int[,]>();

        public override void End()
        {
            Console.WriteLine("Game Over... See you next time...");
        }

        private Random random = new Random();

        public override bool SelectPosition(ref int player, out bool isGameOver, ref int round)
        {
            isGameOver = false;
            bool isValid;
            (int, int) pos;
            notaktoBoard.CurrentPlayer = player;

            if (player == 1) // AI player
            {
                int boardIndex = random.Next(0, notaktoBoard.Count); // Randomly select a board index
                notaktoBoard.SwitchBoard(boardIndex); // Switch to the selected board

                pos = player1.GetPosition(notaktoBoard); // AI gets the position based on the selected board
            }
            else // Human player
            {
                int boardIndex = player2.GetBoardNum();
                notaktoBoard.SwitchBoard(boardIndex - 1); // Switch to the selected board (index is 0-based)

                pos = player2.GetPosition();
            }
            //save
            if (pos == (999, 999))
            {
                string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                //save to desktop
                saver.SaveBoardInfo(notaktoBoard, baseDir);
                //save then game over;
                isGameOver = true;
                return true;
            }
            //load
            else if (pos == (998, 998))
            {
                Console.WriteLine("Please enter the FULL PATH to load the game:");

                try
                {
                    while (true)
                    {
                        string savePath = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(savePath) || !File.Exists(savePath))
                        {
                            Console.WriteLine("Error: file does not exist!");
                            continue;
                        }

                        string jsonStr = File.ReadAllText(savePath);
                        if (gameType.Equals(GlobalVar.NOTAKTO))
                        {
                            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                            NotaktoBoard board = JsonSerializer.Deserialize<NotaktoBoard>(jsonStr, options);

                            if (board.ValidationStr.Equals(GlobalVar.NOTAKTO))
                            {
                                notaktoBoard = board;
                                Console.WriteLine("\nLoading Successfully!");
                                notaktoBoard.PrintBoard(round);
                                return false;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            //undo
            else if (pos == (997, 997))
            {
                while (true)
                {
                    Console.WriteLine("Which round do you want to undo to?");
                    int inputRound;
                    if (int.TryParse(Console.ReadLine(), out inputRound))
                    {
                        if (inputRound > 0 && inputRound < round)
                        {
                            if (am.Undo(boardHistory, inputRound, notaktoBoard))
                            {
                                int temp = round;
                                round = inputRound;
                                //redo
                                Console.WriteLine("Confirm undo: enter undo to confirm; enter redo to cancel.");
                                (int, int) confirm = player2.GetPosition();
                                if (confirm == (996, 996))
                                {
                                    am.Redo(notaktoBoard);
                                    round = temp;
                                }
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Error: enter a number between 1 and {0}", (round - 1));
                            }
                        }
                        else
                        {
                            Console.WriteLine(GlobalVar.USERINPUTSINVALIDMSG);
                        }
                    }
                }
                return true;
            }

            isValid = checker.IsValidPlace(notaktoBoard, pos.Item1 - 1, pos.Item2 - 1);

            //check validity then place chess
            if (isValid)//input valid
            {
                if (notaktoBoard.PlaceChess(pos.Item1 - 1, pos.Item2 - 1, player))
                {
                    //am.RecordMove(new Move(pos.Item1 - 1, pos.Item2 - 1, player));
                    SaveBoardHistory();
                    Console.WriteLine($"Player{player} places at {pos.Item1}, {pos.Item2}");
                }
                else
                {
                    Console.WriteLine("Failed to place move");

                }
                notaktoBoard.PrintBoard(round);
                if (checker.IsDraw(notaktoBoard))
                {

                    ui.DisplayInfo("Game End: Draw!");
                    isGameOver = true;

                }
                else if (checker.IsWin(notaktoBoard, pos.Item1 - 1, pos.Item2 - 1, player))
                {
                    // The winning player is the opponent
                    int winningPlayer = player == 1 ? 2 : 1;
                    // In Notakto, forming a three-in-a-row means losing
                    Console.WriteLine("Game End: Player{0} Won!", winningPlayer);
                    isGameOver = true;

                }
                return true;
            }
            else //invalid
            {
                Console.WriteLine(GlobalVar.USERINPUTSINVALIDMSG);

                return false;
            }
        }

        public override void SetUp()
        {
            notaktoBoard = new NotaktoBoard(); //Use parameterless constructor
            notaktoBoard.PrintBoard(1);
            checker = new NotaktoChecker();
            player1 = PlayerFactory.CreatePlayer(GlobalVar.COMPUTER);
            player2 = PlayerFactory.CreatePlayer(GlobalVar.HUMAN);
            Console.WriteLine("\nPlayer1: Computer");
            Console.WriteLine("Player2: Human");
            notaktoBoard.GameMode = gameMode;
            saver = new NotaktoSaver();
            am = new NotaktoActionManager();
        }

        public void SaveBoardHistory()
        {
            int[,] currentBoard = new int[notaktoBoard.Size, notaktoBoard.Size];
            for (int i = 0; i < notaktoBoard.Size; i++)
            {
                for (int j = 0; j < notaktoBoard.Size; j++)
                {
                    currentBoard[i, j] = notaktoBoard.Boards[notaktoBoard.CurrentBoardIndex][i][j];
                }
            }
            boardHistory.Add(currentBoard);
        }
    }
}
