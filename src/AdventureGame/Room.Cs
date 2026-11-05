using static Program;

namespace AdventureGame;

public class Room
{
	public string Name { get; }
	public string Description { get; set; }
	public Item? Item { get; private set; }

	public Room? North { get; private set; }
	public Room? South { get; private set; }
	public Room? East { get; private set; }
	public Room? West { get; private set; }

	public Room(string name, string description)
	{
		Name = name;
		Description = description;
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
		Item = item ?? throw new ArgumentNullException(nameof(item));
	}

	public void RemoveItem()
	{
		Item = null;
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
}
