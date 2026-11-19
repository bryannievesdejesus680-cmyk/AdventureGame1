using System.Collections.Generic;
using static Program;
namespace AdventureGame;

public class Room
{
	public string Name { get; }
	public string Description { get; set; }
	private readonly List<Item> _items = new List<Item>();
	private readonly Dictionary<string, Door> _doors = new Dictionary<string, Door>();
	public bool IsLit { get; }

	public IReadOnlyList<Item> Items => _items.AsReadOnly();
	public Room? North { get; private set; }
	public Room? South { get; private set; }
	public Room? East { get; private set; }
	public Room? West { get; private set; }

	public Room(string name, string description) : this(name, description, true)
	{

	}

	public Room(string name, string description, bool isLit)
	{
		Name = name;
		Description = description;
		IsLit = isLit;
	}

	public Room? GetNeighbor(string direction)
	{
		return direction switch
		{
			NORTH => North,
			SOUTH => South,
			EAST => East,
			WEST => West,
			_ => null
		};
	}

	public static string GetReciprocalDirection(string direction)
	{
		return direction switch
		{
			NORTH => SOUTH,
			SOUTH => NORTH,
			EAST => WEST,
			WEST => EAST,
			_ => throw new ArgumentException(
					$"Use {NORTH}, {SOUTH}, {EAST}, or {WEST}.",
					nameof(direction))
		};
	}

	public void Connect(string direction, Room neighbor)
	{
		ArgumentNullException.ThrowIfNull(neighbor);

		string opposite = direction switch
		{
			NORTH => SOUTH,
			SOUTH => NORTH,
			EAST => WEST,
			WEST => EAST,
			_ => throw new ArgumentException($"Use {NORTH}, {SOUTH}, {EAST}, or {WEST}.", nameof(direction))
		};

		if (ReferenceEquals(this, neighbor))
		{
			throw new InvalidOperationException("A room cannot be its own neighbor.");
		}

		Room? there = GetNeighbor(direction);
		Room? back = neighbor.GetNeighbor(opposite);

		if (there is not null || back is not null)
		{
			throw new InvalidOperationException("Either this room direction or the neighbor reciprocal direction is already connected.");
		}

		SetNeighbor(direction, neighbor);
		neighbor.SetNeighbor(opposite, this);
	}

	public void PlaceItem(Item item)
	{
		ArgumentNullException.ThrowIfNull(item);

		bool found = _items.Exists(current => ReferenceEquals(current, item));

		if (found)
		{
			throw new InvalidOperationException("This room already contains the item.");
		}

		_items.Add(item);
	}

	public void RemoveItem(Item item)
	{
		ArgumentNullException.ThrowIfNull(item);

		int itemIndex = _items.FindIndex(current => ReferenceEquals(current, item));

		if (itemIndex < 0)
		{
			throw new InvalidOperationException("This room does not contain the item.");
		}

		_items.RemoveAt(itemIndex);
	}

	public Item? FindItem(string itemName)
	{
		return _items.Find(item =>
				string.Equals(item.Name, itemName, StringComparison.OrdinalIgnoreCase));
	}

	private void SetNeighbor(string direction, Room neighbor)
	{
		switch (direction)
		{
			case NORTH: North = neighbor; break;
			case SOUTH: South = neighbor; break;
			case EAST: East = neighbor; break;
			case WEST: West = neighbor; break;
			default: throw new ArgumentException($"Use {NORTH}, {SOUTH}, {EAST}, or {WEST}.", nameof(direction));
		}
	}
	public Door? GetDoor(string direction)
	{
		return _doors.TryGetValue(direction, out Door? door) ? door : null;
	}

	public Door? FindDoor(string doorName)
	{
		foreach (Door door in _doors.Values)
		{
			if (string.Equals(door.Name, doorName, StringComparison.OrdinalIgnoreCase))
			{
				return door;
			}
		}

		return null;
	}

	public void AttachDoor(string direction, Door door)
	{
		ArgumentNullException.ThrowIfNull(door);

		string opposite = direction switch
		{
			NORTH => SOUTH,
			SOUTH => NORTH,
			EAST => WEST,
			WEST => EAST,
			_ => throw new ArgumentException("Use a cardinal direction.", nameof(direction))
		};

		Room? neighbor = GetNeighbor(direction);

		if (neighbor is null || !ReferenceEquals(neighbor.GetNeighbor(opposite), this))
		{
			throw new InvalidOperationException("Attach a door to an existing reciprocal passage.");
		}

		if (GetDoor(direction) is not null || neighbor.GetDoor(opposite) is not null)
		{
			throw new InvalidOperationException("This passage already has a door.");
		}

		_doors.Add(direction, door);
		neighbor._doors.Add(opposite, door);
	}
}
