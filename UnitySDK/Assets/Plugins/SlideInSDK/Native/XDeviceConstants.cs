//=============================================================================
//
// Copyright 2018 Ximmerse, LTD. All rights reserved.
//
//=============================================================================

//////////////////////////////////////////////////////////////////////////
/// \namespace Ximmerse.InputSystem
/// \brief Contains constants and classes about the input device
namespace Ximmerse.InputSystem {

    #region Enums

    //////////////////////////////////////////////////////////////////////////
    /// \enum ControllerRawAxis
    /// \brief Device raw axis definition
    /// \see https://msdn.microsoft.com/en-us/library/windows/apps/microsoft.directx_sdk.reference.xinput_gamepad
    public enum ControllerRawAxis:int {
        LeftTrigger,
        RightTrigger,
        LeftThumbX,
        LeftThumbY,
        RightThumbX,
        RightThumbY,
        Max,
    }

    //////////////////////////////////////////////////////////////////////////
    /// \enum ControllerRawButton
    /// \brief Device raw button definition
    public enum ControllerRawButton {
        DpadUp        = 0x0001,
        DpadDown      = 0x0002,
        DpadLeft      = 0x0004,
        DpadRight     = 0x0008,
        Start         = 0x0010,
        Back          = 0x0020,
        LeftThumb     = 0x0040,
        RightThumb    = 0x0080,
        LeftShoulder  = 0x0100,
        RightShoulder = 0x0200,
        Guide         = 0x0400,
        A             = 0x1000,
        B             = 0x2000,
        X             = 0x4000,
        Y             = 0x8000,
        // Emulation
        LeftThumbMove   = 0x10000,
        RightThumbMove  = 0x20000,
        LeftTrigger     = 0x40000,
        RightTrigger    = 0x80000,
        LeftThumbUp     = 0x100000,
        LeftThumbDown   = 0x200000,
        LeftThumbLeft   = 0x400000,
        LeftThumbRight  = 0x800000,
        RightThumbUp    = 0x1000000,
        RightThumbDown  = 0x2000000,
        RightThumbLeft  = 0x4000000,
        RightThumbRight = 0x8000000,
        //
        None          = 0x0,
        Any           = ~None,
    }

    //////////////////////////////////////////////////////////////////////////
    /// \enum ControllerAxis
    /// \brief Controller axis definition
    public enum ControllerAxis:int {
        PrimaryTrigger   = ControllerRawAxis.LeftTrigger,
        SecondaryTrigger = ControllerRawAxis.RightTrigger,
        PrimaryThumbX    = ControllerRawAxis.LeftThumbX,
        PrimaryThumbY    = ControllerRawAxis.LeftThumbY,
        SecondaryThumbX  = ControllerRawAxis.RightThumbX,
        SecondaryThumbY  = ControllerRawAxis.RightThumbY,
        Max              = ControllerRawAxis.Max,
    }

    //////////////////////////////////////////////////////////////////////////
    /// \enum ControllerButton
    /// \brief Controller Button definition
    public enum ControllerButton {
        DpadUp              = ControllerRawButton.DpadUp,
        DpadDown            = ControllerRawButton.DpadDown,
        DpadLeft            = ControllerRawButton.DpadLeft,
        DpadRight           = ControllerRawButton.DpadRight,
        Start               = ControllerRawButton.Start,
        Back                = ControllerRawButton.Back,
        PrimaryThumb        = ControllerRawButton.LeftThumb,
        SecondaryThumb      = ControllerRawButton.RightThumb,
        PrimaryShoulder     = ControllerRawButton.LeftShoulder,
        SecondaryShoulder   = ControllerRawButton.RightShoulder,
        Guide               = ControllerRawButton.Guide,
        A                   = ControllerRawButton.A,
        B                   = ControllerRawButton.B,
        X                   = ControllerRawButton.X,
        Y                   = ControllerRawButton.Y,
        PrimaryThumbMove    = ControllerRawButton.LeftThumbMove,
        SecondaryThumbMove  = ControllerRawButton.RightThumbMove,
        PrimaryTrigger      = ControllerRawButton.LeftTrigger,
        SecondaryTrigger    = ControllerRawButton.RightTrigger,
        PrimaryThumbUp      = ControllerRawButton.LeftThumbUp,
        PrimaryThumbDown    = ControllerRawButton.LeftThumbDown,
        PrimaryThumbLeft    = ControllerRawButton.LeftThumbLeft,
        PrimaryThumbRight   = ControllerRawButton.LeftThumbRight,
        SecondaryThumbUp    = ControllerRawButton.RightThumbUp,
        SecondaryThumbDown  = ControllerRawButton.RightThumbDown,
        SecondaryThumbLeft  = ControllerRawButton.RightThumbLeft,
        SecondaryThumbRight = ControllerRawButton.RightThumbRight,
        None                = ControllerRawButton.None,
        Any                 = ControllerRawButton.Any,
    }

    #endregion Enums

    //////////////////////////////////////////////////////////////////////////
    /// \enum DeviceConnectionState
    /// \brief The connection status of the host to the device
    public enum DeviceConnectionState {
        /// <summary>
        /// Indicates that the device is disconnected.
        /// </summary>
        Disconnected,
        /// <summary>
        /// Indicates that the host is scanning for devices.
        /// </summary>
        Scanning,
        /// <summary>
        /// Indicates that the device is connecting.
        /// </summary>
        Connecting,
        /// <summary>
        /// Indicates that the device is connected.
        /// </summary>
        Connected,
        /// <summary>
        /// Indicates that an error has occurred.
        /// </summary>
        Error,
    };


    //////////////////////////////////////////////////////////////////////////
    /// \enum TrackingResult
    /// \brief Tracking status
    [System.Flags]
    public enum TrackingResult{
        NotTracked      =    0,  ///< Tracking lost
        RotationTracked = 1<<0,  ///< Only rotation tracking
        PositionTracked = 1<<1,  ///< Only position tracking
        PoseTracked = (RotationTracked|PositionTracked),  ///< Contains position and rotation tracking
        RotationEmulated = 1<<2, ///< Simulated rotation data
        PositionEmulated = 1<<3, ///< Simulated position data
    };


    //////////////////////////////////////////////////////////////////////////
    /// \enum XimmerseButton
    /// \brief Tracking status
    public enum XimmerseButton {
        // Standard
        Touch   = ControllerRawButton.LeftThumbMove, 
        Click   = DpadClick|DpadUp|DpadDown|DpadLeft|DpadRight,
        App     = ControllerRawButton.Back,
        Home    = ControllerRawButton.Start,
        // Touchpad To Dpad
        DpadUp    = ControllerRawButton.DpadUp,
        DpadDown  = ControllerRawButton.DpadDown,
        DpadLeft  = ControllerRawButton.DpadLeft,
        DpadRight = ControllerRawButton.DpadRight,
        DpadClick = ControllerRawButton.LeftThumb,
        // Gestures
        SwipeUp     = ControllerRawButton.LeftThumbUp,
        SwipeDown   = ControllerRawButton.LeftThumbDown,
        SwipeLeft   = ControllerRawButton.LeftThumbLeft,
        SwipeRight  = ControllerRawButton.LeftThumbRight,
        SlashUp    = ControllerRawButton.RightThumbUp,
        SlashDown  = ControllerRawButton.RightThumbDown,
        SlashLeft  = ControllerRawButton.RightThumbLeft,
        SlashRight = ControllerRawButton.RightThumbRight,
        // Ximmerse Ex
        Trigger = ControllerRawButton.LeftTrigger,
        GripL   = ControllerRawButton.LeftShoulder,
        GripR   = ControllerRawButton.RightShoulder,
        Grip    = GripL|GripR,
    }
}
