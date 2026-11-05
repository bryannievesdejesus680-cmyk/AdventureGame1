using static Program;

namespace AdventureGame.Tests;

public class RoomTests
{
	public static (Room Entrance, Room Gallery, Room Archive) CreateDungeon()
	{
		Room entrance = new Room("Entrance", "A dry stone chamber with daylight spilling through the doorway.");
		Room gallery = new Room("Gallery", "A long gallery lined with faded banners.");
		Room archive = new Room("Archive", "An archive whose shelves hold dust-covered chronicles.");

		entrance.East = gallery;
		gallery.West = entrance;
		gallery.North = archive;
		archive.South = gallery;

		return (entrance, gallery, archive);
	}

	[Fact]
	public void CreateDungeon_CreatesThreeDistinctRooms()
	{
		var (entrance, gallery, archive) = CreateDungeon();

		Assert.NotSame(entrance, gallery);
		Assert.NotSame(entrance, archive);
		Assert.NotSame(gallery, archive);
	}

	[Fact]
	public void CreateDungeon_ConnectsEntranceAndGalleryBothWays()
	{
		var (entrance, gallery, _) = CreateDungeon();

		Assert.Same(gallery, entrance.East);
		Assert.Same(entrance, gallery.West);
	}

	[Fact]
	public void CreateDungeon_ConnectsGalleryAndArchiveBothWays()
	{
		var (_, gallery, archive) = CreateDungeon();

		Assert.Same(archive, gallery.North);
		Assert.Same(gallery, archive.South);
	}

	[Fact]
	public void CreateDungeon_LeavesMissingGalleryExitsNull()
	{
		var (_, gallery, _) = CreateDungeon();

		Assert.Null(gallery.East);
		Assert.Null(gallery.South);
	}

	[Fact]
	public void PreviewExit_DoesNotChangeGalleryNorthConnection()
	{
		var (_, gallery, archive) = CreateDungeon();
		Room? originalNorth = gallery.North;

		Program.PreviewExit(gallery, NORTH);

		Assert.Same(archive, originalNorth);
		Assert.Same(originalNorth, gallery.North);
	}
}