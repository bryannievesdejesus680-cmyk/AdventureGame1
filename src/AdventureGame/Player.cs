using static Program;

namespace AdventureGame;

public class Player
{
	public Room CurrentRoom { get; private set; }

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
}