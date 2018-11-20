namespace Ximmerse.SlideInSDK
{
    /// <summary>
    /// Enumeration that defines the AR camera's tracking mode.
    /// </summary>
    public enum ARCameraTrackingMode 
    {
        /// <summary>
        /// Camera's position and rotation remain unchanged at runtime.
        /// </summary>
        Static = 0,

        /// <summary>
        /// Camera's position remain unchanged, but rotation is update per frame according to IMU rotation.
        /// </summary>
        RotateLocally = 1,
//
//        /// <summary>
//        /// the world center is the specific marker target. 
//        /// Camera performs 6DOF (position + rotation) tracking around the specific marker target.
//        /// </summary>
//        WorldCenterOnSpecificMarker = 2,
//
//        /// <summary>
//        /// Linked benchmark is a mode that take multi-visible marker into a linked network, 
//        /// once a node become invisible, the rest visible node become new benchmark for head 6DOF tracking.
//        /// </summary>
//        LinkedBenchmarks = 3,
//
//        /// <summary>
//        /// Use multiple marker to composite as a marker table.
//        /// </summary>
//        CompositeMarkerTable = 4,
    }

}