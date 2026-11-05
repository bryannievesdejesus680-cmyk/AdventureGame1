using static Program;

namespace AdventureGame;

public class Player
{
	public Room CurrentRoom { get; private set; }
	private readonly List<Item> _inventory = new List<Item>();

	public IReadOnlyList<Item> Inventory => _inventory.AsReadOnly();

	public Player(Room startingRoom)
	{
		CurrentRoom = startingRoom ?? throw new ArgumentNullException(nameof(startingRoom));
	}

	public bool TryMove(string direction, out string message)
	{
		if (direction != NORTH && direction != SOUTH && direction != EAST && direction != WEST)
		{
			message = $"Unknown direction {direction}. Choose {NORTH}, {SOUTH}, {EAST}, or {WEST}.";
			return false;
		}

		Room? destination = CurrentRoom.GetNeighbor(direction);

		if (destination is null)
		{
			message = $"There is no exit {direction} from {CurrentRoom.Name}.";
			return false;
		}

		CurrentRoom = destination;
		message = $"You move {direction} to {CurrentRoom.Name}.";
		return true;
	}
	public bool TryTake(string itemName, out string message)
	{
		Item? item = CurrentRoom.FindItem(itemName);

		if (item is null)
		{
			message = $"There is no {itemName} in {CurrentRoom.Name}.";
			return false;
		}

		CurrentRoom.RemoveItem(item);
		_inventory.Add(item);
		message = $"You take the {item.Name}.";
		return true;
	}
	public bool TryDrop(string itemName, out string message)
	{
		int matchIndex = _inventory.FindIndex(item =>
			string.Equals(item.Name, itemName, StringComparison.OrdinalIgnoreCase));

		if (matchIndex < 0)
		{
			message = $"You are not carrying {itemName}.";
			return false;
		}

		Item item = _inventory[matchIndex];

		CurrentRoom.PlaceItem(item);
		_inventory.RemoveAt(matchIndex);

		message = $"You drop the {item.Name}.";
		return true;
	}
}
