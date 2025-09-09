using System;

public enum HeightChangeDirection
{
    Increased,
    Decreased
}

public class StackHeightChangedEventArgs : EventArgs
{
    public int NewHeight { get; }
    public HeightChangeDirection Direction { get; }

    public StackHeightChangedEventArgs(int newHeight, HeightChangeDirection direction)
    {
        NewHeight = newHeight;
        Direction = direction;
    }
}