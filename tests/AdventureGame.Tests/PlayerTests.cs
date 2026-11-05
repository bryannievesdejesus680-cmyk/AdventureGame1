using static Program;

namespace AdventureGame.Tests;

public class PlayerTests
{
	[Fact]
	public void Constructor_StartsInOriginalRoom()
	{
		var (entrance, _, _) = RoomTests.CreateDungeon();
		Player player = new Player(entrance);

		Assert.Same(entrance, player.CurrentRoom);
	}

	[Fact]
	public void TryMove_UsesExistingGalleryInstance()
	{
		var (entrance, gallery, _) = RoomTests.CreateDungeon();
		Player player = new Player(entrance);

		bool moved = player.TryMove(EAST, out string message);

		Assert.True(moved);
		Assert.Same(gallery, player.CurrentRoom);
		Assert.Contains("You move EAST to Gallery.", message);
	}

	[Fact]
	public void TryMove_MissingExitPreservesLocation()
	{
		var (entrance, _, _) = RoomTests.CreateDungeon();
		Player player = new Player(entrance);

		bool moved = player.TryMove(NORTH, out string message);

		Assert.False(moved);
		Assert.Same(entrance, player.CurrentRoom);
		Assert.Contains("There is no exit NORTH from Entrance.", message);
	}

	[Fact]
	public void TryMove_UnsupportedDirectionPreservesLocation()
	{
		var (entrance, _, _) = RoomTests.CreateDungeon();
		Player player = new Player(entrance);

		bool moved = player.TryMove("UP", out string message);

		Assert.False(moved);
		Assert.Same(entrance, player.CurrentRoom);
		Assert.Contains("Unknown direction UP", message);
	}

	[Fact]
	public void TryMove_RoundTripPreservesOriginalRoomIdentity()
	{
		var (entrance, gallery, archive) = RoomTests.CreateDungeon();
		Player player = new Player(entrance);

		Assert.True(player.TryMove(EAST, out _));
		Assert.Same(gallery, player.CurrentRoom);
		Assert.True(player.TryMove(NORTH, out _));
		Assert.Same(archive, player.CurrentRoom);
		Assert.True(player.TryMove(SOUTH, out _));
		Assert.Same(gallery, player.CurrentRoom);
		Assert.True(player.TryMove(WEST, out _));
		Assert.Same(entrance, player.CurrentRoom);
	}

	[Fact]
	public void ReadingCurrentRoom_DoesNotChangeLocation()
	{
		var (entrance, gallery, archive) = RoomTests.CreateDungeon();
		Player player = new Player(entrance);
		Assert.True(player.TryMove(EAST, out _));

		Room inspected = player.CurrentRoom;
		string description = inspected.Description;
		Room? north = inspected.North;

		Assert.Equal("A long gallery lined with faded banners.", description);
		Assert.Same(archive, north);
		Assert.Same(gallery, player.CurrentRoom);
	}
}
