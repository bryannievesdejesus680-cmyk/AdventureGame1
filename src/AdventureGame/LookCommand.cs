namespace AdventureGame;

public class LookCommand : ICommand
{
	public string Verb => ICommand.LOOK;

	public bool HasValidArguments(string[] words, out string message)
	{
		if (words.Length == 1)
		{
			message = string.Empty;
			return true;
		}

		message = $"{Verb} takes no arguments.";
		return false;
	}

	public void Execute(Player player, string target)
	{
		Program.ShowRoom(player.CurrentRoom);
	}

	public string DescribeInterruptedAttempt()
	{
		return "A heavy movement in the darkness interrupts your thoughts.";
	}
}