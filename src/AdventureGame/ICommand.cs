namespace AdventureGame;

public interface ICommand
{
	public const string QUIT = "QUIT";
	public const string LOOK = "LOOK";
	public const string GO = "GO";
	public const string TAKE = "TAKE";
	public const string DROP = "DROP";
	public const string INVENTORY = "INVENTORY";

	string Verb { get; }
	bool HasValidArguments(string[] words, out string message);
	void Execute(Player player, string target);
	string DescribeInterruptedAttempt();
}