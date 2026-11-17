namespace AdventureGame;

public class Lamp : Item
{
	public bool IsOn { get; private set; }

	public Lamp(string name, string description) : base(name, description)
	{

	}

	public string Toggle(Room room)
	{
		IsOn = !IsOn;

		if (IsOn) { return "The Lamp is now on."; }

		return room.IsLit
			? "The Lamp is now off."
			: "The Lamp is now off." + System.Environment.NewLine + "The room falls dark.";
	}
}