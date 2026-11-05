
using static Program;

namespace AdventureGame.Tests;

public class RoomTests
{
   
    public static (Room Entrance, Room Gallery, Room Archive) CreateDungeon()
    {
        Room entrance = new Room("Entrance", "A dry stone chamber with daylight spilling through the doorway.");
        Room gallery = new Room("Gallery", "A long gallery lined with faded banners.");
        Room archive = new Room("Archive", "An archive whose shelves hold dust-covered chronicles.");

        entrance.Connect(EAST, gallery);
        gallery.Connect(NORTH, archive);

        return (entrance, gallery, archive);
    }

    [Fact]
    public void Connect_RejectsOccupiedForwardExitWithoutChangingLinks()
    {
        var (entrance, gallery, _) = RoomTests.CreateDungeon();
        Room workshop = new Room("Workshop", "A workroom.");

        Assert.Throws<InvalidOperationException>(() => entrance.Connect(EAST, workshop));
        Assert.Same(gallery, entrance.East);
        Assert.Same(entrance, gallery.West);
        Assert.Null(workshop.West);
    }

    [Fact]
    public void Connect_RejectsOccupiedReverseExitWithoutChangingLinks()
    {
        var (entrance, gallery, _) = RoomTests.CreateDungeon();
        Room workshop = new Room("Workshop", "A workroom.");

        Assert.Throws<InvalidOperationException>(() => workshop.Connect(EAST, gallery));
        Assert.Null(workshop.East);
        Assert.Same(entrance, gallery.West);
    }

    [Fact]
    public void Connect_RejectsSelfConnection()
    {
        Room room = new Room("Isolated", "No exits.");

        Assert.Throws<InvalidOperationException>(() => room.Connect(NORTH, room));
        Assert.Null(room.North);
        Assert.Null(room.South);
    }

    [Fact]
    public void Connect_RejectsUnknownDirection()
    {
        Room first = new Room("First", "First room.");
        Room second = new Room("Second", "Second room.");

        Assert.Throws<ArgumentException>(() => first.Connect("UP", second));
        Assert.Null(first.North);
        Assert.Null(second.South);
    }

    [Fact]
    public void Connect_RejectsNullNeighbor()
    {
        Room first = new Room("First", "First room.");

        Assert.Throws<ArgumentNullException>(() => first.Connect(EAST, null!));
        Assert.Null(first.East);
    }
}
