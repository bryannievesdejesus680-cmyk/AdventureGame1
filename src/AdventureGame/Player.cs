using static Program;
namespace AdventureGame;

public class Player
{
	public Room CurrentRoom { get; private set; }
	public string? ReturnDirection { get; private set; }
	private readonly List<Item> _inventory = new List<Item>();

	public IReadOnlyList<Item> Inventory => _inventory.AsReadOnly();

	public Player(Room startingRoom)
	{
		CurrentRoom = startingRoom ?? throw new ArgumentNullException(nameof(startingRoom));
	}
	public Item? FindItem(string itemName)
	{
		return _inventory.Find(item =>
		string.Equals(item.Name, itemName, StringComparison.OrdinalIgnoreCase));
	}

	public bool TryTake(string itemName, out string message)
	{
		Item? item = CurrentRoom.FindItem(itemName);

		if (item is null)
		{
			message = $"There is no {itemName} here.";
			return false;
		}

		CurrentRoom.RemoveItem(item);
		_inventory.Add(item);

		message = $"You take the {item.Name}.";
		return true;
	}



	public bool TryDrop(string itemName, out string message)
	{
		Item? item = FindItem(itemName);

		if (item is null)
		{
			message = $"You do not have {itemName}.";
			return false;
		}

		CurrentRoom.PlaceItem(item);
		_inventory.Remove(item);

		message = $"You drop the {item.Name}.";
		return true;
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

		Door? door = CurrentRoom.GetDoor(direction);

		if (door is not null && !door.IsOpen)
		{
			message = $"The {door.Name} is closed. You cannot pass.";
			return false;
		}

		ReturnDirection = direction switch
		{
			NORTH => SOUTH,
			SOUTH => NORTH,
			EAST => WEST,
			WEST => EAST,
			_ => throw new InvalidOperationException("Unsupported movement direction.")
		};

		CurrentRoom = destination;
		message = $"You move {direction} to {CurrentRoom.Name}.";
		return true;
	}
}