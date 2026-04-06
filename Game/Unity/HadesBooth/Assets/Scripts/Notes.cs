/*
Class for Notes
Used by our Lyre, Conductor, Flower wall, lights, etc.
By: Taylor Roberts
*/

using UnityEngine;

public enum NoteColor { Red, Blue, Yellow, Cyan }
public struct Note
{
    public NoteColor noteColor;
    public Note(NoteColor noteColor)
    {
        this.noteColor = noteColor;
    }

    public Color GetColor()
    {
        switch (noteColor)
        {
            case NoteColor.Red:
                return Color.red;
            case NoteColor.Blue:
                return Color.blue;
            case NoteColor.Yellow:
                return Color.yellow;
            case NoteColor.Cyan:
                return Color.cyan;
            default:
                return Color.black;
        }
    }
    public string GetColorString()
    {
        switch (noteColor)
        {
            case NoteColor.Red:
                return "r";
            case NoteColor.Blue:
                return "b";
            case NoteColor.Yellow:
                return "y";
            case NoteColor.Cyan:
                return "c";
            default:
                return string.Empty;
        }
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

    public override string ToString()
    {
        return noteColor.ToString();
    }
}

public static class Notes
{
    public static readonly Note Red = new Note(NoteColor.Red);
    public static readonly Note Blue = new Note(NoteColor.Blue);
    public static readonly Note Yellow = new Note(NoteColor.Yellow);
    public static readonly Note Cyan = new Note(NoteColor.Cyan);
}