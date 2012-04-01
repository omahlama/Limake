/**
 * <summary>
 * Enumeration for the type of piece that exists on the board at a certain position.
 * Checks can be made with bitwise and operator, eg. board[7] & NotBlue
 * </summary
 */
public enum Piece : byte
{
    None = 0,
    Green = 1,
    Red = 2,
    Blue = 3,
    Yellow = 4
}

public enum Position : byte
{
    GreenStart,
    Green1,
    Green2,
    Green3,
    Green4,
    Green5,
    Green6,
    RedStart,
    Red1,
    Red2,
    Red3,
    Red4,
    Red5,
    Red6,
    BlueStart,
    Blue1,
    Blue2,
    Blue3,
    Blue4,
    Blue5,
    Blue6,
    YellowStart,
    Yellow1,
    Yellow2,
    Yellow3,
    Yellow4,
    Yellow5,
    Yellow6,
    GreenGoal1,
    GreenGoal2,
    GreenGoal3,
    GreenGoal4,
    RedGoal1,
    RedGoal2,
    RedGoal3,
    RedGoal4,
    BlueGoal1,
    BlueGoal2,
    BlueGoal3,
    BlueGoal4,
    YellowGoal1,
    YellowGoal2,
    YellowGoal3,
    YellowGoal4,
    GreenHome1,
    GreenHome2,
    GreenHome3,
    GreenHome4,
    RedHome1,
    RedHome2,
    RedHome3,
    RedHome4,
    BlueHome1,
    BlueHome2,
    BlueHome3,
    BlueHome4,
    YellowHome1,
    YellowHome2,
    YellowHome3,
    YellowHome4,
    None
}

public enum MoveType
{
    Move,
    OutOfHome,
    Eat,
    SelfTackle,
    DoubleUp
}