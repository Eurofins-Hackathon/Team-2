public class Program
{
    static void Main(string[] args)
    {
        var game = new BattleshipGame();
        game.Start();
    }
}

internal class BattleshipGame
{
    private const int BoardSize = 7;
    private const char EmptyCell = '-';
    private const char ShipCell = 'S';
    private const char HitCell = 'X';
    private const char MissCell = 'O';

    private readonly char[,] playerBoard = new char[BoardSize, BoardSize];
    private readonly char[,] computerBoard = new char[BoardSize, BoardSize];
    private readonly Random random = new();

    public void Start()
    {
        InitializeBoards();
        PlaceShips(playerBoard, "Player");
        PlaceShips(computerBoard, "Computer");
        PlayGame();
    }

    private void InitializeBoards()
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                playerBoard[i, j] = EmptyCell;
                computerBoard[i, j] = EmptyCell;
            }
        }
    }

    private void PlaceShips(char[,] board, string owner)
    {
        int shipsToPlace = 2;
        Console.WriteLine($"{owner}, place your ships on the board.");

        while (shipsToPlace > 0)
        {
            if (owner == "Player")
            {
                Console.WriteLine("Enter row and column (0-4) to place a ship (e.g., 1 2):");
                if (!TryParseInput(Console.ReadLine(), out int row, out int col) || !IsValidPlacement(board, row, col))
                {
                    Console.WriteLine("Invalid input or position already occupied. Please try again.");
                    continue;
                }
                board[row, col] = ShipCell;
            }
            else
            {
                int row, col;
                do
                {
                    row = random.Next(BoardSize);
                    col = random.Next(BoardSize);
                } while (!IsValidPlacement(board, row, col));
                board[row, col] = ShipCell;
            }
            shipsToPlace--;
        }
    }

    private void PlayGame()
    {
        while (true)
        {
            if (PlayerTurn()) break;
            if (ComputerTurn()) break;
        }
    }

    private bool PlayerTurn()
    {
        Console.WriteLine("Player's turn:");
        DisplayBoard(computerBoard, hideShips: true);
        Console.WriteLine("Enter row and column to attack (e.g., 1 2):");

        if (!TryParseInput(Console.ReadLine(), out int row, out int col) || !IsValidAttack(computerBoard, row, col))
        {
            Console.WriteLine("Invalid input or position already attacked. Please try again.");
            return false;
        }

        ProcessAttack(computerBoard, row, col);
        if (CheckVictory(computerBoard))
        {
            Console.WriteLine("Player wins!");
            return true;
        }
        return false;
    }

    private bool ComputerTurn()
    {
        Console.WriteLine("Computer's turn:");
        int row, col;
        do
        {
            row = random.Next(BoardSize);
            col = random.Next(BoardSize);
        } while (!IsValidAttack(playerBoard, row, col));

        ProcessAttack(playerBoard, row, col, isComputer: true);
        if (CheckVictory(playerBoard))
        {
            Console.WriteLine("Computer wins!");
            return true;
        }
        return false;
    }

    private static void ProcessAttack(char[,] board, int row, int col, bool isComputer = false)
    {
        if (board[row, col] == ShipCell)
        {
            Console.WriteLine(isComputer ? $"Computer hits at ({row}, {col})!" : "Hit!");
            board[row, col] = HitCell;
        }
        else
        {
            Console.WriteLine(isComputer ? $"Computer misses at ({row}, {col})." : "Miss.");
            board[row, col] = MissCell;
        }
    }

    private static void DisplayBoard(char[,] board, bool hideShips)
    {
        for (int i = 0; i < BoardSize; i++)
        {
            for (int j = 0; j < BoardSize; j++)
            {
                Console.Write(hideShips && board[i, j] == ShipCell ? $"{EmptyCell} " : $"{board[i, j]} ");
            }
            Console.WriteLine();
        }
    }

    private static bool CheckVictory(char[,] board)
    {
        foreach (var cell in board)
        {
            if (cell == ShipCell) return false;
        }
        return true;
    }

    private static bool TryParseInput(string? input, out int row, out int col)
    {
        row = col = -1;
        if (string.IsNullOrEmpty(input)) return false;
        var parts = input.Split(' ');
        return parts.Length == 2 && int.TryParse(parts[0], out row) && int.TryParse(parts[1], out col) && row >= 0 && row < BoardSize && col >= 0 && col < BoardSize;
    }

    private static bool IsValidPlacement(char[,] board, int row, int col) => board[row, col] == EmptyCell;

    private static bool IsValidAttack(char[,] board, int row, int col) => board[row, col] != HitCell && board[row, col] != MissCell;
}
