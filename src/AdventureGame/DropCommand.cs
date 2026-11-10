namespace AdventureGame;

public class DropCommand : ICommand
{
	public string Verb => ICommand.DROP;

	public bool HasValidArguments(string[] words, out string message)
	{
		if (words.Length >= 2)
		{
			message = string.Empty;
			return true;
		}

		message = $"Use {Verb} followed by an item name.";
		return false;
	}

	public void Execute(Player player, string target)
	{
		player.TryDrop(target, out string message);
		Console.WriteLine(message);
	}

	public string DescribeInterruptedAttempt()
	{
		return "You drop one of your belongings. The sound it makes echoes and serves as a beacon for the thing that lurks in the darkness.";
	}
}