namespace GRIFTools;

public static class GrodEnums
{
    public enum GrodItemType
    {
        Bool = 1,
        String = 2,
        Number = 3,
        List = 4,
        Obj = 5,
    }

    public enum GrodNumberType
    {
        Int = 1,
        Long = 2,
        Float = 3,
        Decimal = 4,
    }

    public enum WhichData
    {
        Both = 0,
        Base = 1,
        Overlay = 2
    }
}
