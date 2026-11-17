namespace AdventureGame;

public class UseCommand : ICommand
{
	public string Verb => ICommand.USE;

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
		Item? selectedItem = player.FindItem(target);

		if (selectedItem is null)
		{
			Console.WriteLine($"You are not carrying {target}.");
			return;
		}

		if (selectedItem is Lamp lamp)
		{
			Console.WriteLine(lamp.Toggle(player.CurrentRoom));
			return;
		}

		Console.WriteLine($"You cannot use the {selectedItem.Name}.");
	}

	public string DescribeInterruptedAttempt()
	{
		return "A heavy movement in the darkness interrupts your thoughts.";
	}
}