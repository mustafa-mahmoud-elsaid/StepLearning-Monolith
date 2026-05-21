namespace Courses.Application.Utilities;

public static class DisplayOrderCalculator
{
    public static int GetNext(int lastOrder)
    {
        if (lastOrder == 0)
            return 10;
        else
            return ((lastOrder / 10) + 1) * 10; // should be match the pattern: 10, 20, 30, ...
    }
}
