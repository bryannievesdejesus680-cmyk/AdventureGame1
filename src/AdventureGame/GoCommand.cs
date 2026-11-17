namespace AdventureGame;

public class GoCommand : ICommand
{
	public string Verb => ICommand.GO;

	public bool HasValidArguments(string[] words, out string message)
	{
		if (words.Length == 2)
		{
			message = string.Empty;
			return true;
		}

		message = $"Use {Verb} followed by one direction.";
		return false;
	}

	public void Execute(Player player, string target)
	{
		bool moved = player.TryMove(target, out string message);
		Console.WriteLine(message);

		if (moved)
		{
			Program.ShowRoom(player);
		}
		else
		{
			Console.WriteLine($"You remain in {player.CurrentRoom.Name}.");
		}
	}

	public string DescribeInterruptedAttempt()
	{
		return "You rush blindly ahead and stumble upon an angry woken-up beast.";
	}
}