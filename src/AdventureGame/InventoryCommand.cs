namespace AdventureGame;

public class InventoryCommand : ICommand
{
	public string Verb => ICommand.INVENTORY;

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
		Program.ShowInventory(player);
	}

	public string DescribeInterruptedAttempt()
	{
		return "You rummage through your belongings looking for something, but something else finds you first.";
	}
}