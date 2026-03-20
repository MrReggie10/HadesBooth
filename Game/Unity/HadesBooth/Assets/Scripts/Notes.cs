/*
Class for Notes
Used by our Lyre, Conductor, Flower wall, lights, etc.
By: Taylor Roberts
*/

public enum NoteColor { Red, Blue, Yellow, Cyan }
public struct Note
{
    public NoteColor noteColor;
    // TODO: add rgb values and note pitch
    public Note(NoteColor noteColor)
    {
        this.noteColor = noteColor;
    }

    // boilerplate so == works
    public static bool operator ==(Note a, Note b)
    {
        return a.noteColor == b.noteColor;
    }

    public static bool operator !=(Note a, Note b)
    {
        return a.noteColor != b.noteColor;
    }

    public override bool Equals(object obj)
    {
        if (obj is Note other)
            return this == other;
        return false;
    }

    public override int GetHashCode()
    {
        return noteColor.GetHashCode();
    }
}

public static class Notes
{
    public static readonly Note Red = new Note(NoteColor.Red);
    public static readonly Note Blue = new Note(NoteColor.Blue);
    public static readonly Note Yellow = new Note(NoteColor.Yellow);
    public static readonly Note Cyan = new Note(NoteColor.Cyan);
}