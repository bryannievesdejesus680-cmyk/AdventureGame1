using static Program;

namespace AdventureGame.Tests;

[Collection("Console I/O")]
public class CommandDispatchTests
{
	[Fact]
	public void GoCommand_ExecutesThroughInterface()
	{
		Room entrance = new Room("Entrance", "A lit entrance.");
		Room gallery = new Room("Gallery", "A lit gallery.");
		entrance.Connect(EAST, gallery);
		Player player = new Player(entrance);

		ICommand command = new GoCommand();
		Assert.Equal(GO, command.Verb);
		Assert.True(command.HasValidArguments(new[] { GO, EAST }, out _));

		command.Execute(player, EAST);
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Equal(WEST, player.ReturnDirection);
	}

	[Fact]
	public void InterfaceDispatch_UsesDifferentDomainOperations()
	{
		Room entrance = new Room("Entrance", "A lit entrance.");
		Room gallery = new Room("Gallery", "A lit gallery.");
		entrance.Connect(EAST, gallery);
		Item coin = new Item("Old Coin", "A worn coin.");
		gallery.PlaceItem(coin);
		Player player = new Player(entrance);

		ICommand go = new GoCommand();
		ICommand take = new TakeCommand();

		Assert.Equal(GO, go.Verb);
		Assert.Equal(TAKE, take.Verb);
		Assert.True(go.HasValidArguments(new[] { GO, EAST }, out _));
		Assert.True(take.HasValidArguments(new[] { TAKE, "OLD", "COIN" }, out _));
		Assert.False(go.HasValidArguments(new[] { GO }, out string error));
		Assert.Equal("Use GO followed by one direction.", error);

		go.Execute(player, EAST);
		Assert.Same(gallery, player.CurrentRoom);
		take.Execute(player, "OLD COIN");
		Assert.Same(coin, Assert.Single(player.Inventory));
		Assert.Empty(gallery.Items);
	}
}