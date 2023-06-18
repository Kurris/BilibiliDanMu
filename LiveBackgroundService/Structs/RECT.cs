using System.Runtime.InteropServices;

namespace LiveBackgroundService.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct RECT
{
    //
    // 摘要:
    //     The x-coordinate of the upper-left corner of the rectangle.
    public int left;

    //
    // 摘要:
    //     The y-coordinate of the upper-left corner of the rectangle.
    public int top;

    //
    // 摘要:
    //     The x-coordinate of the lower-right corner of the rectangle.
    public int right;

    //
    // 摘要:
    //     The y-coordinate of the lower-right corner of the rectangle.
    public int bottom;
}