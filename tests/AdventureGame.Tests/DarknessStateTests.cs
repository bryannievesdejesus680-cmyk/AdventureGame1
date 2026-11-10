using static Program;

namespace AdventureGame.Tests;

public class DarknessStateTests
{
	[Fact]
	public void RoomLighting_PreservesDefaultAndAllowsDarkArchive()
	{
		Room gallery = new Room("Gallery", "A long gallery.");
		Room archive = new Room("Archive", "A dusty archive.", false);
		Item token = new Item("Brass Token", "A square token.");
		archive.PlaceItem(token);

		Assert.True(gallery.IsLit);
		Assert.False(archive.IsLit);
		Assert.Same(token, Assert.Single(archive.Items));
	}

	[Fact]
	public void Movement_RecordsReturnAndRejectsChangesAfterFailure()
	{
		Room entrance = new Room("Entrance", "A lit entrance.");
		Room gallery = new Room("Gallery", "A lit gallery.");
		Room archive = new Room("Archive", "A dark archive.", false);
		entrance.Connect(EAST, gallery);
		gallery.Connect(NORTH, archive);
		Player player = new Player(entrance);

		Assert.Null(player.ReturnDirection);
		Assert.True(player.TryMove(EAST, out _));
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Equal(WEST, player.ReturnDirection);
		Assert.True(player.TryMove(NORTH, out _));
		Assert.Same(archive, player.CurrentRoom);
		Assert.Equal(SOUTH, player.ReturnDirection);

		Assert.False(player.TryMove(WEST, out _));
		Assert.False(player.TryMove("UP", out _));
		Assert.Same(archive, player.CurrentRoom);
		Assert.Equal(SOUTH, player.ReturnDirection);

		Assert.True(player.TryMove(SOUTH, out _));
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Equal(NORTH, player.ReturnDirection);
	}
}