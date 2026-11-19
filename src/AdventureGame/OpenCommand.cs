using System;

namespace AdventureGame;

public class OpenCommand : ICommand
{
	public string Verb => Program.OPEN;

	public bool HasValidArguments(string[] words, out string message)
	{
		if (words.Length >= 2)
		{
			message = string.Empty;
			return true;
		}

		message = $"Use {Verb} followed by an object name.";
		return false;
	}

	public void Execute(Player player, string target)
	{
		Door? door = player.CurrentRoom.FindDoor(target);

		if (door is null)
		{
			Console.WriteLine($"There is no {target} to open in {player.CurrentRoom.Name}.");
			return;
		}

		door.TryOpen(player, out string message);
		Console.WriteLine(message);
	}

	public string DescribeInterruptedAttempt()
	{
		return "You reach for something in the darkness. Something reaches you first.";
	}
}