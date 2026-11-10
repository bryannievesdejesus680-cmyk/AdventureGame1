namespace AdventureGame;

public class TakeCommand : ICommand
{
	public string Verb => ICommand.TAKE;

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
		player.TryTake(target, out string message);
		Console.WriteLine(message);
	}

	public string DescribeInterruptedAttempt()
	{
		return "You reach into the blackness. Something reaches back.";
	}
}