using System.Numerics;

namespace R3Modeller.Core.Engine {
    public class MatrixUtils {
        /// <summary>
        /// Creates a matrix that can be used to transform world coordinates into local coordinates, using the given position, rotation and scale
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotate"></param>
        /// <param name="scale"></param>
        /// <returns></returns>
        public static Matrix4x4 WorldToLocal(Vector3 position, Vector3 rotate, Vector3 scale) {
            Vector3 s = new Vector3(1.0f / scale.X, 1.0f / scale.Y, 1.0f / scale.Z);
            return Matrix4x4.CreateScale(s) * CreateNegativeRotationZXY(rotate) * Matrix4x4.CreateTranslation(-position);
        }

        /// <summary>
        /// Creates a matrix that can be used to transform local coordinates into world coordinates, using the given position, rotation and scale
        /// <para>
        /// A Translation matrix is created, then it is multiplied by 3 rotation matrices (X, Y then Z), then that is multiplied by a scale matrix
        /// </para>
        /// </summary>
        /// <param name="position">Position (aka origin of the translation)</param>
        /// <param name="rotate">Rotation, relative to the new position</param>
        /// <param name="scale">Scale, relative to the new position</param>
        /// <returns>A transformation matrix, aka model matrix</returns>
        public static Matrix4x4 LocalToWorld(Vector3 position, Vector3 rotate, Vector3 scale) {
            return Matrix4x4.CreateTranslation(position) * CreateRotationYXZ(rotate) * Matrix4x4.CreateScale(scale);
        }

        /// <summary>
        /// Creates three rotation matrices and multiplies them in the order of Y, X and Z
        /// </summary>
        /// <param name="rotation">Input rotation vector</param>
        /// <returns>Output rotation matrix</returns>
        public static Matrix4x4 CreateRotationYXZ(Vector3 rotation) => Matrix4x4.CreateRotationY(rotation.Y) * Matrix4x4.CreateRotationX(rotation.X) * Matrix4x4.CreateRotationZ(rotation.Z);

        /// <summary>
        /// Creates three rotation matrices and multiplies them in the order of Z, X and Y
        /// </summary>
        /// <param name="rotation">Input rotation vector</param>
        /// <returns>Output rotation matrix</returns>
        public static Matrix4x4 CreateNegativeRotationZXY(Vector3 rotation) => Matrix4x4.CreateRotationZ(-rotation.Z) * Matrix4x4.CreateRotationX(-rotation.X) * Matrix4x4.CreateRotationY(-rotation.Y);
    }
}