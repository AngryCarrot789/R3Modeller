using System;
using System.Runtime.InteropServices;

namespace R3Modeller.RawInputs {
    // https://www.pinvoke.net/default.aspx

    /// <summary>
    /// Enumeration containing the type device the raw input is coming from.
    /// </summary>
    public enum RawInputType {
        /// <summary>
        /// Mouse input.
        /// </summary>
        Mouse = 0,

        /// <summary>
        /// Keyboard input.
        /// </summary>
        Keyboard = 1,

        /// <summary>
        ///  Human interface device input.
        /// </summary>
        HID = 2,

        /// <summary>
        /// Another device that is not the keyboard or the mouse.
        /// </summary>
        Other = 3
    }

    /// <summary>
    /// Value type for a raw input header.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RAWINPUTHEADER {
#if X86
        public const int SIZEOF = 16;
#else
        public const int SIZEOF = 24;
#endif

        /// <summary>Type of device the input is coming from.</summary>
        public RawInputType Type;

        /// <summary>Size of the packet of data.</summary>
        public int Size;

        /// <summary>Handle to the device sending the data.</summary>
        public IntPtr Device;

        /// <summary>wParam from the window message.</summary>
        public IntPtr wParam;
    }

    /// <summary>
    /// Enumeration containing the flags for raw mouse data.
    /// </summary>
    [Flags()]
    public enum RawMouseFlags : ushort {
        /// <summary>Relative to the last position.</summary>
        MoveRelative = 0,

        /// <summary>Absolute positioning.</summary>
        MoveAbsolute = 1,

        /// <summary>Coordinate data is mapped to a virtual desktop.</summary>
        VirtualDesktop = 2,

        /// <summary>Attributes for the mouse have changed.</summary>
        AttributesChanged = 4
    }

    /// <summary>
    /// Enumeration containing the button data for raw mouse input.
    /// </summary>
    [Flags]
    public enum RawMouseButtons : ushort {
        /// <summary>No button.</summary>
        None = 0,
        /// <summary>Left (button 1) down.</summary>
        LeftDown = 0x0001,
        /// <summary>Left (button 1) up.</summary>
        LeftUp = 0x0002,
        /// <summary>Right (button 2) down.</summary>
        RightDown = 0x0004,
        /// <summary>Right (button 2) up.</summary>
        RightUp = 0x0008,
        /// <summary>Middle (button 3) down.</summary>
        MiddleDown = 0x0010,
        /// <summary>Middle (button 3) up.</summary>
        MiddleUp = 0x0020,
        /// <summary>Button 4 down.</summary>
        Button4Down = 0x0040,
        /// <summary>Button 4 up.</summary>
        Button4Up = 0x0080,
        /// <summary>Button 5 down.</summary>
        Button5Down = 0x0100,
        /// <summary>Button 5 up.</summary>
        Button5Up = 0x0200,
        /// <summary>Mouse wheel moved.</summary>
        MouseWheel = 0x0400
    }

    /// <summary>
    /// Contains information about the state of the mouse.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RawMouse {
        /// <summary>
        /// The mouse state.
        /// </summary>
        [FieldOffset(0)] public RawMouseFlags Flags;

        /// <summary>
        /// Flags for the event.
        /// </summary>
        [FieldOffset(4)] public RawMouseButtons ButtonFlags;

        /// <summary>
        /// If the mouse wheel is moved, this will contain the delta amount.
        /// </summary>
        [FieldOffset(6)] public ushort ButtonData;

        /// <summary>
        /// Raw button data.
        /// </summary>
        [FieldOffset(8)] public uint RawButtons;

        /// <summary>
        /// The motion in the X direction. This is signed relative motion or
        /// absolute motion, depending on the value of usFlags.
        /// </summary>
        [FieldOffset(12)] public int LastX;

        /// <summary>
        /// The motion in the Y direction. This is signed relative motion or absolute motion,
        /// depending on the value of usFlags.
        /// </summary>
        [FieldOffset(16)] public int LastY;

        /// <summary>
        /// The device-specific additional information for the event.
        /// </summary>
        [FieldOffset(20)] public uint ExtraInformation;
    }

    /// <summary>
    /// Value type for raw input.
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct RAWINPUT {
        /// <summary>Header for the data.</summary>
        [FieldOffset(0)] public RAWINPUTHEADER Header;

        /// <summary>Mouse raw input data.</summary>
        [FieldOffset(RAWINPUTHEADER.SIZEOF)]
        public RawMouse Mouse;

        // Not used
        // /// <summary>Keyboard raw input data.</summary>
        // [FieldOffset(RAWINPUTHEADER.SIZEOF)] public RawKeyboard Keyboard;
        // /// <summary>HID raw input data.</summary>
        // [FieldOffset(RAWINPUTHEADER.SIZEOF)] public RawInputHid Hid;
    }
}