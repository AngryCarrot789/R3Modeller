namespace R3Modeller.Core.Engine.Utils {
    public enum RotationType {
        /// <summary>
        /// Rotation uses euler angles
        /// </summary>
        Euler,
        /// <summary>
        /// Rotation uses heading/yaw, elevation/pitch and bank/roll (aka YPR)
        /// </summary>
        Bearings
    }
}