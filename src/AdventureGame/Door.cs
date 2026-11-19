using System;

namespace AdventureGame;

public class Door
{
	private readonly Item? _requiredKey;

	public string Name { get; }
	public bool IsLocked { get; private set; }
	public bool IsOpen { get; private set; }

	public Door(string name, Item? requiredKey = null)
	{
		Name = name;
		_requiredKey = requiredKey;
		IsLocked = requiredKey is not null;
	}

	public bool TryOpen(Player player, out string message)
	{
		if (IsOpen)
		{
			message = $"The {Name} is already open.";
			return false;
		}

		if (IsLocked)
		{
			bool carriesKey = false;

			foreach (Item item in player.Inventory)
			{
				if (ReferenceEquals(item, _requiredKey))
				{
					carriesKey = true;
					break;
				}
			}

			if (!carriesKey)
			{
				message = $"The {Name} is locked. You need its key.";
				return false;
			}

			IsLocked = false;
			IsOpen = true;
			message = $"You unlock and open the {Name}.";
			return true;
		}

		IsOpen = true;
		message = $"You open the {Name}.";
		return true;
	}
}