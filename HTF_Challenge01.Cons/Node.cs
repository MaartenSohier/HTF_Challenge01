class Node
{
    public Position Position { get; }
    public int FScore { get; }

    public Node(Position position, int fScore)
    {
        Position = position;
        FScore = fScore;
    }
}
