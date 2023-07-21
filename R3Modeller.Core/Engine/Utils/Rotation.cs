using System;
using System.Numerics;

namespace R3Modeller.Core.Engine.Utils {
    public static class Rotation {
        public static Vector3 GetOrbitPosition(float yaw, float pitch) {
            Vector3 direction = new Vector3(
                (float) (Math.Cos(-pitch) * Math.Sin(yaw)),
                (float) Math.Sin(-pitch),
                (float) (Math.Cos(-pitch) * Math.Cos(yaw))
            );

            return direction;
        }
    }
}