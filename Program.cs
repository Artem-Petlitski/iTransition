using System.Text;
using ConsoleTables;

using System.Security.Cryptography;


namespace ConsoleApplication9
{
    class Table
    {
        string[] Names;
        public Table(string[] names)
        {
            Names = names;
        }

        public void Print()
        {
            var headerItems = Names.Prepend("PC \\ User");
            var table = new ConsoleTable(headerItems.ToArray());

            var judge = new Judge(Names.Length);

            for (int i = 0; i < Names.Length; i++)
            {
                var currentRow = new string[Names.Length + 1];
                currentRow[0] = Names[i];
            
                for (int j = 0; j < Names.Length; j++)
                {
                    currentRow[j + 1] = Enum.GetName(typeof(Outcome), judge.Decide(i, j));
                }

                table.AddRow(currentRow.ToArray());
            }

            table.Write(Format.Alternative);
        }
    }
    enum Outcome
    {
        WIN,
        LOSE,
        DRAW
    }
    class Key
    {
        public string GenerateKey()
        {
            var rand = RandomNumberGenerator.Create();
            byte[] bytes = new byte[32];
            rand.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "");
        }


       public string GenerateHMAC(string key, string message)
        {
            var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(message));
            return BitConverter.ToString(hash).Replace("-", "");
        }

    }
    
    class Judge
    {
        
        int MovesCount;

        public Judge(int movesCount)
        {
            MovesCount = movesCount;
        }

        public Outcome Decide(int firstMove, int secondMove)
        {
            if (firstMove == secondMove)
            {
                return Outcome.DRAW;
            }

            if ((secondMove > firstMove && secondMove - firstMove <= MovesCount / 2) || (secondMove < firstMove && firstMove - secondMove > MovesCount / 2))
            {
                return Outcome.WIN;
            }

            return Outcome.LOSE;
        }
    }
  

    
        class Program
    {
        static bool CheckArgs(string[] args)
        {
            if (args.Length < 3 || args.Length % 2 == 0)
            {
                Console.WriteLine("Invalid options: please pass odd number of moves (3 or more).");
                return false;
            }

            if (args.Length != args.Distinct().Count())
            {
                Console.WriteLine("Invalid options: all moves must be distinct.");
                return false;
            }

            return true;
        }

        static void Main(string[] args)
        {
            if (!CheckArgs(args))
            {
                return;
            }

            var sec = new Key();
            var a = new Table(args);
            var judge = new Judge(args.Length);

            bool gameFinished = false;

            while (!gameFinished)
            {
                var key = sec.GenerateKey();
                var computerMove = RandomNumberGenerator.GetInt32(args.Length);
                var hmac = sec.GenerateHMAC(key, args[computerMove]);

                Console.WriteLine("HMAC: " + hmac);

                Console.WriteLine("Available Moves:");
                for (int i = 0; i < args.Length; i++)
                {
                    Console.WriteLine(i + 1 + " - " + args[i]);
                }
                Console.WriteLine("0 - Exit");
                Console.WriteLine("? - Help");

                Console.Write("Enter your move: ");
                var ans = Console.ReadLine();

                if (ans == "?")
                {
                    a.Print();
                    Console.Write("\n\n\n");
                    continue;
                }

                if (ans == "0")
                {
                    gameFinished = true;
                    continue;
                }

                int playerMove = 0;

                if (!int.TryParse(ans, out playerMove) || playerMove <= 0 || playerMove > args.Length)
                {
                    Console.Write("\n\n\n");
                    continue;
                }

                Console.WriteLine("Your move: " + args[playerMove - 1]);
                Console.WriteLine("Computer move: " + args[computerMove]);

                switch (judge.Decide(computerMove, playerMove - 1))
                {
                    case Outcome.WIN:
                        Console.WriteLine("You won!");
                        break;

                    case Outcome.LOSE:
                        Console.WriteLine("You lost!");
                        break;

                    default:
                        Console.WriteLine("Draw!");
                        break;
                }

                Console.WriteLine("HMAC key: " + key);
                Console.Write("\n\n\n");
            }
        }
    }
  
}
